using Application.Common.Result.Message;
using Application.Error;
using Application.Factory;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.UnitOfWork;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.Passport.Register
{
    internal sealed class RegisterPassportCommandHandler : ICommandHandler<RegisterPassportCommand, IMessageResult<Guid>>
    {
        private readonly ITimeProvider prvTime;
        private readonly IUnitOfWork<IPassportDataAccess> uowUnitOfWork;
        private readonly IPassportSetting ppSetting;

        private readonly IPassportRepository repoPassport;
        private readonly IPassportHolderRepository repoHolder;
        private readonly IPassportTokenRepository repoToken;

        public RegisterPassportCommandHandler(
            ITimeProvider prvTime,
            IUnitOfWork<IPassportDataAccess> uowUnitOfWork,
            IPassportSetting ppSetting,
            IPassportRepository repoPassport,
            IPassportHolderRepository repoHolder,
            IPassportTokenRepository repoToken)
        {
            this.prvTime = prvTime;
            this.uowUnitOfWork = uowUnitOfWork;
            this.ppSetting = ppSetting;
            this.repoPassport = repoPassport;
            this.repoToken = repoToken;
            this.repoHolder = repoHolder;
        }

        public async ValueTask<IMessageResult<Guid>> Handle(RegisterPassportCommand cmdRegisterPassport, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<Guid>(DefaultMessageError.TaskAborted);

            IPassportHolder? ppHolder = DomainFactory.Authorization.PassportHolder.Create(
                sCultureName: cmdRegisterPassport.CultureName,
                sEmailAddress: cmdRegisterPassport.EmailAddress,
                sFirstName: cmdRegisterPassport.FirstName,
                sLastName: cmdRegisterPassport.LastName,
                sPhoneNumber: cmdRegisterPassport.PhoneNumber,
                ppSetting: ppSetting);

            if (ppHolder is null)
                return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Holder has not been created." });

            IPassport? ppPassport = DomainFactory.Authorization.Passport.Create(
                dtExpiredAt: prvTime.GetUtcNow().Add(ppSetting.ExpiresAfterDuration),
                guHolderId: ppHolder.Id,
                guIssuedBy: cmdRegisterPassport.IssuedBy,
                dtLastCheckedAt: prvTime.GetUtcNow());

            if (ppPassport is null)
                return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Passport has not been created." });

            IPassportToken? ppToken = DomainFactory.Authorization.PassportToken.Create(
                guPassportId: ppPassport.Id,
                sProvider: cmdRegisterPassport.CredentialToRegister.Provider,
                bTwoFactorIsEnabled: false);

            if (ppToken is null)
                return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Token has not been created." });

            bool bIsCommited = false;

            await uowUnitOfWork.TransactionAsync(async () =>
            {
                await repoHolder.InsertAsync(ppHolder, prvTime.GetUtcNow(), tknCancellation);
                await repoPassport.InsertAsync(ppPassport, prvTime.GetUtcNow(), tknCancellation);
                await repoToken.InsertAsync(ppToken, cmdRegisterPassport.CredentialToRegister, prvTime.GetUtcNow(), tknCancellation);

                bIsCommited = uowUnitOfWork.TryCommit();

                if (bIsCommited == false)
                    uowUnitOfWork.TryRollback();
            });

            if (bIsCommited == false)
                return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Transaction has not been committed." });

            return new MessageResult<Guid>(ppPassport.Id);
        }
    }
}
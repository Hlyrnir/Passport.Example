using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.Passport;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Query.Authorization.Passport.ById
{
    internal sealed class PassportByIdQueryHandler : IQueryHandler<PassportByIdQuery, IMessageResult<PassportByIdResult>>
    {
        private readonly IPassportRepository repoPassport;

        public PassportByIdQueryHandler(IPassportRepository repoPassport)
        {
            this.repoPassport = repoPassport;
        }

        public async ValueTask<IMessageResult<PassportByIdResult>> Handle(PassportByIdQuery qryQuery, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<PassportByIdResult>(DefaultMessageError.TaskAborted);

            RepositoryResult<IPassport> rsltPassport = await repoPassport.FindByIdAsync(qryQuery.PassportId, tknCancellation);

            return rsltPassport.Match(
                msgError => new MessageResult<PassportByIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                ppPassport =>
                {
                    PassportByIdResult qryResult = new PassportByIdResult()
                    {
                        Passport = ppPassport
                    };

                    return new MessageResult<PassportByIdResult>(qryResult);
                });
        }
    }
}
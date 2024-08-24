using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.Passport;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Query.Authorization.PassportVisa.ById
{
    internal sealed class PassportVisaByIdQueryHandler : IQueryHandler<PassportVisaByIdQuery, IMessageResult<PassportVisaByIdResult>>
    {
        private readonly IPassportVisaRepository repoVisa;

        public PassportVisaByIdQueryHandler(IPassportVisaRepository repoVisa)
        {
            this.repoVisa = repoVisa;
        }

        public async ValueTask<IMessageResult<PassportVisaByIdResult>> Handle(PassportVisaByIdQuery msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<PassportVisaByIdResult>(DefaultMessageError.TaskAborted);

            RepositoryResult<IPassportVisa> rsltPassportVisa = await repoVisa.FindByIdAsync(msgMessage.PassportVisaId, tknCancellation);

            return rsltPassportVisa.Match(
                msgError => new MessageResult<PassportVisaByIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                ppPassportVisa =>
                {
                    PassportVisaByIdResult qryResult = new PassportVisaByIdResult()
                    {
                        PassportVisa = ppPassportVisa
                    };

                    return new MessageResult<PassportVisaByIdResult>(qryResult);
                });
        }
    }
}
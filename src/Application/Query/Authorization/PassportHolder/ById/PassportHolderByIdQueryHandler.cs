using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.Passport;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Query.Authorization.PassportHolder.ById
{
    internal sealed class PassportHolderByIdQueryHandler : IQueryHandler<PassportHolderByIdQuery, IMessageResult<PassportHolderByIdResult>>
	{
		private readonly IPassportHolderRepository repoHolder;

		public PassportHolderByIdQueryHandler(IPassportHolderRepository repoHolder)
		{
			this.repoHolder = repoHolder;
		}

		public async ValueTask<IMessageResult<PassportHolderByIdResult>> Handle(PassportHolderByIdQuery qryQuery, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<PassportHolderByIdResult>(DefaultMessageError.TaskAborted);

			RepositoryResult<IPassportHolder> rsltPassportHolder = await repoHolder.FindByIdAsync(qryQuery.PassportHolderId, tknCancellation);

			return rsltPassportHolder.Match(
				msgError => new MessageResult<PassportHolderByIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				ppPassportHolder =>
				{
					PassportHolderByIdResult qryResult = new PassportHolderByIdResult()
					{
						PassportHolder = ppPassportHolder
					};

					return new MessageResult<PassportHolderByIdResult>(qryResult);
				});
		}
	}
}
using Application.Interface.Message;
using Application.Interface.Result;

namespace Application.Interface.Authorization
{
	public interface IAuthorization<in T> where T : IRestrictedAuthorization
    {
        internal ValueTask<IMessageResult<bool>> AuthorizeAsync(T msgMessage, IEnumerable<Guid> enumPassportVisaId, CancellationToken tknCancellation);
    }
}

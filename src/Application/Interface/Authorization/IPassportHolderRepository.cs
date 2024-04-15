using Application.Common.Result.Repository;
using Domain.Interface.Authorization;

namespace Application.Interface.Passport
{
	public interface IPassportHolderRepository
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppHolder"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> DeleteAsync(IPassportHolder ppHolder, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guHolderId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ExistsAsync(Guid guHolderId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guHolderId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IPassportHolder>> FindByIdAsync(Guid guHolderId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppHolder"></param>
		/// <param name="dtCreatedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> InsertAsync(IPassportHolder ppHolder, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppHolder"></param>
		/// <param name="dtEditedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> UpdateAsync(IPassportHolder ppHolder, DateTimeOffset dtEditedAt, CancellationToken tknCancellation);
	}
}

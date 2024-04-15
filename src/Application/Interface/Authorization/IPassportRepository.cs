using Application.Common.Result.Repository;
using Domain.Interface.Authorization;

namespace Application.Interface.Passport
{
	public interface IPassportRepository
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppPassport"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> DeleteAsync(IPassport ppPassport, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guPassportId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ExistsAsync(Guid guPassportId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guPassportId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IPassport>> FindByIdAsync(Guid guPassportId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppPassport"></param>
		/// <param name="dtCreatedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> InsertAsync(IPassport ppPassport, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppPassport"></param>
		/// <param name="dtEditedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> UpdateAsync(IPassport ppPassport, DateTimeOffset dtEditedAt, CancellationToken tknCancellation);
	}
}
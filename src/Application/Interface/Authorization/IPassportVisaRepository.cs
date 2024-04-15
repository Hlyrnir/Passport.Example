using Application.Common.Result.Repository;
using Domain.Interface.Authorization;

namespace Application.Interface.Passport
{
	public interface IPassportVisaRepository
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppVisa"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> DeleteAsync(IPassportVisa ppVisa, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guVisaId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ExistsAsync(Guid guVisaId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="enumVisaId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ExistsAsync(IEnumerable<Guid> enumVisaId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sName"></param>
		/// <param name="iLevel"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ByNameAtLevelExistsAsync(string sName, int iLevel, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guVisaId"></param>
		/// <param name="iLevel"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IPassportVisa>> FindByIdAsync(Guid guVisaId, CancellationToken tknCancellation);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sName"></param>
		/// <param name="iLevel"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IPassportVisa>> FindByNameAsync(string sName, int iLevel, CancellationToken tknCancellation);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppPassport"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IEnumerable<IPassportVisa>>> FindByPassportAsync(Guid guPassportId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppVisa"></param>
		/// <param name="dtCreatedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> InsertAsync(IPassportVisa ppVisa, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppVisa"></param>
		/// <param name="dtEditedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> UpdateAsync(IPassportVisa ppVisa, DateTimeOffset dtEditedAt, CancellationToken tknCancellation);
	}
}

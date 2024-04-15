using Application.Common.Result.Repository;
using Domain.Interface.PhysicalData;

namespace Application.Interface.PhysicalData
{
	public interface ITimePeriodRepository
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdTimePeriod"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> DeleteAsync(ITimePeriod pdTimePeriod, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guPassportId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ExistsAsync(Guid guTimePeriodId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<ITimePeriod>> FindByIdAsync(Guid guId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IEnumerable<ITimePeriod>>> FindByFilterAsync(ITimePeriodByFilterOption optFilter, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdTimePeriod"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> InsertAsync(ITimePeriod pdTimePeriod, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="optFilter"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<int>> QuantityByFilterAsync(ITimePeriodByFilterOption optFilter, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdTimePeriod"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> UpdateAsync(ITimePeriod pdTimePeriod, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation);
	}
}
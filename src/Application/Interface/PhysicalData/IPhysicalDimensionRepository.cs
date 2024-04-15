using Application.Common.Result.Repository;
using Domain.Interface.PhysicalData;

namespace Application.Interface.PhysicalData
{
	public interface IPhysicalDimensionRepository
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdPhysicalDimension"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> DeleteAsync(IPhysicalDimension pdPhysicalDimension, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guPhysicalDimensionId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ExistsAsync(Guid guPhysicalDimensionId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IPhysicalDimension>> FindByIdAsync(Guid guId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IEnumerable<IPhysicalDimension>>> FindByFilterAsync(IPhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdPhysicalDimension"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> InsertAsync(IPhysicalDimension pdPhysicalDimension, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="optFilter"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<int>> QuantityByFilterAsync(IPhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdPhysicalDimension"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> UpdateAsync(IPhysicalDimension pdPhysicalDimension, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation);
	}
}
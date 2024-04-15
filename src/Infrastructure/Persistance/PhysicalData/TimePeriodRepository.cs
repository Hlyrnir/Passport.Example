using Application.Common.Result.Repository;
using Application.Interface.DataAccess;
using Application.Interface.PhysicalData;
using Dapper;
using Domain.Aggregate.PhysicalData.TimePeriod;
using Domain.Interface.PhysicalData;
using Domain.Interface.Transfer;
using Infrastructure.Error;
using Infrastructure.Extension.PhysicalData;
using Infrastructure.Transfer.PhysicalData;
using Infrastructure.TypeHandler;
using System.Data;

namespace Infrastructure.Persistance.PhysicalData
{
	internal sealed class TimePeriodRepository : ITimePeriodRepository
	{
		private readonly ISqliteDataAccess sqlDataAccess;

		public TimePeriodRepository(IPhysicalDataAccess sqlDataAccess)
		{
			this.sqlDataAccess = sqlDataAccess;

			SqlMapper.AddTypeHandler(new GuidTypeHandler());
			SqlMapper.AddTypeHandler(typeof(double[]), new DoubleArrayTypeHandler());
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> DeleteAsync(ITimePeriod pdTimePeriod, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"DELETE FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{TimePeriodColumn.Id}] = @Id";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualStamp", pdTimePeriod.ConcurrencyStamp);
				dpParameter.Add("Id", pdTimePeriod.Id);

				int iResult = (-1);

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = TimePeriodError.Code.Method,
						Description = $"Could not delete {pdTimePeriod.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> ExistsAsync(
			Guid guTimePeriodId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guTimePeriodId);

				bool bResult = await sqlDataAccess.Connection.ExecuteScalarAsync<bool>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				return await Task.FromResult(new RepositoryResult<bool>(bResult));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<IEnumerable<ITimePeriod>>> FindByFilterAsync(ITimePeriodByFilterOption optFilter, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IEnumerable<ITimePeriod>>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									[{TimePeriodColumn.ConcurrencyStamp}], 
									[{TimePeriodColumn.Id}], 
									[{TimePeriodColumn.Magnitude}], 
									[{TimePeriodColumn.Offset}], 
									[{TimePeriodColumn.PhysicalDimensionId}] 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE (@PhysicalDimensionId IS NULL OR [{TimePeriodColumn.PhysicalDimensionId}] = @PhysicalDimensionId) 
									LIMIT @PageSize 
									OFFSET @PageOffset;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("PhysicalDimensionId", optFilter.PhysicalDimensionId);
				dpParameter.Add("PageOffset", (optFilter.Page + (-1)) * optFilter.PageSize);
				dpParameter.Add("PageSize", optFilter.PageSize);

				IEnumerable<ITimePeriodTransferObject> enumTimePeriodTransferObject = await sqlDataAccess.Connection.QueryAsync<TimePeriodTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumTimePeriodTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<IEnumerable<ITimePeriod>>(new RepositoryError()
					{
						Code = TimePeriodError.Code.Method,
						Description = $"No data has been found."
					}));

				IList<ITimePeriod> lstTimePeriod = new List<ITimePeriod>();

				foreach (ITimePeriodTransferObject dtoTimePeriod in enumTimePeriodTransferObject)
				{
					ITimePeriod? pdTimePeriod = TimePeriod.Initialize(dtoTimePeriod);

					if (pdTimePeriod is null)
						return await Task.FromResult(new RepositoryResult<IEnumerable<ITimePeriod>>(new RepositoryError()
						{
							Code = TimePeriodError.Code.Method,
							Description = $"Time period has invalid data."
						}));

					lstTimePeriod.Add(pdTimePeriod);
				}

				return await Task.FromResult(new RepositoryResult<IEnumerable<ITimePeriod>>(lstTimePeriod.AsEnumerable()));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IEnumerable<ITimePeriod>>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<ITimePeriod>> FindByIdAsync(Guid guTimePeriodId, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<ITimePeriod>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									[{TimePeriodColumn.ConcurrencyStamp}], 
									[{TimePeriodColumn.Id}], 
									[{TimePeriodColumn.Magnitude}], 
									[{TimePeriodColumn.Offset}], 
									[{TimePeriodColumn.PhysicalDimensionId}] 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guTimePeriodId);

				IEnumerable<ITimePeriodTransferObject> enumTimePeriodTransferObject = await sqlDataAccess.Connection.QueryAsync<TimePeriodTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumTimePeriodTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<ITimePeriod>(new RepositoryError()
					{
						Code = TimePeriodError.Code.Method,
						Description = $"Time period {guTimePeriodId} has not been found."
					}));

				ITimePeriod? pdTimePeriod = TimePeriod.Initialize(enumTimePeriodTransferObject.First());

				if (pdTimePeriod is null)
					return await Task.FromResult(new RepositoryResult<ITimePeriod>(new RepositoryError()
					{
						Code = TimePeriodError.Code.Method,
						Description = $"Time period {guTimePeriodId} has invalid data."
					}));

				return await Task.FromResult(new RepositoryResult<ITimePeriod>(pdTimePeriod));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<ITimePeriod>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> InsertAsync(ITimePeriod pdTimePeriod, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"INSERT INTO [{TimePeriodTable.TimePeriod}] (
									[{TimePeriodColumn.ConcurrencyStamp}], 
									[{TimePeriodColumn.Id}], 
									[{TimePeriodColumn.Magnitude}], 
									[{TimePeriodColumn.Offset}], 
									[{TimePeriodColumn.PhysicalDimensionId}]) 
									SELECT 
									@ConcurrencyStamp, 
									@Id, 
									@Magnitude, 
									@Offset, 
									@PhysicalDimensionId
									WHERE NOT EXISTS(
									SELECT 1 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE [{TimePeriodColumn.Id}] = @Id);";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", pdTimePeriod.ConcurrencyStamp);
				dpParameter.Add("Id", pdTimePeriod.Id);
				dpParameter.Add("Magnitude", pdTimePeriod.Magnitude);
				dpParameter.Add("Offset", pdTimePeriod.Offset);
				dpParameter.Add("PhysicalDimensionId", pdTimePeriod.PhysicalDimensionId);

				int iResult = (-1);

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = TimePeriodError.Code.Method,
						Description = $"Could not create {pdTimePeriod.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<int>> QuantityByFilterAsync(ITimePeriodByFilterOption optFilter, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<int>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									COUNT([{TimePeriodColumn.Id}]) 
									FROM [{TimePeriodTable.TimePeriod}] 
									WHERE (@PhysicalDimensionId IS NULL OR [{TimePeriodColumn.PhysicalDimensionId}] = @PhysicalDimensionId);";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("PhysicalDimensionId", optFilter.PhysicalDimensionId);

				int iQuantity = 0;

				iQuantity = await sqlDataAccess.Connection.QuerySingleAsync<int>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				return await Task.FromResult(new RepositoryResult<int>(iQuantity));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<int>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> UpdateAsync(ITimePeriod pdTimePeriod, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"UPDATE [{TimePeriodTable.TimePeriod}] SET 
									[{TimePeriodColumn.ConcurrencyStamp}] = @ConcurrencyStamp, 
									[{TimePeriodColumn.Id}] = @Id, 
									[{TimePeriodColumn.Magnitude}] = @Magnitude, 
									[{TimePeriodColumn.Offset}] = @Offset, 
									[{TimePeriodColumn.PhysicalDimensionId}] = @PhysicalDimensionId 
									WHERE [{TimePeriodColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{TimePeriodColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualStamp", pdTimePeriod.ConcurrencyStamp);
				dpParameter.Add("ConcurrencyStamp", Guid.NewGuid());
				dpParameter.Add("Id", pdTimePeriod.Id);
				dpParameter.Add("Magnitude", pdTimePeriod.Magnitude);
				dpParameter.Add("Offset", pdTimePeriod.Offset);
				dpParameter.Add("PhysicalDimensionId", pdTimePeriod.PhysicalDimensionId);
				dpParameter.Add("Id", pdTimePeriod.Id);

				int iResult = (-1);

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = TimePeriodError.Code.Method,
						Description = $"Could not update {pdTimePeriod.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = TimePeriodError.Code.Exception, Description = exException.Message }));
			}
		}
	}
}
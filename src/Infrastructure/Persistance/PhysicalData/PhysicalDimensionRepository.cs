using Application.Common.Result.Repository;
using Application.Interface.DataAccess;
using Application.Interface.PhysicalData;
using Dapper;
using Domain.Aggregate.PhysicalData.PhysicalDimension;
using Domain.Interface.PhysicalData;
using Domain.Interface.Transfer;
using Infrastructure.Error;
using Infrastructure.Extension.PhysicalData;
using Infrastructure.Transfer.PhysicalData;
using Infrastructure.TypeHandler;
using System.Data;

namespace Infrastructure.Persistance.PhysicalData
{
	internal sealed class PhysicalDimensionRepository : IPhysicalDimensionRepository
	{
		private readonly IDataAccess sqlDataAccess;

		public PhysicalDimensionRepository(IPhysicalDataAccess sqlDataAccess)
		{
			this.sqlDataAccess = sqlDataAccess;

			SqlMapper.AddTypeHandler(new GuidTypeHandler());
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> DeleteAsync(IPhysicalDimension pdPhysicalDimension, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"DELETE FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{PhysicalDimensionColumn.Id}] = @Id";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualStamp", pdPhysicalDimension.ConcurrencyStamp);
				dpParameter.Add("Id", pdPhysicalDimension.Id);

				int iResult = (-1);

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PhysicalDimensionError.Code.Method,
						Description = $"Could not delete {pdPhysicalDimension.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> ExistsAsync(
			Guid guPhysicalDimensionId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guPhysicalDimensionId);

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
		public async Task<RepositoryResult<IEnumerable<IPhysicalDimension>>> FindByFilterAsync(IPhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IEnumerable<IPhysicalDimension>>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									[{PhysicalDimensionColumn.ExponentOfAmpere}], 
									[{PhysicalDimensionColumn.ExponentOfCandela}], 
									[{PhysicalDimensionColumn.ExponentOfKelvin}], 
									[{PhysicalDimensionColumn.ExponentOfKilogram}], 
									[{PhysicalDimensionColumn.ExponentOfMetre}], 
									[{PhysicalDimensionColumn.ExponentOfMole}], 
									[{PhysicalDimensionColumn.ExponentOfSecond}], 
									[{PhysicalDimensionColumn.ConcurrencyStamp}], 
									[{PhysicalDimensionColumn.ConversionFactorToSI}], 
									[{PhysicalDimensionColumn.CultureName}], 
									[{PhysicalDimensionColumn.Id}], 
									[{PhysicalDimensionColumn.Name}], 
									[{PhysicalDimensionColumn.Symbol}], 
									[{PhysicalDimensionColumn.Unit}] 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE (@ConversionFactorToSI IS NULL OR [{PhysicalDimensionColumn.ConversionFactorToSI}] = @ConversionFactorToSI) 
									AND (@CultureName IS NULL OR [{PhysicalDimensionColumn.CultureName}] LIKE('%'|| @CultureName ||'%')) 
									AND (@ExponentOfAmpere IS NULL OR [{PhysicalDimensionColumn.ExponentOfAmpere}] = @ExponentOfAmpere) 
									AND (@ExponentOfCandela IS NULL OR [{PhysicalDimensionColumn.ExponentOfCandela}] = @ExponentOfCandela) 
									AND (@ExponentOfKelvin IS NULL OR [{PhysicalDimensionColumn.ExponentOfKelvin}] = @ExponentOfKelvin) 
									AND (@ExponentOfKilogram IS NULL OR [{PhysicalDimensionColumn.ExponentOfKilogram}] = @ExponentOfKilogram) 
									AND (@ExponentOfMetre IS NULL OR [{PhysicalDimensionColumn.ExponentOfMetre}] = @ExponentOfMetre) 
									AND (@ExponentOfMole IS NULL OR [{PhysicalDimensionColumn.ExponentOfMole}] = @ExponentOfMole) 
									AND (@ExponentOfSecond IS NULL OR [{PhysicalDimensionColumn.ExponentOfSecond}] = @ExponentOfSecond) 
									AND (@Name IS NULL OR [{PhysicalDimensionColumn.Name}] LIKE('%'|| @Name ||'%')) 
									AND (@Symbol IS NULL OR [{PhysicalDimensionColumn.Symbol}] LIKE('%'|| @Symbol ||'%')) 
									AND (@Unit IS NULL OR [{PhysicalDimensionColumn.Unit}] LIKE('%'|| @Unit ||'%')) 
									LIMIT @PageSize 
									OFFSET @PageOffset;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConversionFactorToSI", optFilter.ConversionFactorToSI);
				dpParameter.Add("CultureName", optFilter.CultureName);
				dpParameter.Add("ExponentOfAmpere", optFilter.ExponentOfAmpere);
				dpParameter.Add("ExponentOfCandela", optFilter.ExponentOfCandela);
				dpParameter.Add("ExponentOfKelvin", optFilter.ExponentOfKelvin);
				dpParameter.Add("ExponentOfKilogram", optFilter.ExponentOfKilogram);
				dpParameter.Add("ExponentOfMetre", optFilter.ExponentOfMetre);
				dpParameter.Add("ExponentOfMole", optFilter.ExponentOfMole);
				dpParameter.Add("ExponentOfSecond", optFilter.ExponentOfSecond);
				dpParameter.Add("Name", optFilter.Name);
				dpParameter.Add("Symbol", optFilter.Symbol);
				dpParameter.Add("Unit", optFilter.Unit);
				dpParameter.Add("PageOffset", (optFilter.Page + (-1)) * optFilter.PageSize);
				dpParameter.Add("PageSize", optFilter.PageSize);

				IEnumerable<IPhysicalDimensionTransferObject> enumPhysicalDimensionTransferObject = await sqlDataAccess.Connection.QueryAsync<PhysicalDimensionTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumPhysicalDimensionTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<IEnumerable<IPhysicalDimension>>(new RepositoryError()
					{
						Code = PhysicalDimensionError.Code.Method,
						Description = $"No data has been found."
					}));

				IList<IPhysicalDimension> lstPhysicalDimension = new List<IPhysicalDimension>();

				foreach (IPhysicalDimensionTransferObject dtoPhysicalDimension in enumPhysicalDimensionTransferObject)
				{
					IPhysicalDimension? pdPhysicalDimension = PhysicalDimension.Initialize(dtoPhysicalDimension);

					if (pdPhysicalDimension is null)
						return await Task.FromResult(new RepositoryResult<IEnumerable<IPhysicalDimension>>(new RepositoryError()
						{
							Code = PhysicalDimensionError.Code.Method,
							Description = $"Physical dimension has invalid data."
						}));

					lstPhysicalDimension.Add(pdPhysicalDimension);
				}

				return await Task.FromResult(new RepositoryResult<IEnumerable<IPhysicalDimension>>(lstPhysicalDimension.AsEnumerable()));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IEnumerable<IPhysicalDimension>>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<IPhysicalDimension>> FindByIdAsync(Guid guPhysicalDimensionId, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IPhysicalDimension>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									[{PhysicalDimensionColumn.ExponentOfAmpere}], 
									[{PhysicalDimensionColumn.ExponentOfCandela}], 
									[{PhysicalDimensionColumn.ExponentOfKelvin}], 
									[{PhysicalDimensionColumn.ExponentOfKilogram}], 
									[{PhysicalDimensionColumn.ExponentOfMetre}], 
									[{PhysicalDimensionColumn.ExponentOfMole}], 
									[{PhysicalDimensionColumn.ExponentOfSecond}], 
									[{PhysicalDimensionColumn.ConcurrencyStamp}], 
									[{PhysicalDimensionColumn.ConversionFactorToSI}], 
									[{PhysicalDimensionColumn.CultureName}], 
									[{PhysicalDimensionColumn.Id}], 
									[{PhysicalDimensionColumn.Name}], 
									[{PhysicalDimensionColumn.Symbol}], 
									[{PhysicalDimensionColumn.Unit}] 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guPhysicalDimensionId);

				IEnumerable<IPhysicalDimensionTransferObject> enumPhysicalDimensionTransferObject = await sqlDataAccess.Connection.QueryAsync<PhysicalDimensionTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumPhysicalDimensionTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<IPhysicalDimension>(new RepositoryError()
					{
						Code = PhysicalDimensionError.Code.Method,
						Description = $"Physical dimension {guPhysicalDimensionId} has not been found."
					}));

				IPhysicalDimension? pdPhysicalDimension = PhysicalDimension.Initialize(enumPhysicalDimensionTransferObject.First());

				if (pdPhysicalDimension is null)
					return await Task.FromResult(new RepositoryResult<IPhysicalDimension>(new RepositoryError()
					{
						Code = PhysicalDimensionError.Code.Method,
						Description = $"Physical dimension {guPhysicalDimensionId} has invalid data."
					}));

				return await Task.FromResult(new RepositoryResult<IPhysicalDimension>(pdPhysicalDimension));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IPhysicalDimension>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> InsertAsync(IPhysicalDimension pdPhysicalDimension, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"INSERT INTO [{PhysicalDimensionTable.PhysicalDimension}] (
									[{PhysicalDimensionColumn.ConcurrencyStamp}], 
									[{PhysicalDimensionColumn.ConversionFactorToSI}], 
									[{PhysicalDimensionColumn.CultureName}], 
									[{PhysicalDimensionColumn.ExponentOfAmpere}], 
									[{PhysicalDimensionColumn.ExponentOfCandela}], 
									[{PhysicalDimensionColumn.ExponentOfKelvin}], 
									[{PhysicalDimensionColumn.ExponentOfKilogram}], 
									[{PhysicalDimensionColumn.ExponentOfMetre}], 
									[{PhysicalDimensionColumn.ExponentOfMole}], 
									[{PhysicalDimensionColumn.ExponentOfSecond}], 
									[{PhysicalDimensionColumn.Id}], 
									[{PhysicalDimensionColumn.Name}], 
									[{PhysicalDimensionColumn.Symbol}], 
									[{PhysicalDimensionColumn.Unit}]) 
									SELECT 
									@ConcurrencyStamp, 
									@ConversionFactorToSI, 
									@CultureName, 
									@ExponentOfAmpere, 
									@ExponentOfCandela, 
									@ExponentOfKelvin, 
									@ExponentOfKilogram, 
									@ExponentOfMetre, 
									@ExponentOfMole, 
									@ExponentOfSecond, 
									@Id, 
									@Name, 
									@Symbol, 
									@Unit 
									WHERE NOT EXISTS(
									SELECT 1 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE [{PhysicalDimensionColumn.Id}] = @Id);";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", pdPhysicalDimension.ConcurrencyStamp);
				dpParameter.Add("ConversionFactorToSI", pdPhysicalDimension.ConversionFactorToSI);
				dpParameter.Add("CultureName", pdPhysicalDimension.CultureName);
				dpParameter.Add("ExponentOfSecond", pdPhysicalDimension.ExponentOfUnit.Second);
				dpParameter.Add("ExponentOfMetre", pdPhysicalDimension.ExponentOfUnit.Metre);
				dpParameter.Add("ExponentOfKilogram", pdPhysicalDimension.ExponentOfUnit.Kilogram);
				dpParameter.Add("ExponentOfAmpere", pdPhysicalDimension.ExponentOfUnit.Ampere);
				dpParameter.Add("ExponentOfKelvin", pdPhysicalDimension.ExponentOfUnit.Kelvin);
				dpParameter.Add("ExponentOfMole", pdPhysicalDimension.ExponentOfUnit.Mole);
				dpParameter.Add("ExponentOfCandela", pdPhysicalDimension.ExponentOfUnit.Candela);
				dpParameter.Add("Id", pdPhysicalDimension.Id);
				dpParameter.Add("Name", pdPhysicalDimension.Name);
				dpParameter.Add("Symbol", pdPhysicalDimension.Symbol);
				dpParameter.Add("Unit", pdPhysicalDimension.Unit);

				int iResult = (-1);

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PhysicalDimensionError.Code.Method,
						Description = $"Could not create {pdPhysicalDimension.Name}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<int>> QuantityByFilterAsync(IPhysicalDimensionByFilterOption optFilter,CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<int>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									COUNT ([{PhysicalDimensionColumn.Id}]) 
									FROM [{PhysicalDimensionTable.PhysicalDimension}] 
									WHERE (@ConversionFactorToSI IS NULL OR [{PhysicalDimensionColumn.ConversionFactorToSI}] = @ConversionFactorToSI) 
									AND (@CultureName IS NULL OR [{PhysicalDimensionColumn.CultureName}] LIKE('%'|| @CultureName ||'%')) 
									AND (@ExponentOfAmpere IS NULL OR [{PhysicalDimensionColumn.ExponentOfAmpere}] = @ExponentOfAmpere) 
									AND (@ExponentOfCandela IS NULL OR [{PhysicalDimensionColumn.ExponentOfCandela}] = @ExponentOfCandela) 
									AND (@ExponentOfKelvin IS NULL OR [{PhysicalDimensionColumn.ExponentOfKelvin}] = @ExponentOfKelvin) 
									AND (@ExponentOfKilogram IS NULL OR [{PhysicalDimensionColumn.ExponentOfKilogram}] = @ExponentOfKilogram) 
									AND (@ExponentOfMetre IS NULL OR [{PhysicalDimensionColumn.ExponentOfMetre}] = @ExponentOfMetre) 
									AND (@ExponentOfMole IS NULL OR [{PhysicalDimensionColumn.ExponentOfMole}] = @ExponentOfMole) 
									AND (@ExponentOfSecond IS NULL OR [{PhysicalDimensionColumn.ExponentOfSecond}] = @ExponentOfSecond) 
									AND (@Name IS NULL OR [{PhysicalDimensionColumn.Name}] LIKE('%'|| @Name ||'%')) 
									AND (@Symbol IS NULL OR [{PhysicalDimensionColumn.Symbol}] LIKE('%'|| @Symbol ||'%')) 
									AND (@Unit IS NULL OR [{PhysicalDimensionColumn.Unit}] LIKE('%'|| @Unit ||'%'));";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConversionFactorToSI", optFilter.ConversionFactorToSI);
				dpParameter.Add("CultureName", optFilter.CultureName);
				dpParameter.Add("ExponentOfAmpere", optFilter.ExponentOfAmpere);
				dpParameter.Add("ExponentOfCandela", optFilter.ExponentOfCandela);
				dpParameter.Add("ExponentOfKelvin", optFilter.ExponentOfKelvin);
				dpParameter.Add("ExponentOfKilogram", optFilter.ExponentOfKilogram);
				dpParameter.Add("ExponentOfMetre", optFilter.ExponentOfMetre);
				dpParameter.Add("ExponentOfMole", optFilter.ExponentOfMole);
				dpParameter.Add("ExponentOfSecond", optFilter.ExponentOfSecond);
				dpParameter.Add("Name", optFilter.Name);
				dpParameter.Add("Symbol", optFilter.Symbol);
				dpParameter.Add("Unit", optFilter.Unit);

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
					new RepositoryResult<int>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> UpdateAsync(IPhysicalDimension pdPhysicalDimension, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"UPDATE [{PhysicalDimensionTable.PhysicalDimension}] SET 
									[{PhysicalDimensionColumn.ConcurrencyStamp}] = @ConcurrencyStamp, 
									[{PhysicalDimensionColumn.ConversionFactorToSI}] = @ConversionFactorToSI, 
									[{PhysicalDimensionColumn.CultureName}] = @CultureName, 
									[{PhysicalDimensionColumn.ExponentOfAmpere}] = @ExponentOfAmpere,
									[{PhysicalDimensionColumn.ExponentOfCandela}] = @ExponentOfCandela, 
									[{PhysicalDimensionColumn.ExponentOfKelvin}] = @ExponentOfKelvin,
									[{PhysicalDimensionColumn.ExponentOfKilogram}] = @ExponentOfKilogram, 
									[{PhysicalDimensionColumn.ExponentOfMetre}] = @ExponentOfMetre, 
									[{PhysicalDimensionColumn.ExponentOfMole}] = @ExponentOfMole, 
									[{PhysicalDimensionColumn.ExponentOfSecond}] = @ExponentOfSecond, 
									[{PhysicalDimensionColumn.Name}] = @Name, 
									[{PhysicalDimensionColumn.Symbol}] = @Symbol, 
									[{PhysicalDimensionColumn.Unit}] = @Unit 
									WHERE [{PhysicalDimensionColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{PhysicalDimensionColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualStamp", pdPhysicalDimension.ConcurrencyStamp);
				dpParameter.Add("ConcurrencyStamp", Guid.NewGuid());
				dpParameter.Add("ConversionFactorToSI", pdPhysicalDimension.ConversionFactorToSI);
				dpParameter.Add("CultureName", pdPhysicalDimension.CultureName);
				dpParameter.Add("ExponentOfSecond", pdPhysicalDimension.ExponentOfUnit.Second);
				dpParameter.Add("ExponentOfMetre", pdPhysicalDimension.ExponentOfUnit.Metre);
				dpParameter.Add("ExponentOfKilogram", pdPhysicalDimension.ExponentOfUnit.Kilogram);
				dpParameter.Add("ExponentOfAmpere", pdPhysicalDimension.ExponentOfUnit.Ampere);
				dpParameter.Add("ExponentOfKelvin", pdPhysicalDimension.ExponentOfUnit.Kelvin);
				dpParameter.Add("ExponentOfMole", pdPhysicalDimension.ExponentOfUnit.Mole);
				dpParameter.Add("ExponentOfCandela", pdPhysicalDimension.ExponentOfUnit.Candela);
				dpParameter.Add("Id", pdPhysicalDimension.Id);
				dpParameter.Add("Name", pdPhysicalDimension.Name);
				dpParameter.Add("Symbol", pdPhysicalDimension.Symbol);
				dpParameter.Add("Unit", pdPhysicalDimension.Unit);

				int iResult = (-1);

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PhysicalDimensionError.Code.Method,
						Description = $"Could not update {pdPhysicalDimension.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PhysicalDimensionError.Code.Exception, Description = exException.Message }));
			}
		}
	}
}
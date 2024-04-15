using Application.Common.Result.Repository;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Dapper;
using Domain.Aggregate.Authorization.PassportVisa;
using Domain.Interface.Authorization;
using Domain.Interface.Transfer;
using Infrastructure.Error;
using Infrastructure.Extension.Passport;
using Infrastructure.Transfer.Authorization;
using Infrastructure.TypeHandler;

namespace Infrastructure.Persistance.Authorization
{
    internal sealed class PassportVisaRepository : IPassportVisaRepository
	{
		private readonly ISqliteDataAccess sqlDataAccess;

		public PassportVisaRepository(IPassportDataAccess sqlDataAccess)
		{
			this.sqlDataAccess = sqlDataAccess;

			SqlMapper.AddTypeHandler(new GuidTypeHandler());
			SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> DeleteAsync(
			IPassportVisa ppVisa,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"DELETE FROM [{PassportVisaTable.PassportVisa}] 
									WHERE [{PassportVisaColumn.ConcurrencyStamp}] = @ConcurrencyStamp 
									AND [{PassportVisaColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", ppVisa.ConcurrencyStamp);
				dpParameter.Add("Id", ppVisa.Id);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not delete visa {ppVisa.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> ExistsAsync(
			Guid guVisaId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PassportVisaTable.PassportVisa}] 
									WHERE [{PassportVisaColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guVisaId);

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
		public async Task<RepositoryResult<bool>> ExistsAsync(
			IEnumerable<Guid> enumVisaId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT COUNT(*) 
									FROM [{PassportVisaTable.PassportVisa}] 
									WHERE [{PassportVisaColumn.Id}] IN @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", enumVisaId);

				int iResult = 0;

				iResult = await sqlDataAccess.Connection.ExecuteScalarAsync<int>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				return await Task.FromResult(new RepositoryResult<bool>(iResult == enumVisaId.Count()));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> ByNameAtLevelExistsAsync(
			string sName,
			int iLevel,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PassportVisaTable.PassportVisa}] 
									WHERE [{PassportVisaColumn.Name}] = @Name 
									AND [{PassportVisaColumn.Level}] = @Level) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Name", sName);
				dpParameter.Add("Level", iLevel);

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
		public async Task<RepositoryResult<IPassportVisa>> FindByIdAsync(
			Guid guVisaId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IPassportVisa>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									[{PassportVisaColumn.ConcurrencyStamp}],
									[{PassportVisaColumn.Id}],
									[{PassportVisaColumn.Name}],
									[{PassportVisaColumn.Level}] 
									FROM [{PassportVisaTable.PassportVisa}] 
									WHERE [{PassportVisaColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guVisaId);

				IEnumerable<IPassportVisaTransferObject> enumPassportTransferObject = await sqlDataAccess.Connection.QueryAsync<PassportVisaTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumPassportTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<IPassportVisa>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"No data for {guVisaId} has been found."
					}));

				IPassportVisa? ppVisa = PassportVisa.Initialize(enumPassportTransferObject.First());

				if (ppVisa is null)
					return await Task.FromResult(new RepositoryResult<IPassportVisa>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Visa {guVisaId} has invalid data."
					}));

				return await Task.FromResult(new RepositoryResult<IPassportVisa>(ppVisa));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IPassportVisa>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<IPassportVisa>> FindByNameAsync(
			string sName,
			int iLevel,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IPassportVisa>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									[{PassportVisaColumn.ConcurrencyStamp}],
									[{PassportVisaColumn.Id}],
									[{PassportVisaColumn.Name}],
									[{PassportVisaColumn.Level}] 
									FROM [{PassportVisaTable.PassportVisa}] 
									WHERE [{PassportVisaColumn.Name}] = @Name  
									AND [{PassportVisaColumn.Level}] = @Level;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Name", sName);
				dpParameter.Add("Level", iLevel);

				IEnumerable<IPassportVisaTransferObject> enumPassportTransferObject = await sqlDataAccess.Connection.QueryAsync<PassportVisaTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumPassportTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<IPassportVisa>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"No data for visa of name {sName} at level {iLevel} has been found."
					}));

				IPassportVisa? ppVisa = PassportVisa.Initialize(enumPassportTransferObject.First());

				if (ppVisa is null)
					return await Task.FromResult(new RepositoryResult<IPassportVisa>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Visa of name {sName} at level {iLevel} has invalid data."
					}));

				return await Task.FromResult(new RepositoryResult<IPassportVisa>(ppVisa));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IPassportVisa>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<IEnumerable<IPassportVisa>>> FindByPassportAsync(
			Guid guPassportId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IEnumerable<IPassportVisa>>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									A.[{PassportVisaColumn.ConcurrencyStamp}],
									A.[{PassportVisaColumn.Id}],
									A.[{PassportVisaColumn.Name}],
									A.[{PassportVisaColumn.Level}] 
									FROM [{PassportVisaTable.PassportVisa}] AS A 
									INNER JOIN [{PassportVisaRegisterTable.PassportVisaRegister}] 
									ON [{PassportVisaRegisterTable.PassportVisaRegister}].[{PassportVisaRegisterColumn.PassportVisaId}] = A.[{PassportVisaColumn.Id}] 
									INNER JOIN [{PassportTable.Passport}] AS B 
									ON [{PassportVisaRegisterTable.PassportVisaRegister}].[{PassportVisaRegisterColumn.PassportId}] = B.[{PassportColumn.Id}] 
									WHERE B.[{PassportColumn.Id}] = @PassportId;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("PassportId", guPassportId);

				IEnumerable<IPassportVisaTransferObject> enumPassportTransferObject = await sqlDataAccess.Connection.QueryAsync<PassportVisaTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				IList<IPassportVisa> lstPassportVisa = new List<IPassportVisa>();

				foreach(IPassportVisaTransferObject dtoPassportVisa in enumPassportTransferObject)
				{
					IPassportVisa? ppVisa = PassportVisa.Initialize(dtoPassportVisa);

					if (ppVisa is null)
						return new RepositoryResult<IEnumerable<IPassportVisa>>(new RepositoryError()
						{
							Code = PassportError.Code.Method,
							Description = $"Visa of name {dtoPassportVisa.Name} at level {dtoPassportVisa.Level} has invalid data."
						});

					lstPassportVisa.Add(ppVisa);
				}

				return await Task.FromResult(new RepositoryResult<IEnumerable<IPassportVisa>>(lstPassportVisa.AsEnumerable()));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IEnumerable<IPassportVisa>>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> InsertAsync(
			IPassportVisa ppVisa,
			DateTimeOffset dtCreatedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				IPassportVisaTransferObject dtoVisa = ppVisa.WriteTo<PassportVisaTransferObject>();

				string sStatement = @$"INSERT INTO [{PassportVisaTable.PassportVisa}](
									[{PassportVisaColumn.ConcurrencyStamp}], 
									[{PassportVisaColumn.CreatedAt}], 
									[{PassportVisaColumn.EditedAt}], 
									[{PassportVisaColumn.Id}], 
									[{PassportVisaColumn.Name}], 
									[{PassportVisaColumn.Level}]) 
									SELECT 
									@ConcurrencyStamp, 
									@CreatedAt, 
									@EditedAt, 
									@Id, 
									@Name, 
									@Level 
									WHERE NOT EXISTS (
									SELECT 1
									FROM [{PassportVisaTable.PassportVisa}] 
									WHERE (
									[{PassportVisaColumn.Id}] = @Id
									) OR (
									[{PassportVisaColumn.Name}] = @Name 
									AND [{PassportVisaColumn.Level}] = @Level));";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", dtoVisa.ConcurrencyStamp);
				dpParameter.Add("CreatedAt", dtCreatedAt);
				dpParameter.Add("EditedAt", dtCreatedAt);
				dpParameter.Add("Id", dtoVisa.Id);
				dpParameter.Add("Name", dtoVisa.Name);
				dpParameter.Add("Level", dtoVisa.Level);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
				sql: sStatement,
				param: dpParameter,
				transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not create visa {ppVisa.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> UpdateAsync(
			IPassportVisa ppVisa,
			DateTimeOffset dtEditedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				IPassportVisaTransferObject dtoVisa = ppVisa.WriteTo<PassportVisaTransferObject>();

				string sStatement = @$"UPDATE [{PassportVisaTable.PassportVisa}] SET 
									[{PassportVisaColumn.ConcurrencyStamp}] = @ConcurrencyStamp, 
									[{PassportVisaColumn.EditedAt}] = @EditedAt, 
									[{PassportVisaColumn.Name}] = @Name, 
									[{PassportVisaColumn.Level}] = @Level 
									WHERE [{PassportVisaColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{PassportVisaColumn.Id}] = @Id";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualStamp", dtoVisa.ConcurrencyStamp);
				dpParameter.Add("ConcurrencyStamp", Guid.NewGuid());
				dpParameter.Add("EditedAt", dtEditedAt);
				dpParameter.Add("Id", dtoVisa.Id);
				dpParameter.Add("Name", dtoVisa.Name);
				dpParameter.Add("Level", dtoVisa.Level);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not update visa {ppVisa.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}
	}
}
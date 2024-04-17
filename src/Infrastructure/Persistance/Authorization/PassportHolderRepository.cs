using Application.Common.Result.Repository;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Dapper;
using Domain.Aggregate.Authorization.PassportHolder;
using Domain.Interface.Authorization;
using Domain.Interface.Transfer;
using Infrastructure.Error;
using Infrastructure.Extension.Passport;
using Infrastructure.Transfer.Authorization;
using Infrastructure.TypeHandler;

namespace Infrastructure.Persistance.Authorization
{
    internal sealed class PassportHolderRepository : IPassportHolderRepository
	{
		private readonly IDataAccess sqlDataAccess;
		private readonly IPassportSetting ppSetting;

		public PassportHolderRepository(IPassportDataAccess sqlDataAccess, IPassportSetting ppSetting)
		{
			this.sqlDataAccess = sqlDataAccess;
			this.ppSetting = ppSetting;

			SqlMapper.AddTypeHandler(new GuidTypeHandler());
			SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> DeleteAsync(
			IPassportHolder ppHolder,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"DELETE FROM [{PassportHolderTable.PassportHolder}] 
									WHERE [{PassportHolderColumn.ConcurrencyStamp}] = @ConcurrencyStamp 
									AND [{PassportHolderColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", ppHolder.ConcurrencyStamp);
				dpParameter.Add("Id", ppHolder.Id);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not delete holder {ppHolder.Id}."
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
			Guid guHolderId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PassportHolderTable.PassportHolder}] 
									WHERE [{PassportHolderColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guHolderId);

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
		public async Task<RepositoryResult<IPassportHolder>> FindByIdAsync(
			Guid guHolderId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IPassportHolder>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT 
									[{PassportHolderColumn.ConcurrencyStamp}], 
									[{PassportHolderColumn.CultureName}], 
									[{PassportHolderColumn.EmailAddress}], 
									[{PassportHolderColumn.EmailAddressIsConfirmed}], 
									[{PassportHolderColumn.FirstName}], 
									[{PassportHolderColumn.Id}], 
									[{PassportHolderColumn.LastName}], 
									[{PassportHolderColumn.PhoneNumber}], 
									[{PassportHolderColumn.PhoneNumberIsConfirmed}], 
									[{PassportHolderColumn.SecurityStamp}] 
									FROM [{PassportHolderTable.PassportHolder}] 
									WHERE [{PassportHolderColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guHolderId);

				IEnumerable<IPassportHolderTransferObject> enumPassportHolderTransferObject = await sqlDataAccess.Connection.QueryAsync<PassportHolderTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumPassportHolderTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<IPassportHolder>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"No data for {guHolderId} has been found."
					}));

				IPassportHolder? ppHolder = PassportHolder.Initialize(enumPassportHolderTransferObject.First(), ppSetting);

				if (ppHolder is null)
					return await Task.FromResult(new RepositoryResult<IPassportHolder>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Holder {guHolderId} has invalid data."
					}));

				return await Task.FromResult(new RepositoryResult<IPassportHolder>(ppHolder));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IPassportHolder>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> InsertAsync(
			IPassportHolder ppHolder,
			DateTimeOffset dtCreatedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				IPassportHolderTransferObject dtoHolder = ppHolder.WriteTo<PassportHolderTransferObject>();

				string sStatement = @$"INSERT INTO [{PassportHolderTable.PassportHolder}](
									[{PassportHolderColumn.ConcurrencyStamp}], 
									[{PassportHolderColumn.CreatedAt}], 
									[{PassportHolderColumn.CultureName}], 
									[{PassportHolderColumn.EditedAt}], 
									[{PassportHolderColumn.EmailAddress}], 
									[{PassportHolderColumn.EmailAddressIsConfirmed}], 
									[{PassportHolderColumn.FirstName}], 
									[{PassportHolderColumn.Id}], 
									[{PassportHolderColumn.LastName}], 
									[{PassportHolderColumn.PhoneNumber}], 
									[{PassportHolderColumn.PhoneNumberIsConfirmed}], 
									[{PassportHolderColumn.SecurityStamp}]) 
									SELECT 
									@ConcurrencyStamp, 
									@CreatedAt, 
									@CultureName, 
									@EditedAt, 
									@EmailAddress, 
									@EmailAddressIsConfirmed, 
									@FirstName, 
									@Id, 
									@LastName, 
									@PhoneNumber, 
									@PhoneNumberIsConfirmed, 
									@SecurityStamp 
									WHERE NOT EXISTS (
									SELECT 1 
									FROM [{PassportHolderTable.PassportHolder}] 
									WHERE [{PassportHolderColumn.EmailAddress}] = @EmailAddress 
									OR [{PassportHolderColumn.Id}] = @Id);";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", dtoHolder.ConcurrencyStamp);
				dpParameter.Add("CreatedAt", dtCreatedAt);
				dpParameter.Add("CultureName", dtoHolder.CultureName);
				dpParameter.Add("EditedAt", dtCreatedAt);
				dpParameter.Add("EmailAddress", dtoHolder.EmailAddress);
				dpParameter.Add("EmailAddressIsConfirmed", dtoHolder.EmailAddressIsConfirmed);
				dpParameter.Add("FirstName", dtoHolder.FirstName);
				dpParameter.Add("Id", dtoHolder.Id);
				dpParameter.Add("LastName", dtoHolder.LastName);
				dpParameter.Add("PhoneNumber", dtoHolder.PhoneNumber);
				dpParameter.Add("PhoneNumberIsConfirmed", dtoHolder.PhoneNumberIsConfirmed);
				dpParameter.Add("SecurityStamp", dtoHolder.SecurityStamp);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not create holder {ppHolder.Id}."
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
			IPassportHolder ppHolder,
			DateTimeOffset dtEditedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				IPassportHolderTransferObject dtoHolder = ppHolder.WriteTo<PassportHolderTransferObject>();

				string sStatement = @$"UPDATE [{PassportHolderTable.PassportHolder}] SET 
									[{PassportHolderColumn.ConcurrencyStamp}] = @ConcurrencyStamp, 
									[{PassportHolderColumn.CultureName}] = @CultureName, 
									[{PassportHolderColumn.EditedAt}] = @EditedAt, 
									[{PassportHolderColumn.EmailAddress}] = @EmailAddress, 
									[{PassportHolderColumn.EmailAddressIsConfirmed}] = @EmailAddressIsConfirmed, 
									[{PassportHolderColumn.FirstName}] = @FirstName, 
									[{PassportHolderColumn.Id}] = @Id, 
									[{PassportHolderColumn.LastName}] = @LastName, 
									[{PassportHolderColumn.PhoneNumber}] = @PhoneNumber, 
									[{PassportHolderColumn.PhoneNumberIsConfirmed}] = @PhoneNumberIsConfirmed, 
									[{PassportHolderColumn.SecurityStamp}] = @SecurityStamp 
									WHERE [{PassportHolderColumn.ConcurrencyStamp}] = @ActualConcurrencyStamp 
									AND [{PassportHolderColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualConcurrencyStamp", dtoHolder.ConcurrencyStamp);
				dpParameter.Add("ConcurrencyStamp", Guid.NewGuid());
				dpParameter.Add("CultureName", dtoHolder.CultureName);
				dpParameter.Add("EditedAt", dtEditedAt);
				dpParameter.Add("EmailAddress", dtoHolder.EmailAddress);
				dpParameter.Add("EmailAddressIsConfirmed", dtoHolder.EmailAddressIsConfirmed);
				dpParameter.Add("FirstName", dtoHolder.FirstName);
				dpParameter.Add("Id", dtoHolder.Id);
				dpParameter.Add("LastName", dtoHolder.LastName);
				dpParameter.Add("PhoneNumber", dtoHolder.PhoneNumber);
				dpParameter.Add("PhoneNumberIsConfirmed", dtoHolder.PhoneNumberIsConfirmed);
				dpParameter.Add("SecurityStamp", dtoHolder.SecurityStamp);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not update holder {ppHolder.Id}."
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

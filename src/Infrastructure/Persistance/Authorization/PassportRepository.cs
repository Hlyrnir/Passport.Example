using Application.Common.Result.Repository;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Dapper;
using Domain.Interface.Authorization;
using Domain.Interface.Transfer;
using Infrastructure.Error;
using Infrastructure.Extension.Passport;
using Infrastructure.Transfer.Authorization;
using Infrastructure.TypeHandler;
using Microsoft.IdentityModel.Tokens;
using Passport = Domain.Aggregate.Authorization.Passport.Passport;

namespace Infrastructure.Persistance.Authorization
{
    internal sealed class PassportRepository : IPassportRepository
	{
		private readonly IDataAccess sqlDataAccess;

		public PassportRepository(IPassportDataAccess sqlDataAccess)
		{
			this.sqlDataAccess = sqlDataAccess;

			SqlMapper.AddTypeHandler(new GuidTypeHandler());
			SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
		}

		public async Task<RepositoryResult<bool>> DeleteAsync(
			IPassport ppPassport,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"DELETE FROM [{PassportTable.Passport}] 
									WHERE [{PassportColumn.ConcurrencyStamp}] = @ConcurrencyStamp 
									AND [{PassportColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", ppPassport.ConcurrencyStamp);
				dpParameter.Add("Id", ppPassport.Id);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not delete passport {ppPassport.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		public async Task<RepositoryResult<bool>> ExistsAsync(
			Guid guPassportId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PassportTable.Passport}] 
									WHERE [{PassportColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guPassportId);

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

		public async Task<RepositoryResult<IPassport>> FindByIdAsync(
			Guid guPassportId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<IPassport>(DefaultRepositoryError.TaskAborted));

			try
			{
				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guPassportId);

				string sPassportVisaIdStatement =
					@$"SELECT 
					[{PassportVisaRegisterColumn.PassportVisaId}] 
					FROM 
					[{PassportVisaRegisterTable.PassportVisaRegister}] 
					WHERE [{PassportVisaRegisterColumn.PassportId}] = @Id;";

				IEnumerable<Guid> enumPassportVisaId = await sqlDataAccess.Connection.QueryAsync<Guid>(
					sql: sPassportVisaIdStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				string sPassportStatement = @$"SELECT 
					[{PassportColumn.ConcurrencyStamp}], 
					[{PassportColumn.ExpiredAt}], 
					[{PassportColumn.HolderId}], 
					[{PassportColumn.Id}], 
					[{PassportColumn.IsAuthority}], 
					[{PassportColumn.IsEnabled}], 
					[{PassportColumn.IssuedBy}], 
					[{PassportColumn.LastCheckedAt}], 
					[{PassportColumn.LastCheckedBy}] 
					FROM [{PassportTable.Passport}] 
					WHERE [{PassportColumn.Id}] = @Id;";

				IEnumerable<IPassportTransferObject> enumPassportTransferObject = await sqlDataAccess.Connection.QueryAsync<PassportTransferObject>(
					sql: sPassportStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumPassportTransferObject.Count() == 0)
					return await Task.FromResult(new RepositoryResult<IPassport>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"No data for {guPassportId} has been found."
					}));

				IPassport? ppPassport = Passport.Initialize(enumPassportTransferObject.First(), enumPassportVisaId);

				if (ppPassport is null)
					return await Task.FromResult(new RepositoryResult<IPassport>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Passport {guPassportId} has invalid data."
					}));

				return await Task.FromResult(new RepositoryResult<IPassport>(ppPassport));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IPassport>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		//public async Task<RepositoryResult<IPassport>> FindByFilterAsync(
		//	Guid guPassportId,
		//	CancellationToken tknCancellation)
		//{
		//	if (tknCancellation.IsCancellationRequested)
		//		return await Task.FromResult(new RepositoryResult<IPassport>(DefaultRepositoryError.TaskAborted));

		//	try
		//	{
		//		string sStatement = @"SELECT 
		//							A.[ConcurrencyStamp], 
		//							A.[ExpiredAt], 
		//							A.[HolderId], 
		//							A.[Id], 
		//							A.[IssuedBy], 
		//							A.[LastCheckedAt], 
		//							A.[LastCheckedBy], 
		//							B.[ConcurrencyStamp], 
		//							B.[Id], 
		//							B.[Name], 
		//							B.[Level] 
		//							FROM [Passport] AS A 
		//							LEFT JOIN [PassportVisaRegister] ON [PassportVisaRegister].[PassportId] = A.[Id] 
		//							LEFT JOIN [PassportVisa] AS B ON [PassportVisaRegister].[PassportVisaId] = B.[Id] 
		//							WHERE A.[HolderId] = @HolderId;";

		//		DynamicParameters dpParameter = new DynamicParameters();
		//		dpParameter.Add("HolderId", ppHolder.Id);

		//		IEnumerable<IPassport> ppPassport = await sqlDataAccess.Connection.QueryAsync<
		//			Domain.Aggregate.Authorization.Passport.Passport,
		//			PassportVisa,
		//			Domain.Aggregate.Authorization.Passport.Passport>(
		//			sql: sStatement,
		//			map: (Passport, Visa) =>
		//			{
		//				Passport.TryAddVisa(Visa);

		//				return Passport;
		//			},
		//			param: dpParameter,
		//			transaction: sqlDataAccess.Transaction,
		//			splitOn: "ConcurrencyStamp");

		//		if (ppPassport.Count() < 1)
		//			return await Task.FromResult(new RepositoryResult<IPassport>(new RepositoryError()
		//			{
		//				Code = PassportError.Code.Method,
		//				Description = $"Holder {ppHolder.Id} has no passport registered."
		//			}));

		//		if (ppPassport.Count() > 1)
		//			return await Task.FromResult(new RepositoryResult<IPassport>(new RepositoryError()
		//			{
		//				Code = PassportError.Code.Method,
		//				Description = $"Could only find ambiguous results for holder {ppHolder.Id}."
		//			}));

		//		return await Task.FromResult(new RepositoryResult<IPassport>(ppPassport.First()));
		//	}
		//	catch (Exception exException)
		//	{
		//		return await Task.FromResult(
		//			new RepositoryResult<IPassport>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
		//	}
		//}

		public async Task<RepositoryResult<bool>> InsertAsync(
			IPassport ppPassport,
			DateTimeOffset dtCreatedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				IPassportTransferObject dtoPassport = ppPassport.WriteTo<PassportTransferObject>();

				string sStatement = @$"INSERT INTO [{PassportTable.Passport}](
									[{PassportColumn.ConcurrencyStamp}], 
									[{PassportColumn.CreatedAt}], 
									[{PassportColumn.EditedAt}], 
									[{PassportColumn.ExpiredAt}], 
									[{PassportColumn.HolderId}], 
									[{PassportColumn.Id}], 
									[{PassportColumn.IsAuthority}], 
									[{PassportColumn.IsEnabled}], 
									[{PassportColumn.IssuedBy}],
									[{PassportColumn.LastCheckedAt}], 
									[{PassportColumn.LastCheckedBy}]) 
									SELECT 
									@ConcurrencyStamp, 
									@CreatedAt, 
									@EditedAt, 
									@ExpiredAt, 
									@HolderId, 
									@Id, 
									@IsAuthority, 
									@IsEnabled, 
									@IssuedBy, 
									@LastCheckedAt, 
									@LastCheckedBy 
									WHERE NOT EXISTS (
									SELECT 1 
									FROM [{PassportTable.Passport}] 
									WHERE [{PassportColumn.Id}] = @Id);";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ConcurrencyStamp", dtoPassport.ConcurrencyStamp);
				dpParameter.Add("CreatedAt", dtCreatedAt);
				dpParameter.Add("EditedAt", dtCreatedAt);
				dpParameter.Add("ExpiredAt", dtoPassport.ExpiredAt);
				dpParameter.Add("HolderId", dtoPassport.HolderId);
				dpParameter.Add("Id", dtoPassport.Id);
				dpParameter.Add("IsAuthority", dtoPassport.IsAuthority);
				dpParameter.Add("IsEnabled", dtoPassport.IsEnabled);
				dpParameter.Add("IssuedBy", dtoPassport.IssuedBy);
				dpParameter.Add("LastCheckedAt", dtoPassport.LastCheckedAt);
				dpParameter.Add("LastCheckedBy", dtoPassport.LastCheckedBy);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not create passport {ppPassport.Id}."
					}));

				if (ppPassport.VisaId.IsNullOrEmpty() == true)
					return await Task.FromResult(new RepositoryResult<bool>(true));

				int iVisaIdResult = await RegisterPassportVisaAsync(ppPassport, dtCreatedAt, tknCancellation);

				if (iVisaIdResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not register visa to passport {ppPassport.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		public async Task<RepositoryResult<bool>> UpdateAsync(
			IPassport ppPassport,
			DateTimeOffset dtEditedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				IPassportTransferObject dtoPassport = ppPassport.WriteTo<PassportTransferObject>();

				string sStatement = @$"UPDATE [{PassportTable.Passport}] SET 
									[{PassportColumn.ConcurrencyStamp}] = @ConcurrencyStamp, 
									[{PassportColumn.EditedAt}] = @EditedAt, 
									[{PassportColumn.ExpiredAt}] = @ExpiredAt, 
									[{PassportColumn.HolderId}] = @HolderId, 
									[{PassportColumn.Id}] = @Id, 
									[{PassportColumn.IsAuthority}] = @IsAuthority, 
									[{PassportColumn.IsEnabled}] = @IsEnabled, 
									[{PassportColumn.IssuedBy}] = @IssuedBy, 
									[{PassportColumn.LastCheckedAt}] = @LastCheckedAt, 
									[{PassportColumn.LastCheckedBy}] = @LastCheckedBy 
									WHERE [{PassportColumn.ConcurrencyStamp}] = @ActualStamp 
									AND [{PassportColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualStamp", dtoPassport.ConcurrencyStamp);
				dpParameter.Add("ConcurrencyStamp", Guid.NewGuid());
				dpParameter.Add("EditedAt", dtEditedAt);
				dpParameter.Add("ExpiredAt", dtoPassport.ExpiredAt);
				dpParameter.Add("HolderId", dtoPassport.HolderId);
				dpParameter.Add("Id", dtoPassport.Id);
				dpParameter.Add("IsAuthority", dtoPassport.IsAuthority);
				dpParameter.Add("IsEnabled", dtoPassport.IsEnabled);
				dpParameter.Add("IssuedBy", dtoPassport.IssuedBy);
				dpParameter.Add("LastCheckedAt", dtoPassport.LastCheckedAt);
				dpParameter.Add("LastCheckedBy", dtoPassport.LastCheckedBy);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(
							new RepositoryResult<bool>(new RepositoryError()
							{
								Code = PassportError.Code.Method,
								Description = $"Could not update passport {ppPassport.Id}."
							}));

				if (ppPassport.VisaId.IsNullOrEmpty() == true)
					return await Task.FromResult(new RepositoryResult<bool>(true));

				int iVisaIdResult = await RegisterPassportVisaAsync(ppPassport, dtEditedAt, tknCancellation);

				if (iVisaIdResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not register visa to passport {ppPassport.Id}."
					}));

				return await Task.FromResult(new RepositoryResult<bool>(true));
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<bool>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		private async Task<int> RegisterPassportVisaAsync(
			IPassport ppPassport, 
			DateTimeOffset dtRegisteredAt, 
			CancellationToken tknCancellation)
		{
			string sDeleteStatement = @$"DELETE FROM [{PassportVisaRegisterTable.PassportVisaRegister}] 
									WHERE [{PassportVisaRegisterColumn.PassportId}] = @Id 
									AND [{PassportVisaRegisterColumn.PassportVisaId}] NOT IN @VisaId;";

			DynamicParameters dpParameter = new DynamicParameters();
			dpParameter.Add("Id", ppPassport.Id);
			dpParameter.Add("VisaId", ppPassport.VisaId);

			await sqlDataAccess.Connection.ExecuteAsync(
				sql: sDeleteStatement,
				param: dpParameter,
				transaction: sqlDataAccess.Transaction);

			string sInsertStatement = @$"INSERT INTO [{PassportVisaRegisterTable.PassportVisaRegister}](
									[{PassportVisaRegisterColumn.Id}], 
									[{PassportVisaRegisterColumn.PassportId}], 
									[{PassportVisaRegisterColumn.PassportVisaId}], 
									[{PassportVisaRegisterColumn.RegisteredAt}]) 
									SELECT 
									@Id, 
									@PassportId, 
									@PassportVisaId, 
									@RegisteredAt 
									WHERE NOT EXISTS(
									SELECT 1 
									FROM [{PassportVisaRegisterTable.PassportVisaRegister}] 
									WHERE [{PassportVisaRegisterColumn.PassportId}] = @PassportId 
									AND [{PassportVisaRegisterColumn.PassportVisaId}] = @PassportVisaId)
									AND EXISTS( 
									SELECT 1 
									FROM [{PassportTable.Passport}] 
									WHERE [{PassportColumn.Id}] = @PassportId) 
									AND EXISTS( 
									SELECT 1 
									FROM[{PassportVisaTable.PassportVisa}] 
									WHERE [{PassportVisaColumn.Id}] = @PassportVisaId);";

			IList<DynamicParameters> lstParameter = new List<DynamicParameters>();

			foreach(Guid guVisaId in ppPassport.VisaId)
			{
				DynamicParameters dpParameterToList = new DynamicParameters();
				dpParameterToList.Add("Id", Guid.NewGuid());
				dpParameterToList.Add("PassportId", ppPassport.Id);
				dpParameterToList.Add("PassportVisaId", guVisaId);
				dpParameterToList.Add("RegisteredAt", dtRegisteredAt);

				lstParameter.Add(dpParameterToList);
			}

			return await sqlDataAccess.Connection.ExecuteAsync(
				sql: sInsertStatement,
				param: lstParameter,
				transaction: sqlDataAccess.Transaction);
		}
	}
}
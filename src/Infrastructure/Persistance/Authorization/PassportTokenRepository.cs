using Application.Common.Result.Repository;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Dapper;
using Domain.Aggregate.Authorization.PassportToken;
using Domain.Interface.Authorization;
using Domain.Interface.Transfer;
using Infrastructure.Error;
using Infrastructure.Extension.Passport;
using Infrastructure.Transfer.Authorization;
using Infrastructure.TypeHandler;

namespace Infrastructure.Persistance.Authorization
{
	internal sealed class PassportTokenRepository : IPassportTokenRepository
	{
		private readonly ISqliteDataAccess sqlDataAccess;

		private readonly IPassportSetting ppSetting;
		private readonly IPassportHasher ppHasher;

		public PassportTokenRepository(IPassportDataAccess sqlDataAccess, IPassportSetting ppSetting, IPassportHasher ppHasher)
		{
			this.sqlDataAccess = sqlDataAccess;

			this.ppSetting = ppSetting;
			this.ppHasher = ppHasher;

			SqlMapper.AddTypeHandler(new GuidTypeHandler());
			SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> CredentialAtProviderExistsAsync(
			string sCredential,
			string sProvider,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PassportTokenTable.PassportToken}] 
									WHERE [{PassportTokenColumn.Credential}] = @Credential 
									AND [{PassportTokenColumn.Provider}] = @Provider) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Credential", sCredential);
				dpParameter.Add("Provider", sProvider);

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
		public async Task<RepositoryResult<bool>> DeleteAsync(
			IPassportToken ppToken,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"DELETE FROM [{PassportTokenTable.PassportToken}] 
									WHERE [{PassportTokenColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", ppToken.Id);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not delete token of passport {ppToken.PassportId} at {ppToken.Provider}."
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
		public async Task<RepositoryResult<bool>> EnableTwoFactorAuthenticationAsync(
			IPassportToken ppToken, 
			bool bIsEnabled,
			DateTimeOffset dtEditedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"UPDATE [{PassportTokenTable.PassportToken}] SET 
									[{PassportTokenColumn.EditedAt}] = @EditedAt, 
									[{PassportTokenColumn.TwoFactorIsEnabled}] = @IsEnabled, 
									[{PassportTokenColumn.RefreshToken}] = @RefreshToken 
									WHERE [{PassportTokenColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("EditedAt", dtEditedAt);
				dpParameter.Add("IsEnabled", bIsEnabled);
				dpParameter.Add("Id", ppToken.Id);
				dpParameter.Add("RefreshToken", Guid.NewGuid());

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Two factor authentication is enabled: {bIsEnabled}."
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
			Guid guTokenId,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"SELECT CASE WHEN EXISTS(
									SELECT 1 FROM [{PassportTokenTable.PassportToken}] 
									WHERE [{PassportTokenColumn.Id}] = @Id) 
									THEN 1 ELSE 0 END;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Id", guTokenId);

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
		public async Task<RepositoryResult<IPassportToken>> FindTokenByCredentialAsync(
			IPassportCredential ppCredential,
			DateTimeOffset dtAttemptedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new RepositoryResult<IPassportToken>(DefaultRepositoryError.TaskAborted);

			try
			{
				string sStatement = @$"UPDATE [{PassportTokenTable.PassportToken}] SET 
									[{PassportTokenColumn.FailedAttemptCounter}] = @FailedAttemptCounter, 
									[{PassportTokenColumn.RefreshToken}] = @RefreshToken 
									WHERE [{PassportTokenColumn.FailedAttemptCounter}] <= @MaximalAllowedAccessAttempt 
									AND [{PassportTokenColumn.Credential}] = @Credential 
									AND [{PassportTokenColumn.Provider}] = @Provider 
									AND [{PassportTokenColumn.Signature}] = @Signature; 
									SELECT 
									[{PassportTokenColumn.Id}], 
									[{PassportTokenColumn.PassportId}], 
									[{PassportTokenColumn.Provider}], 
									[{PassportTokenColumn.RefreshToken}], 
									[{PassportTokenColumn.TwoFactorIsEnabled}] 
									FROM [{PassportTokenTable.PassportToken}] 
									WHERE [{PassportTokenColumn.FailedAttemptCounter}] <= @MaximalAllowedAccessAttempt 
									AND [{PassportTokenColumn.Credential}] = @Credential 
									AND [{PassportTokenColumn.Provider}] = @Provider 
									AND [{PassportTokenColumn.Signature}] = @Signature;";

				DateTimeOffset dtExpiredAt = dtAttemptedAt.Add(ppSetting.MaximalRefreshTokenEffectivity);

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Credential", ppCredential.Credential);
				dpParameter.Add("FailedAttemptCounter", 0); // Reset the failed attempt counter.
				dpParameter.Add("MaximalAllowedAccessAttempt", ppSetting.MaximalAllowedAccessAttempt);
				dpParameter.Add("Provider", ppCredential.Provider);
				dpParameter.Add("RefreshToken", Guid.NewGuid());
				dpParameter.Add("Signature", ppCredential.HashSignature(ppHasher));

				IEnumerable<IPassportTokenTransferObject> enumTokenTransferObject = await sqlDataAccess.Connection.QueryAsync<PassportTokenTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumTokenTransferObject.Count() < 1)
					return new RepositoryResult<IPassportToken>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Credential and signature does not match at provider {ppCredential.Provider}."
					});

				if (enumTokenTransferObject.Count() > 1)
					return new RepositoryResult<IPassportToken>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could only find ambiguous results for credential at provider {ppCredential.Provider}."
					});

				IPassportToken? ppToken = PassportToken.Initialize(enumTokenTransferObject.First());

				if (ppToken is null)
					return new RepositoryResult<IPassportToken>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Token has invalid data."
					});

				return new RepositoryResult<IPassportToken>(ppToken);
			}
			catch (Exception exException)
			{
				return new RepositoryResult<IPassportToken>(new RepositoryError()
				{
					Code = PassportError.Code.Exception,
					Description = exException.Message
				});
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<IPassportToken>> FindTokenByRefreshTokenAsync(
			Guid guPassportId,
			string sProvider,
			string sRefreshToken,
			DateTimeOffset dtAttemptedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new RepositoryResult<IPassportToken>(DefaultRepositoryError.TaskAborted);

			try
			{
				string sStatement = @$"UPDATE [{PassportTokenTable.PassportToken}] SET 
									[{PassportTokenColumn.FailedAttemptCounter}] = @FailedAttemptCounter, 
									[{PassportTokenColumn.RefreshToken}] = @RefreshToken 
									WHERE [{PassportTokenColumn.FailedAttemptCounter}] <= @MaximalAllowedAccessAttempt 
									AND [{PassportTokenColumn.PassportId}] = @PassportId 
									AND [{PassportTokenColumn.Provider}] = @Provider 
									AND [{PassportTokenColumn.RefreshToken}] = @ActualToken; 
									SELECT 
									[{PassportTokenColumn.Id}], 
									[{PassportTokenColumn.PassportId}], 
									[{PassportTokenColumn.Provider}], 
									[{PassportTokenColumn.RefreshToken}], 
									[{PassportTokenColumn.TwoFactorIsEnabled}] 
									FROM [{PassportTokenTable.PassportToken}] 
									WHERE [{PassportTokenColumn.FailedAttemptCounter}] <= @MaximalAllowedAccessAttempt 
									AND [{PassportTokenColumn.PassportId}] = @PassportId 
									AND [{PassportTokenColumn.Provider}] = @Provider 
									AND [{PassportTokenColumn.RefreshToken}] = @RefreshToken;";

				DateTimeOffset dtExpiredAt = dtAttemptedAt.Add(ppSetting.MaximalRefreshTokenEffectivity);

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("ActualToken", sRefreshToken);
				dpParameter.Add("FailedAttemptCounter", 0); // Reset the failed attempt counter.
				dpParameter.Add("MaximalAllowedAccessAttempt", ppSetting.MaximalAllowedAccessAttempt);
				dpParameter.Add("PassportId", guPassportId);
				dpParameter.Add("Provider", sProvider);
				dpParameter.Add("RefreshToken", Guid.NewGuid());

				IEnumerable<IPassportTokenTransferObject> enumTokenTranferObject = await sqlDataAccess.Connection.QueryAsync<PassportTokenTransferObject>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (enumTokenTranferObject.Count() < 1)
					return new RepositoryResult<IPassportToken>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Refresh token does not match at provider {sProvider}."
					});

				if (enumTokenTranferObject.Count() > 1)
					return new RepositoryResult<IPassportToken>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could only find ambiguous results for refresh token at provider {sProvider}."
					});

				IPassportToken? ppToken = PassportToken.Initialize(enumTokenTranferObject.First());

				if (ppToken is null)
					return new RepositoryResult<IPassportToken>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = "Token has invalid data."
					});

				return new RepositoryResult<IPassportToken>(ppToken);
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<IPassportToken>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<bool>> InsertAsync(
			IPassportToken ppToken,
			IPassportCredential ppCredential,
			DateTimeOffset dtCreatedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"INSERT INTO [{PassportTokenTable.PassportToken}](
										[{PassportTokenColumn.CreatedAt}], 
										[{PassportTokenColumn.Credential}], 
										[{PassportTokenColumn.EditedAt}], 
										[{PassportTokenColumn.FailedAttemptCounter}], 
										[{PassportTokenColumn.Id}], 
										[{PassportTokenColumn.LastFailedAt}], 
										[{PassportTokenColumn.PassportId}], 
										[{PassportTokenColumn.Provider}], 
										[{PassportTokenColumn.RefreshToken}], 
										[{PassportTokenColumn.Signature}], 
										[{PassportTokenColumn.TwoFactorIsEnabled}]) 
										SELECT
										@CreatedAt,
										@Credential,
										@EditedAt,
										@FailedAttemptCounter,
										@Id,
										@LastFailedAt,
										@PassportId,
										@Provider,
										@RefreshToken,
										@Signature, 
										@TwoFactorIsEnabled 
										WHERE NOT EXISTS (
										SELECT 1
										FROM [{PassportTokenTable.PassportToken}] 
										WHERE [{PassportTokenColumn.PassportId}] = @PassportId 
										AND [{PassportTokenColumn.Provider}] != @Provider) 
										AND EXISTS (
										SELECT 1
										FROM [{PassportTable.Passport}] 
										WHERE [{PassportColumn.Id}] = @PassportId);";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("CreatedAt", dtCreatedAt);
				dpParameter.Add("Credential", ppCredential.Credential);
				dpParameter.Add("EditedAt", dtCreatedAt);
				dpParameter.Add("FailedAttemptCounter", 0);
				dpParameter.Add("Id", ppToken.Id);
				dpParameter.Add("LastFailedAt", dtCreatedAt);
				dpParameter.Add("PassportId", ppToken.PassportId);
				dpParameter.Add("Provider", ppToken.Provider);
				dpParameter.Add("RefreshToken", ppToken.RefreshToken);
				dpParameter.Add("Signature", ppCredential.HashSignature(ppHasher));
				dpParameter.Add("TwoFactorIsEnabled", ppToken.TwoFactorIsEnabled);

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Could not create token for {ppToken.PassportId} at {ppToken.Provider}."
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
		public async Task<RepositoryResult<bool>> ResetCredentialAsync(
			IPassportToken ppToken,
			IPassportCredential ppCredentialToApply,
			DateTimeOffset dtResetAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<bool>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"UPDATE [{PassportTokenTable.PassportToken}] SET 
									[{PassportTokenColumn.EditedAt}] = @EditedAt, 
									[{PassportTokenColumn.Signature}] = @Signature 
									WHERE [{PassportTokenColumn.Credential}] = @Credential 
									AND [{PassportTokenColumn.Id}] = @Id;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Credential", ppCredentialToApply.Credential);
				dpParameter.Add("EditedAt", dtResetAt);
				dpParameter.Add("Id", ppToken.Id);
				dpParameter.Add("Provider", ppToken.Provider);
				dpParameter.Add("Signature", ppCredentialToApply.HashSignature(ppHasher));

				int iResult = -1;

				iResult = await sqlDataAccess.Connection.ExecuteAsync(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				if (iResult < 1)
					return await Task.FromResult(new RepositoryResult<bool>(new RepositoryError()
					{
						Code = PassportError.Code.Method,
						Description = $"Credential has not been reset at provider {ppCredentialToApply.Provider}."
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
		public async Task<RepositoryResult<int>> VerifyCredentialAsync(
			IPassportCredential ppCredential,
			DateTimeOffset dtVerifiedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<int>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"UPDATE [{PassportTokenTable.PassportToken}] SET 
									[{PassportTokenColumn.FailedAttemptCounter}] = [{PassportTokenColumn.FailedAttemptCounter}] + 1, 
									[{PassportTokenColumn.LastFailedAt}] = @LastFailedAt
									WHERE [{PassportTokenColumn.Credential}] = @Credential 
									AND [{PassportTokenColumn.Provider}] = @Provider 
									AND [{PassportTokenColumn.Signature}] != @Signature;
									SELECT 
									[{PassportTokenColumn.FailedAttemptCounter}] 
									FROM [{PassportTokenTable.PassportToken}] 
									WHERE [{PassportTokenColumn.Credential}] = @Credential 
									AND [{PassportTokenColumn.Provider}] = @Provider;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("Credential", ppCredential.Credential);
				dpParameter.Add("LastFailedAt", dtVerifiedAt);
				dpParameter.Add("Provider", ppCredential.Provider);
				dpParameter.Add("Signature", ppCredential.HashSignature(ppHasher));

				int iFailedAttemptCounter = await sqlDataAccess.Connection.ExecuteScalarAsync<int>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				return new RepositoryResult<int>(ppSetting.MaximalAllowedAccessAttempt + -1 * iFailedAttemptCounter);
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<int>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}

		/// <inheritdoc/>
		public async Task<RepositoryResult<int>> VerifyRefreshTokenAsync(
			Guid guPassportId,
			string sProvider,
			string sRefreshToken,
			DateTimeOffset dtVerifiedAt,
			CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return await Task.FromResult(new RepositoryResult<int>(DefaultRepositoryError.TaskAborted));

			try
			{
				string sStatement = @$"UPDATE [{PassportTokenTable.PassportToken}] SET 
									[{PassportTokenColumn.FailedAttemptCounter}] = [{PassportTokenColumn.FailedAttemptCounter}] + 1, 
									[{PassportTokenColumn.LastFailedAt}] = @LastFailedAt
									WHERE [{PassportTokenColumn.PassportId}] = @PassportId 
									AND [{PassportTokenColumn.Provider}] = @Provider 
									AND [{PassportTokenColumn.RefreshToken}] <> @RefreshToken;
									SELECT 
									[{PassportTokenColumn.FailedAttemptCounter}] 
									FROM [{PassportTokenTable.PassportToken}] 
									WHERE [{PassportTokenColumn.PassportId}] = @PassportId 
									AND [{PassportTokenColumn.Provider}] = @Provider;";

				DynamicParameters dpParameter = new DynamicParameters();
				dpParameter.Add("LastFailedAt", dtVerifiedAt);
				dpParameter.Add("PassportId", guPassportId);
				dpParameter.Add("Provider", sProvider);
				dpParameter.Add("RefreshToken", sRefreshToken);

				int iFailedAttemptCounter = await sqlDataAccess.Connection.ExecuteScalarAsync<int>(
					sql: sStatement,
					param: dpParameter,
					transaction: sqlDataAccess.Transaction);

				return new RepositoryResult<int>(ppSetting.MaximalAllowedAccessAttempt + -1 * iFailedAttemptCounter);
			}
			catch (Exception exException)
			{
				return await Task.FromResult(
					new RepositoryResult<int>(new RepositoryError() { Code = PassportError.Code.Exception, Description = exException.Message }));
			}
		}
	}
}

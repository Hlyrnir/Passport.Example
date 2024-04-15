using Application.Common.Result.Repository;
using Domain.Interface.Authorization;

namespace Application.Interface.Passport
{
	public interface IPassportTokenRepository
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sCredential"></param>
		/// <param name="sProvider"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> CredentialAtProviderExistsAsync(string sCredential, string sProvider, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppToken"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> DeleteAsync(IPassportToken ppToken, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppToken"></param>
		/// <param name="bIsEnabled"></param>
		/// <param name="dtEditedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> EnableTwoFactorAuthenticationAsync(IPassportToken ppToken, bool bIsEnabled, DateTimeOffset dtEditedAt, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guTokenId"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ExistsAsync(Guid guTokenId, CancellationToken tknCancellation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppCredential"></param>
		/// <param name="dtAttemptedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IPassportToken>> FindTokenByCredentialAsync(IPassportCredential ppCredential, DateTimeOffset dtAttemptedAt, CancellationToken tknCancellation);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="guPassportId"></param>
		/// <param name="sProvider"></param>
		/// <param name="sRefreshToken"></param>
		/// <param name="dtAttemptedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<IPassportToken>> FindTokenByRefreshTokenAsync(Guid guPassportId, string sProvider, string sRefreshToken, DateTimeOffset dtAttemptedAt, CancellationToken tknCancellation);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppToken"></param>
		/// <param name="ppCredential"></param>
		/// <param name="dtCreatedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> InsertAsync(IPassportToken ppToken, IPassportCredential ppCredential, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ppToken"></param>
		/// <param name="ppCredentialToApply"></param>
		/// <param name="dtResetAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<bool>> ResetCredentialAsync(IPassportToken ppToken, IPassportCredential ppCredentialToApply, DateTimeOffset dtResetAt, CancellationToken tknCancellation);

		/// <summary>
		/// If <paramref name="ppCredential"/> does not match the database, the <see cref="int">number of failed attempts</see> is increased and a <see cref="RepositoryError"/> is returned.
		/// Otherwise the <see cref="int">number of remaining attempts</see> is returned.
		/// </summary>
		/// <param name="ppCredential">Define a credential to verify.</param>
		/// <param name="dtVerifiedAt">Define when this credential will be verified.</param>
		/// <param name="tknCancellation">Notification when a task should be cancelled.</param>
		/// <returns><see cref="RepositoryResult{T}"/> where <typeparamref name="T"/> is the <see cref="int">number of remaining attempts</see> or a <see cref="RepositoryError"/>.</returns>
		Task<RepositoryResult<int>> VerifyCredentialAsync(IPassportCredential ppCredential, DateTimeOffset dtVerifiedAt, CancellationToken tknCancellation);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="guPassportId"></param>
		/// <param name="sProvider"></param>
		/// <param name="sRefreshToken"></param>
		/// <param name="dtVerifiedAt"></param>
		/// <param name="tknCancellation"></param>
		/// <returns></returns>
		Task<RepositoryResult<int>> VerifyRefreshTokenAsync(Guid guPassportId, string sProvider, string sRefreshToken, DateTimeOffset dtVerifiedAt, CancellationToken tknCancellation);
	}
}
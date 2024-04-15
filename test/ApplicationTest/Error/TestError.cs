using Application.Common.Result.Repository;
using Application.Interface.Result;

namespace ApplicationTest.Error
{
	internal static class TestError
	{
		internal static class Repository
		{
			internal static class Passport
			{
				internal static class Code
				{
					public const string Method = "TEST_METHOD_PASSPORT";
				}

				internal static IRepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Passport does already exist in repository." };
				internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Passport does not exist in repository." };
				internal static IRepositoryError VisaRegister = new RepositoryError() { Code = Code.Method, Description = "Could not register visa to passport." };
			}

			internal static class PassportHolder
			{
				internal static class Code
				{
					public const string Method = "TEST_METHOD_PASSPORT_HOLDER";
				}

				internal static IRepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Holder does already exist in repository." };
				internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Holder does not exist in repository." };
			}

			internal static class PassportToken
			{
				internal static class Code
				{
					public const string Method = "TEST_METHOD_PASSPORT_TOKEN";
				}

				internal static IRepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Token does already exist in repository." };
				internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Token does not exist in repository." };
				
				internal static class Credential
				{
					internal static IRepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Credential does already exist in repository." };
					internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Credential does not exist in repository." };
					internal static IRepositoryError Invalid = new RepositoryError() { Code = Code.Method, Description = "Credential and signature does not match." };
				}

				internal static class RefreshToken
				{
					internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Refresh token does not exist in repository." };
					internal static IRepositoryError Invalid = new RepositoryError() { Code = Code.Method, Description = "Refresh token does not match." };
				}

				internal static class FailedAttemptCounter
				{
					internal static IRepositoryError NotAdded = new RepositoryError() { Code = Code.Method, Description = "Could not add failed attempt counter to passport." };
					internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Could not found failed attempt counter of passport." };
				}
			}

			internal static class PassportVisa
			{
				internal static class Code
				{
					public const string Method = "TEST_METHOD_PASSPORT_VISA";
				}

				internal static IRepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Visa does already exist in repository." };
				internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Visa does not exist in repository." };
				internal static IRepositoryError VisaRegister = new RepositoryError() { Code = Code.Method, Description = "No visa is registered to this passport." };
			}

			internal static class PhysicalDimension
			{
				internal static class Code
				{
					public const string Method = "TEST_METHOD_PHYSICAL_DIMENSION";
				}

				internal static IRepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Physical dimension does already exist in repository." };
				internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Physical dimension does not exist in repository." };
			}

			internal static class TimePeriod
			{
				internal static class Code
				{
					public const string Method = "TEST_METHOD_TIME_PERIOD";
				}

				internal static IRepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Time period does already exist in repository." };
				internal static IRepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Time period does not exist in repository." };
			}
		}
	}
}
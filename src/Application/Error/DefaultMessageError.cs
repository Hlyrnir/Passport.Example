using Application.Common.Result.Message;
using Application.Interface.Result;

namespace Application.Error
{
	internal static class DefaultMessageError
	{
		public static IMessageError TaskAborted => new MessageError() { Code = "METHOD_APPLICATION", Description = "Task has been cancelled." };
	}

	internal static class AuthorizationError
	{
		internal static class Code
		{
			public static string Method = "METHOD_AUTHORIZATION";
		}

		internal static class Passport
		{
			public static IMessageError IsDisabled => new MessageError() { Code = Code.Method, Description = "Passport is disabled." };
			public static IMessageError IsExpired => new MessageError() { Code = Code.Method, Description = "Passport is expired." };
			public static IMessageError TooManyAttempts => new MessageError() { Code = Code.Method, Description = "Passport is blocked due too many failed attempts." };
		}

		internal static class PassportVisa
		{
			public static IMessageError VisaDoesNotExist => new MessageError() { Code = Code.Method, Description = "Passport has no valid visa for this request." };
		}
	}

	internal static class ValidationError
	{
		internal static class Code
		{
			public static string Method = "METHOD_VALIDATION";
		}

		internal static class Passport
		{
			public static IMessageError BusinessLogic => new MessageError() { Code = Code.Method, Description = "Business logic has been violated." };
			public static IMessageError InvalidRefreshToken => new MessageError() { Code = Code.Method, Description = "Refresh token is invalid." };
		}
	}

	internal static class DomainError
	{
		internal static class Code
		{
			public static string Method = "METHOD_DOMAIN";
		}
	}
}
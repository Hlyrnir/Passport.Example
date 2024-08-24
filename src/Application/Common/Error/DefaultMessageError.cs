using Application.Common.Result.Message;
using Application.Interface.Result;

namespace Application.Common.Error
{
    internal static class DefaultMessageError
    {
        public static IMessageError TaskAborted => new MessageError() { Code = "METHOD_APPLICATION", Description = "Task has been cancelled." };
        public static IMessageError ConcurrencyViolation => new MessageError() { Code = "METHOD_APPLICATION", Description = "Data has been modified. Refresh and try again." };
    }

    public static class AuthorizationError
    {
        public static class Code
        {
            public static string Method = "METHOD_AUTHORIZATION";
        }

        public static class Passport
        {
            public static IMessageError IsDisabled => new MessageError() { Code = Code.Method, Description = "Passport is disabled." };
            public static IMessageError IsExpired => new MessageError() { Code = Code.Method, Description = "Passport is expired." };
            public static IMessageError TooManyAttempts => new MessageError() { Code = Code.Method, Description = "Passport is blocked due too many failed attempts." };
        }

        public static class PassportVisa
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
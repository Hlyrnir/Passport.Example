using Application.Interface.Result;

namespace Application.Common.Result.Message
{
    // see Microsoft.AspNetCore.Identity.IdentityError

    public readonly struct MessageError : IMessageError
    {
        /// <summary>
        /// Gets or sets the code for this error.
        /// </summary>
        /// <value>
        /// The code for this error.
        /// </value>
        public string? Code { get; init; }

        /// <summary>
        /// Gets or sets the description for this error.
        /// </summary>
        /// <value>
        /// The description for this error.
        /// </value>
        public string? Description { get; init; }

        // Types of message errors
        // Failure
        // Unexpected
        // Validation
        // Conflict
        // NotFound
    }
}

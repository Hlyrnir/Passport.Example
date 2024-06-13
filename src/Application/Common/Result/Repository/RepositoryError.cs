using Application.Interface.Result;

namespace Application.Common.Result.Repository
{
    public readonly struct RepositoryError : IRepositoryError
    {
        /// <summary>
        /// Gets or sets the code for this error.
        /// </summary>
        /// <value>
        /// The code for this error.
        /// </value>
        public required string Code { get; init; }

        /// <summary>
        /// Gets or sets the description for this error.
        /// </summary>
        /// <value>
        /// The description for this error.
        /// </value>
        public required string Description { get; init; }
    }
}

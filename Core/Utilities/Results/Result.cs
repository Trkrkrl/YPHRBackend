namespace Core.Utilities.Results
{
    /// <summary>
    /// Base implementation of <see cref="IResult"/>.
    /// Typically used for commands (operations without return data).
    /// </summary>
    public class Result : IResult
    {
        /// <summary>
        /// Creates a new result with success flag and message.
        /// </summary>
        public Result(bool success, string message) : this(success)
        {
            Message = message;
        }

        /// <summary>
        /// Creates a new result with only success flag.
        /// </summary>
        public Result(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
        public string Message { get; } = string.Empty;
    }
}

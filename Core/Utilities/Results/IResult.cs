namespace Core.Utilities.Results
{
    /// <summary>
    /// Represents the result of an operation without a return value.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Optional message describing the result.
        /// </summary>
        string Message { get; }
    }
}

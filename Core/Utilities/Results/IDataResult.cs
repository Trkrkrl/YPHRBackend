namespace Core.Utilities.Results
{
    /// <summary>
    /// Represents the result of an operation that returns data.
    /// </summary>
    public interface IDataResult<T> : IResult
    {
        /// <summary>
        /// The data returned by the operation.
        /// </summary>
        T Data { get; }
    }
}

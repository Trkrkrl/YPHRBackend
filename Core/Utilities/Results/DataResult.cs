namespace Core.Utilities.Results
{
    /// <summary>
    /// Base implementation of <see cref="IDataResult{T}"/>.
    /// Wraps operation success, message and data together.
    /// </summary>
    public class DataResult<T> : Result, IDataResult<T>
    {
        /// <summary>
        /// Creates a new data result with data, success flag and message.
        /// </summary>
        public DataResult(T data, bool success, string message)
            : base(success, message)
        {
            Data = data;
        }

        /// <summary>
        /// Creates a new data result with data and success flag.
        /// </summary>
        public DataResult(T data, bool success)
            : base(success)
        {
            Data = data;
        }

        public T Data { get; }
    }
}

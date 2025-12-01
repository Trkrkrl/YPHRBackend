namespace Core.Utilities.Results
{
    /// <summary>
    /// Convenience wrapper for failed operations that were expected to return data.
    /// </summary>
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(T data, string message)
            : base(data, false, message)
        {
        }

        public ErrorDataResult(T data)
            : base(data, false)
        {
        }

        public ErrorDataResult(string message)
            : base(default!, false, message)
        {
        }

        public ErrorDataResult()
            : base(default!, false)
        {
        }
    }
}

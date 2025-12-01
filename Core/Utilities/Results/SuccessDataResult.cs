namespace Core.Utilities.Results
{
    /// <summary>
    /// Convenience wrapper for successful operations that return data.
    /// </summary>
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult(T data, string message)
            : base(data, true, message)
        {
        }

        public SuccessDataResult(T data)
            : base(data, true)
        {
        }

        public SuccessDataResult(string message)
            : base(default!, true, message)
        {
        }

        public SuccessDataResult()
            : base(default!, true)
        {
        }
    }
}

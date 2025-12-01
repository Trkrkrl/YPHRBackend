namespace Core.Utilities.Results
{
    /// <summary>
    /// Convenience result type for successful operations without data.
    /// </summary>
    public class SuccessResult : Result
    {
        public SuccessResult(string message) : base(true, message)
        {
        }

        public SuccessResult() : base(true)
        {
        }
    }
}

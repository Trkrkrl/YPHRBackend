namespace Core.Utilities.Results
{
    /// <summary>
    /// Convenience result type for failed operations without data.
    /// </summary>
    public class ErrorResult : Result
    {
        public ErrorResult(string message) : base(false, message)
        {
        }

        public ErrorResult() : base(false)
        {
        }
    }
}

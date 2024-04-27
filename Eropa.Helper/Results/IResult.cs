namespace Eropa.Helper.Results
{
    public interface IResult
    {
        public bool Success { get; }
        public string Message { get; }
        public List<string> Errors{ get; }
    }
}

namespace Eropa.Helper.Results
{
    public interface IDataResult<T> : IResult
    {
        T Data { get; }
    }
}

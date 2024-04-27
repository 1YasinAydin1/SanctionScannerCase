namespace Eropa.Helper.Results
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public DataResult(T data, bool success = true, string message="") : base(success, message)
        {
            Data = data;
        }

        public T Data { get; }


    }
}

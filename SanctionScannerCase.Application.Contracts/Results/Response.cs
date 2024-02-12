namespace SanctionScannerCase.Application.Contracts.Results
{
    /// <summary>
    /// Response Yapısı üzerinden çalışılmıştır
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        public T Data { get; private set; }
        public bool IsSuccessful { get; private set; }
        public List<string> Errors { get; set; } = new List<string>() { };


        #region static factory methods

        public static Response<T> Success(T data)
        {
            return new Response<T>()
            {
                Data = data,
                IsSuccessful = true
            };
        }
        public static Response<T> Fail(T data, List<string> errors = null)
        {
            return new Response<T>()
            {
                Data = data,
                IsSuccessful = false,
                Errors = errors
            };
        }

        #endregion
    }
}

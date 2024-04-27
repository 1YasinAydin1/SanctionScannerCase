using Eropa.Helper.Results;

namespace Eropa.Domain.SAPConnection
{
    public interface ISapOperationsService
    {
        Task<IDataResult<string>> SapGetApiAsync(string method, string key = "");
        Task<IDataResult<string>> SapPatchServiceAsync<T>(T input,string key, string method);
        Task<IDataResult<string>> SapPostServiceAsync<T>(T input,string method);
        Task<IDataResult<string>> SapDeleteServiceAsync(string key, string method);
        Task<IResult> SapLoginServiceAsync(SapLogin sapLogin=null);
        Task<IQueryable<T>> RunQuery<T>(string query);
    }
}

using Dapper;
using Eropa.Domain.SAPConnection;
using Eropa.Helper.Results;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using System.Net;

namespace Eropa.Eropa.Persistence.SAPConnection
{
    public class SapOperationsService : ISapOperationsService
    {
        private readonly IConfiguration configuration;
        private static string b1SessionId = "";
        private static string _sapUrl = "";
        private static SapLogin _sapLogin;
        private static string _connectionUrl;

        public SapOperationsService(IConfiguration configuration)
        {
            this.configuration = configuration;
            _sapUrl = configuration.GetSection("SapUrl").Get<string>()!;
            _connectionUrl = configuration.GetSection("SapDbConnectionUrl").Get<string>()!;
        }

        public async Task<IResult> SapLoginServiceAsync(SapLogin sapLogin = null)
        {
            if (sapLogin != null)
                _sapLogin = sapLogin;
            else
            {
                if (_sapLogin != null)
                    sapLogin = _sapLogin;
                else
                    throw new NullReferenceException("SapLogin is null");
            }

            if (string.IsNullOrEmpty(_sapUrl))
                throw new NullReferenceException("SapUrl is null");

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var client = new RestClient(_sapUrl + "Login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                CompanyDB = sapLogin.Company,
                UserName = sapLogin.UserName,
                Password = sapLogin.Password,
                Language = "31"
            };
            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            var sapResponse = await client.ExecuteAsync(request);
            if (sapResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception(sapResponse.Content);
            var response = JsonConvert.DeserializeObject<SapLoginResponse>(sapResponse.Content);
            b1SessionId = response.SessionId;
            return new Result(true);
        }

        public async Task<IDataResult<string>> SapGetApiAsync(string method, string key = "")
        {
            return await SapOperationBase("", Method.GET, $"{_sapUrl}{method}({key})");
        }

        public async Task<IDataResult<string>> SapPatchServiceAsync<T>(T input, string key, string method)
        {
            return await SapOperationBase(input, Method.PATCH, string.Format(_sapUrl + method + "(" + key + ")"));
        }

        public async Task<IDataResult<string>> SapPostServiceAsync<T>(T input, string method)
        {
            return await SapOperationBase(input, Method.POST, string.Format(_sapUrl + method));
        }

        private async Task<IDataResult<string>> SapOperationBase<T>(T input, Method method, string restClientString)
        {
            if (string.IsNullOrEmpty(b1SessionId))
                await SapLoginServiceAsync();

            var request = new RestRequest(restClientString,method);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddCookie("B1SESSION", b1SessionId);
            if (input.GetType() == typeof(string))
                request.AddParameter("application/json", input, ParameterType.RequestBody);
            else if (input != null)
                request.AddParameter("application/json", JsonConvert.SerializeObject(input, Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                }), ParameterType.RequestBody);

            var client = new RestClient();
            client.Timeout = -1;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("Unauthorized");
            if (!response.IsSuccessful)
                throw new Exception(response.Content);
            return new DataResult<string>(response.Content);
        }

        public async Task<IDataResult<string>> SapDeleteServiceAsync(string key, string method)
        {
            return await SapOperationBase<string>("", Method.DELETE, string.Format(_sapUrl + method + "(" + key + ")"));
        }

        public async Task<IQueryable<T>> RunQuery<T>(string query)
        {
            IEnumerable<T> list = null;
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                list = await dbConnection.QueryAsync<T>(query, commandType: CommandType.Text);
            }
            return list.AsQueryable();
        }

        private SqlConnection ConnectionSql
        {
            get
            {
                try
                {
                    return new SqlConnection(_connectionUrl.Replace("DATABASE", _sapLogin?.Company ?? "SBO-COMMON"));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }
        public IDbConnection Connection
        {
            get
            {
                try
                {
                    return ConnectionSql;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }
    }
}

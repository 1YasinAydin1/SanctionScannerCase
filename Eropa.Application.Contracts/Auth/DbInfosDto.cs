using System.ComponentModel.DataAnnotations;

namespace Eropa.Application.Contracts.Auth
{
    public class DbInfosDto
    {
        [Key]
        public string dbName { get; set; }
        public string cmpName { get; set; }
    }
}

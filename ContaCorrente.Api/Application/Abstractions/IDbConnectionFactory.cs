using System.Data;

namespace ContaCorrente.Api.Application.Abstractions
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
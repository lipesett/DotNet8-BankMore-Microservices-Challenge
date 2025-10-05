using System.Data;

namespace Transferencia.Api.Application.Abstractions
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
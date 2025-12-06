using System.Data;

namespace FiloShop.SharedKernel.Interfaces;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
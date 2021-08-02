using Microsoft.Extensions.Configuration;

namespace videoapp.api
{
    internal sealed class DbConnectionStringProvider
    {
        public string ConnectionString { get; private init; }

        public DbConnectionStringProvider(IConfiguration config)
        {
            ConnectionString = config.GetConnectionString("Main");
        }
    }
}
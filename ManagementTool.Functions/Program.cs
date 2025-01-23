using System;
using ManagementTool.Functions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ManagementTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionSetting");
                var dbName = Environment.GetEnvironmentVariable("CosmosDbDatabaseName");
                var dbContainerName = "Items"; Environment.GetEnvironmentVariable("CosmosDbContainerName");

                var host = new HostBuilder()
                    .ConfigureFunctionsWebApplication()
                    .ConfigureServices(services =>
                    {
                        services.AddHttpClient();

                        services.AddSingleton<IUserRepository>(provider =>
                            new UserRepository(connectionString, dbName, dbContainerName));
                        services.AddScoped<IUserService, UserService>();

                        services.AddLogging();


                    })
                    .Build();

                host.Run();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using videoapp.api.Services;

namespace videoapp.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    //services.AddTransient(typeof(DbConnectionStringProvider));
                    services.AddSingleton(typeof(MovieTrailerQueryResultsCache));
                    services.AddTransient(typeof(YoutubeResolver));
                    services.AddTransient(typeof(ImdbResolver));
                    services.AddTransient(typeof(MovieTrailerResolver));
                    //services.AddTransient(typeof(DbContext));
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}

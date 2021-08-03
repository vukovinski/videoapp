using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace videoapp.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "videoapp.api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "videoapp.api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallback(TransferFileResponse);
                endpoints.Map("{filename}.{extension}", TransferFileResponse);
            });
        }

        private async Task TransferFileResponse(HttpContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var filename = context.Request.RouteValues["filename"];
            var extension = context.Request.RouteValues["extension"];
            var requested = $"{filename}.{extension}";
            if (requested == ".")
            {
                requested = "index.html";
            }

            using var filestream = assembly.GetManifestResourceStream($"videoapp.api.Content.{requested}");

            if (filestream == null)
                context.Response.StatusCode = 404;
            else
                await TransferFile(context, filestream, requested);
        }

        private async Task TransferFile(HttpContext context, Stream filestream, string filename)
        {
            context.Response.StatusCode = 200;
            if (filename.EndsWith(".html")) context.Response.ContentType = "text/html";
            if (filename.EndsWith(".js")) context.Response.ContentType = "text/javascript";

            var buffer = new byte[1 << 10];
            while (true)
            {
                var read = await filestream.ReadAsync(buffer, 0, 1 << 10);
                if (read == 0) break;
                await context.Response.Body.WriteAsync(buffer, 0, read);
            }
            await context.Response.Body.FlushAsync();
        }
    }
}

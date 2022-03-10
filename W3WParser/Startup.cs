using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using W3WParser.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Routing;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using static Microsoft.AspNetCore.Mvc.Versioning.ApiVersionMapping;

using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;

namespace W3WParser
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //public IConfiguration Configuration { get; }
        internal static IConfiguration Configuration { get; private set; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                }
            );
            
            services.AddVersionedApiExplorer(
                o =>
                {
                    o.GroupNameFormat = "'v'VVV";
                    o.SubstituteApiVersionInUrl = true;
                });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(c =>
            {
               
                c.OperationFilter<SwaggerDefaultValues>();
                c.IncludeXmlComments(XmlCommentsFilePath);
            }
            );

            services.AddHttpClient("googleApiClient", client =>
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/place/findplacefromtext/");
                client.DefaultRequestHeaders.Add("User-Agent", "w3wparser");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient("w3wApiClient", client =>
            {
                client.BaseAddress = new Uri("https://api.what3words.com/");
                client.DefaultRequestHeaders.Add("User-Agent", "w3wparser");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            
           }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                    });
                }
            );

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(options =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", $"v1");
                //c.SwaggerEndpoint("/swagger/v2/swagger.json", $"v2");
                options.RoutePrefix = string.Empty;

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
                
                
            }); 

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
            });
        }

        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}

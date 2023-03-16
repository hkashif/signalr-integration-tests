namespace MySignalR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public IConfiguration Configuration { get; }
    private IWebHostEnvironment CurrentEnvironment { get; set; }
    private const string ENV_LOCAL = "Local";
    private const string SIGNALR_CORS = "_signalRCors";

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        CurrentEnvironment = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SignalR", Version = "v1" });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        // Add services to the container.
        services.AddHttpContextAccessor();
        services.AddSignalR();

        services.AddCors(options => 
        {
            options.AddPolicy(name: SIGNALR_CORS,
                policy => 
                {
                    policy.WithOrigins("localhost")
                        .WithMethods("GET", "POST")
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            );
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {AppDomain.CurrentDomain.Load(typeof(SignalHub).Assembly.FullName);

        if (CurrentEnvironment.IsDevelopment() || CurrentEnvironment.IsEnvironment(ENV_LOCAL))
        {
            app.UseDeveloperExceptionPage();
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials
        }
        else 
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseCors(SIGNALR_CORS);
        }
        
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                var serverUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}";
                swaggerDoc.Servers = new List<OpenApiServer>() { new OpenApiServer { Url = serverUrl } };
            });
        });

        app.UseSwaggerUI(c => 
        {
            c.SwaggerEndpoint("v1/swagger.json", "SignalR v1");
            c.RoutePrefix = "swagger";
        });

        app.UseExceptionHandler("/error");

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<SignalHub>("/signalr-hub");
        });
    }
}
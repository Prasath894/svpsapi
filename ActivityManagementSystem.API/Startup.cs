using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ActivityManagementSystem.Service.Logging.Serilog;
using CorrelationId.DependencyInjection;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using ActivityManagementSystem.Domain.AppSettings;
using ActivityManagementSystem.Service.Middleware;
using ActivityManagementSystem.BLL;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;

namespace ActivityManagementSystem.Service
{
    public class Startup
    {
        public AppSettings AppSettings = new AppSettings();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json");
            //.AddJsonFile($"appsettings.{env.EnvironmentName}.json")
            //.AddJsonFile("Email.NewUserCreation.json");
            builder.AddEnvironmentVariables();
            
            Configuration = builder.Build();
            Configuration.GetSection("AppSettings").Bind(AppSettings);
          
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelationId();
            services.AddMemoryCache();
            services.AddSingleton<CorrelationIdEnricher>();
            services.AddSingleton<ILoggerFactory>(svc =>
            {

                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration) // fetching configuration from appsettings file
                    .Enrich
                    .With(svc.GetService<CorrelationIdEnricher>())
                    .CreateLogger();
                Log.Logger = logger;

                return new Serilog.Extensions.Logging.SerilogLoggerFactory(logger, true);
            });


            services.AddControllers();
            services.AddSingleton(AppSettings);
            services.AddSingleton(typeof(IServices<>), typeof(Services<>));


            var policy = new CorsPolicy();
            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add(AppSettings.Settings.UiUrl);
            policy.SupportsCredentials = true;
            services.AddCors(options =>
            {
                options.AddPolicy(name: "MyAllowSpecificOrigins",
                    builder =>
                    {
                       // builder.AllowAnyOrigin()
                  //    .AllowAnyHeader()
                  //    .AllowAnyMethod().WithExposedHeaders("Content-Disposition") // If returning files
                  //.AllowCredentials();
                  builder.WithOrigins("http://localhost:3000") // ✅ Specify frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("Content-Disposition")// If returning files
                  .AllowCredentials();
                    });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ActivityManagementSystem.Service.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "ActivityManagementSystem.Service.API Authorization",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(name: "MyAllowSpecificOrigins",
            //    builder =>
            //    {
            //        builder.WithOrigins("http://103.53.52.215").
            //            AllowAnyOrigin()
            //     .AllowAnyHeader()
            //     .AllowAnyMethod();
            //    });
            //});
            //services.AddSwaggerGen(c =>
            //{
            //	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //});

            services.AddLocalization();
            services.AddMvc();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(options =>
        {
            var secretKey = AppSettings.JWTSettings.SecretKey ?? string.Empty;
            var issuer = AppSettings.JWTSettings.Issuer;
            var audience = AppSettings.JWTSettings.Audience;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true // Ensures token expiration is validated
            };
        });

            // Register HttpClient
            services.AddHttpClient("ApiClient", client =>
            {
                var apiBaseUrl = Configuration["ApiBaseUrl"] ?? string.Empty;
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

          
            services.AddControllers();
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ActivityManagementSystem.Service.API v1"));
            app.UserSerilogMiddleware();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("MyAllowSpecificOrigins");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}


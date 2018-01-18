using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SquadLocker.Common.Web.Middleware;
using SquadLocker.Common.Constants;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;

namespace SquadLocker.Products.Web.Api
{
    public class Startup
    {
        private static log4net.ILog _logger;
        public IConfiguration Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;

            InitLog4net();
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Options services
            services.AddOptions();

            // Require HTTPS by default except if RequireHttps setting is set to false
            var requireHttps = true;
            requireHttps = bool.TryParse(Configuration["RequireHttps"], out requireHttps) ? requireHttps : true;
            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options => { if (requireHttps) options.Filters.Add(new Microsoft.AspNetCore.Mvc.RequireHttpsAttribute()); });

            // DI services 
            services.AddScoped<Data.IUnitOfWork>(_ => Data.EF.UnitOfWorkFactory.CreateFromConnectionString(Configuration.GetConnectionString("ProductsDatabase")));
            services.AddScoped<Services.ICatalogService, Services.CatalogService>();

            // Cors
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(new Common.Web.Filters.MonitorActionExecutedAttribute() { IncludeBody = true });
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });


            // Add Jwt Bearer Authentication
            var identityCertificate = Common.Identity.IdentityHelper.LoadPublicIdentityCertificate(CurrentEnvironment.IsProduction());
            var issuerSigningKey = new X509SecurityKey(identityCertificate);
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = issuerSigningKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuers = new List<string> { Configuration.GetSection("IdentityServer").GetValue<string>("AuthorityUrl"), "https://squadlocker-identity-develop.azurewebsites.net" },
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudiences = new List<string> { IdentityConstants.ProductsApiResource },
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.FromSeconds(5),
                // Claim types for User
                NameClaimType = IdentityModel.JwtClaimTypes.Email,
                RoleClaimType = IdentityModel.JwtClaimTypes.Role,
            };
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration.GetSection("IdentityServer").GetValue<string>("AuthorityUrl");
                options.TokenValidationParameters = tokenValidationParameters;
                options.RequireHttpsMetadata = requireHttps;
            });
            //This is important, without this original claim type names (jwt) are mapped to MS compatibility type names
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


            //Authorization Policies config
            services.AddAuthorization(options =>
            {
                options.AddPolicy(IdentityConstants.ProductsApiPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(IdentityModel.JwtClaimTypes.Audience, IdentityConstants.ProductsApiResource);
                    policy.RequireClaim(IdentityModel.JwtClaimTypes.Scope, IdentityConstants.ProductsApiScope);
                });
            });


            // Add Swagger for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SquadLocker Product Catalog API", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.AddSecurityDefinition("Product Catalog API Security", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "application",   //grant_type: client_credentials 
                    Description = "Full access to the Product Catalog API",
                    TokenUrl = Configuration.GetSection("IdentityServer").GetValue<string>("AuthorityUrl") + "/connect/token",
                    Scopes = new Dictionary<string, string>
                    {
                        { IdentityConstants.ProductsApiScope, "Full access to the Product Catalog API" },
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("CorsPolicy");
            app.UseAddResponseHeadersMiddleware();

            var useDevelopmentExceptionHandler = false;
            bool.TryParse(Configuration.GetSection("Exceptions")["UseDevelopmentExceptionHandler"], out useDevelopmentExceptionHandler);
            app.UseExceptionHandler(options => options.UseApiExceptionHandlerMiddleware(isDevelopmentMode: useDevelopmentExceptionHandler));

            app.UseAuthentication();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Expose Swagger API documentation
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SquadLocker Products Catalogue Services API V1");
            });
        }

        private void InitLog4net()
        {
            var log4netConfig = new System.Xml.XmlDocument();
            log4netConfig.Load(System.IO.File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(System.Reflection.Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            var configuration = Configuration.GetSection("log4net").AsEnumerable(true).ToDictionary(x => x.Key, x => x.Value);
            repo.Properties["Configuration"] = configuration;
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                _logger.Fatal(e.ExceptionObject);
            }
            else
            {
                _logger.Error(e.ExceptionObject);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.Data.Contexts;
using core.Data.Repositories.Auth;
using core.Data.Repositories.User;
using core.Domain.Entities.Auth;
using core.Domain.Entities.User;
using core.Services.Services.Auth;
using core.Services.Services.Text;
using core.Services.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace core.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string corsPolicy = "corsAll";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // getting the connection string from docker-compose. If they don't exist, use the appsetting.production.json file
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? Configuration.GetConnectionString("PostgresConnection");

            services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequiredUniqueChars = 2;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.RequireUniqueEmail = true;
                    //options.SignIn.RequireConfirmedEmail = true;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // getting the env variables from docker-compose. If they don't exist, use the appsetting.production.json file
            var tokenHttpsMeta = Environment.GetEnvironmentVariable("TOKEN_HTTPS_META") ?? Configuration.GetSection("Tokens")["HttpsMeta"];
            var tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY") ?? Configuration.GetSection("Tokens")["Key"];
            var tokenIssuer = Environment.GetEnvironmentVariable("TOKEN_ISSUER") ?? Configuration.GetSection("Tokens")["Issuer"];
            var tokenAudience = Environment.GetEnvironmentVariable("TOKEN_AUDIENCE") ?? Configuration.GetSection("Tokens")["Audience"];

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = Convert.ToBoolean(tokenHttpsMeta);
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // The signing key must match!
                        RequireSignedTokens = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = tokenIssuer,

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = tokenAudience,

                        // Validate the token expiry
                        ValidateLifetime = true,
                        RequireExpirationTime = true,

                        // If you want to allow a certain amount of clock drift, set that here:
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services
                .AddCors(options =>
                {
                    options.AddPolicy(corsPolicy,
                        bldr => bldr
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                    );
                });

            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Formatting = Formatting.Indented;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Configuration Settings
            services.Configure<TokenConfig>(tokenConfig =>
            {
                tokenConfig.Key = tokenKey;
                tokenConfig.Issuer = tokenIssuer;
                tokenConfig.Audience = tokenAudience;
            });

            // Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Application services
            services.AddTransient<ITextService, TextService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            app.UseCors(corsPolicy);

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}

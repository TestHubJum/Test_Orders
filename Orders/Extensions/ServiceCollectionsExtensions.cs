using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Application.Abstractions;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Options;
using Domain.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Orders.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Orders API",
                    Version = "v1"
                });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            
            return builder;
        }

        public static WebApplicationBuilder AddData(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<OrdersDbContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("Orders")));
            
            return builder;
        }

        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICartsService, CartsService>();
            builder.Services.AddScoped<IOrdersService, OrdersService>();
            builder.Services.AddScoped<IMerchantService,MerchantService>();

            return builder;
        }  
        
        public static WebApplicationBuilder AddIntegrationServices(this WebApplicationBuilder builder)
        {
            return builder;
        }

        public static WebApplicationBuilder AddBearerAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthentication(x=>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x=>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.UseSecurityTokenValidators = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                            builder.Configuration["Authentication:TokenPrivateKey"]!)),

                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = false

                    };
                });
            builder.Services.AddAuthorization(option =>
            {
                option.AddPolicy("Admin", policy => policy.RequireRole(RoleConsts.Admin));
                option.AddPolicy("Merchant", policy => policy.RequireRole(RoleConsts.Merchant));
                option.AddPolicy("User", policy => policy.RequireRole(RoleConsts.User));
            });
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddDefaultIdentity<UserEntity>(option =>
               {
                   option.SignIn.RequireConfirmedAccount = false;
                   option.Password.RequiredLength = 6;
                   option.Password.RequireNonAlphanumeric = false;
                   
                   

               })
            .AddEntityFrameworkStores<OrdersDbContext>()
            .AddUserManager<UserManager<UserEntity>>()
            .AddUserStore<UserStore<UserEntity, IdentityRoleEntity, OrdersDbContext, long>>();

            return builder;
        }

        public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Authentication"));

            return builder;
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Week7Sample.Data;
using Week7Sample.Data.Repositories.Implementation;
using Week7Sample.Data.Repositories.Interfaces;
using Week7Sample.Model;
using NLog;
using NLog.Web;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Week7Sample.Service;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Fast Authentication Scheme",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },

                },
                new List<string>()
        }
    });
    });

    builder.Logging.ClearProviders();   
    builder.Services.AddDbContext<Week7Context>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    );

    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(option =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Key").Value);
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RegularRole", policy => policy.RequireRole("regular"));
        options.AddPolicy("AdminOrSuper", policy => policy.RequireAssertion(
                          context => context.User.IsInRole("admin")
                          || context.User.IsInRole("super-admin")
        ));
    });
    //builder.Services.AddIdentity<User, IdentityRole>(
    //    option =>
    //{
    ////option.SignIn.RequireConfirmedEmail = true;
    ////    option.Password.RequiredUniqueChars = 0;
    ////    option.Password.RequireDigit = false;
    ////    option.Password.RequireUppercase = false;
    //}
    //).AddEntityFrameworkStores<Week7Context>();

    //builder.Services.AddIdentity<User, IdentityRole>(/* ... */)
    //.AddEntityFrameworkStores<Week7Context>()
    //.AddSignInManager<SignInManager<User>>()
    //.AddUserManager<UserManager<User>>();

    builder.Services.AddIdentity<User, IdentityRole>(/* ... */)
    .AddEntityFrameworkStores<Week7Context>()
    .AddDefaultTokenProviders();



    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddScoped<IUploadService, UploadService>();


    //Pipelines are below **********************************************8
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseExceptionHandler(builder =>
        //{
        //    builder.Run(async context =>
        //    {
        //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        var error = context.Features.Get<IExceptionHandlerPathFeature>();

        //        logger.Error($"Error path: {error.Path}, Error thrown: { error.Error.Message}, " +
        //            $"Inner Message: {error.Error.InnerException}");
        //    });
        //});
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();


}
catch (Exception exception)
{
    //NLog: catch setup for errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

using JurayKV.Application.Caching.Repositories;
using JurayKV.Application.Commands.IdentityCommands.UserCommands;
using JurayKV.Application.Services;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Persistence.Cache.Repositories;
using JurayKV.Persistence.Cache;
using JurayKV.Persistence.RelationalDB; // Import your DbContext namespace 
using JurayKV.Persistence.RelationalDB.Repositories;
using JurayKV.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Import EF Core namespace
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TanvirArjel.EFCore.GenericRepository;
using JurayKV.Persistence.RelationalDB.Extensions;
using JurayKV.Domain.Aggregates.BucketAggregate;
using JurayKV.Application.Queries.BucketQueries;
using JurayKV.Application.Queries.DashboardQueries;
using JurayKV.Application.Queries.IdentityKvAdQueries;
using JurayKV.Application.Caching.Handlers;
using JurayKV.Persistence.Cache.Handlers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
 

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Koboview API", Version = "v1" });
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
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
            },
            new string[]{}
        }
    });
});
// Inside ConfigureServices method of Startup.cs
builder.Services.AddAuthentication(optiones =>
{
    optiones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    optiones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    optiones.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = builder.Configuration["JwtSecurityToken:Audience"],
                    ValidIssuer = builder.Configuration["JwtSecurityToken:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityToken:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddRelationalDbContext(builder.Configuration.GetConnectionString("DefaultConnection"));

 
// Register SymmetricSecurityKeyService
builder.Services.AddTransient<SymmetricSecurityKeyService>();
//builder.Services.AddCaching();

 
// Add your command handler
builder.Services.AddTransient<IRequestHandler<LoginCommand, string>, LoginCommandHandler>();

//register services and handlers
builder.Services.AddTransient<IRequestHandler<GetAllBucketListQuery, List<BucketListDto>>, GetAllBucketListQuery.GetAllBucketListQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetBucketListQuery, List<BucketListDto>>, GetBucketListQuery.GetBucketListQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetBucketDropdownListQuery, List<BucketDropdownListDto>>, GetBucketDropdownListQuery.GetBucketDropdownListQueryHandler>();
builder.Services.AddTransient<IBucketCacheRepository, BucketCacheRepository>();

// Register services
builder.Services.AddTransient<IRequestHandler<DashboardQuery, DashboardDto>, DashboardQuery.DashboardQueryHandler>();
builder.Services.AddTransient<IDashboardCacheRepository, DashboardCacheRepository>(); // Replace YourDashboardCacheRepositoryImplementation with the actual implementation class.
//
builder.Services.AddTransient<IRequestHandler<GetIdentityKvAdActiveByUserIdListQuery, List<IdentityKvAdListDto>>, GetIdentityKvAdActiveByUserIdListQuery.GetIdentityKvAdActiveByUserIdListQueryHandler>();
builder.Services.AddTransient<IIdentityKvAdCacheRepository, IdentityKvAdCacheRepository>();
//
builder.Services.AddTransient<IRequestHandler<GetIdentityKvAdByUserIdListQuery, List<IdentityKvAdListDto>>, GetIdentityKvAdByUserIdListQuery.GetIdentityKvAdByUserIdListQueryHandler>();
builder.Services.AddTransient<IIdentityKvAdCacheRepository, IdentityKvAdCacheRepository>();
//

//
builder.Services.AddTransient<IRequestHandler<GetBucketByIdQuery, BucketDetailsDto>, GetBucketByIdQuery.GetBucketByIdQueryHandler>();
builder.Services.AddTransient<IIdentityKvAdCacheHandler, IdentityKvAdCacheHandler>();

builder.Services.AddDistributedMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

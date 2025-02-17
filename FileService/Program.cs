using Azure.Storage.Blobs;
using Business.Contracts;
using Business.DTOs.File;
using Business.DTOs.FileShare;
using Business.DTOs.Folder;
using Business.Mappers;
using Business.Services;
using DataAccess.Context;
using DataAccess.Repositories;
using DataAccess.Utils;
using Domain.Entities;
using Infrastructure;
using Infrastructure.BlobStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using File  = Domain.Entities.File;
using FileShare = Domain.Entities.FileShare;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddHttpContextAccessor();

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    IHttpContextAccessor accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    WorkContext.SetHttpContextAccessor(accessor);

}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.Authority = "https://myauth-bebhf3f4czfdb9fe.canadacentral-01.azurewebsites.net"; // IdentityServer URL
       options.RequireHttpsMetadata = false; // Set to true in production
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateAudience = true,
           ValidAudience = "file-service",
           ValidateIssuer = true,
           ValidIssuer = "https://myauth-bebhf3f4czfdb9fe.canadacentral-01.azurewebsites.net",
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true, 
       };
   });
       

builder.Services.AddAuthorization();


// Configure the DbContext with the connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpContextAccessor();

// Add BlobServiceClient as a singleton service
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("BlobConnectionString")));

// Add scoped services for unit of work and repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add AutoMapper profiles
builder.Services.AddAutoMapper(typeof(FolderMapper));
builder.Services.AddAutoMapper(typeof(FileMapper));
builder.Services.AddAutoMapper(typeof(FileShareMapper));

// Add scoped services for business logic
builder.Services.AddScoped<IBaseService<Folder, FolderCreateDto, FolderUpdateDto>, FolderService>();
builder.Services.AddScoped<IBaseService<File, FileCreateDto, FileUpdateDto>, Business.Services.FileService>();
builder.Services.AddScoped<IBaseService<FileShare, FileShareDto, FileShareUpdateDto>, FileShareService>();

builder.Services.AddScoped<IFileShareService, FileShareService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IFileService, Business.Services.FileService>();

// Add scoped services for repositories
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<IFileVersionRepository, FileVersionRepository>();
builder.Services.AddScoped<IFileShareRepository, FileShareRepository>();

// Add BlobStorageService as a scoped service
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } // for integration tests
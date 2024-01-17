using Inventario.Controllers;
using Inventario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Inventario.Authorization;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de DbContext

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("MemoryDB"));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithHeaders("Authorization"); // Permitir la cabecera "Authorization"
    });
});
builder.Services.AddScoped<LoginController>();

// builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.Authority = "http://localhost:5173"; // Servidor Local de React para autenticar el token
            options.Audience = "dispositivos"; // Audiencia esperada del token (puede ser el nombre de tu API)
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkExOUEwNTA0OCIsInN1YiI6IkExOUEwNTA0OCIsImp0aSI6IjIwNDcxMTQ4IiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6MTAzMDgiLCJodHRwczovL2xvY2FsaG9zdDo0NDM0NiIsImh0dHA6Ly9sb2NhbGhvc3Q6NTE5OCIsImh0dHBzOi8vbG9jYWxob3N0OjcyNDAiXSwibmJmIjoxNzA0ODI0NjQ0LCJleHAiOjE3MTI2ODcwNDQsImlhdCI6MTcwNDgyNDY0NywiaXNzIjoiZG90bmV0LXVzZXItand0cyJ9.Qz3vx7gEdDZjuG1L6HGebH6nZ1zkVQUtDkoCskTC8q0";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            // Configura la validación del token 
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false, // Valida el emisor del token
                ValidateAudience = false, // Valida la audiencia del token
                ValidateLifetime = true, // Valida la fecha de expiración del token
                ValidateIssuerSigningKey = true, // Valida la firma del token
                IssuerSigningKey = signingKey
            };
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
        options.DocumentTitle = "API INVI";
    }
    );
}

app.UseHttpsRedirection();

app.UseCors("MyPolicy");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapSwagger().RequireAuthorization();

app.Run();
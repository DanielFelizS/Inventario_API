using Inventario.Controllers;
using Inventario.Data;
using Inventario.Authorization;
using Inventario.Services;
using Inventario.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Inventario.Authorization;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using Inventario.AutoMapperConfig;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de DbContext

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAutoMapper(typeof(AutoMapperConfigProfile));
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("MemoryDB"));

// builder.Services.AddIdentityApiEndpoints<IdentityUser>()
//     .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// Add Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Config Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase= false;
    options.Password.RequireUppercase= false;
    options.Password.RequireNonAlphanumeric= false;
    options.SignIn.RequireConfirmedEmail = false;
});

// Autenticacion
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata= false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:ValidIssuer"],
            ValidAudience = builder.Configuration["JwtSettings:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };
    });

    // Inject app Dependencies (Dependency Injection)
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your token with this format: ''Bearer YOUR_TOKEN''",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
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

// builder.Services.AddScoped<LoginController>();
builder.Services.AddScoped<IAuthService, AuthService>(); 
// builder.Services.AddScoped<Ihost, host>(); 
// builder.Services.AddAuthorization();
// builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
//     {
//         options.Authority = "http://localhost:5173"; // Servidor Local de React para autenticar el token
//         options.Audience = "dispositivos"; // Audiencia esperada del token (puede ser el nombre de tu API)
//         options.SaveToken = true;
//         options.RequireHttpsMetadata = false;
//         var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkExOUEwNTA0OCIsInN1YiI6IkExOUEwNTA0OCIsImp0aSI6IjIwNDcxMTQ4IiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6MTAzMDgiLCJodHRwczovL2xvY2FsaG9zdDo0NDM0NiIsImh0dHA6Ly9sb2NhbGhvc3Q6NTE5OCIsImh0dHBzOi8vbG9jYWxob3N0OjcyNDAiXSwibmJmIjoxNzA0ODI0NjQ0LCJleHAiOjE3MTI2ODcwNDQsImlhdCI6MTcwNDgyNDY0NywiaXNzIjoiZG90bmV0LXVzZXItand0cyJ9.Qz3vx7gEdDZjuG1L6HGebH6nZ1zkVQUtDkoCskTC8q0";
//         var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
//         var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
//         // Configura la validación del token 
//         options.TokenValidationParameters = new TokenValidationParameters()
//         {
//             ValidateIssuer = false, // Valida el emisor del token
//             ValidateAudience = false, // Valida la audiencia del token
//             ValidateLifetime = true, // Valida la fecha de expiración del token
//             ValidateIssuerSigningKey = true, // Valida la firma del token
//             IssuerSigningKey = signingKey
//         };
//     });

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
// app.MapIdentityApi<IdentityUser>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapSwagger().RequireAuthorization();
app.MapControllers();
app.Run();
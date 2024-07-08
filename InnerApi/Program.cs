using InnerApi;
using InnerApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// services di
builder.Services.AddSingleton<IK8SClientService, K8SClientService>();

// Add authentication services
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// swagger add basic auth part
AddCustomSwagger(builder);

// disable cors
builder.Services.AddCors(x =>
    x.AddPolicy("AllowAll", c =>
    {
        c.AllowAnyOrigin();
        c.AllowAnyMethod();
        c.AllowAnyHeader();
    })
);  

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void AddCustomSwagger(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "InnerApi", Version = "v1" });

        // Define the basic security scheme
        c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
        {
            Description = "Basic Authorization header. Enter 'Basic' [space] and then your username and password in the format username:password base64 encoded.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "basic"
        });

        // Apply the security scheme globally to all API endpoints
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Basic"
                    }
                },
                new string[] {}
            }
        });
    });
}
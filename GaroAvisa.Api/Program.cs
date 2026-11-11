using System.Reflection;
using System.Text;
using GaroAvisa.Api.Extensions;
using GaroAvisa.Api.Handlers;
using GaroAvisa.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados, repositórios, serviços e token (camada Infrastructure)
builder.Services.AddInfrastructure(builder.Configuration);

// JWT: a chave vem da configuração local ou das variáveis de ambiente, nunca do código.
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Rate limiting: limita a quantidade de pedidos por minuto (responde 429 quando passa)
builder.Services.AddRateLimitingGaroAvisa(builder.Configuration);

// Health check de vivacidade (tag "live"): só confirma que a API está de pé.
// Os de prontidão (Oracle e Open-Meteo) são registrados no AddInfrastructure.
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

// Tratamento global de erros: resposta curta e amigável em vez do erro técnico
builder.Services.AddExceptionHandler<TratamentoErrosHandler>();
builder.Services.AddProblemDetails();

// Controllers e documentação
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Scalar: documento OpenAPI com o botão para informar o token
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "GaroAvisa API";
        document.Info.Version = "v1";

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Informe o token JWT gerado no login."
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

// Swagger: mesma configuração do token
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe o token JWT gerado no login."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

var app = builder.Build();

// Deve ser o primeiro da lista, para pegar erros de todas as etapas seguintes.
app.UseExceptionHandler();

// A documentação fica disponível também na nuvem, para os testes da entrega.
app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("GaroAvisa API");
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

// Liveness: responde 200 se a API não estiver travada.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
}).DisableRateLimiting();

// Readiness: verifica o Oracle e a Open-Meteo e mostra o resultado de cada um em JSON.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).DisableRateLimiting();

app.Run();

// Necessário para os testes de integração (WebApplicationFactory<Program>) enxergarem a API.
public partial class Program { }
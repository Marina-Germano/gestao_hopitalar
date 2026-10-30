using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Infra.EF;
using GestaoHospitalarApi.Infra.Repositories;
using GestaoHospitalarApi.Application.Authentication;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)
            )
        };
    });

// 1. Configurar Conexão do MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



// =========== INJEÇÃO DE DEPENDÊNCIA ===========
// Aplicação
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IConvenioRepository, ConvenioRepository>();
builder.Services.AddScoped<IEspecialidadeRepository, EspecialidadeRepository>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<ITriagemRepository, TriagemRepository>();
builder.Services.AddScoped<ILeitoRepository, LeitoRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();
builder.Services.AddScoped<IAlaRepository, AlaRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
builder.Services.AddScoped<IExameRepository, ExameRepository>();
builder.Services.AddScoped<ISolicitacaoExameRepository, SolicitacaoExameRepository>();
builder.Services.AddScoped<IPrescricaoRepository, PrescricaoRepository>();
builder.Services.AddScoped<IConsumoItemRepository, ConsumoItemRepository>();
builder.Services.AddScoped<IInteracaoRepository, InteracaoRepository>();
builder.Services.AddScoped<IInternacaoRepository, InternacaoRepository>();
builder.Services.AddScoped<IPacienteConvenioRepository, PacienteConvenioRepository>();
builder.Services.AddScoped<IFaturamentoRepository, FaturamentoRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IConvenioService, ConvenioService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IEspecialidadeService, EspecialidadeService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<ITriagemService, TriagemService>();
builder.Services.AddScoped<ILeitoService, LeitoService>();
builder.Services.AddScoped<ISalaService, SalaService>();
builder.Services.AddScoped<IAlaService, AlaService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
builder.Services.AddScoped<IExameService, ExameService>();
builder.Services.AddScoped<IPrescricaoService, PrescricaoService>();
builder.Services.AddScoped<IConsumoItemService, ConsumoItemService>();
builder.Services.AddScoped<IInteracaoService, InteracaoService>();
builder.Services.AddScoped<IInternacaoService, InternacaoService>();
builder.Services.AddScoped<IPacienteConvenioService, PacienteConvenioService>();
builder.Services.AddScoped<IFaturamentoService, FaturamentoService>();
builder.Services.AddScoped<JwtService, JwtService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddMemoryCache();


// =========== CONFIGURAÇÃO DO SWAGGER ===========
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("PermitirTudo");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

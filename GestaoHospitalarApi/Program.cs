using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Infra.EF;

var builder = WebApplication.CreateBuilder(args);


// =========== INJEÇÃO DE DEPENDÊNCIA ===========
// Aplicação
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// 1. Configurar Conexão do MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("11.8.2-mariadb")));


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Registrar o Repositório Genérico
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

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
builder.Services.AddEndpointsApiExplorer();
// =========== CONFIGURAÇÃO DO SWAGGER ===========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("PermitirTudo");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

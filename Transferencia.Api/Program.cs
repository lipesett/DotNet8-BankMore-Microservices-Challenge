using Microsoft.EntityFrameworkCore;
using Transferencia.Api.Infrastructure;
using Transferencia.Api.Application.Abstractions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TransferenciaDbContext>(options =>
    options.UseSqlite(connectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("ContaCorrenteClient", client =>
{
    var serviceUrl = builder.Configuration["ServiceUrls:ContaCorrenteApi"];
    if (string.IsNullOrEmpty(serviceUrl))
    {
        throw new InvalidOperationException("URL da ContaCorrenteApi não está configurada.");
    }
    client.BaseAddress = new Uri(serviceUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

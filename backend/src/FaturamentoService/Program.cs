using Microsoft.EntityFrameworkCore;
using FaturamentoService.Data;
using FaturamentoService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<FaturamentoContext>(options => options.UseSqlite("Data Source=faturamento.db"));
builder.Services.AddHttpClient<EstoqueServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FaturamentoContext>();
    db.Database.EnsureCreated();
}

app.Run();

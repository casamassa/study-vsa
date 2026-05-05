using Microsoft.EntityFrameworkCore;
using SuperTodo.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Registrando o MediatR todos os Handlers da solution
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// 2. Configura o Banco de Dados em Memória
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("SuperTodoDb"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();


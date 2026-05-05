using System;
using Microsoft.EntityFrameworkCore;
using SuperTodo.API.Domain.Entities;

namespace SuperTodo.API.Infrastructure;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

    public DbSet<Tarefa> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aqui dizemos ao EF que a entidade Tarefa tem uma chave
        modelBuilder.Entity<Tarefa>().HasKey(t => t.Id);

        // Como o Id tem 'private set', o EF consegue lidar com isso via Reflection
        base.OnModelCreating(modelBuilder);
    }
}

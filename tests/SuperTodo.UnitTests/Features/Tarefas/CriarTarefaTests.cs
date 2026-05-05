using System;
using Microsoft.EntityFrameworkCore;
using SuperTodo.API.Domain.Enums;
using SuperTodo.API.Features.Tarefas.CriarTarefa;
using SuperTodo.API.Infrastructure;

namespace SuperTodo.UnitTests.Features.Tarefas;

public class CriarTarefaTests
{
    [Fact]
    public async Task Handle_DadosValidos_DeveSalvarNoBanco()
    {
        // ARRANGE
        // No VSA, como usamos o DbContext direto, o melhor é usar o InMemoryDatabase no teste também
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: "TesteCriacao")
            .Options;

        using var context = new TodoContext(options);
        var handler = new CriarTarefaHandler(context);
        var command = new CriarTarefaCommand("Nova Tarefa", "Descricao", Prioridade.Media);

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, result);
        Assert.Equal(1, await context.Tarefas.CountAsync());
    }
}

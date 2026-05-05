using System;
using MediatR;
using SuperTodo.API.Infrastructure;

namespace SuperTodo.API.Features.Tarefas.ConcluirTarefa;

// O Comando: precisamos apenas do ID da tarefa
public record ConcluirTarefaCommand(Guid Id) : IRequest;

// O Handler - Note que injetamos o TodoContext diretamente!
public class ConcluirTarefaHandler(TodoContext context) : IRequestHandler<ConcluirTarefaCommand>
{
    public async Task Handle(ConcluirTarefaCommand request, CancellationToken ct)
    {
        // No VSA, buscamos direto pelo contexto
        var tarefa = await context.Tarefas.FindAsync([request.Id], ct);
        if (tarefa == null) throw new Exception("Tarefa não encontrada");

        // Usamos o método do Domínio para garantir a lógica de conclusão
        tarefa.Concluir();
        // Salva direto usando o Contexto
        await context.SaveChangesAsync(ct);
    }
}


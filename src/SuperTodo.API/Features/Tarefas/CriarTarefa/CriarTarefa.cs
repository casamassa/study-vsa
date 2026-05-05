using System;
using MediatR;
using SuperTodo.API.Domain.Entities;
using SuperTodo.API.Domain.Enums;
using SuperTodo.API.Infrastructure;

namespace SuperTodo.API.Features.Tarefas.CriarTarefa;

// O Comando
public record CriarTarefaCommand(string Titulo, string Descricao, Prioridade Prioridade) : IRequest<Guid>;

// O Handler - Note que injetamos o TodoContext diretamente!
public class CriarTarefaHandler(TodoContext context) : IRequestHandler<CriarTarefaCommand, Guid>
{
    public async Task<Guid> Handle(CriarTarefaCommand request, CancellationToken ct)
    {
        // 1. Usa o domínio para validar e criar
        var tarefa = Tarefa.Criar(request.Titulo, request.Descricao, request.Prioridade);

        // 2. Salva direto usando o Contexto
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync(ct);

        return tarefa.Id;
    }
}
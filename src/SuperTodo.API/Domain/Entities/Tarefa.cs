using System;
using SuperTodo.API.Domain.Enums;

namespace SuperTodo.API.Domain.Entities;

public class Tarefa
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public Prioridade Prioridade { get; private set; }
    public bool Concluida { get; private set; }
    public DateTime? DataConclusao { get; private set; }

    // Construtor privado para forçar o uso do método de criação (Fábrica)
    private Tarefa(string titulo, string descricao, Prioridade prioridade)
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
        Concluida = false;
    }

    // Regra de Negócio (DDD): Encapsulando a criação
    public static Tarefa Criar(string titulo, string descricao, Prioridade prioridade)
    {
        if (string.IsNullOrWhiteSpace(titulo)) throw new Exception("Título obrigatório");

        if (prioridade == Prioridade.Alta && string.IsNullOrWhiteSpace(descricao))
            throw new Exception("Tarefas de alta prioridade exigem uma descrição!");

        return new Tarefa(titulo, descricao, prioridade);
    }

    public void Concluir()
    {
        Concluida = true;
        DataConclusao = DateTime.Now;
    }
}

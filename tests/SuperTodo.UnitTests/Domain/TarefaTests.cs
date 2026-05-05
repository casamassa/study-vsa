using System;
using SuperTodo.API.Domain.Entities;
using SuperTodo.API.Domain.Enums;

namespace SuperTodo.UnitTests.Domain;

public class TarefaTests
{
    [Fact]
    public void Criar_TarefaAltaSemDescricao_DeveRetornarErro()
    {
        // Assert & Act
        var ex = Assert.Throws<Exception>(() =>
            Tarefa.Criar("Teste", "", Prioridade.Alta));

        Assert.Equal("Tarefas de alta prioridade exigem uma descrição!", ex.Message);
    }

    [Fact]
    public void Concluir_Tarefa_DeveDefinirDataConclusao()
    {
        // Arrange
        var tarefa = Tarefa.Criar("Estudar", "Descricao", Prioridade.Media);

        // Act
        tarefa.Concluir();

        // Assert
        Assert.True(tarefa.Concluida);
        Assert.NotNull(tarefa.DataConclusao);
    }
}

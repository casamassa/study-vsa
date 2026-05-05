# Estudo do Vertical Slice Arch (VSA)

Esse projeto surgiu como uma ideia de refatorar outro projeto que usava outra arquitetura (Clean Arch https://github.com/casamassa/study-arch-clean-ddd), assim ficou mais fácil de assimilar a diferença entre as 2 arquiteturas.

Mudando a mentalidade: em vez de organizar o código como no Clean Arch por "o que a classe é" (Repositório, Serviço, Controller), organiza-se por "o que o sistema faz" (Criar Tarefa, Concluir Tarefa). No VSA, cada funcionalidade é uma fatia vertical completa. Se uma fatia precisar de banco de dados e a outra não, elas são independentes.

## Por que VSA é melhor que a Clean Arch?

1. Mudança sem medo: Se precisar mudar a lógica de "Criar Tarefa", abre-se uma única pasta e altera o Handler. Não corre o risco de quebrar o "Concluir Tarefa", pois eles não compartilham mais o mesmo TarefaService.
2. Fim do Construtor Gigante: Se a criação de tarefa precisar de um serviço de E-mail, você injeta o e-mail apenas no CriarTarefaHandler. O Controller continua limpo, recebendo apenas o Mediator.
3. Acoplamento por funcionalidade: O código está agrupado pelo que ele faz (negócio) e não pelo que ele é (técnico).

## Organização do projeto

Para VSA, a tendência é o Minimalismo. Menos projetos significa menos "vai e vem" de referências. Para o meu estudo, usei apenas dois projetos:

1. SuperTodo.Web (ASP.NET Core API): Aqui fica tudo. Sim, a UI (Controllers), a Infra (Contexto/EF) e as Features (Handlers/Business).
2. SuperTodo.UnitTests (xUnit): Para manter os testes separados do código de produção.

```Text
src/
 └── SuperTodo.API/
      ├── Domain/             <-- (Músculos: Tarefa.cs, Prioridade.cs)
      ├── Infrastructure/     <-- (Dados: TodoContext.cs)
      ├── Features/           <-- (O Coração do VSA)
      │    ├── Tarefas/
      │    │    ├── CriarTarefa/
      │    │    │    ├── CriarTarefaCommand.cs
      │    │    │    └── CriarTarefaHandler.cs
      │    │    └── ConcluirTarefa/
      │    │         ├── ConcluirTarefaCommand.cs
      │    │         └── ConcluirTarefaHandler.cs
      └── Program.cs
tests/
 └── SuperTodo.UnitTests/
```

### No mercado: 1 projeto ou vários?

A escolha padrão para VSA é manter tudo em um único projeto de API.

- Por que? Porque o isolamento não é feito por "projetos", mas sim por Pastas. Se você precisar trocar a API por outra coisa no futuro, você move as pastas de Features.
- Exceção: Se você tem regras de negócio que realmente precisam ser compartilhadas com outros sistemas (como um App Mobile e um Worker), aí você cria um projeto de Domain separado. Caso contrário, mantenha no projeto da API.

### VSA usa DDD?

Sim, mas de um jeito mais "pragmático".

- Na Clean Arch: O DDD é obrigatório em tudo.
- No VSA: Você usa o "músculo" do DDD (Entidades ricas, Value Objects) apenas onde a lógica é complexa.
- O segredo: No VSA, se uma fatia é um simples "Listar todas as tarefas", você não precisa de Repositório ou Entidade complexa; você faz um SELECT direto no banco dentro do Handler. Mas, se a fatia é "Criar Tarefa com validação de prioridade", você usa suas classes de Domínio (que ficam em uma pasta Domain dentro do próprio projeto da API).

## Notas da Refatoração Clean Arch -> VSA:

- O código fica muito mais "direto ao ponto" no VSA. Como agora temos tudo em um único projeto, a barreira entre as camadas sumiu, mas a organização por pastas mantém tudo limpo.
- O Domínio (Ainda usamos o "Músculo"): Mesmo no VSA, as regras de negócio complexas devem ficar na Entidade. Na pasta Domain mantive a classe Tarefa.cs e o enum Prioridade.cs que criei no projeto Clean Arch. O código é o mesmo: o método Criar valida a prioridade alta.
- A Infraestrutura (Direto na API): Na pasta Infrastructure mantive apenas o TodoContext.cs lá.
- A "Fatia" de Criação (VSA Puro): Aqui está a grande mudança. No VSA, não se usa Repositório. O Handler fala direto com o banco de dados (ou com o que ele precisar). Criei a pasta Features/Tarefas/CriarTarefa/ e o arquivo CriarTarefa.cs (no VSA é comum colocar o Command e o Handler no mesmo arquivo para facilitar a leitura)

### Por que isso não é "errado"?

Quem vem da Clean Architecture pode pensar: "Mas agora o Handler está acoplado ao Entity Framework!".

A resposta do VSA é: Sim, e isso é bom! Se essa funcionalidade específica (Criar Tarefa) sempre salvará no SQL, não há razão para criar uma interface de repositório que só tem um único uso. Se amanhã você precisar trocar o banco de apenas uma funcionalidade por causa de performance, você altera apenas aquele Handler, sem afetar o resto do sistema.

### O Desafio da "Fatia de Consulta"

Para ver o poder do VSA, imagine uma funcionalidade de Relatório de Tarefas. Na Clean Arch, você teria que criar DTOs e métodos no Repositório. No VSA, você cria uma pasta Features/Tarefas/ListarTarefas, cria um Handler e faz um context.Tarefas.Select(...).ToListAsync(). Simples assim.

### Por que VSA foi mais simples que na Clean Arch?

1. Sem Interfaces Extras: Você não precisou adicionar um método Atualizar ou ObterPorId em uma interface de repositório e depois implementá-lo.
2. Independência Total: Se você quiser deletar a funcionalidade de "Criar Tarefa" amanhã, a de "Concluir" continua funcionando intacta.
3. Localidade: Se houver um bug na conclusão, você sabe que o erro está exclusivamente naquela pasta ou no método Concluir da Entidade.

### Quando usar um ou o outro?

- Clean Architecture: Excelente para grandes times e sistemas onde a padronização técnica e a troca de tecnologias (como banco de dados) são prioridades reais.
- VSA: Excelente para agilidade, facilidade de manutenção e para evitar que o código vire um "emaranhado" de serviços gigantes.

## Por que usei o MediatoR?

O Mediator não é obrigatório, mas ele é o que faz o VSA funcionar tão bem. Poderia fazer VSA sem ele, mas teria que injetar cada Handler manualmente no Controller, o que mataria a agilidade do padrão.

Aqui está o real papel dele no VSA:

1. Desacoplamento de Dependências (O principal motivo)

   Sem o Mediator, se tiver 20 fatias (funcionalidades), seu Controller precisaria de 20 Handlers ou Serviços injetados no construtor. Seria um caos.
   - Com Mediator: O Controller só conhece uma coisa: o IMediator. Ele despacha o comando e o Mediator se vira para achar quem resolve.

2. Criação de um "Funil" Único

   Como toda funcionalidade passa pelo mediator.Send(), ganha-se um lugar central para colocar comportamentos que valem para o sistema todo sem repetir código. Isso é feito via Behaviors (como se fossem Middlewares do MediatR).
   - Exemplo: pode-se criar um Behavior que faz Log de toda requisição ou um que faz a Validação de todos os comandos automaticamente antes de chegarem ao Handler.

3. Localidade de Código

   O Mediator permite que o Handler seja uma classe isolada que não precisa de uma interface (IService) para ser chamada. Isso reforça a ideia da "fatia vertical": o comando e o executor são um par único.

### E se eu NÃO quiser usar MediatR?

Pode fazer VSA usando Minimal APIs do .NET (aquelas que ficam direto no Program.cs ou em módulos). Nelas, pode-se injetar o Handler direto no método da rota, eliminando o Mediator:

```bash
//Exemplo sem Mediator usando Minimal API
app.MapPost("/api/tarefas", async (CriarTarefaHandler handler, CriarTarefaCommand cmd) =>
{
    return await handler.Handle(cmd);
});
```

### Resumo:

- Use MediatR se prefere organizar tudo em Controllers e quer um construtor limpo e extensível (Behaviors).
- Não use se achar que o "vai e vem" entre o comando e o handler adiciona complexidade desnecessária para o tamanho do projeto.

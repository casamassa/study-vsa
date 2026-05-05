using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperTodo.API.Features.Tarefas.ConcluirTarefa;
using SuperTodo.API.Features.Tarefas.CriarTarefa;

namespace SuperTodo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarefasController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Criar(CriarTarefaCommand command)
        {
            try
            {
                // Você envia o comando para o "buraco negro" do MediatR 
                // e ele se vira para achar o Handler correto. O Controller não sabe o que acontece, ele apenas "manda" para o Mediator
                var id = await mediator.Send(command);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/concluir")]
        public async Task<IActionResult> Concluir(Guid id)
        {
            try
            {
                // Criamos o comando com o ID da URL e enviamos
                await mediator.Send(new ConcluirTarefaCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

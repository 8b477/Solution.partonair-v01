using BLL.partonair_v01.MediatR.Commands.Evaluations;
using BLL.partonair_v01.MediatR.Queries.Evaluations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;


namespace API.partonair_v01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByGuidAsync(Guid id)
        {
            var evaluation = await _mediator.Send(new GetByGuidEvaluationQuery(id));

            return
                  evaluation is null
                ? NoContent()
                : Ok(new { evaluation });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var evaluations = await _mediator.Send(new GetAllEvaluationFilteredbyUserQuery());

            return
                  evaluations is null
                ? NoContent()
                : Ok(new { evaluations });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromQuery] Guid idOwner, EvaluationCreateDTO eval)
        {
            var evaluationCreated = await _mediator.Send(new AddEvaluationCommand(idOwner, eval));

            return Ok(evaluationCreated);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAsync(Guid idEval, EvaluationUpdateDTO eval)
        {
            var evaluationCreated = await _mediator.Send(new UpdateEvaluationCommand(idEval, eval));

            return Ok(evaluationCreated);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteEvaluationCommand(id));

            return NoContent();
        }
    }
}

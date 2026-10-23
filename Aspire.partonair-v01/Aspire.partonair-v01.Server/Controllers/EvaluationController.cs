using API.partonair_v01.Hubs;
using API.partonair_v01.Token;
using BLL.partonair_v01.MediatR.Commands.Evaluations;
using BLL.partonair_v01.MediatR.Queries.Evaluations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SharedModels.partonair_v01.DTOS;


namespace API.partonair_v01.Controllers
{
    /// <summary>
    /// Avis entre membres. L'auteur est toujours déduit du JWT, jamais du corps de la requête.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvaluationController(IMediator mediator, IHttpContextAccesor httpContextAccesor, IHubContext<RealtimeHub> hubContext) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IHttpContextAccesor _httpContextAccesor = httpContextAccesor;
        private readonly IHubContext<RealtimeHub> _hubContext = hubContext;


        /// <summary>
        /// Avis reçus par un membre : moyenne, nombre, liste, mon avis éventuel et si je peux l'évaluer.
        /// </summary>
        /// <param name="userId">Identifiant du membre évalué.</param>
        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(typeof(UserEvaluationsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForUserAsync(Guid userId)
        {
            if (!TryGetCurrentUserId(out var viewerId))
                return Unauthorized();

            return Ok(await _mediator.Send(new GetUserEvaluationsQuery(viewerId, userId)));
        }

        /// <summary>
        /// Crée ou modifie mon avis sur un membre (note 0-5 + commentaire).
        /// </summary>
        /// <remarks>Réservé aux amis non bloqués. Un seul avis par membre évalué.</remarks>
        /// <param name="userId">Identifiant du membre évalué.</param>
        /// <param name="evaluation">Note et commentaire.</param>
        /// <response code="200">Succès - Avis enregistré.</response>
        /// <response code="409">Pas ami avec ce membre, ou tentative d'auto-évaluation.</response>
        [HttpPut("user/{userId:guid}")]
        [ProducesResponseType(typeof(EvaluationUpsertResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpsertAsync(Guid userId, EvaluationUpsertDTO evaluation)
        {
            if (!TryGetCurrentUserId(out var senderId))
                return Unauthorized();

            var result = await _mediator.Send(new UpsertEvaluationCommand(senderId, userId, evaluation));

            await _hubContext.Clients.User(userId.ToString()).SendAsync("EvaluationReceived", result);

            return Ok(result);
        }

        /// <summary>
        /// Supprime mon avis sur un membre.
        /// </summary>
        /// <param name="userId">Identifiant du membre évalué.</param>
        /// <response code="204">Succès - Avis supprimé.</response>
        /// <response code="404">Aucun avis de ma part sur ce membre.</response>
        [HttpDelete("user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMineAsync(Guid userId)
        {
            if (!TryGetCurrentUserId(out var senderId))
                return Unauthorized();

            await _mediator.Send(new DeleteMyEvaluationCommand(senderId, userId));

            await _hubContext.Clients.User(userId.ToString()).SendAsync("EvaluationRemoved", new { senderId });

            return NoContent();
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            return Guid.TryParse(_httpContextAccesor.UserId(), out userId);
        }
    }
}

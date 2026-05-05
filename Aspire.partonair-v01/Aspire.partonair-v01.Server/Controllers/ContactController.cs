using BLL.partonair_v01.MediatR.Commands.Contacts;
using BLL.partonair_v01.MediatR.Queries.Contacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;

 
namespace API.partonair_v01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;



        /// <summary>
        /// Récupère la liste complète des contacts.
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne tous les contacts actifs de la base de données.
        /// Utilisez les query parameters pour la pagination si nécessaire (non implémenté ici).
        /// Exemple de réponse: [{"id":1,"name":"John Doe","email":"john@example.com"}]
        /// </remarks>
        /// <response code="200">Succès - Liste des contacts retournée.</response>
        /// <response code="500">Erreur interne serveur (ex: problème base de données).</response>
        /// <returns>Une collection de ContactViewDTO.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<ContactViewDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllAsync()
        {
            var result = await _mediator.Send(new GetAllContactQuery());
            return Ok(result);
        }

        [HttpGet("PendingStatus")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllPendingStatusAsync(string status)
        {
            var result = await _mediator.Send(new GetAllPendingStatusQuery(status));
            return Ok(result);
        }

        /// <summary>
        /// Récupère tous les contacts avec le statut "Accepté".
        /// </summary>
        /// <remarks>
        /// Cet endpoint retourne les contacts dont la demande a été acceptée.
        /// </remarks>
        /// <param name="status">Le statut à filtrer (généralement "Accepted").</param>
        /// <response code="200">Succès - Liste des contacts acceptés retournée.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>Une collection de ContactViewDTO avec statut "Accepté".</returns>
        [HttpGet("AcceptedStatus")]
        [ProducesResponseType(typeof(ICollection<ContactViewDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllAcceptedAsync(string status)
        {
            var result = await _mediator.Send(new GetAllAcceptedStatusQuery(status));
            return Ok(result);
        }

        [HttpGet("BlockedStatus")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllBlockedAsync(string status)
        {
            var result = await _mediator.Send(new GetAllBlockedStatusQuery(status));
            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetByGuidAsync(Guid id)
        {
            var result = await _mediator.Send(new GetByGuidContactQuery(id));
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid idSender, Guid idRecieper)
        {
            await _mediator.Send(new DeleteContactCommand(idSender, idRecieper));

            return NoContent();
        }

        /// <summary>
        /// Accepte une demande de contact en attente.
        /// </summary>
        /// <remarks>
        /// Cet endpoint met à jour le statut d'une demande de contact de "En attente" à "Acceptée".
        /// </remarks>
        /// <param name="idContact">L'identifiant du contact à accepter.</param>
        /// <response code="200">Succès - Demande acceptée.</response>
        /// <response code="400">Requête invalide - Identifiant manquant ou invalide.</response>
        /// <response code="500">Erreur interne serveur.</response>
        /// <returns>La réponse de l'opération d'acceptation.</returns>
        [HttpPatch(nameof(AcceptedInvitation))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptedInvitation([FromQuery] Guid idContact)
        {
            var response = await _mediator.Send(new AcceptedRequestContactCommand(idContact));
            return Ok(new { response });
        }

        [HttpPatch(nameof(RefusedInvitation))]
        public async Task<IActionResult> RefusedInvitation([FromQuery] Guid idContact)
        {
            var response = await _mediator.Send(new RefusedRequestCommand(idContact));
            return Ok(new { response });
        }

        [HttpPatch(nameof(LockContact))]
        public async Task<IActionResult> LockContact([FromQuery] Guid idSender, UserToLock userToLock)
        {
            var response = await _mediator.Send(new LockContactRequestCommand(idSender, userToLock));
            return Ok(new { response });
        }

        [HttpPatch(nameof(UnlockContact))]
        public async Task<IActionResult> UnlockContact([FromQuery] Guid idSender, UserToUnlock idUserToBlock)
        {
            var response = await _mediator.Send(new UnlockContactRequestCommand(idSender, idUserToBlock));
            return Ok(new { response });
        }

    }
}

using BLL.partonair_v01.MediatR.Commands.Contacts;
using BLL.partonair_v01.MediatR.Queries.Contacts;
using API.partonair_v01.Hubs;
using API.partonair_v01.Token;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SharedModels.partonair_v01.DTOS;

 
namespace API.partonair_v01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController(IMediator mediator, IHttpContextAccesor httpContextAccesor, IHubContext<RealtimeHub> hubContext) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IHttpContextAccesor _httpContextAccesor = httpContextAccesor;
        private readonly IHubContext<RealtimeHub> _hubContext = hubContext;



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
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(typeof(ICollection<ContactViewDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllAsync()
        {
            var result = await _mediator.Send(new GetAllContactQuery());
            return Ok(result);
        }

        [HttpGet("PendingStatus")]
        [Authorize(Policy = "RequireAdminRole")]
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
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(typeof(ICollection<ContactViewDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllAcceptedAsync(string status)
        {
            var result = await _mediator.Send(new GetAllAcceptedStatusQuery(status));
            return Ok(result);
        }

        [HttpGet("BlockedStatus")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllBlockedAsync(string status)
        {
            var result = await _mediator.Send(new GetAllBlockedStatusQuery(status));
            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetByGuidAsync(Guid id)
        {
            var result = await _mediator.Send(new GetByGuidContactQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequestAsync(ContactCreateDTO request)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var created = await _mediator.Send(new AddContactCommand(userId, request.Id_Receiver));

            await NotifyFriendRequestAsync(request.Id_Receiver, created.ContactId);

            return Ok(new { contact = created });
        }

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriendsAsync()
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();
            return Ok(new { contacts = await _mediator.Send(new GetMyFriendsQuery(userId)) });
        }

        [HttpGet("requests/incoming")]
        public async Task<IActionResult> GetIncomingRequestsAsync()
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();
            return Ok(new { contacts = await _mediator.Send(new GetMyIncomingRequestsQuery(userId)) });
        }

        [HttpGet("requests/outgoing")]
        public async Task<IActionResult> GetOutgoingRequestsAsync()
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();
            return Ok(new { contacts = await _mediator.Send(new GetMyOutgoingRequestsQuery(userId)) });
        }

        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedContactsAsync()
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();
            return Ok(new { contacts = await _mediator.Send(new GetMyBlockedContactsQuery(userId)) });
        }

        /// <summary>
        /// Contacts de la messagerie : amis + relations bloquées (Status = Accepted, BlockedByMe ou BlockedByOther).
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversationContactsAsync()
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();
            return Ok(new { contacts = await _mediator.Send(new GetMyConversationContactsQuery(userId)) });
        }

        /// <summary>
        /// Relation entre l'utilisateur courant et <paramref name="otherUserId"/> (voir <see cref="RelationshipStatus"/>).
        /// </summary>
        [HttpGet("relationship/{otherUserId:guid}")]
        public async Task<IActionResult> GetRelationshipAsync(Guid otherUserId)
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();
            return Ok(await _mediator.Send(new GetRelationshipQuery(userId, otherUserId)));
        }

        [HttpDelete("{idContact:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid idContact)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            await _mediator.Send(new DeleteContactCommand(userId, idContact));

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
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new AcceptedRequestContactCommand(userId, idContact));
            return Ok(new { response });
        }

        [HttpPatch(nameof(RefusedInvitation))]
        public async Task<IActionResult> RefusedInvitation([FromQuery] Guid idContact)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new RefusedRequestCommand(userId, idContact));
            return Ok(new { response });
        }

        [HttpPatch(nameof(LockContact))]
        public async Task<IActionResult> LockContact(UserToLock userToLock)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new LockContactRequestCommand(userId, userToLock));

            await NotifyRelationshipChangedAsync(userId, userToLock.Id_UserToLock);

            return Ok(new { response });
        }

        [HttpPatch(nameof(UnlockContact))]
        public async Task<IActionResult> UnlockContact(UserToUnlock idUserToBlock)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var response = await _mediator.Send(new UnlockContactRequestCommand(userId, idUserToBlock));

            await NotifyRelationshipChangedAsync(userId, idUserToBlock.Id_UserToUnlock);

            return Ok(new { response });
        }

        /// <summary>
        /// Prévient le destinataire connecté (tous ses onglets) qu'il a reçu une demande.
        /// La charge utile a le même format qu'un élément de « requests/incoming ».
        /// </summary>
        private async Task NotifyFriendRequestAsync(Guid receiverId, Guid contactId)
        {
            var incoming = await _mediator.Send(new GetMyIncomingRequestsQuery(receiverId));
            var request = incoming.FirstOrDefault(contact => contact.ContactId == contactId);

            if (request is not null)
                await _hubContext.Clients.User(receiverId.ToString()).SendAsync("FriendRequestReceived", request);
        }

        /// <summary>
        /// Blocage / déblocage : chacun reçoit l'id de l'autre pour rafraîchir sa messagerie et son réseau.
        /// </summary>
        private async Task NotifyRelationshipChangedAsync(Guid userId, Guid otherUserId)
        {
            await _hubContext.Clients.User(otherUserId.ToString()).SendAsync("RelationshipChanged", new { userId });
            await _hubContext.Clients.User(userId.ToString()).SendAsync("RelationshipChanged", new { userId = otherUserId });
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            return Guid.TryParse(_httpContextAccesor.UserId(), out userId);
        }
    }
}

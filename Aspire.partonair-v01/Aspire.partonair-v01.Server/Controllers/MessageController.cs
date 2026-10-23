using BLL.partonair_v01.MediatR.Commands.Messages;
using BLL.partonair_v01.MediatR.Queries.Messages;
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
    public class MessageController(IMediator mediator, IHttpContextAccesor httpContextAccesor, IHubContext<RealtimeHub> hubContext) : ControllerBase
    {
        private const int DefaultHistorySize = 50;

        private readonly IMediator _mediator = mediator;
        private readonly IHttpContextAccesor _httpContextAccesor = httpContextAccesor;
        private readonly IHubContext<RealtimeHub> _hubContext = hubContext;

        /// <summary>
        /// Récupère l'historique d'une conversation 1:1 entre l'utilisateur courant et un autre utilisateur.
        /// </summary>
        /// <remarks>
        /// Refusé (409) si les deux utilisateurs ne sont pas amis réciproques ou si une relation de blocage existe.
        /// </remarks>
        /// <param name="otherUserId">Identifiant de l'autre participant de la conversation.</param>
        /// <param name="take">Nombre maximum de messages à retourner (les plus récents), par défaut 50.</param>
        [HttpGet("{otherUserId:guid}")]
        public async Task<IActionResult> GetConversationAsync(Guid otherUserId, [FromQuery] int take = DefaultHistorySize)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var conversation = await _mediator.Send(new GetConversationQuery(userId, otherUserId, take));
            return Ok(conversation);
        }

        /// <summary>
        /// Envoie un message privé texte à un autre utilisateur.
        /// </summary>
        /// <remarks>
        /// L'expéditeur est toujours déduit du JWT, jamais du corps de la requête.
        /// Refusé (409) si les deux utilisateurs ne sont pas amis réciproques ou si une relation de blocage existe.
        /// </remarks>
        /// <param name="receiverId">Identifiant du destinataire.</param>
        /// <param name="request">Contenu texte du message.</param>
        [HttpPost("{receiverId:guid}")]
        public async Task<IActionResult> SendMessageAsync(Guid receiverId, SendMessageDTO request)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var message = await _mediator.Send(new SendMessageCommand(userId, receiverId, request.Content));

            // Envoi de secours (hors WebSocket) : on diffuse quand même en temps réel aux deux participants.
            await _hubContext.Clients.Users(userId.ToString(), receiverId.ToString()).SendAsync("MessageReceived", message);

            return Ok(new { message });
        }

        /// <summary>
        /// Nombre de messages non lus, par contact, pour l'utilisateur courant.
        /// </summary>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadAsync()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var conversations = await _mediator.Send(new GetUnreadMessagesQuery(userId));
            return Ok(new { total = conversations.Sum(conversation => conversation.Count), conversations });
        }

        /// <summary>
        /// Marque comme lus tous les messages reçus de <paramref name="otherUserId"/>.
        /// </summary>
        [HttpPost("{otherUserId:guid}/read")]
        public async Task<IActionResult> MarkConversationReadAsync(Guid otherUserId)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            await _mediator.Send(new MarkConversationReadCommand(userId, otherUserId));
            return NoContent();
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            return Guid.TryParse(_httpContextAccesor.UserId(), out userId);
        }
    }
}

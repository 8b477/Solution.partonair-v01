using System.Security.Claims;

using BLL.partonair_v01.MediatR.Commands.Messages;
using Domain.partonair_v01.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace API.partonair_v01.Hubs
{
    /// <summary>
    /// Hub SignalR unique de l'application (messagerie privée 1:1 + notifications). Une seule connexion par session. L'identité de l'expéditeur vient exclusivement
    /// du claim JWT de la connexion, jamais d'un paramètre client. La même garde d'autorisation
    /// (amis réciproques Accepted, non bloqués) que l'API REST est appliquée via <see cref="SendMessageCommand"/>
    /// avant toute persistance ou diffusion.
    /// </summary>
    [Authorize]
    public class RealtimeHub(IMediator mediator) : Hub
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Envoie un message au destinataire indiqué. Persiste d'abord via la couche métier,
        /// puis diffuse uniquement après succès vers l'expéditeur (autres appareils) et le destinataire.
        /// </summary>
        public async Task SendMessage(Guid receiverId, string content)
        {
            if (!TryGetCurrentUserId(out var senderId))
                throw new HubException("Utilisateur non authentifié.");

            try
            {
                var message = await _mediator.Send(new SendMessageCommand(senderId, receiverId, content));

                await Clients.User(senderId.ToString()).SendAsync("MessageReceived", message);
                await Clients.User(receiverId.ToString()).SendAsync("MessageReceived", message);
            }
            catch (ApplicationLayerException ex)
            {
                // Message renvoyé tel quel à l'appelant uniquement (pas de diffusion) : pas de donnée privée dedans.
                throw new HubException(ex.Message);
            }
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out userId);
        }
    }
}

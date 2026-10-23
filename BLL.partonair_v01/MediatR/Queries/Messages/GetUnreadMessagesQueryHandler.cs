using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Messages
{
    public class GetUnreadMessagesQueryHandler(IMessageService messageService) : IRequestHandler<GetUnreadMessagesQuery, ICollection<UnreadConversationDTO>>
    {
        private readonly IMessageService _messageService = messageService;

        public async Task<ICollection<UnreadConversationDTO>> Handle(GetUnreadMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _messageService.GetUnreadAsyncService(request.UserId);
        }
    }
}

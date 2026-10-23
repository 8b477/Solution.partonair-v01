using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.MediatR.Queries.Contacts
{
    public record GetMyFriendsQuery(Guid UserId) : IRequest<ICollection<MyContactViewDTO>>;
    public record GetMyIncomingRequestsQuery(Guid UserId) : IRequest<ICollection<MyContactViewDTO>>;
    public record GetMyOutgoingRequestsQuery(Guid UserId) : IRequest<ICollection<MyContactViewDTO>>;
    public record GetMyBlockedContactsQuery(Guid UserId) : IRequest<ICollection<MyContactViewDTO>>;
    public record GetMyConversationContactsQuery(Guid UserId) : IRequest<ICollection<MyContactViewDTO>>;
    public record GetRelationshipQuery(Guid UserId, Guid OtherUserId) : IRequest<RelationshipDTO>;

    public class GetMyFriendsQueryHandler(IContactService contactService) : IRequestHandler<GetMyFriendsQuery, ICollection<MyContactViewDTO>>
    {
        public Task<ICollection<MyContactViewDTO>> Handle(GetMyFriendsQuery request, CancellationToken cancellationToken) =>
            contactService.GetFriendsAsyncService(request.UserId);
    }

    public class GetMyIncomingRequestsQueryHandler(IContactService contactService) : IRequestHandler<GetMyIncomingRequestsQuery, ICollection<MyContactViewDTO>>
    {
        public Task<ICollection<MyContactViewDTO>> Handle(GetMyIncomingRequestsQuery request, CancellationToken cancellationToken) =>
            contactService.GetIncomingRequestsAsyncService(request.UserId);
    }

    public class GetMyOutgoingRequestsQueryHandler(IContactService contactService) : IRequestHandler<GetMyOutgoingRequestsQuery, ICollection<MyContactViewDTO>>
    {
        public Task<ICollection<MyContactViewDTO>> Handle(GetMyOutgoingRequestsQuery request, CancellationToken cancellationToken) =>
            contactService.GetOutgoingRequestsAsyncService(request.UserId);
    }

    public class GetMyBlockedContactsQueryHandler(IContactService contactService) : IRequestHandler<GetMyBlockedContactsQuery, ICollection<MyContactViewDTO>>
    {
        public Task<ICollection<MyContactViewDTO>> Handle(GetMyBlockedContactsQuery request, CancellationToken cancellationToken) =>
            contactService.GetBlockedContactsAsyncService(request.UserId);
    }

    public class GetMyConversationContactsQueryHandler(IContactService contactService) : IRequestHandler<GetMyConversationContactsQuery, ICollection<MyContactViewDTO>>
    {
        public Task<ICollection<MyContactViewDTO>> Handle(GetMyConversationContactsQuery request, CancellationToken cancellationToken) =>
            contactService.GetConversationContactsAsyncService(request.UserId);
    }

    public class GetRelationshipQueryHandler(IContactService contactService) : IRequestHandler<GetRelationshipQuery, RelationshipDTO>
    {
        public Task<RelationshipDTO> Handle(GetRelationshipQuery request, CancellationToken cancellationToken) =>
            contactService.GetRelationshipAsyncService(request.UserId, request.OtherUserId);
    }
}

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public record RefusedRequestCommand(Guid UserId, Guid IdContact) : IRequest<string>;
}
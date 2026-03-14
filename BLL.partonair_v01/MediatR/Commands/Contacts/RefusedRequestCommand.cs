using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public record RefusedRequestCommand(Guid IdContact) : IRequest<string>;
}
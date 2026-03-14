using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public record AcceptedRequestContactCommand(Guid IdContact) : IRequest<string>;
}

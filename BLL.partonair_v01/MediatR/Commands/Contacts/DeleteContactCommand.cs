using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public record DeleteContactCommand(Guid IdSender, Guid IdContact) : IRequest;
}

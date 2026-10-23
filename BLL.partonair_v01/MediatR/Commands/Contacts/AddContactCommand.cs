using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public record AddContactCommand(Guid SenderId, Guid ReceiverId) : IRequest<ContactRequestResultDTO>;
}

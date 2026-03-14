using SharedModels.partonair_v01.DTOS;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public record AddContactCommand(ContactCreateDTO Contact) : IRequest<ContactViewDTO>;
}

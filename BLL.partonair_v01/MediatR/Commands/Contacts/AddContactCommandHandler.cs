using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public class AddContactCommandHandler(IContactService contactService) : IRequestHandler<AddContactCommand, ContactViewDTO>
    {
        private readonly IContactService _contactService = contactService;
        public async Task<ContactViewDTO> Handle(AddContactCommand request, CancellationToken cancellationToken)
        {
            return await _contactService.CreateAsyncService(request.Contact);
        }
    }
}

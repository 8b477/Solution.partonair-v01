using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Contacts
{
    public class GetByEmailContactQueryHandler(IContactService contactService) : IRequestHandler<GetByEmailContactQuery, ContactViewDTO>
    {
        private readonly IContactService _contactService = contactService;
        public async Task<ContactViewDTO> Handle(GetByEmailContactQuery request, CancellationToken cancellationToken)
        {
            return await _contactService.GetByEmailAsyncService(request.Email);
        }
    }
}

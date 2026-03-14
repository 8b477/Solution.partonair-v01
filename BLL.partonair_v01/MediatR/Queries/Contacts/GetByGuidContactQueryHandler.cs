using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Contacts
{
    public class GetByGuidContactQueryHandler(IContactService contactService) : IRequestHandler<GetByGuidContactQuery, ContactViewDTO>
    {
        private readonly IContactService _contactService = contactService;
        public async Task<ContactViewDTO> Handle(GetByGuidContactQuery request, CancellationToken cancellationToken)
        {
            return await _contactService.GetByGuidAsyncService(request.Id);
        }
    }
}

using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Contacts
{
    public class GetByNameContactQueryHandler(IContactService contactService) : IRequestHandler<GetByNameContactQuery, ICollection<ContactViewDTO>>
    {
        private readonly IContactService _contactService = contactService;
        public async Task<ICollection<ContactViewDTO>> Handle(GetByNameContactQuery request, CancellationToken cancellationToken)
        {
            return await _contactService.GetByNameAsyncService(request.Name);
        }
    }
}

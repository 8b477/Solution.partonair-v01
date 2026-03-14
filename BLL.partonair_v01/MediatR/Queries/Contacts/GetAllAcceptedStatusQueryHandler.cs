using SharedModels.partonair_v01.DTOS;
using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Contacts
{
    public class GetAllAcceptedStatusQueryHandler(IContactService contactService) : IRequestHandler<GetAllAcceptedStatusQuery, ICollection<ContactViewDTO>>
    {
        private readonly IContactService _contactService = contactService;
        public async Task<ICollection<ContactViewDTO>> Handle(GetAllAcceptedStatusQuery request, CancellationToken cancellationToken)
        {
            return await _contactService.GetAllAcceptedStatusAsyncService(request.Status);
        }
    }
}

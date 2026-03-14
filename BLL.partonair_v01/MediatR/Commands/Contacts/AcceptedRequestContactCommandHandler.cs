using BLL.partonair_v01.Interfaces;
using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public class AcceptedRequestContactCommandHandler(IContactService contactService) : IRequestHandler<AcceptedRequestContactCommand, string>
    {
        protected readonly IContactService _contactService = contactService;
        public async Task<string> Handle(AcceptedRequestContactCommand request, CancellationToken cancellationToken)
        {
            return await _contactService.AcceptedRequestAsyncService(request.IdContact);
        }
    }
}

using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public class RefusedRequestCommandHandler(IContactService contactService) : IRequestHandler<RefusedRequestCommand, string>
    {
        private readonly IContactService _contactService = contactService;
        public async Task<string> Handle(RefusedRequestCommand request, CancellationToken cancellationToken)
        {
            return await _contactService.RefusedRequestAsyncService(request.IdContact);
        }
    }
}

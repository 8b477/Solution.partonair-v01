using BLL.partonair_v01.Interfaces;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Contacts
{
    public class DeleteContactCommandHandler(IContactService contactService) : IRequestHandler<DeleteContactCommand>
    {
        private readonly IContactService _contactService = contactService;
        public Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            return _contactService.DeleteAsyncService(request.IdSender,request.IdContact);
        }
    }
}

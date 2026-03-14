using BLL.partonair_v01.Interfaces;
using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public class DeleteProfileCommandHandler(IProfileService profilService) : IRequestHandler<DeleteProfileCommand>
    {
        private readonly IProfileService _profileService = profilService;
        public Task Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            return  _profileService.DeleteAsyncService(request.Id);
        }
    }
}

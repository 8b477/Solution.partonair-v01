using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public class AddProfileCommandHandler(IProfileService profileService) : IRequestHandler<AddProfileCommand, ProfileViewDTO>
    {
        private readonly IProfileService _profileService = profileService;
        public async Task<ProfileViewDTO> Handle(AddProfileCommand request, CancellationToken cancellationToken)
        {
            return await _profileService.CreateAsyncService(request.IdUser, request.Entity);
        }
    }
}

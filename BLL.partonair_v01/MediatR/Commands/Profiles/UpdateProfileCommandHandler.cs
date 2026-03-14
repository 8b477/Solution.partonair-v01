using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public class UpdateProfileCommandHandler(IProfileService profileService) : IRequestHandler<UpdateProfileCommand, ProfileViewDTO>
    {
        private readonly IProfileService _profileService = profileService;
        public async Task<ProfileViewDTO> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            return await _profileService.UpdateAsyncService(request.Id, request.Profile);
        }
    }
}

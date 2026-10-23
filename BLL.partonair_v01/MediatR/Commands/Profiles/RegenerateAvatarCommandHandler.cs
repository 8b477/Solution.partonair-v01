using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public class RegenerateAvatarCommandHandler(IProfileService profileService) : IRequestHandler<RegenerateAvatarCommand, AvatarRegenerateResultDTO>
    {
        private readonly IProfileService _profileService = profileService;

        public async Task<AvatarRegenerateResultDTO> Handle(RegenerateAvatarCommand request, CancellationToken cancellationToken)
        {
            return await _profileService.RegenerateAvatarAsyncService(request.IdUser);
        }
    }
}

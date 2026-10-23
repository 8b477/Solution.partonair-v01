using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Profiles
{
    public class UpdateMyProfileCommandHandler(IProfileService profileService) : IRequestHandler<UpdateMyProfileCommand, MyProfileViewDTO>
    {
        private readonly IProfileService _profileService = profileService;

        public async Task<MyProfileViewDTO> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            return await _profileService.UpdateMyProfileAsyncService(request.IdUser, request.Profile);
        }
    }
}

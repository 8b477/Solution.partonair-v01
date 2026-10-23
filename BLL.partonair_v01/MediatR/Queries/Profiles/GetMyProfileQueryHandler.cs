using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public class GetMyProfileQueryHandler(IProfileService profileService) : IRequestHandler<GetMyProfileQuery, MyProfileViewDTO>
    {
        private readonly IProfileService _profileService = profileService;

        public async Task<MyProfileViewDTO> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            return await _profileService.GetMyProfileAsyncService(request.IdUser);
        }
    }
}

using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public class GetByIdProfileQueryHandler(IProfileService profileService) : IRequestHandler<GetByIdProfileQuery, ProfileViewDTO>
    {
        private readonly IProfileService _profileService = profileService;
        public async Task<ProfileViewDTO> Handle(GetByIdProfileQuery request, CancellationToken cancellationToken)
        {
            return await _profileService.GetByGuidAsyncService(request.IdUser);
        }
    }
}

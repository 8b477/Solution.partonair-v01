using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public class GetByRoleProfileQueryHandler(IProfileService profileService) : IRequestHandler<GetByRoleProfileQuery, ICollection<ProfileAndUserViewDTO>>
    {
        private readonly IProfileService _profileService = profileService;
        public async Task<ICollection<ProfileAndUserViewDTO>> Handle(GetByRoleProfileQuery request, CancellationToken cancellationToken)
        {
            return await _profileService.GetByRoleAsyncService(request.Role);
        }
    }
}

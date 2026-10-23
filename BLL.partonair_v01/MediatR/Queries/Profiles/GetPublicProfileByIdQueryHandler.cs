using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public class GetPublicProfileByIdQueryHandler(IProfileService profileService)
        : IRequestHandler<GetPublicProfileByIdQuery, PublicProfileViewDTO?>
    {
        private readonly IProfileService _profileService = profileService;

        public async Task<PublicProfileViewDTO?> Handle(
            GetPublicProfileByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _profileService.GetPublicByGuidAsyncService(request.Id, request.ViewerId);
        }
    }
}

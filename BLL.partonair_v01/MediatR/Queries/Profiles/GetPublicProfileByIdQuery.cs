using MediatR;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public record GetPublicProfileByIdQuery(Guid Id, Guid? ViewerId) : IRequest<PublicProfileViewDTO?>;
}

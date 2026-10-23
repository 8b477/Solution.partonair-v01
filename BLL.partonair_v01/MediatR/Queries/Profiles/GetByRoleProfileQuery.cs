using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public record GetByRoleProfileQuery(string Role, Guid? CurrentUserId) : IRequest<ICollection<PublicProfileViewDTO>>;
}

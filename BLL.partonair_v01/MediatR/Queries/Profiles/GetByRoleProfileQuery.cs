using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Profiles
{
    public record GetByRoleProfileQuery(string Role) : IRequest<ICollection<ProfileAndUserViewDTO>>;
}

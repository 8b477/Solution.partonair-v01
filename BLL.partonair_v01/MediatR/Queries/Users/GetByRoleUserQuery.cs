using SharedModels.partonair_v01.DTOS;

using MediatR;


namespace BLL.partonair_v01.MediatR.Queries.Users
{
    public record GetByRoleUserQuery(string Role) : IRequest<ICollection<UserViewDTO>>;
}

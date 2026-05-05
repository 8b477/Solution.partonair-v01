using MediatR;

namespace BLL.partonair_v01.MediatR.Commands.Users
{
    public record ChangeUserRoleCommand(Guid Id, string Role) : IRequest<bool>;
}

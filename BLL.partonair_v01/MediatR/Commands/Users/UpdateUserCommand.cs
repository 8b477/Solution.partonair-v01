using SharedModels.partonair_v01.DTOS;

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Users
{
    public record UpdateUserCommand(Guid Id, UserUpdateNameOrMailOrPasswordDTO User) : IRequest<UserViewDTO>;
}

using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Users
{
    public record DeleteUserCommand(Guid Id) : IRequest;
}

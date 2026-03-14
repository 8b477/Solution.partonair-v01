using BLL.partonair_v01.Interfaces;
using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Users
{
    public class DeleteUserCommandHandler(IUserService userService) : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserService _userService = userService;
        public Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return _userService.DeleteService(request.Id);
        }
    }
}

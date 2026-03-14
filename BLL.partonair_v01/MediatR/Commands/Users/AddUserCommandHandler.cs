using SharedModels.partonair_v01.DTOS;
using MediatR;
using BLL.partonair_v01.Interfaces;


namespace BLL.partonair_v01.MediatR.Commands.Users
{
    public class AddUserCommandHandler(IUserService userService) : IRequestHandler<AddUserCommand, UserViewDTO>
    {
        private readonly IUserService _userService = userService;

        public async Task<UserViewDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateAsyncService(request.User);
        }
    }
}

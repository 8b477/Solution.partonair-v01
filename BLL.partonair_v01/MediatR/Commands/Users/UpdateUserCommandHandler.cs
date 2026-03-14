using SharedModels.partonair_v01.DTOS;

using MediatR;
using BLL.partonair_v01.Interfaces;


namespace BLL.partonair_v01.MediatR.Commands.Users
{
    public class UpdateUserCommandHandler(IUserService userService) : IRequestHandler<UpdateUserCommand, UserViewDTO>
    {
        private readonly IUserService _userService = userService;
        public async Task<UserViewDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateService(request.Id, request.User);
        }
    }
}

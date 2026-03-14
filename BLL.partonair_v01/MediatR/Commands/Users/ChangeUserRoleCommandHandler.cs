//using BLL.partonair_v01.Interfaces;

//using MediatR;


//namespace BLL.partonair_v01.MediatR.Commands.Users
//{
//    public class ChangeUserRoleCommandHandler(IUserService userService) : IRequestHandler<ChangeUserRoleCommand, bool>
//    {
//        private readonly IUserService _userService = userService;
//        public async Task<bool> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
//        {
//            return await _userService.ChangeRoleAsyncService(request.id, request.NewRole);
//        }
//    }
//}

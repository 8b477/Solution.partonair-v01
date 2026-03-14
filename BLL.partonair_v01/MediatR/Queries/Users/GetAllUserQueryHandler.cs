using SharedModels.partonair_v01.DTOS;


using MediatR;
using BLL.partonair_v01.Interfaces;

namespace BLL.partonair_v01.MediatR.Queries.Users
{
    public class GetAllUserQueryHandler(IUserService userService) : IRequestHandler<GetAllUserQuery, ICollection<UserViewDTO>>
    {
        private readonly IUserService _userService = userService;
        public async Task<ICollection<UserViewDTO>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetAllAsyncService();
        }
    }
}

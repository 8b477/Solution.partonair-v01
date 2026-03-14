using SharedModels.partonair_v01.DTOS;

using MediatR;
using BLL.partonair_v01.Interfaces;


namespace BLL.partonair_v01.MediatR.Queries.Users
{
    public class GetByEmailUserQueryHandler(IUserService userService) : IRequestHandler<GetByEmailUserQuery, UserViewDTO>
    {
        private readonly IUserService _userService = userService;
        public async Task<UserViewDTO> Handle(GetByEmailUserQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetByEmailAsyncService(request.Email);
        }
    }
}

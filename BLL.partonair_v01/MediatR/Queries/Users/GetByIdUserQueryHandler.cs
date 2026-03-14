using SharedModels.partonair_v01.DTOS;

using MediatR;
using BLL.partonair_v01.Interfaces;

namespace BLL.partonair_v01.MediatR.Queries.Users
{
    public class GetByIdUserQueryHandler(IUserService userService) : IRequestHandler<GetByIdUserQuery, UserViewDTO?>
    {
        private readonly IUserService _userService = userService;

        public async Task<UserViewDTO> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetByIdAsyncService(request.Id);
        }
    }

}

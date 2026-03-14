using SharedModels.partonair_v01.DTOS;

using MediatR;
using BLL.partonair_v01.Interfaces;

namespace BLL.partonair_v01.MediatR.Queries.Users
{
    public class GetByNameUserQueryHandler(IUserService userService) : IRequestHandler<GetByNameUserQuery, ICollection<UserViewDTO>>
    {
        private readonly IUserService _userService = userService;

        public async Task<ICollection<UserViewDTO>> Handle(GetByNameUserQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetByNameAsyncService(request.Name);
        }
    }
}

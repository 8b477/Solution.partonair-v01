using BLL.partonair_v01.MediatR.Commands.Profiles;
using BLL.partonair_v01.MediatR.Queries.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;


namespace API.partonair_v01.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {


        // <--------------------------------> TODO <-------------------------------->
        // 
        // <--------------------------------> **** <-------------------------------->



        #region DI

        private readonly IMediator _mediator;
        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion



        #region <-------------> CREATE <------------->

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Guid idUser, ProfileCreateDTO profileCreateDTO)
        {
            var profile = await _mediator.Send(new AddProfileCommand(idUser, profileCreateDTO));

            return
                profile is null
                ? BadRequest()
                : Ok(new { profile });
        }

        #endregion



        #region <-------------> GET <------------->

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByGuidAsync(Guid id)
        {
            var profile = await _mediator.Send(new GetByIdProfileQuery(id));

            return
                profile is null
                ? NoContent()
                : Ok(new { profile });
        }


        [HttpGet]
        public async Task<IActionResult> GetByRoleAsync(string role)
        {
            var profiles = await _mediator.Send(new GetByRoleProfileQuery(role));

            return
                profiles is null
                ? NoContent()
                : Ok(new { profiles });
        }

        #endregion



        #region <-------------> UPDATE <------------->

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, ProfileUpdateDTO profile)
        {
            var result = await _mediator.Send(new UpdateProfileCommand(id, profile));

            return
                result is null
                ? BadRequest()
                : Ok(result);
        }

        #endregion



        #region <-------------> DELETE <------------->

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteProfileCommand(id));
            return NoContent();
        }

        #endregion

    }
}

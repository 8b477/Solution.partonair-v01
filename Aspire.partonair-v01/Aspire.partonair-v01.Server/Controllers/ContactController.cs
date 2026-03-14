using BLL.partonair_v01.MediatR.Commands.Contacts;
using BLL.partonair_v01.MediatR.Queries.Contacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;

 
namespace API.partonair_v01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;


        // Only Admin -> Need to move into AdminContact controller
        [HttpGet]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllAsync()
        {
            var result = await _mediator.Send(new GetAllContactQuery());
            return Ok(result);
        }

        [HttpGet("PendingStatus")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllPendingStatusAsync(string status)
        {
            var result = await _mediator.Send(new GetAllPendingStatusQuery(status));
            return Ok(result);
        }

        [HttpGet("AcceptedStatus")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllAcceptedAsync(string status)
        {
            var result = await _mediator.Send(new GetAllAcceptedStatusQuery(status));
            return Ok(result);
        }

        [HttpGet("BlockedStatus")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetAllBlockedAsync(string status)
        {
            var result = await _mediator.Send(new GetAllBlockedStatusQuery(status));
            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ICollection<ContactViewDTO>>> GetByGuidAsync(Guid id)
        {
            var result = await _mediator.Send(new GetByGuidContactQuery(id));
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid idSender, Guid idRecieper)
        {
            await _mediator.Send(new DeleteContactCommand(idSender, idRecieper));

            return NoContent();
        }

        [HttpPatch(nameof(AcceptedInvitation))]
        public async Task<IActionResult> AcceptedInvitation([FromQuery] Guid idContact)
        {
            var response = await _mediator.Send(new AcceptedRequestContactCommand(idContact));
            return Ok(new { response });
        }

        [HttpPatch(nameof(RefusedInvitation))]
        public async Task<IActionResult> RefusedInvitation([FromQuery] Guid idContact)
        {
            var response = await _mediator.Send(new RefusedRequestCommand(idContact));
            return Ok(new { response });
        }

        [HttpPatch(nameof(LockContact))]
        public async Task<IActionResult> LockContact([FromQuery] Guid idSender, UserToLock userToLock)
        {
            var response = await _mediator.Send(new LockContactRequestCommand(idSender, userToLock));
            return Ok(new { response });
        }

        [HttpPatch(nameof(UnlockContact))]
        public async Task<IActionResult> UnlockContact([FromQuery] Guid idSender, UserToUnlock idUserToBlock)
        {
            var response = await _mediator.Send(new UnlockContactRequestCommand(idSender, idUserToBlock));
            return Ok(new { response });
        }

    }
}

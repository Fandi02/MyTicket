using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Request;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyTicket.WebApi.Endpoints.UserProfile;

public class UpdatePasswordController : BaseEndpointWithoutResponse<UpdatePasswordModelRequest>
{
    public UpdatePasswordController()
    {
    }

    [HttpPost("update-password")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Update password user",
        Description = "",
        Tags = new[] { "UserProfile" })
    ]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdatePasswordModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
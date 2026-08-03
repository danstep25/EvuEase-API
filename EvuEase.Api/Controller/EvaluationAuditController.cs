using EvuEase.Application.DTOs.EvaluationAudit;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class EvaluationAuditController : BaseController
{
    private readonly IEvaluationAuditService _evaluationAuditService;

    public EvaluationAuditController(IEvaluationAuditService evaluationAuditService)
    {
        _evaluationAuditService = evaluationAuditService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] EvaluationAuditRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _evaluationAuditService.GetAllAsync(request, cancellationToken);
            return Success(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving evaluation audit records.", ex.Message);
        }
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _evaluationAuditService.GetByIdAsync(id, cancellationToken);
            return data == null ? NotFound("Evaluation audit record was not found.") : Success(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving evaluation audit record {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEvaluationAuditRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var evaluatedBy = User.FindFirst("UserName")?.Value
                ?? User.FindFirst("Email")?.Value
                ?? User.FindFirst("UserId")?.Value;

            var data = await _evaluationAuditService.CreateAsync(request, evaluatedBy, cancellationToken);
            return Created(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while saving the evaluation audit record.", ex.Message);
        }
    }
}

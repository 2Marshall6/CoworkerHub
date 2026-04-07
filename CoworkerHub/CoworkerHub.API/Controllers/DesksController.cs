using CoworkerHub.Application.DTOs.Desk;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoworkerHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DesksController : ControllerBase
    {
        private readonly IDeskService _deskService;
        private readonly IValidator<CreateDeskDTO> _createValidator;
        private readonly IValidator<UpdateDeskDTO> _updateValidator;

        public DesksController(IDeskService deskService, IValidator<CreateDeskDTO> createValidator, IValidator<UpdateDeskDTO> updateValidator)
        {
            _deskService = deskService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeskDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var desk = await _deskService.GetDeskByIdAsync(id, cancellationToken);
            return Ok(desk);
        }

        [HttpPost]
        public async Task<ActionResult<DeskDTO>> Create(CreateDeskDTO createModel, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(createModel, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var desk = await _deskService.CreateDeskAsync(createModel, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = desk.Id }, desk);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateDeskDTO updateModel, CancellationToken cancellationToken)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateModel, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            await _deskService.UpdateDeskAsync(id, updateModel, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _deskService.DeleteDeskAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> ChangeStatus(int id, [FromBody] DeskStatus newStatus, CancellationToken cancellationToken)
        {
            await _deskService.ChangeDeskStatusAsync(id, newStatus, cancellationToken);
            return NoContent();
        }
    }
}
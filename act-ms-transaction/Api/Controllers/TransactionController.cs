using act_ms_transaction.Api.DTOs.Parameters;
using act_ms_transaction.Api.DTOs.Requests;
using act_ms_transaction.Application.Interfaces;
using act_ms_transaction.Application.Mappers;
using act_ms_transaction.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace act_ms_transaction.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new transaction")]
        [SwaggerResponse(200, "Transaction created successfully")]
        public async Task<ActionResult<TransactionEntity>> Create(CreateTransactionRequest transactionRequest)
        {
            var transactionEntity = TransactionMapper.MapToDomain(transactionRequest);
            var createdTransaction = await _transactionService.CreateAsync(transactionEntity);
            return CreatedAtAction(nameof(GetById), new { id = createdTransaction.Id }, createdTransaction);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Update a transaction by ID")]
        public async Task<IActionResult> Update(Guid id, UpdateTransactionRequest transactionRequest)
        {
            var transactionEntity = TransactionMapper.MapToDomain(transactionRequest);
            
            var updatedTransaction = await _transactionService.UpdateAsync(transactionEntity, id);

            return Ok(updatedTransaction);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all transactions with pagination")]
        public async Task<ActionResult<IEnumerable<TransactionEntity>>> Get([FromQuery] PaginationQueryParameters query)
        {
            var result = await _transactionService.GetAllAsync(query.PageNumber, query.PageSize, query.OrderBy, query.Asc);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get a transaction by ID")]
        public async Task<ActionResult<TransactionEntity>> GetById(Guid id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
                return NotFound($"Transaction with ID {id} not found.");

            return Ok(transaction);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete a transaction by ID")]
        [SwaggerResponse(204, "Transaction deleted successfully")]
        [SwaggerResponse(404, "Transaction not found")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _transactionService.DeleteAsync(id);

            if (!isDeleted)
                return NotFound($"Transaction with ID {id} not found.");

            return NoContent();
        }
    }
}

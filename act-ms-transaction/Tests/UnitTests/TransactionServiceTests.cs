using act_ms_transaction.Application.DTOs;
using act_ms_transaction.Application.Interfaces;
using act_ms_transaction.Application.Services;
using act_ms_transaction.Domain.Entities;
using act_ms_transaction.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace act_ms_transaction.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMessageService = new Mock<IMessageService>();
            _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockMessageService.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedTransaction()
        {
            var transaction = new TransactionEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Value = 1500.5m,
                Description = "Compra de Equipamentos"
            };

            _mockTransactionRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<TransactionEntity>()))
                .ReturnsAsync(transaction);

            _mockMessageService
                .Setup(service => service.PublishAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _transactionService.CreateAsync(transaction);

            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            _mockMessageService.Verify(service => service.PublishAsync(transaction.Date.ToString("yyyy-MM-dd")), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedTransaction()
        {
            var transaction = new TransactionEntity
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Value = 2000m,
                Description = "Atualização de Transação"
            };

            _mockTransactionRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<TransactionEntity>()))
                .ReturnsAsync(transaction);

            _mockMessageService
                .Setup(service => service.PublishAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _transactionService.UpdateAsync(transaction);

            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal(transaction.Value, result.Value);
            _mockMessageService.Verify(service => service.PublishAsync(transaction.Date.ToString("yyyy-MM-dd")), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenTransactionExists()
        {
            var transactionId = Guid.NewGuid();
            var transaction = new TransactionEntity
            {
                Id = transactionId,
                Date = DateTime.UtcNow
            };

            _mockTransactionRepository
                .Setup(repo => repo.GetByIdAsync(transactionId))
                .ReturnsAsync(transaction);

            _mockTransactionRepository
                .Setup(repo => repo.DeleteAsync(transactionId))
                .ReturnsAsync(true);

            _mockMessageService
                .Setup(service => service.PublishAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _transactionService.DeleteAsync(transactionId);

            Assert.True(result);
            _mockMessageService.Verify(service => service.PublishAsync(transaction.Date.ToString("yyyy-MM-dd")), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTransactionDoesNotExist()
        {
            var transactionId = Guid.NewGuid();

            _mockTransactionRepository
                .Setup(repo => repo.GetByIdAsync(transactionId))
                .ReturnsAsync(default(TransactionEntity));

            var result = await _transactionService.DeleteAsync(transactionId);

            Assert.False(result);
            _mockMessageService.Verify(service => service.PublishAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTransaction_WhenTransactionExists()
        {
            var transactionId = Guid.NewGuid();
            var transaction = new TransactionEntity { Id = transactionId };

            _mockTransactionRepository
                .Setup(repo => repo.GetByIdAsync(transactionId))
                .ReturnsAsync(transaction);

            var result = await _transactionService.GetByIdAsync(transactionId);

            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedTransactions()
        {
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Value = 100m },
                new TransactionEntity { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Value = 200m }
            };

            var pagedResult = new PagedResult<TransactionEntity>
            {
                Items = transactions,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _mockTransactionRepository
                .Setup(repo => repo.GetAllAsync(1, 10, null, true))
                .ReturnsAsync(pagedResult);

            var result = await _transactionService.GetAllAsync(1, 10, null, true);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
        }
    }
}

using act_ms_transaction.Application.DTOs;
using act_ms_transaction.Application.Exeptions;
using act_ms_transaction.Application.Interfaces;
using act_ms_transaction.Application.Services;
using act_ms_transaction.Domain.Entities;
using act_ms_transaction.Domain.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly Mock<ILogger<TransactionService>> _mockLogger;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMessageService = new Mock<IMessageService>();
            _mockLogger = new Mock<ILogger<TransactionService>>();

            _transactionService = new TransactionService(
                _mockTransactionRepository.Object,
                _mockMessageService.Object,
                _mockLogger.Object);
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

            // Verifica log de criação
            _mockLogger.Verify(log => log.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Creating transaction")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowBusinessException_WhenTransactionIdIsEmpty()
        {
            var transaction = new TransactionEntity
            {
                Date = DateTime.UtcNow,
                Value = 1500.5m,
                Description = "Compra de Equipamentos"
            };

            var exception = await Assert.ThrowsAsync<BusinessException>(() =>
                _transactionService.UpdateAsync(transaction, Guid.Empty));

            Assert.Equal("id field is required.", exception.Message);

            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Id field not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTransactionNotFound()
        {
            var transactionId = Guid.NewGuid();
            var transaction = new TransactionEntity
            {
                Id = transactionId,
                Date = DateTime.UtcNow,
                Value = 1500.5m,
                Description = "Atualização"
            };

            _mockTransactionRepository
                .Setup(repo => repo.GetByIdAsync(transactionId))
                .ReturnsAsync(default(TransactionEntity));

            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _transactionService.UpdateAsync(transaction, transactionId));

            Assert.Equal($"Transaction with ID {transactionId} not found", exception.Message);

            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Transaction with ID {transactionId} not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
        {
            var transactionId = Guid.NewGuid();

            _mockTransactionRepository
                .Setup(repo => repo.GetByIdAsync(transactionId))
                .ReturnsAsync(default(TransactionEntity));

            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _transactionService.DeleteAsync(transactionId));

            Assert.Equal($"Transaction with ID {transactionId} not found", exception.Message);

            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Transaction with ID {transactionId} not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowBusinessException_WhenTransactionIdIsEmpty()
        {
            var exception = await Assert.ThrowsAsync<BusinessException>(() =>
                _transactionService.GetByIdAsync(Guid.Empty));

            Assert.Equal("id field is required.", exception.Message);

            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Id field not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenTransactionNotFound()
        {
            var transactionId = Guid.NewGuid();

            _mockTransactionRepository
                .Setup(repo => repo.GetByIdAsync(transactionId))
                .ReturnsAsync(default(TransactionEntity));

            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _transactionService.GetByIdAsync(transactionId));

            Assert.Equal($"Transaction with ID {transactionId} not found", exception.Message);

            _mockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Transaction with ID {transactionId} not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}

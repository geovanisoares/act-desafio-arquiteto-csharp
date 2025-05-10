using act_ms_consolidation.Api.DTOs;
using act_ms_consolidation.Application.Services;
using act_ms_consolidation.Domain.Entities;
using act_ms_consolidation.Domain.Interfaces;
using Moq;
using Xunit;

namespace act_ms_consolidation.Tests.Application.Services
{
    public class ConsolidationServiceTests
    {
        private readonly Mock<IConsolidationRepository> _mockRepository;
        private readonly Mock<IConsolidationCacheRepository> _mockCacheRepository;
        private readonly ConsolidationService _service;

        public ConsolidationServiceTests()
        {
            _mockRepository = new Mock<IConsolidationRepository>();
            _mockCacheRepository = new Mock<IConsolidationCacheRepository>();
            _service = new ConsolidationService(_mockRepository.Object, _mockCacheRepository.Object);
        }

        [Fact]
        public async Task GetDailyConsolidationAsync_ShouldReturnCachedData_WhenCacheExists()
        {
            // Arrange
            string date = "2025-05-07";
            string cacheKey = $"consolidation_{date}";
            var expectedResponse = new ConsolidationResponse
            {
                Date = date,
                TotalIncome = 1500.0M,
                TotalExpense = 500.0M,
                Balance = 1000.0M
            };

            _mockCacheRepository
                .Setup(repo => repo.GetAsync<ConsolidationResponse>(cacheKey))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.GetDailyConsolidationAsync(date);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Date, result.Date);
            Assert.Equal(expectedResponse.TotalIncome, result.TotalIncome);
            Assert.Equal(expectedResponse.TotalExpense, result.TotalExpense);
            Assert.Equal(expectedResponse.Balance, result.Balance);

            _mockCacheRepository.Verify(repo => repo.GetAsync<ConsolidationResponse>(cacheKey), Times.Once);
            _mockRepository.Verify(repo => repo.GetDailyConsolidationAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetDailyConsolidationAsync_ShouldFetchFromRepository_WhenCacheMiss()
        {
            // Arrange
            string date = "2025-05-07";
            string cacheKey = $"consolidation_{date}";

            var consolidationEntity = new ConsolidationEntity
            {
                Date = date,
                TotalIncome = 2000.0M,
                TotalExpense = 800.0M,
                Balance = 1200.0M
            };

            var expectedResponse = new ConsolidationResponse
            {
                Date = date,
                TotalIncome = consolidationEntity.TotalIncome,
                TotalExpense = consolidationEntity.TotalExpense,
                Balance = consolidationEntity.Balance
            };

            _mockCacheRepository
                .Setup(repo => repo.GetAsync<ConsolidationResponse>(cacheKey))
                .ReturnsAsync(default(ConsolidationResponse));

            _mockRepository
                .Setup(repo => repo.GetDailyConsolidationAsync(date))
                .ReturnsAsync(consolidationEntity);

            _mockCacheRepository
                .Setup(repo => repo.SetAsync(cacheKey, It.IsAny<ConsolidationResponse>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.GetDailyConsolidationAsync(date);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Date, result.Date);
            Assert.Equal(expectedResponse.TotalIncome, result.TotalIncome);
            Assert.Equal(expectedResponse.TotalExpense, result.TotalExpense);
            Assert.Equal(expectedResponse.Balance, result.Balance);

            _mockCacheRepository.Verify(repo => repo.GetAsync<ConsolidationResponse>(cacheKey), Times.Once);
            _mockRepository.Verify(repo => repo.GetDailyConsolidationAsync(date), Times.Once);
            _mockCacheRepository.Verify(repo => repo.SetAsync(cacheKey, It.IsAny<ConsolidationResponse>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}

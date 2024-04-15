using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Common;
using InfrastructureTest.PhysicalData.Common;
using Xunit;

namespace InfrastructureTest.UnitOfWork
{
    public class UnitOfWorkSpecification : IClassFixture<PhysicalDataFixture>
    {
        private readonly PhysicalDataFixture fxtAuthorizationData;

        private readonly ITimeProvider prvTime;

        public UnitOfWorkSpecification(PhysicalDataFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldCreateTimePeriod_WhenTransactionIsCommitted()
        {
            // Arrange
            IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
            ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

            bool bIsCommitted = false;
            bool bIsRolledBack = false;

            // Act
            await fxtAuthorizationData.UnitOfWork.TransactionAsync(async () =>
            {
                await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
                await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

				bIsCommitted = fxtAuthorizationData.UnitOfWork.TryCommit();

				if (bIsCommitted == false)
                    bIsRolledBack = fxtAuthorizationData.UnitOfWork.TryRollback();
            });

			// Assert

			IRepositoryResult<ITimePeriod> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

			bIsCommitted.Should().BeTrue();
            bIsRolledBack.Should().BeFalse();

            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                pdTimePeriodById =>
                {
                    pdTimePeriodById.Should().BeEquivalentTo(pdTimePeriod);

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
            await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldNotCreateTimePeriod_WhenTransactionIsRolledBack()
        {
            // Arrange
            IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
            ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

            bool bIsRolledBack = false;

            // Act
            await fxtAuthorizationData.UnitOfWork.TransactionAsync(async () =>
            {
                await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
                await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

                bIsRolledBack = fxtAuthorizationData.UnitOfWork.TryRollback();
            });

            IRepositoryResult<ITimePeriod> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

            // Assert
            bIsRolledBack.Should().BeTrue();

            rsltTimePeriod.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TimePeriodError.Code.Method);
                    msgError.Description.Should().Be($"Time period {pdTimePeriod.Id} has not been found.");

                    return false;
                },
                pdTimePeriodById =>
                {
                    pdTimePeriodById.Should().BeNull();

                    return true;
                });

            // Clean up
            //await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
            //await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
        }
    }
}

﻿using Bot.GameData;
using Bot.GameSense;
using Bot.Tagging;
using Bot.Tests.Fixtures;
using Moq;
using SC2APIProtocol;

namespace Bot.Tests.GameSense;

// This is because we play with the global UnitsTracker.Instance
[Collection("Sequential")]
public class EnemyRaceTrackerTests : IClassFixture<KnowledgeBaseFixture>, IDisposable {
    private readonly Mock<ITaggingService> _taggingServiceMock;

    public EnemyRaceTrackerTests() {
        UnitsTracker.Instance.Reset(); // TODO GD Review the test setup, right now the ResponseGameObservationUtils depend on the UnitsTracker.Instance
        _taggingServiceMock = new Mock<ITaggingService>();
    }

    public void Dispose() {
        UnitsTracker.Instance.Reset(); // TODO GD Review the test setup, right now the ResponseGameObservationUtils depend on the UnitsTracker.Instance
    }

    [Theory]
    [InlineData(Race.Zerg, Race.Random)]
    [InlineData(Race.Terran, Race.Zerg)]
    [InlineData(Race.Protoss, Race.Terran)]
    [InlineData(Race.Random, Race.Protoss)]
    public void GivenPlayersWithDistinctRaces_WhenUpdate_SetsEnemyRace(Race playerRace, Race enemyRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: playerRace, enemyRace: enemyRace);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: BaseTestClass.GetInitialUnits());
        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);

        // Act
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        Assert.Equal(enemyRace, enemyRaceTracker.EnemyRace);
    }

    [Theory]
    [InlineData(Race.Terran, Race.Terran)]
    [InlineData(Race.Protoss, Race.Protoss)]
    [InlineData(Race.Zerg, Race.Zerg)]
    [InlineData(Race.Random, Race.Random)]
    public void GivenPlayersWithSameRaces_WhenUpdate_SetsEnemyRace(Race playerRace, Race enemyRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: playerRace, enemyRace: enemyRace);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: BaseTestClass.GetInitialUnits());
        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);

        // Act
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        Assert.Equal(enemyRace, enemyRaceTracker.EnemyRace);
    }

    [Fact]
    public void GivenEnemyRandomRaceAndNoVisibleUnits_WhenUpdate_ThenRaceDoesntChange() {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: Race.Zerg, enemyRace: Race.Random);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: BaseTestClass.GetInitialUnits());
        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);
        enemyRaceTracker.Update(observation, gameInfo);

        // Act
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        Assert.Equal(Race.Random, enemyRaceTracker.EnemyRace);
    }

    public static IEnumerable<object[]> EnemyRandomRaceAndVisibleUnitsTestData() {
        var units = BaseTestClass.GetInitialUnits();

        yield return new object[] { units.Concat(new List<Unit> { TestUtils.CreateUnit(Units.Scv, alliance: Alliance.Enemy) }), Race.Terran };
        yield return new object[] { units.Concat(new List<Unit> { TestUtils.CreateUnit(Units.Probe, alliance: Alliance.Enemy) }), Race.Protoss };
        yield return new object[] { units.Concat(new List<Unit> { TestUtils.CreateUnit(Units.Drone, alliance: Alliance.Enemy) }), Race.Zerg };
    }

    [Theory]
    [MemberData(nameof(EnemyRandomRaceAndVisibleUnitsTestData))]
    public void GivenRandomPlayerAndVisibleUnits_WhenUpdate_ThenResolvesEnemyRace(IEnumerable<Unit> units, Race expectedRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: Race.Zerg, enemyRace: Race.Random);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: units);

        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);
        UnitsTracker.Instance.Update(observation, gameInfo);

        // Act
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        Assert.Equal(expectedRace, enemyRaceTracker.EnemyRace);
    }

    [Theory]
    [MemberData(nameof(EnemyRandomRaceAndVisibleUnitsTestData))]
    public void GivenEnemyRandomRaceAndVisibleUnits_WhenUpdate_ThenResolvesEnemyRace(IEnumerable<Unit> units, Race expectedRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: Race.Zerg, enemyRace: Race.Random);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: units);

        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);
        enemyRaceTracker.Update(observation, gameInfo);
        UnitsTracker.Instance.Update(observation, gameInfo);

        // Act
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        Assert.Equal(expectedRace, enemyRaceTracker.EnemyRace);
    }

    [Theory]
    [MemberData(nameof(EnemyRandomRaceAndVisibleUnitsTestData))]
    public void GivenEnemyResolvedRaceAndNoVisibleUnits_WhenUpdate_ThenRaceDoesntChange(IEnumerable<Unit> units, Race expectedRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: Race.Zerg, enemyRace: Race.Random);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: units);

        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);
        enemyRaceTracker.Update(observation, gameInfo); // Race.Random
        UnitsTracker.Instance.Update(observation, gameInfo);
        enemyRaceTracker.Update(observation, gameInfo); // expectedRace

        // Act
        var newObservation = ResponseGameObservationUtils.CreateResponseObservation(Enumerable.Empty<Unit>(), keepPreviousUnits: false);
        UnitsTracker.Instance.Reset();
        enemyRaceTracker.Update(newObservation, gameInfo);

        // Assert
        Assert.Equal(expectedRace, enemyRaceTracker.EnemyRace);
    }

    [Theory]
    [InlineData(Race.Terran, Race.Terran)]
    [InlineData(Race.Protoss, Race.Protoss)]
    [InlineData(Race.Zerg, Race.Zerg)]
    [InlineData(Race.Random, Race.Random)]
    public void GivenPlayerRaces_WhenUpdate_TagsEnemyRaceOnce(Race playerRace, Race enemyRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: playerRace, enemyRace: enemyRace);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: BaseTestClass.GetInitialUnits());
        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);

        // Act
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        _taggingServiceMock.Verify(taggingService => taggingService.TagEnemyRace(It.IsAny<Race>()), Times.Once);
        _taggingServiceMock.Verify(taggingService => taggingService.TagEnemyRace(enemyRace), Times.Once);
    }

    [Theory]
    [InlineData(Race.Terran, Race.Terran)]
    [InlineData(Race.Protoss, Race.Protoss)]
    [InlineData(Race.Zerg, Race.Zerg)]
    [InlineData(Race.Random, Race.Random)]
    public void GivenEnemyRace_WhenFurtherUpdates_TagsNothing(Race playerRace, Race enemyRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: playerRace, enemyRace: enemyRace);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: BaseTestClass.GetInitialUnits());

        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);
        enemyRaceTracker.Update(observation, gameInfo);

        // Act
        _taggingServiceMock.Reset();
        enemyRaceTracker.Update(observation, gameInfo);
        enemyRaceTracker.Update(observation, gameInfo);
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        _taggingServiceMock.Verify(taggingService => taggingService.TagEnemyRace(It.IsAny<Race>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(EnemyRandomRaceAndVisibleUnitsTestData))]
    public void GivenEnemyRandomRaceAndVisibleUnits_WhenUpdate_ThenTagsActualRaceOnce(IEnumerable<Unit> units, Race expectedRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: Race.Zerg, enemyRace: Race.Random);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: units);

        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);
        enemyRaceTracker.Update(observation, gameInfo);
        UnitsTracker.Instance.Update(observation, gameInfo);

        // Act
        _taggingServiceMock.Reset();
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        _taggingServiceMock.Verify(taggingService => taggingService.TagEnemyRace(It.IsAny<Race>()), Times.Once);
        _taggingServiceMock.Verify(taggingService => taggingService.TagEnemyRace(expectedRace), Times.Once);
    }

    [Theory]
    [MemberData(nameof(EnemyRandomRaceAndVisibleUnitsTestData))]
    public void GivenEnemyResolvedRandomRaceAndVisibleUnits_WhenFurtherUpdates_ThenTagsNothing(IEnumerable<Unit> units, Race expectedRace) {
        // Arrange
        var gameInfo = ResponseGameInfoUtils.CreateResponseGameInfo(playerRace: Race.Zerg, enemyRace: Race.Random);
        var observation = ResponseGameObservationUtils.CreateResponseObservation(units: units);

        var enemyRaceTracker = new EnemyRaceTracker(_taggingServiceMock.Object);
        enemyRaceTracker.Update(observation, gameInfo);
        UnitsTracker.Instance.Update(observation, gameInfo);
        enemyRaceTracker.Update(observation, gameInfo);

        // Act
        _taggingServiceMock.Reset();
        enemyRaceTracker.Update(observation, gameInfo);
        enemyRaceTracker.Update(observation, gameInfo);
        enemyRaceTracker.Update(observation, gameInfo);

        // Assert
        _taggingServiceMock.Verify(taggingService => taggingService.TagEnemyRace(It.IsAny<Race>()), Times.Never);
    }
}
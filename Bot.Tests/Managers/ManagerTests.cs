﻿using Bot.Builds;
using Bot.GameData;
using Bot.Managers;

namespace Bot.Tests.Managers;

public class ManagerTests: BaseTestClass {
    [Fact]
    public void GivenNothing_WhenAssigningMultipleUnits_AssignsEachUnit() {
        // Arrange
        var manager = new DummyManager();
        var units = new List<Unit>
        {
            TestUtils.CreateUnit(Units.Zergling),
            TestUtils.CreateUnit(Units.Drone),
            TestUtils.CreateUnit(Units.Zergling),
        };

        // Act
        manager.Assign(units);

        // Assert
        Assert.Equal(units, manager.ManagedUnits.ToList());
    }

    [Fact]
    public void GivenNothing_WhenAssigningUnit_SetsManagerOnUnit() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(unit);

        // Assert
        Assert.Equal(manager, unit.Manager);
    }

    [Fact]
    public void GivenNothing_WhenAssigningUnit_AddsDeathWatcher() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(unit);

        // Assert
        Assert.Contains(manager, unit.DeathWatchers);
    }

    [Fact]
    public void GivenNothing_WhenAssigningUnit_AddsToManagedUnits() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(unit);

        // Assert
        Assert.Contains(unit, manager.ManagedUnits);
    }

    [Fact]
    public void GivenNothing_WhenAssigningUnit_CallsAssigner() {
        // Arrange
        var assigner = new DummyAssigner();
        var manager = new DummyManager(assigner: assigner);
        var unit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(unit);

        // Assert
        Assert.Contains(unit, assigner.AssignedUnits);
    }

    [Fact]
    public void GivenManagedUnit_WhenAssigningOtherUnit_SetsManagerOnUnit() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);
        manager.Assign(unit);

        var otherUnit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(otherUnit);

        // Assert
        Assert.Equal(manager, otherUnit.Manager);
    }

    [Fact]
    public void GivenManagedUnit_WhenAssigningOtherUnit_AddsDeathWatcher() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);
        manager.Assign(unit);

        var otherUnit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(otherUnit);

        // Assert
        Assert.Contains(manager, otherUnit.DeathWatchers);
    }

    [Fact]
    public void GivenManagedUnit_WhenAssigningOtherUnit_AddsToManagedUnits() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);
        manager.Assign(unit);

        var otherUnit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(otherUnit);

        // Assert
        Assert.Contains(otherUnit, manager.ManagedUnits);
    }

    [Fact]
    public void GivenManagedUnit_WhenAssigningOtherUnit_CallsAssigner() {
        // Arrange
        var assigner = new DummyAssigner();
        var manager = new DummyManager(assigner: assigner);
        var unit = TestUtils.CreateUnit(Units.Zergling);
        manager.Assign(unit);

        var otherUnit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        manager.Assign(otherUnit);

        // Assert
        Assert.Contains(otherUnit, assigner.AssignedUnits);
    }

    [Fact]
    public void GivenManagedUnit_WhenAssigningSameUnit_DoesNothing() {
        // Arrange
        var assigner = new DummyAssigner();
        var manager = new DummyManager(assigner: assigner);
        var unit = TestUtils.CreateUnit(Units.Zergling);
        manager.Assign(unit);

        // Act
        manager.Assign(unit);

        // Assert
        var assignedUnits = assigner.AssignedUnits.Where(managed => managed == unit).ToList();
        Assert.Single(assignedUnits);

        var managedUnits = manager.ManagedUnits.Where(managed => managed == unit).ToList();
        Assert.Single(managedUnits);

        var deathWatchers = unit.DeathWatchers.Where(deathWatcher => deathWatcher == manager).ToList();
        Assert.Single(deathWatchers);
    }

    [Fact]
    public void GivenNothing_WhenReleasingMultipleUnits_ReleasesEachUnit() {
        // Arrange
        var manager = new DummyManager();
        var units = new List<Unit>
        {
            TestUtils.CreateUnit(Units.Zergling),
            TestUtils.CreateUnit(Units.Drone),
            TestUtils.CreateUnit(Units.Zergling),
        };

        manager.Assign(units);

        // Act
        manager.Release(units);

        // Assert
        Assert.Empty(manager.ManagedUnits);
    }

    [Fact]
    public void GivenManagedUnit_WhenReleasingUnit_UnsetsManagerFromUnit() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        manager.Assign(unit);

        // Act
        manager.Release(unit);

        // Assert
        Assert.Null(unit.Manager);
    }

    [Fact]
    public void GivenManagedUnit_WhenReleasingUnit_RemovesDeathWatcher() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        manager.Assign(unit);

        // Act
        manager.Release(unit);

        // Assert
        Assert.Empty(unit.DeathWatchers);
    }

    [Fact]
    public void GivenManagedUnit_WhenReleasingUnit_RemovesFromManagedUnits() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        manager.Assign(unit);

        // Act
        manager.Release(unit);

        // Assert
        Assert.Empty(manager.ManagedUnits);
    }

    [Fact]
    public void GivenManagedUnit_WhenReleasingUnit_ReleasesSupervisor() {
        // Arrange
        var manager = new DummyManager();
        var supervisor = new DummySupervisor();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        manager.Assign(unit);
        supervisor.Assign(unit);

        // Act
        manager.Release(unit);

        // Assert
        Assert.Empty(manager.ManagedUnits);
        Assert.Empty(supervisor.SupervisedUnits);
    }

    [Fact]
    public void GivenManagedUnit_WhenReleasingUnit_CallsReleaser() {
        // Arrange
        var releaser = new DummyReleaser();
        var manager = new DummyManager(releaser: releaser);
        var unit = TestUtils.CreateUnit(Units.Zergling);

        manager.Assign(unit);

        // Act
        manager.Release(unit);

        // Assert
        Assert.Contains(unit, releaser.ReleasedUnits);
    }

    [Fact]
    public void GivenNothing_WhenReleasingUnit_DoesNothing() {
        // Arrange
        var manager = new DummyManager();
        var unit = TestUtils.CreateUnit(Units.Zergling);

        // Act
        var exception = Record.Exception(() => manager.Release(unit));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void GivenManagedUnit_WhenReleasingUnmanagedUnit_DoesNothing() {
        // Arrange
        var releaser = new DummyReleaser();
        var manager = new DummyManager(releaser: releaser);
        var supervisor = new DummySupervisor();

        var unit = TestUtils.CreateUnit(Units.Zergling);
        var otherUnit = TestUtils.CreateUnit(Units.Zergling);

        manager.Assign(unit);
        supervisor.Assign(unit);

        // Act
        var exception = Record.Exception(() => manager.Release(otherUnit));

        // Assert
        Assert.Null(exception);
        Assert.Equal(manager, unit.Manager);
        Assert.Equal(supervisor, unit.Supervisor);
        Assert.Contains(unit, manager.ManagedUnits);
        Assert.Contains(unit, supervisor.SupervisedUnits);
        Assert.Contains(manager, unit.DeathWatchers);
    }

    // TODO GD Implement IDispatcher
    private class DummyManager: Manager {
        private readonly IAssigner? _assigner;
        private readonly IDispatcher? _dispatcher;
        private readonly IReleaser? _releaser;

        public override IEnumerable<BuildFulfillment> BuildFulfillments { get; } = Enumerable.Empty<BuildFulfillment>();

        public DummyManager(IAssigner? assigner = null, IDispatcher? dispatcher = null, IReleaser? releaser = null) {
            _assigner = assigner;
            _dispatcher = dispatcher;
            _releaser = releaser;

            Init();
        }

        protected override IAssigner CreateAssigner() {
            return _assigner!;
        }

        protected override IDispatcher CreateDispatcher() {
            return _dispatcher!;
        }

        protected override IReleaser CreateReleaser() {
            return _releaser!;
        }

        protected override void AssignUnits() {}

        protected override void DispatchUnits() {}

        protected override void Manage() {}
    }

    private class DummyAssigner: IAssigner {
        public List<Unit> AssignedUnits { get; } = new List<Unit>();

        public void Assign(Unit unit) {
            AssignedUnits.Add(unit);
        }
    }

    private class DummyReleaser: IReleaser {
        public List<Unit> ReleasedUnits { get; } = new List<Unit>();

        public void Release(Unit unit) {
            ReleasedUnits.Add(unit);
        }
    }

    private class DummySupervisor : Supervisor {
        private readonly IAssigner _assigner = new DummyAssigner();
        private readonly IReleaser _releaser = new DummyReleaser();

        public override IEnumerable<BuildFulfillment> BuildFulfillments { get; } = Enumerable.Empty<BuildFulfillment>();

        public DummySupervisor() {
            Init();
        }

        protected override IAssigner CreateAssigner() {
            return _assigner;
        }

        protected override IReleaser CreateReleaser() {
            return _releaser;
        }

        protected override void Supervise() {
            throw new NotImplementedException();
        }

        public override void Retire() {
            throw new NotImplementedException();
        }
    }
}

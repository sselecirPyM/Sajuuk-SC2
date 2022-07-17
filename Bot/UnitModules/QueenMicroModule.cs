﻿using System.Linq;
using Bot.GameData;

namespace Bot.UnitModules;

public class QueenMicroModule: IUnitModule {
    public const string Tag = "queen-micro-module";

    private readonly Unit _queen;
    private readonly Unit _assignedTownHall;

    public static void Install(Unit queen, Unit assignedTownHall) {
        queen.Modules.Add(Tag, new QueenMicroModule(queen, assignedTownHall));
    }

    private QueenMicroModule(Unit queen, Unit assignedTownHall) {
        _queen = queen;
        _assignedTownHall = assignedTownHall;
    }

    public void Execute() {
        // TODO GD Find the energy cost
        if (_queen.RawUnitData.Energy >= 25 && _queen.Orders.All(order => order.AbilityId != Abilities.InjectLarvae)) {
            _queen.UseAbility(Abilities.InjectLarvae, targetUnitTag: _assignedTownHall.Tag);
        }

        // TODO GD Spawn some creep with energy overflow
    }
}

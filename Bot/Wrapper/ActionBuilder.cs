﻿using System.Collections.Generic;
using System.Numerics;
using SC2APIProtocol;

namespace Bot.Wrapper;

public static class ActionBuilder {
    public static Action TrainUnit(uint unitType, ulong producerTag) {
        var unitAbilityId = (int)GameData.GetUnitTypeData(unitType).AbilityId;

        return UnitCommand(unitAbilityId, producerTag);
    }

    public static Action PlaceBuilding(uint buildingType, ulong producerTag, Vector3 position) {
        var buildingAbilityId = (int)GameData.GetUnitTypeData(buildingType).AbilityId;

        return UnitCommand(buildingAbilityId, producerTag, position: new Point2D { X = position.X, Y = position.Y });
    }

    public static Action PlaceExtractor(uint buildingType, ulong producerTag, ulong gasTag) {
        var buildingAbilityId = (int)GameData.GetUnitTypeData(buildingType).AbilityId;

        return UnitCommand(buildingAbilityId, producerTag, targetUnitTag: gasTag);
    }

    public static Action ResearchUpgrade(uint upgradeType, ulong producerTag) {
        var upgradeAbilityId = (int)GameData.GetUpgradeData(upgradeType).AbilityId;

        return UnitCommand(upgradeAbilityId, producerTag);
    }

    public static Action Move(ulong unitTag, Vector3 position) {
        return UnitCommand(Abilities.Move, unitTag, position: new Point2D { X = position.X, Y = position.Y });
    }

    public static Action Attack(IEnumerable<ulong> unitTags, Vector3 position) {
        return UnitCommand(Abilities.Attack, unitTags: unitTags, position: new Point2D { X = position.X, Y = position.Y });
    }

    public static Action Smart(ulong unitTag, ulong targetUnitTag) {
        return UnitCommand(Abilities.Smart, unitTag, targetUnitTag: targetUnitTag);
    }

    public static Action Chat(string message, bool toTeam = false) {
        return new Action
        {
            ActionChat = new ActionChat
            {
                Channel = toTeam ? ActionChat.Types.Channel.Team : ActionChat.Types.Channel.Broadcast,
                Message = message
            }
        };
    }

    private static Action UnitCommand(int abilityId, ulong unitTag, Point2D position = null, ulong targetUnitTag = ulong.MaxValue) {
        return UnitCommand(abilityId, new List<ulong> { unitTag }, position, targetUnitTag);
    }

    private static Action UnitCommand(int abilityId, IEnumerable<ulong> unitTags = null, Point2D position = null, ulong targetUnitTag = ulong.MaxValue) {
        var action = new Action
        {
            ActionRaw = new ActionRaw
            {
                UnitCommand = new ActionRawUnitCommand
                {
                    AbilityId = abilityId,
                    UnitTags = { unitTags },
                    TargetWorldSpacePos = position,
                }
            }
        };

        if (targetUnitTag != ulong.MaxValue) {
            action.ActionRaw.UnitCommand.TargetUnitTag = targetUnitTag;
        }

        return action;
    }
}

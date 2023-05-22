﻿using SC2APIProtocol;

namespace Bot.GameSense.RegionsEvaluationsTracking.RegionsEvaluations;

public interface IRegionsEvaluatorFactory {
    public RegionsThreatEvaluator CreateRegionsThreatEvaluator(RegionsForceEvaluator enemyForceEvaluator, RegionsValueEvaluator selfValueEvaluator);
    public RegionsForceEvaluator CreateRegionsForceEvaluator(Alliance alliance);
    public RegionsValueEvaluator CreateRegionsValueEvaluator(Alliance alliance);
}

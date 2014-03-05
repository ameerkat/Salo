using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Salo
{
    public class Configuration
    {
        public Configuration()
        {
            Settings = new List<cStringStringKeyValuePair>();
        }

        [Key]
        public string Name { get; set; }
        public Configuration Parent { get; set; }
        public List<cStringStringKeyValuePair> Settings { get; set; }

        public string GetSetting(string key)
        {
            cStringStringKeyValuePair setting = Settings.FirstOrDefault(x => x.Key == key);
            if (setting != null)
            {
                return setting.Value;
            }
            else if (Parent != null)
            {
                return Parent.GetSetting(key);
            }

            throw new KeyNotFoundException(String.Format("Could not locate setting with name {0}", key));
        }

        public int GetSettingAsInt(string key)
        {
            return int.Parse(GetSetting(key));
        }

        public double GetSettingAsDouble(string key)
        {
            return double.Parse(GetSetting(key));
        }

        public static class ConfigurationKeys
        {
            public const string StarsForVictoryPercent = "StarsForVictoryPercent"; // [0,1]
            public const string ProductionRate = "ProductionRate"; // ticks per production
            public const string CashPerEconomoy = "CashPerEconomy"; // 10
            public const string CashPerBanking = "CashPerBanking"; // 75
            public const string ResearchPerExperimentation = "ResearchPerExperimentation"; // 72
            public const string BaseRequiredResearch = "BaseRequiredResearch"; // 144
            public const string ManufacturingLevelCoeffienct = "ManufacturingLevelCoefficient"; // 5
            public const string ResearchPerScience = "ResearchPerScience"; // 1
            public const string WarpGateSpeedCoefficient = "WarpGateSpeedCoefficient"; // 3
            public const string FleetSpeed = "FleetSpeed"; // unsure, in units per tick
            public const string DefenderBonusWeapons = "DefenderBonusWeapons"; // 1
            public const string DevelopmentCostEconomy = "DevelopmentCostEconomy"; // 2
            public const string DevelopmentCostIndustry = "DevelopmentCostIndustry"; // 2
            public const string DevelopmentCostScience = "DevelopmentCostScience"; // 2
            public const string BaseVisibilityRange = "BaseVisibilityRange"; // 0.75
            public const string ScanningResearchVisibilityBonus = "ScanningResearchVisibilityBonus"; // 0.1 (per level)
            public const string BaseFleetRange = "BaseFleetRange"; // 1
            public const string PropulsionMultiplier = "PropulsionMultiplier"; // 0.15
            public const string FleetBaseCost = "FleetBaseCost"; // 25
            public const string TerraformingResourceCoefficient = "TerraformingResourceCoefficient"; // 5

            /*
             * Currently Unused
             */

            public const string TickRate = "TickRate"; // in ms, usually ignored
            public const string TradeCostPerLevel = "TradeCostPerLevel"; // 15
            public const string StartingCash = "StartingCash"; // 500
            public const string StartingShips = "StartingShips"; // 10 per planet
            public const string HomeStarEconomy = "HomeStarEconomy"; // 5
            public const string HomeStarIndustry = "HomeStarIndustry"; // 5
            public const string HomeStarScience = "HomeStarScience"; // 1
            public const string StartingFleets = "StartingFleets"; // 2
            public const string StartingStars = "StartingStars"; // 6
            public const string StarsPerPlayer = "StarsPerPlayer"; // 20
        }
    }
}

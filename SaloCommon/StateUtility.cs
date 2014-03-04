using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salo.Live.Models;

namespace Salo
{
    public class StateUtility
    {
        public readonly State State;
        public readonly Configuration Configuration;
        public StateUtility(State state, Configuration configuration)
        {
            this.State = state;
            this.Configuration = configuration;
        }

        public int CalculateUpgradeCost(Star star, string upgrade)
        {
            switch(upgrade){
                case Star.Upgrade.Economy:
                    return (int)(2.5 * Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DevelopmentCostEconomy) * (star.Economy + 1) / (star.TotalResources / 100.0));
                case Star.Upgrade.Industry:
                    return (int)(5 * Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DevelopmentCostIndustry) * (star.Industry + 1) / (star.TotalResources / 100.0));
                case Star.Upgrade.Science:
                    return (int)(20 * Configuration.GetSettingAsInt(Configuration.ConfigurationKeys.DevelopmentCostScience) * (star.Science + 1) / (star.TotalResources / 100.0));
                case Star.Upgrade.WarpGate:
                    return (int)(100 / (star.TotalResources / 100.0));
                default:
                    return 0;
            }
        }

        public double GetFleetRange(Player player)
        {
            return Configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.BaseFleetRange) + (player.Technology[Research.Propulsion].Level * Configuration.GetSettingAsDouble(Configuration.ConfigurationKeys.BaseFleetRange));
        }

        public bool CanReach(Star origin, Star destination)
        {
            var fleetRange = GetFleetRange(this.State.Players[origin.PlayerId]);
            if (origin.DistanceTo(star) <= fleetRange)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

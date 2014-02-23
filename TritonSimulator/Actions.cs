using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TritonSimulator.InternalModels;

namespace TritonSimulator
{
    public static class Actions
    {
        public enum UpgradeType {
            Economy,
            Industry,
            Science,
            WarpGate
        }

        private const int DevelopmentCostEconomy = 2;
        private const int DevelopmentCostIndustry = 2;
        private const int DevelopmentCostScience = 2;

        public static int TrueResources(this Star star){
            return star.Resources + (star.Owner.Tech.Levels[Technology.Technologies.Terraforming] * 5);
        }

        public static int UpgradeCost(this Star star, UpgradeType upgradeType){
            switch(upgradeType){
                case UpgradeType.Economy:
                    return (int)(2.5 * DevelopmentCostEconomy * (star.Economy + 1) / (star.TrueResources() / 100));
                case UpgradeType.Industry:
                    return (int)(5 * DevelopmentCostIndustry * (star.Industry + 1) / (star.TrueResources() / 100));
                case UpgradeType.Science:
                    return (int)(20 * DevelopmentCostScience * (star.Science + 1) / (star.TrueResources() / 100));
                case UpgradeType.WarpGate:
                    return (int)(100 / (star.TrueResources() / 100));
                default:
                    return 0;
            }
        }

        public static void Upgrade(this Star star, UpgradeType upgradeType)
        {
            if (star.WarpGate && upgradeType == UpgradeType.WarpGate)
            {
                return;
            }

            var upgradeCost = star.UpgradeCost(upgradeType);
            if (star.Owner.Cash >= upgradeCost)
            {
                star.Owner.Cash -= upgradeCost;
                switch (upgradeType)
                {
                    case UpgradeType.Economy:
                        star.Economy += 1;
                        break;
                    case UpgradeType.Industry:
                        star.Industry += 1;
                        break;
                    case UpgradeType.Science:
                        star.Science += 1;
                        break;
                    case UpgradeType.WarpGate:
                        star.WarpGate = true;
                        break;
                    default:
                        return;
                }
            }
        }

        public static bool IsVisible(this Star star, Player player)
        {
            return true; // omnipotence! Need to actually calculate
            // visibility is based only on stars, ships don't add to visibility
            // also based on scanning technology
        }
    }
}

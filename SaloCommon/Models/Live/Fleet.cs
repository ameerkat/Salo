using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Salo
{
    public class Fleet
    {
        public Fleet(Fleet fleet)
        {
            this.CurrentStar = fleet.CurrentStar;
            this.DestinationStar = fleet.DestinationStar;
            this.DistanceToDestination = fleet.DistanceToDestination;
            this.Id = fleet.Id;
            this.InTransit = fleet.InTransit;
            this.Name = fleet.Name;
            this.OriginStar = fleet.OriginStar;
            this.PlayerId = fleet.PlayerId;
            this.Ships = fleet.Ships;
            this.ToProcess = fleet.ToProcess;
            this.l = fleet.l;
            this.lx = fleet.lx;
            this.ly = fleet.ly;
            this.o = fleet.o;
            this.ouid = fleet.ouid;
            this.w = fleet.w;
        }

        public Fleet()
        {
            
        }

        /// <summary>
        /// Optional, Unsure?
        /// </summary>
        [Browsable(false)]
        public int ouid { get; set; }

        /// <summary>
        /// Unique ID for Fleet (used as dictionary index)
        /// </summary>
        [JsonProperty("uid")]
        public int Id { get; set; }

        /// <summary>
        /// Unsure, all Zeroes
        /// </summary>
        [Browsable(false)]
        public int l { get; set; }

        /// <summary>
        /// Unsure, array of arrays only shows up for moving carriers
        /// Possible only for your own carriers, orders?
        /// </summary>
        [Browsable(false)]
        public int[][] o { get; set; }

        /// <summary>
        /// Carrier Name
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }

        /// <summary>
        /// Player Unique Id
        /// </summary>
        [JsonProperty("puid")]
        public int PlayerId { get; set; }

        /// <summary>
        /// Unsure, all Zeroes
        /// </summary>
        [Browsable(false)]
        public int w { get; set; }

        private double? _x, _y;

        /// <summary>
        /// Current y coordinate position
        /// </summary>
        [JsonProperty("y")]
        public double Y {
            get
            {
                if (_y != null)
                {
                    return _y.Value;
                }
                else
                {
                    if (this.InTransit)
                    {
                        var deltaX = this.DestinationStar.X - this.OriginStar.X;
                        var deltaY = this.DestinationStar.Y - this.OriginStar.Y;
                        var length = Math.Sqrt(deltaX*deltaX + deltaY*deltaY);
                        return OriginStar.Y + (deltaY*((length - DistanceToDestination)/length));
                    }
                    else
                    {
                        return CurrentStar.Y;
                    }
                }
            }
            set { _y = value; }
        }

        /// <summary>
        /// Current x coordinate position
        /// </summary>
        [JsonProperty("x")]
        public double X {
            get
            {
                if (_x != null)
                {
                    return _x.Value;
                }
                else
                {
                    if (this.InTransit)
                    {
                        var deltaX = this.DestinationStar.X - this.OriginStar.X;
                        var deltaY = this.DestinationStar.Y - this.OriginStar.Y;
                        var length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                        return OriginStar.X + (deltaX * ((length - DistanceToDestination) / length));
                    }
                    else
                    {
                        return CurrentStar.X;
                    }
                }
            }
            set { _x = value; }
        }

        /// <summary>
        /// Number of ships loaded
        /// </summary>
        [JsonProperty("st")]
        public int Ships { get; set; }

        [Browsable(false)]
        public double lx { get; set; }
        [Browsable(false)]
        public double ly { get; set; }

        /*
         * Simulator Use Only
         */
        internal bool InTransit { get; set; }
        internal Star OriginStar { get; set; }
        internal Star DestinationStar { get; set; }
        internal double DistanceToDestination { get; set; }
        internal Star CurrentStar { get; set; }
        internal bool ToProcess { get; set; }
    }
}

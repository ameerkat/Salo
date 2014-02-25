using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Web;
using Salo.Models;

namespace Salo
{
    public class SimulatorWebActionHandler : IActionHandler
    {
        private string _endpoint;
        private string _gameId;
        public string Endpoint { get { return _endpoint; } }
        public string GameId { get { return _gameId; } }
        public SimulatorWebActionHandler(string endpoint, string gameId)
        {
            _endpoint = endpoint;
            _gameId = gameId;
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            if (nvc == null)
            {
                return String.Empty;
            }

            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

        private const string StarIdParam = "starId";
        private const string UpgradeTypeParam = "upgradeType";
        private const string PlayerIdParam = "playerId";
        private const string OriginStarIdParam = "originStarId";
        private const string DestinationStarIdParam = "destinationStarId";
        private const string ShipsParam = "ships";
        private const string TechnologyParam = "technology";
        private const string HttpGet = "GET";
        private const string HttpPost = "POST";

        private T DoAction<T>(String action, NameValueCollection paramsCollection)
        {
            string fullUrl = string.Format("{0}/{1}/{2}{3}", Endpoint, GameId, action, ToQueryString(paramsCollection));

            System.Net.WebRequest oRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fullUrl);
            oRequest.Method = HttpGet;
            T result = default(T);

            using (System.Net.HttpWebResponse oResponse = (System.Net.HttpWebResponse)oRequest.GetResponse())
            {
                var s = new DataContractJsonSerializer(typeof(T));
                var stream = oResponse.GetResponseStream();
                if (stream != null)
                {
                    result = (T) s.ReadObject(stream);
                }
            }

            return result;
        }

        private void DoAction(String action, NameValueCollection paramsCollection)
        {
            string fullUrl = string.Format("{0}/{1}/{2}{3}", Endpoint, GameId, action, ToQueryString(paramsCollection));

            System.Net.WebRequest oRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fullUrl);
            oRequest.Method = HttpPost;

            System.Net.HttpWebResponse oResponse = (System.Net.HttpWebResponse) oRequest.GetResponse();
        }

        public int UpgradeCost(Models.Star star, UpgradeType upgradeType)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, star.Id.ToString());
            nvc.Add(UpgradeTypeParam, upgradeType.ToString());
            return DoAction<int>("UpgradeCost", nvc);
        }

        public void Upgrade(Models.Star star, UpgradeType upgradeType)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, star.Id.ToString());
            nvc.Add(UpgradeTypeParam, upgradeType.ToString());
            DoAction("Upgrade", nvc);
        }

        public bool IsVisible(IEnumerable<Models.Star> stars, Models.Star star, Models.Player player)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, star.Id.ToString());
            nvc.Add(PlayerIdParam, player.Id.ToString());
            return DoAction<bool>("IsVisible", nvc);
        }

        public IEnumerable<Models.Star> GetVisibleStars(IEnumerable<Models.Star> stars, Models.Player player)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(PlayerIdParam, player.Id.ToString());
            return DoAction<IEnumerable<Star>>("GetVisibleStars", nvc);
        }

        public bool IsReachableByPlayer(IEnumerable<Models.Star> stars, Models.Star star, Models.Player player)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, star.Id.ToString());
            nvc.Add(PlayerIdParam, player.Id.ToString());
            return DoAction<bool>("IsReachableByPlayer", nvc);
        }

        public IEnumerable<Models.Star> GetReachableStars(IEnumerable<Models.Star> stars, Models.Player player)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(PlayerIdParam, player.Id.ToString());
            return DoAction<IEnumerable<Star>>("GetReachableStars", nvc);
        }

        public bool IsReachable(IEnumerable<Models.Star> stars, Models.Star originStar, Models.Star destinationStar)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(OriginStarIdParam, originStar.Id.ToString());
            nvc.Add(DestinationStarIdParam, destinationStar.Id.ToString());
            return DoAction<bool>("IsReachable", nvc);
        }

        public void BuildFleet(Models.Game game, Models.Star star)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, star.Id.ToString());
            DoAction("BuildFleet", nvc);
        }

        public void Move(Models.Game game, Models.Star originStar, Models.Star destinationStar, int ships)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(OriginStarIdParam, originStar.Id.ToString());
            nvc.Add(DestinationStarIdParam, destinationStar.Id.ToString());
            nvc.Add(ShipsParam, ships.ToString());
            DoAction("Move", nvc);
        }

        public void MoveAll(Models.Game game, Models.Star originStar, Models.Star destinationStar)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(OriginStarIdParam, originStar.Id.ToString());
            nvc.Add(DestinationStarIdParam, destinationStar.Id.ToString());
            DoAction("MoveAll", nvc);
        }

        public void SetCurrentResearch(Models.Player player, Technologies tech)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(PlayerIdParam, player.Id.ToString());
            nvc.Add(TechnologyParam, tech.ToString());
            DoAction("SetCurrentResearch", nvc);
        }

        public void SetNextResearch(Models.Player player, Technologies tech)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(PlayerIdParam, player.Id.ToString());
            nvc.Add(TechnologyParam, tech.ToString());
            DoAction("SetNextResearch", nvc);
        }

        public Models.Star GetCheapestUpgradeStar(IEnumerable<Models.Star> stars, Models.Player player, UpgradeType upgrade)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(PlayerIdParam, player.Id.ToString());
            nvc.Add(UpgradeTypeParam, upgrade.ToString());
            return DoAction<Star>("GetCheapestUpgradeStar", nvc);
        }

        public int CalculateAttackSuccess(Models.Game game, Models.Star originStar, Models.Star destinationStar)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(OriginStarIdParam, originStar.Id.ToString());
            nvc.Add(DestinationStarIdParam, destinationStar.Id.ToString());
            return DoAction<int>("CalculateAttackSuccess", nvc);
        }

        public bool HasFleet(Models.Game game, Models.Star star)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add(StarIdParam, star.Id.ToString());
            return DoAction<bool>("HasFleet", nvc);
        }

        public int FleetCost
        {
            get
            {
                return DoAction<int>("FleetCost", null);
            }
        }
    }
}

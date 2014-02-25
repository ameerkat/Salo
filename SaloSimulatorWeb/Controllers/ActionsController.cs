﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Salo;
using Salo.Models;

namespace TritonSimulatorWeb.Controllers
{
    public class ActionsController : Controller
    {
        /// <summary>
        /// Returns the current game state
        /// </summary>
        /// <returns>Game state in the form of TritonSimulator.InternalModels.Game</returns>
        [System.Web.Http.HttpGet]
        public JsonResult Index(int id)
        {
            Game game; 
            if(GameIndex.Games.TryGetValue(id, out game)){
                return Json(game, JsonRequestBehavior.AllowGet);
            } else {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        private Game _GetGame(int id)
        {
            Game game;
            if (!GameIndex.Games.TryGetValue(id, out game))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { ReasonPhrase = "Game not found." });
            }
            return game;
        }

        private Star _GetStar(Game game, int id)
        {
            var star = game.Stars.FirstOrDefault(x => x.Id == id);
            if (star == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { ReasonPhrase = "Star not found." });
            }
            return star;
        }

        private Player _GetPlayer(Game game, int id)
        {
            var player = game.Players.FirstOrDefault(x => x.Id == id);
            if (player == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { ReasonPhrase = "Player not found." });
            }
            return player;
        }

        private IActionHandler _GetActionHandler(Game game)
        {
            return GameIndex.ActionHandlers[game];
        }

        [System.Web.Http.HttpPost]
        public void Upgrade(int id, int starId, string upgradeType)
        {
            Game game = _GetGame(id);
            Star star = _GetStar(game, starId);
            UpgradeType upgrade;
            if(!Enum.TryParse<UpgradeType>(upgradeType, out upgrade)){
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid upgrade type." });
            }

            _GetActionHandler(game).Upgrade(star, upgrade);
        }

        [System.Web.Http.HttpGet]
        public void UpgradeCost(int id, int starId, string upgradeType)
        {
            Game game = _GetGame(id);
            Star star = _GetStar(game, starId);
            UpgradeType upgrade;
            if (!Enum.TryParse<UpgradeType>(upgradeType, out upgrade))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid upgrade type." });
            }

            _GetActionHandler(game).UpgradeCost(star, upgrade);
        }

        [System.Web.Http.HttpGet]
        public JsonResult IsVisible(int id, int starId, int playerId)
        {
            Game game = _GetGame(id);
            Star targetStar = _GetStar(game, starId);
            List<Star> stars = game.Stars;
            Player player = _GetPlayer(game, playerId);

            return Json(_GetActionHandler(game).IsVisible(stars, targetStar, player), JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpGet]
        public JsonResult GetVisibleStars(int id, int playerId)
        {
            Game game = _GetGame(id);
            List<Star> stars = game.Stars;
            Player player = _GetPlayer(game, playerId);

            return Json(_GetActionHandler(game).GetVisibleStars(stars, player), JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpGet]
        public JsonResult IsReachable(int id, int starId, int playerId)
        {
            Game game = _GetGame(id);
            Star targetStar = _GetStar(game, starId);
            List<Star> stars = game.Stars;
            Player player = _GetPlayer(game, playerId);

            return Json(_GetActionHandler(game).IsReachable(stars, targetStar, player), JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public void BuildFleet(int id, int starId)
        {
            Game game = _GetGame(id);
            Star star = _GetStar(game, starId);

            _GetActionHandler(game).BuildFleet(game, star);
        }

        [System.Web.Http.HttpPost]
        public void Move(int id, int originStarId, int destinationStarId, int ships)
        {
            Game game = _GetGame(id);
            Star originStar = _GetStar(game, originStarId);
            Star destinationStar = _GetStar(game, destinationStarId);

            _GetActionHandler(game).Move(game, originStar, destinationStar, ships);
        }

        [System.Web.Http.HttpPost]
        public void MoveAll(int id, int originStarId, int destinationStarId)
        {
            Game game = _GetGame(id);
            Star originStar = _GetStar(game, originStarId);
            Star destinationStar = _GetStar(game, destinationStarId);

            _GetActionHandler(game).MoveAll(game, originStar, destinationStar);
        }

        [System.Web.Http.HttpPost]
        public void SetCurrentResearch(int id, int playerId, string technology)
        {
            Game game = _GetGame(id);
            Player player = _GetPlayer(game, playerId);
            Technologies tech;
            if (!Enum.TryParse<Technologies>(technology, out tech))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid technology type." });
            }

            _GetActionHandler(game).SetCurrentResearch(player, tech);
        }

        [System.Web.Http.HttpPost]
        public void SetNextResearch(int id, int playerId, string technology)
        {
            Game game = _GetGame(id);
            Player player = _GetPlayer(game, playerId);
            Technologies tech;
            if (!Enum.TryParse<Technologies>(technology, out tech))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid technology type." });
            }

            _GetActionHandler(game).SetNextResearch(player, tech);
        }

        [System.Web.Http.HttpGet]
        public JsonResult GetCheapestUpgradeStar(int id, string upgradeType)
        {
            Game game = _GetGame(id);
            List<Star> stars = game.Stars;
            UpgradeType upgrade;
            if (!Enum.TryParse<UpgradeType>(upgradeType, out upgrade))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid upgrade type." });
            }

            return Json(_GetActionHandler(game).GetCheapestUpgradeStar(stars, upgrade), JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpGet]
        public JsonResult CalculateAttackSuccess(int id, int originStarId, int destinationStarId)
        {
            Game game = _GetGame(id);
            Star originStar = _GetStar(game, originStarId);
            Star destinationStar = _GetStar(game, destinationStarId);

            return Json(_GetActionHandler(game).CalculateAttackSuccess(game, originStar, destinationStar), JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpGet]
        public JsonResult HasFleet(int id, int starId)
        {
            Game game = _GetGame(id);
            Star star = _GetStar(game, starId);
            return Json(_GetActionHandler(game).HasFleet(game, star), JsonRequestBehavior.AllowGet);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Salo.Models;

namespace SaloSimulatorWeb.Controllers
{
    public class GamesController : ApiController
    {
        /// <summary>
        /// Returns the current game state
        /// </summary>
        /// <returns>Game state in the form of TritonSimulator.InternalModels.Game</returns>
        [System.Web.Http.HttpGet]
        public Game Index(int id)
        {
            Game game;
            if (GameIndex.Games.TryGetValue(id, out game))
            {
                return game;
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Returns the current game state
        /// </summary>
        /// <returns>Game state in the form of TritonSimulator.InternalModels.Game</returns>
        [System.Web.Http.HttpPost]
        public int Create()
        {
            var gameId = GameIndex.GameId++;
            GameIndex.Games.Add(gameId, new Game());
            return gameId;
        }
    }
}

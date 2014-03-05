using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Salo.SaloSimulatorWeb2.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        public Configuration Configuration { get; set; }
        public List<Bot> Players { get; set; }
        public ApplicationUser Creator { get; set; }
        public State InitialState { get; set; }
        public string ActionFilePath { get; set; }
        public DateTime Created { get; set; }
        public DateTime Finished { get; set; }
    }
}
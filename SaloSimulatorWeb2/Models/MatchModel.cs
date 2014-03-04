using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SaloSimulatorWeb2.Models
{
    /// <summary>
    /// Provides a logical grouping for a series of games
    /// </summary>
    public class Match
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Game> Games { get; set; }
    }
}
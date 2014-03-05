using System;
using System.ComponentModel.DataAnnotations;

namespace Salo.SaloSimulatorWeb2.Models
{
    public class Bot
    {
        [Key]
        public int Id { get; set; }

        public ApplicationUser Uploader { get; set; }
        public String BotName { get; set; }
        public String BotVersion { get; set; }
        public String BotDescription { get; set; }
        public String AssemblyPath { get; set; }
        public DateTime Created { get; set; }
    }
}
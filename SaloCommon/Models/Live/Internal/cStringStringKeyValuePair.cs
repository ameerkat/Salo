using System;
using System.ComponentModel.DataAnnotations;

namespace Salo
{
    public class cStringStringKeyValuePair
    {
        [Key]
        public int Id { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }

        public cStringStringKeyValuePair(String Key, String Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}

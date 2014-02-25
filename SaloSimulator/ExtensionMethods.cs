using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salo.Models;

namespace Salo
{
    public static class ExtensionMethods
    {
        public static Random rnd = new Random();
        public static T RandomElement<T>(this List<T> l)
        {
            return l[rnd.Next(0, l.Count)];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{
    public class TargetPlayer
    {
        public Player Target { get; set; }
        public int Damage { get; set; }

        public bool Dodged { get; set; }

        // Negated?
        public bool Warded { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core
{

    public enum TargetResult
    {
        None,
        Success, // Damage
        Failed,  // Dodged
        Warded
    }

    public class TargetPlayerResponse
    {
        public TargetPlayer Player { get; set; }
        public Prompts.UserPrompt Prompt { get; set; }
    }

    public class TargetPlayer
    {
        public Player Target { get; set; }
        public TargetResult Result { get; set; }
        public int Damage { get; set; }

        public TargetPlayer(Player player)
        {
            this.Target = player;
        }
    }
}

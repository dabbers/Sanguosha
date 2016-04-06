using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Prompts
{
    public class YesNoUserPrompt : OptionsUserPrompt
    {

        public YesNoUserPrompt(string display) : base(UserPromptType.YesNo, display, new string[] { "Yes", "No" })
        {
        }
    }
}

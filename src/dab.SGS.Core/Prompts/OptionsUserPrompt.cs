using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dab.SGS.Core.Prompts
{
    public class OptionsUserPrompt : UserPrompt
    {
        public string Display { get; protected set; }
        public string[] Options { get; protected set; }

        /// <summary>
        /// Index of Options that the user selects
        /// </summary>
        public int Option { get; set; }

        public OptionsUserPrompt(UserPromptType type, string display, string[] options) : base(type)
        {
            this.Display = display;
            this.Options = options;
        }

        public override string ToString()
        {
            return this.Display;
        }
    }
}

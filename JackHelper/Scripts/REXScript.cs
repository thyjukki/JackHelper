using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper.Scripts
{
    class REXScript : Script
    {
        public string InputPath;
        public string OutputPath;
        public string Tag;
        public string VersionsPath;
        public override PowerShell GetPSScript()
        {
            PowerShell psinstance = base.GetPSScript();

            string script = string.Format(@"java -jar \\tanas\share\qascripts\REX.jar  {0} {1} {2} {3}",
                InputPath,
                OutputPath,
                Tag,
                VersionsPath);
            Console.WriteLine("Execing " + script);
            psinstance.AddScript(string.Format("Write-Host ('{0}')", script));
            psinstance.AddScript(script);

            return psinstance;
        }

        public REXScript (string input, string output, string tag, string versions)
        {
            InputPath = input;
            OutputPath = output;
            Tag = tag;
            VersionsPath = versions;
        }
    }
}

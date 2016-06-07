using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper.Scripts
{
    public abstract class Script
    {
        public virtual PowerShell GetPSScript()
        {
            PowerShell psinstance = PowerShell.Create();

            /*psinstance.AddCommand("Set-ExecutionPolicy");
            psinstance.AddParameter("-ExecutionPolicy", "Bypass");*/
            return psinstance;
        }
    }
}

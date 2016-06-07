using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper.Tasks
{
    class RebootTask : Task
    {
        public override string GetShortDesc
        {
            get
            {
                return "Reboot";
            }
        }

        public RebootTask(List<Computer> computers, List<string> os) : base(computers, os)
        {
        }

        public override List<Tuple<string, object>> GetJackParameters()
        {
            List<Tuple<string, object>> collector = new List<Tuple<string, object>>();
            collector.Add(new Tuple<string, object>("-Task", "reboot"));
            collector.Add(new Tuple<string, object>("-Computers", ParseComputers()));
            return collector;
        }

        public override string ToString()
        {
            return string.Format("Reboot {0}", string.Join(" ", ParseComputers().ToArray()));
        }
    }
}

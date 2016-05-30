using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper
{
    public class ExecTask : Task
    {
        public string Command;

        public override string GetShortDesc
        {
            get
            {
                return "Exec";
            }
        }

        public ExecTask (string command, string computers, List<string> os) : base(computers, os)
        {
            Command = command;
        }

        public override List<Tuple<string, object>> GetJackParameters()
        {
            List<Tuple<string, object>> collector = new List<Tuple<string, object>>();
            collector.Add(new Tuple<string, object>("-Task", "exec"));
            collector.Add(new Tuple<string, object>("-Command", Command));
            collector.Add(new Tuple<string, object>("-OSes", OS));
            collector.Add(new Tuple<string, object>("-Computers", Computers));

            return collector;
        }
    }
}

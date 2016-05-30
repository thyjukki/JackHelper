using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper
{
    public abstract class Task
    {
        public string Computers;
        public List<string> OS;

        public abstract string GetShortDesc
        {
            get;
        }

        public Task(string computers, List<string> os)
        {
            Computers = computers;
            OS = os;
        }


        public abstract List<Tuple<string, object>> GetJackParameters();
    }
}

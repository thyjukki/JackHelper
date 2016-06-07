using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper
{
    public class Computer : IComparable
    {
        public string name;

        public Computer()
        {

        }

        public Computer(string n)
        {
            name = n;
        }
        public override string ToString()
        {
            return name;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Computer otherName = obj as Computer;
            if (otherName != null)
                return this.name.CompareTo(otherName.name);
            else
                throw new ArgumentException("Object is not a Computer");
        }
    }
    public class Group : Computer
    {
        public List<Computer> Computers;

        public Group(string n) : base("#" + n)
        {
            Computers = new List<Computer>();
        }

        public Group(string n, List<Computer> comps) : this(n)
        {
            Computers = comps;
        }

        public override string ToString()
        {
            string comps = string.Empty;

            foreach (var computer in Computers)
            {
                comps = comps + computer.name + " ";
            }
            comps = comps.Trim();
            return string.Format("Group {0}: {1}", name, comps);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper
{
    public abstract class Task
    {
        public List<Computer> Computers;
        public List<string> OS;

        public abstract string GetShortDesc
        {
            get;
        }

        public Task(List<Computer> computers, List<string> os)
        {
            Computers = computers;
            OS = os;
        }


        public abstract List<Tuple<string, object>> GetJackParameters();

        public virtual void FillFields(MainWindow mainWindow)
        {
            mainWindow.tasksBox.Text = GetShortDesc;
            mainWindow.ComputerWindow.ClearSelections();
            mainWindow.ComputerWindow.SelectComputers(Computers);
        }


        public List<string> ParseComputers()
        {
            List<string> comps = new List<string>();
            foreach (var item in Computers)
            {
                comps.Add(item.name);
            }

            return (comps);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JackHelper.Tasks
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

        public ExecTask (string command, List<Computer> computers, List<string> os) : base(computers, os)
        {
            Command = command;
        }

        public override List<Tuple<string, object>> GetJackParameters()
        {
            List<Tuple<string, object>> collector = new List<Tuple<string, object>>();
            collector.Add(new Tuple<string, object>("-Task", "exec"));
            collector.Add(new Tuple<string, object>("-OSes", OS));
            collector.Add(new Tuple<string, object>("-Computers", ParseComputers()));
            collector.Add(new Tuple<string, object>("-Command", Regex.Split(Command.Trim(), @"\s+")));

            return collector;
        }

        public override string ToString()
        {
            return string.Format("Exec {0}", Command);
        }

        public override void FillFields(MainWindow mainWindow)
        {
            base.FillFields(mainWindow);

            mainWindow.commandTextBox.Text = Command;
        }
    }
}
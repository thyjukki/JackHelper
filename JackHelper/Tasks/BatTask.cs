using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHelper.Tasks
{
    class BatTask : Task
    {
        public enum BatType
        {
            Valeta = 0,
            Dandia = 1
        }
        BatType Type;
        public string Zip;
        public string Output;
        public int TestCount;
        public string Mode;
        public bool NoCopy;

        public override string GetShortDesc
        {
            get
            {
                if (Type == BatType.Dandia)
                    return "Dandia";
                else
                    return "Valeta";
            }
        }

        public BatTask(string zip, string output, int count, string mode, BatType batType, bool noCopy, List<Computer> computers, List<string> os) : base(computers, os)
        {
            Zip = zip;
            Output = output;
            TestCount = count;
            Mode = mode;
            Type = batType;
            NoCopy = noCopy;
        }

        public override List<Tuple<string, object>> GetJackParameters()
        {
            List<Tuple<string, object>> collector = new List<Tuple<string, object>>();
            collector.Add(new Tuple<string, object>("-Task", "exec"));
            collector.Add(new Tuple<string, object>("-OSes", OS));
            collector.Add(new Tuple<string, object>("-Computers", ParseComputers()));
            List<string> command = new List<string>();
            command.Add("powershell");
            switch (Type)
            {
                case BatType.Valeta:
                    command.Add(@"\\tanas\share\qascripts\valeta-gt.ps1");
                    command.Add("-Mode");
                    command.Add(Mode.Trim());
                    break;
                case BatType.Dandia:
                    command.Add(@"\\tanas\share\qascripts\dandia-gt.ps1");
                    break;
            }
            command.Add("-Zip");
            command.Add(Zip.Trim());
            command.Add("-LogDir");
            command.Add(Output.Trim());
            command.Add("-TestCount");
            command.Add(TestCount.ToString());

            if (NoCopy)
                command.Add("-NoCopy");

            collector.Add(new Tuple<string, object>("-Command", command));

            return collector;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", GetShortDesc, Zip);
        }

        public override void FillFields(MainWindow mainWindow)
        {
            base.FillFields(mainWindow);

            mainWindow.zipBox.Text = Zip;
            mainWindow.outputPathcBox.Text = Output;
            mainWindow.modeBox.Text = Mode;
            mainWindow.nocopyBox.IsChecked = NoCopy;
            mainWindow.batTypeBox.SelectedIndex = (int)Type;
        }
    }
}

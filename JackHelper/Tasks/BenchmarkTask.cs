using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JackHelper
{
    class BenchmarkTask : Task
    {
        public string Label;
        public string Product;
        public string Parameters;
        public int Loops;

        public override string GetShortDesc
        {
            get
            {
                return "Benchmark";
            }
        }

        public BenchmarkTask(string label, string product, string parameters, int loops,
            List<Computer> computers, List<string> os) : base(computers, os)
        {
            Label = label;
            Product = product;
            Parameters = parameters;
            Loops = loops;
        }

        public override List<Tuple<string, object>> GetJackParameters()
        {
            List<Tuple<string, object>> collector = new List<Tuple<string, object>>();
            collector.Add(new Tuple<string, object>("-Task", "benchmark"));
            collector.Add(new Tuple<string, object>("-Computers", ParseComputers()));
            collector.Add(new Tuple<string, object>("-Label", Regex.Replace(Label.Trim(), @"\s+", "_")));
            collector.Add(new Tuple<string, object>("-OSes", OS));
            collector.Add(new Tuple<string, object>("-Loops", Loops));
            collector.Add(new Tuple<string, object>("-Product", Product));
            collector.Add(new Tuple<string, object>("-Parameters", Regex.Split(Parameters.Trim(), @"\s+")));

            return collector;
        }

        public override string ToString()
        {
            return string.Format("Benchmark {0} {1}", Product, Label);
        }

        public override void FillFields(MainWindow mainWindow)
        {
            mainWindow.labelBox.Text = Label;
            mainWindow.loopsBox.Text = Loops.ToString();
            mainWindow.productBox.Text = Product;
            mainWindow.parametersBox.Text = Parameters;
            base.FillFields(mainWindow);
        }
    }
}

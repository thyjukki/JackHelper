using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JackHelper
{
    /// <summary>
    /// Interaction logic for ComputerSelect.xaml
    /// </summary>
    public partial class ComputerSelect : Window
    {
        public List<Group> Groups = new List<Group>();
        public List<Computer> Computers = new List<Computer>();
        public ComputerSelect()
        {
            InitializeComponent();
            loadComputers();
        }

        void loadComputers()
        {
            List<Group> groups = new List<Group>();
            List<Computer> computers = new List<Computer>();

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(@"\\tanas\share\jack\testhardware.txt");
            while ((line = file.ReadLine()) != null)
            {
                Char commentor = ';';
                string entry = line.Split(commentor)[0];//Remove commented lines

                entry = Regex.Replace(entry, @"\s+", " ");// replace all whitespace on hte line with single space

                var values = entry.Split('=');

                if (values.Length == 2)
                {
                    Group group = new Group(values[0].Trim()); //create new group

                    var computerLists = new List<Computer>();

                    foreach (var nameS in values[1].Trim().Split(' ').ToList())
                    {
                        string name = nameS.Trim();
                        Computer computer;
                        if ((computer = computers.Find(x => x.name == name)) == null)
                        {
                            if (name.Length > 0)
                            {
                                if (name[0] == '#')
                                {
                                    Console.WriteLine(string.Format("Error: Computer group {0} not defined", name));
                                }
                                else
                                {
                                    computer = new Computer(name);
                                    computers.Add(computer);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error: computer with no name");
                            }
                        }

                        if (computer != null)
                        {
                            group.computers.Add(computer);
                        }
                    }

                    group.computers.Sort();
                    groups.Add(group);
                    computers.Add(group);


                }
                counter++;
            }

            file.Close();
        }
    }

    public class Computer : DependencyObject, IComparable
    {
        public string name;

        public Computer()
        {

        }

        public Computer (string n)
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
        public List<Computer> computers;

        public Group(string n) : base("#" + n)
        {
            computers = new List<Computer>();
        }

        public Group(string n, List<Computer> comps) : this(n)
        {
            computers = comps;
        }

        public override string ToString()
        {
            string comps = string.Empty;

            foreach (var computer in computers)
            {
                comps = comps + computer.name + " ";
            }
            comps = comps.Trim();
            return string.Format("Group {0}: {1}", name, comps);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace JackHelper
{
    /// <summary>
    /// Interaction logic for ComputerSelect.xaml
    /// </summary>
    public partial class ComputerSelect : Window
    {
        public ObservableCollection<Group> Groupss { get; set; }
        public List<Group> Groups = new List<Group>();
        Dictionary<Computer, TreeViewModel> entries;
        public void ClearSelections()
        {
            tv.IsChecked = false;
        }

        public void SelectComputers(List<Computer> computers)
        {
            foreach (var computer in computers)
            {
                if (entries.ContainsKey(computer))
                {
                    TreeViewModel tvItem = entries[computer];

                    tvItem.IsChecked = true;
                }
            }
        }

        TreeViewModel tv;
        public ComputerSelect()
        {
            InitializeComponent();
            loadComputers();
        }

        TreeViewModel TreeViewModelCollector(TreeViewModel treeView,
            Dictionary<Computer, TreeViewModel> entries, Computer entry)
        {
            if (!entries.ContainsKey(entry))
            {
                TreeViewModel item = new TreeViewModel(entry.name);
                entries.Add(entry, item);
                treeView.Children.Add(item);
                item.AttachedComp = entry;


                if (entry.GetType() == typeof(Group))
                {
                    Group group = (Group)entry;
                    foreach (var computer in group.Computers)
                    {
                        TreeViewModel child = TreeViewModelCollector(item, entries, computer);

                        if (!item.Children.Contains(child))
                            item.Children.Add(child);
                    }
                }
                else
                {
                    Computer computer = (Computer)entry;
                    return item;
                }
            }

            return entries[entry];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void CollapseTreeviewItems(TreeViewModel Item)
        {
            Item.IsExpanded = false;

            foreach (TreeViewModel item in Item.Children)
            {
                item.IsExpanded = false;

                CollapseTreeviewItems(item);
            }
        }

        void buildTreeview()
        {
            entries = new Dictionary<Computer, TreeViewModel>();


            tv = new TreeViewModel("Computers");
            foreach (var group in Groups)
            {
                TreeViewModelCollector(tv, entries, group);
            }
            tv.Initialize();

            treeView1.ItemsSource = new List<TreeViewModel> { tv };
            foreach (TreeViewModel item in treeView1.Items)
                CollapseTreeviewItems(item);
            
        }

        void loadComputers()
        {
            List<Group> groups = new List<Group>();
            List<Computer> computers = new List<Computer>();

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(@"\\tanas.ldm.name\share\jack\testhardware.txt");
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
                            group.Computers.Add(computer);
                        }
                    }

                    group.Computers.Sort();
                    groups.Add(group);
                    computers.Add(group);


                }
                counter++;
            }

            file.Close();

            //TODO(Jukki) FIX ME
            Groups = groups;

            List<Group> parents = new List<Group>(groups);
            CheckSubGroups(ref parents);

            Groups = parents;

            buildTreeview();
        }

        private void CheckSubGroups(ref List<Group> groups)
        {
            foreach (var item in Groups)
            {
                foreach (var entry in item.Computers)
                {
                    if (Groups.Contains(entry))
                    {
                        //Console.WriteLine(entry.name);
                        groups.Remove((Group)entry);
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //Do some stuff here 

            //Hide Window
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object o)
            {
                Hide();
                return null;
            }, null);

            //Do not close application
            e.Cancel = true;

        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            List<Computer> computers = tv.GetTree();
            

            string comps = string.Empty;
            foreach (var item in computers)
            {
                comps = comps + item.name + " ";
            }

            Console.WriteLine(comps.Trim());
        }

        public List<Computer> SelectedComputers()
        {
            return tv.GetTree();
        }
    }
}

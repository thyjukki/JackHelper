using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management.Automation;
using System.Threading;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace JackHelper
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// TODO(Jukki) Clean this
    /// TODO(Jukki) Do better computer screen
    /// TODO(Jukki) Do templates
    /// </summary>
    public partial class MainWindow : Window
    {
        string SCRIPT_PATH = @"\\tanas\share\jack\add_task_test.ps1";
        public ComputerSelect ComputerWindow;
        public MainWindow()
        {
            InitializeComponent();
            outputBox.Clear();
            outputBox.IsReadOnly = true;

            taskHistoryBox.MouseDoubleClick += TaskHistoryBox_MouseDoubleClick;
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }

        private void TaskHistoryBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (taskHistoryBox.SelectedItem != null)
            {
                Task task = (Task)taskHistoryBox.SelectedItem;
                if (!Dispatcher.CheckAccess())
                {
                    Action action = delegate ()
                    {
                        task.FillFields(this);
                    };
                    Dispatcher.Invoke(DispatcherPriority.Normal, action);
                }
                else
                {
                    task.FillFields(this);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComputerWindow = new ComputerSelect { Owner = this };
            ComputerWindow.Show();
        }

        public void AppendOutputText(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Action action = delegate ()
                {
                    outputBox.AppendText(text);
                };
                Dispatcher.Invoke(DispatcherPriority.Normal,action);
            }
            else
            {
                outputBox.AppendText(text);
            }
        }

        void myInformationEventHandler(object sender, DataAddedEventArgs e)
        {
            VerboseRecord newRecord = ((PSDataCollection<VerboseRecord>)sender)[e.Index];
            AppendOutputText(newRecord.ToString() + "\n");
            Console.WriteLine(newRecord.ToString());
        }

        void myProgressEventHandler(object sender, DataAddedEventArgs e)
        {
            ProgressRecord newRecord = ((PSDataCollection<ProgressRecord>)sender)[e.Index];
            AppendOutputText(newRecord.Activity.ToString() + "\n");
            Console.WriteLine(newRecord.Activity.ToString());
        }

        void myDebugEventHandler(object sender, DataAddedEventArgs e)
        {
            DebugRecord newRecord = ((PSDataCollection<DebugRecord>)sender)[e.Index];
            AppendOutputText("Debug: " + newRecord.ToString() + "\n");
            Console.WriteLine(newRecord.ToString());
        }

        void myErrorEventHandler(object sender, DataAddedEventArgs e)
        {
            ErrorRecord newRecord = ((PSDataCollection<ErrorRecord>)sender)[e.Index];
            AppendOutputText("ERROR: " + newRecord.ToString() + "\n");
 
            Console.WriteLine(newRecord.ToString());
        }


        private void runTaskButton_Click(object sender, RoutedEventArgs e)
        {
            Task task;
            string taskName = tasksBox.Text;
            switch (taskName)
            {
                case "Benchmark":
                    try
                    {
                        task = new BenchmarkTask(labelBox.Text, productBox.Text, parametersBox.Text, int.Parse(loopsBox.Text), ComputerWindow.SelectedComputers(), parseOSList);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("ERROR", "Loops is not a number!");
                        return;
                    }
                    
                    break;
                case "Boot":
                    task = new BootTask(ComputerWindow.SelectedComputers(), parseOSList);
                    break;
                case "Reboot":
                    task = new RebootTask(ComputerWindow.SelectedComputers(), parseOSList);
                    break;
                case "Shutdown":
                    task = new ShutdownTask(ComputerWindow.SelectedComputers(), parseOSList);
                    break;
                case "Exec":
                    task = new ExecTask(commandTextBox.Text, ComputerWindow.SelectedComputers(), parseOSList);
                    break;
                case "Winupdate":
                    task = new WinUpdateTask(ComputerWindow.SelectedComputers(), parseOSList);
                    break;
                case "Update":
                    task = new UpdateTask(ComputerWindow.SelectedComputers(), parseOSList);
                    break;
                default:
                    return;
            }

            List<Tuple<string, object>> parameters = task.GetJackParameters();


            PowerShell psinstance = PowerShell.Create();
            psinstance.Streams.Verbose.DataAdded += myInformationEventHandler;
            psinstance.Streams.Progress.DataAdded += myProgressEventHandler;
            psinstance.Streams.Debug.DataAdded += myDebugEventHandler;
            psinstance.Streams.Error.DataAdded += myErrorEventHandler;

            //scriptInvoker.Invoke("Set-ExecutionPolicy Unrestricted Process");
            psinstance.AddCommand("Set-ExecutionPolicy");
            psinstance.AddParameter("-ExecutionPolicy", "Bypass");
            psinstance.AddCommand(SCRIPT_PATH);
            foreach (var param in parameters)
            {
                psinstance.AddParameter(param.Item1, param.Item2);
            }
            var results = psinstance.BeginInvoke();
            /*foreach (PSObject result in results)
            {
                AppendOutputText(result.ToString());
                Console.WriteLine(result.ToString());
            }*/
            ListBoxItem item = new ListBoxItem();
            item.Content = task.ToString();
            taskHistoryBox.Items.Add(task);
        }

        private List<string> parseOSList
        {
            get
            {
                var collector = new List<string>();
                foreach (CheckBox item in OSList.Items)
                {
                    if (item.IsChecked.HasValue && item.IsChecked.Value)
                    {
                        switch ((string)item.Content)
                        {
                            case "Current":
                                collector.Add("current");
                                break;
                            case "Windows 10":
                                collector.Add("10");
                                break;
                            case "Windows 8.1":
                                collector.Add("8.1");
                                break;
                            case "Windows 8":
                                collector.Add("8");
                                break;
                            case "Windows 7":
                                collector.Add("7");
                                break;
                            default:
                                break;
                        }
                    }
                }

                return collector;
            }
        }

        /// <summary>
        /// Event handler for when data is added to the output stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {
            // do something when an object is written to the output stream
            Console.WriteLine("Object added to output.");


        }

        /// <summary>
        /// Event handler for when Data is added to the Error stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all error output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            // do something when an error is written to the error stream
            Console.WriteLine("An error was written to the Error stream!");
        }

        private void computtersButton_Click(object sender, RoutedEventArgs e)
        {
            ComputerWindow.Show();
        }

        private void tasksBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void loopsBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "Unhandled exception");
        }
    }
}

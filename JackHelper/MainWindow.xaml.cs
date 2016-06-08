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
using JackHelper.Scripts;
using JackHelper.Tasks;
using System.Windows.Interop;

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

        #region initFunctions
        public MainWindow()
        {
            InitializeComponent();
            outputBox.Clear();
            outputBox.IsReadOnly = true;

            taskHistoryBox.MouseDoubleClick += TaskHistoryBox_MouseDoubleClick;
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "Unhandled exception");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComputerWindow = new ComputerSelect { Owner = this };
            WindowInteropHelper helper = new WindowInteropHelper(ComputerWindow);
            helper.EnsureHandle();
        }
        #endregion

        #region jackFunctions
        public void AppendOutputText(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Action action = delegate ()
                {
                    outputBox.AppendText(text);
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
            else
            {
                outputBox.AppendText(text);
            }
        }

        void jackInformationEventHandler(object sender, DataAddedEventArgs e)
        {
            VerboseRecord newRecord = ((PSDataCollection<VerboseRecord>)sender)[e.Index];
            AppendOutputText(newRecord.ToString() + "\n");
            Console.WriteLine(newRecord.ToString());
        }

        void jackProgressEventHandler(object sender, DataAddedEventArgs e)
        {
            ProgressRecord newRecord = ((PSDataCollection<ProgressRecord>)sender)[e.Index];
            AppendOutputText(newRecord.Activity.ToString() + "\n");
            Console.WriteLine(newRecord.Activity.ToString());
        }

        void jackDebugEventHandler(object sender, DataAddedEventArgs e)
        {
            DebugRecord newRecord = ((PSDataCollection<DebugRecord>)sender)[e.Index];
            AppendOutputText("Debug: " + newRecord.ToString() + "\n");
            Console.WriteLine(newRecord.ToString());
        }

        void jackErrorEventHandler(object sender, DataAddedEventArgs e)
        {
            ErrorRecord newRecord = ((PSDataCollection<ErrorRecord>)sender)[e.Index];
            AppendOutputText("ERROR: " + newRecord.ToString() + "\n");
 
            Console.WriteLine(newRecord.ToString());
        }
        #endregion

        #region tasks
        private void runTaskButton_Click(object sender, RoutedEventArgs e)
        {
            Tasks.Task task;
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
                case "Valeta/Dandia":
                    try
                    {
                        task = new BatTask(zipBox.Text, outputPathcBox.Text, int.Parse(countBox.Text), modeBox.Text, (BatTask.BatType) batTypeBox.SelectedIndex, nocopyBox.IsChecked.Value, ComputerWindow.SelectedComputers(), parseOSList);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("ERROR", "Test count is not a number!");
                        return;
                    }
                    break;
                default:
                    return;
            }

            List<Tuple<string, object>> parameters = task.GetJackParameters();


            PowerShell psinstance = PowerShell.Create();
            psinstance.Streams.Verbose.DataAdded += jackInformationEventHandler;
            psinstance.Streams.Progress.DataAdded += jackProgressEventHandler;
            psinstance.Streams.Debug.DataAdded += jackDebugEventHandler;
            psinstance.Streams.Error.DataAdded += jackErrorEventHandler;

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
                Console.WriteLine(String.Join(" ", collector));
                return collector;
            }
        }

        private void TaskHistoryBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (taskHistoryBox.SelectedItem != null)
            {
                Tasks.Task task = (Tasks.Task)taskHistoryBox.SelectedItem;
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

        #endregion




        #region scriptFunctions
        public void ScriptsAppendOutputText(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Action action = delegate ()
                {
                    scriptOutputBox.AppendText(text);
                };
                Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
            else
            {
                scriptOutputBox.AppendText(text);
            }
        }

        void scriptInformationEventHandler(object sender, DataAddedEventArgs e)
        {
            VerboseRecord newRecord = ((PSDataCollection<VerboseRecord>)sender)[e.Index];
            ScriptsAppendOutputText(newRecord.ToString() + "\n");
            Console.WriteLine(newRecord.ToString());
        }

        void scriptProgressEventHandler(object sender, DataAddedEventArgs e)
        {
            ProgressRecord newRecord = ((PSDataCollection<ProgressRecord>)sender)[e.Index];
            AppendOutputText(newRecord.Activity.ToString() + "\n");
            Console.WriteLine(newRecord.Activity.ToString());
        }

        void scriptDebugEventHandler(object sender, DataAddedEventArgs e)
        {
            DebugRecord newRecord = ((PSDataCollection<DebugRecord>)sender)[e.Index];
            ScriptsAppendOutputText("Debug: " + newRecord.ToString() + "\n");
            Console.WriteLine(newRecord.ToString());
        }

        void scriptErrorEventHandler(object sender, DataAddedEventArgs e)
        {
            ErrorRecord newRecord = ((PSDataCollection<ErrorRecord>)sender)[e.Index];
            ScriptsAppendOutputText("ERROR: " + newRecord.ToString() + "\n");

            Console.WriteLine(newRecord.ToString());
        }

        private void Output_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<PSObject> data = (PSDataCollection<PSObject>)sender;
            
            string entry = data[e.Index].ToString();

            ScriptsAppendOutputText(entry + "\n");

            Console.WriteLine(entry.ToString());
        }
        #endregion

        #region scripts
        private void runScriptButton_Click(object sender, RoutedEventArgs e)
        {
            Script script;
            string scriptName = scriptBox.Text;
            switch (scriptName)
            {
                case "REX":
                    script = new REXScript(inputFolderBox.Text, outputFolderBox.Text, tagBox.Text, versionsPathBox.Text);
                    break;
                default:
                    return;
            }

            PowerShell psinstance = script.GetPSScript();
            
            psinstance.Streams.Verbose.DataAdded += scriptInformationEventHandler;
            psinstance.Streams.Progress.DataAdded += scriptProgressEventHandler;
            psinstance.Streams.Debug.DataAdded += scriptDebugEventHandler;
            psinstance.Streams.Error.DataAdded += scriptErrorEventHandler;
            PSDataCollection<PSObject> output = new PSDataCollection<PSObject>();
            output.DataAdded += Output_DataAdded;
            var results = psinstance.BeginInvoke<PSObject, PSObject>(null, output);
            /*foreach (PSObject result in results)
            {
                ScriptsAppendOutputText(result.ToString());
                Console.WriteLine(result.ToString());
            }*/
        }

        #endregion
    }
}

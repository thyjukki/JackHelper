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
        
        public MainWindow()
        {
            InitializeComponent();
            outputBox.Clear();
            outputBox.IsReadOnly = true;
        }

        public void AppendOutputText(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(
                        () => outputBox.AppendText(text), DispatcherPriority.Normal);
            }
            else
            {
                outputBox.AppendText(text);
            }
        }

        void myInformationEventHandler(object sender, DataAddedEventArgs e)
        {
            InformationRecord newRecord = ((PSDataCollection<InformationRecord>)sender)[e.Index];
            Console.WriteLine(newRecord.ToString());
            AppendOutputText(newRecord.ToString() + "\n");
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
                case "Exec":
                    task = new ExecTask(commandTextBox.Text ,"TEST-A01"/*computersTextBox.Text*/, parseOSList);
                    break;
                default:
                    return;
            }

            List<Tuple<string, object>> parameters = task.GetJackParameters();


            PowerShell psinstance = PowerShell.Create();
            psinstance.Streams.Information.DataAdded += myInformationEventHandler;
            psinstance.Streams.Error.DataAdded += myErrorEventHandler;
            psinstance.AddCommand(SCRIPT_PATH);
            foreach (var item in parameters)
            {
                psinstance.AddParameter(item.Item1, item.Item2);
            }
            var results = psinstance.BeginInvoke();



            //Console.WriteLine(SCRIPT_PATH + " " + parameters);
            //Process.Start("powershell", jackPath + " " + task.GetJackParameters());

            //powerShell.AddCommand("Set-ExecutionPolicy").AddArgument("Unrestricted").AddArgument("Process");
            //powerShell.AddScript(jackPath + " " + task.GetJackParameters());

            // create Powershell runspace 
            /*Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it 
            runspace.Open();

            RunspaceInvoke runSpaceInvoker = new RunspaceInvoke(runspace);
            runSpaceInvoker.Invoke("Set-ExecutionPolicy Unrestricted");

            // create a pipeline and feed it the script text 
            Pipeline pipeline = runspace.CreatePipeline();


            Command command = new Command(SCRIPT_PATH);

            foreach (var item in parameters)
            {
                command.Parameters.Add(item.Item1, item.Item2);
            }
            pipeline.Commands.Add(command);
            ///pipeline.Commands.AddScript(jackPath + " " + task.GetJackParameters());

            // add an extra command to transform the script output objects into nicely formatted strings 
            // remove this line to get the actual objects that the script returns. For example, the script 
            // "Get-Process" returns a collection of System.Diagnostics.Process instances. 
            pipeline.Commands.Add("Out-String");

            // execute the script 
            Collection<PSObject> results = pipeline.Invoke();

            // close the runspace 
            runspace.Close();

            // convert the script result into a single string 
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            // return the results of the script that has 
            // now been converted to text 
            Console.WriteLine(stringBuilder.ToString());*/
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
                            case "Windows 10":
                                collector.Add("10");
                                break;
                            case "Windows 8.1":
                                collector.Add("8.1");
                                break;
                            case "Windows 8":
                                collector.Add("8");
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
            ComputerSelect window = new ComputerSelect();
            window.Show();
        }
    }
}

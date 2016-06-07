using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JackHelper
{
    public static class TextBoxApppendBehaviors
    {
        #region AppendText Attached Property
        public static readonly DependencyProperty AppendTextProperty =
            DependencyProperty.RegisterAttached(
                "AppendText",
                typeof(string),
                typeof(TextBoxApppendBehaviors),
                new UIPropertyMetadata(null, OnAppendTextChanged));

        public static string GetAppendText(TextBox textBox)
        {
            return (string)textBox.GetValue(AppendTextProperty);
        }

        public static void SetAppendText(
            TextBox textBox,
            string value)
        {
            textBox.SetValue(AppendTextProperty, value);
        }

        private static void OnAppendTextChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
            {
                return;
            }

            string toAppend = args.NewValue.ToString();

            if (toAppend == "")
            {
                return;
            }

            TextBox textBox = d as TextBox;
            textBox?.AppendText(toAppend);
            textBox?.ScrollToEnd();
        }
        #endregion
    }

    public static class TextBoxClearBehavior
    {
        public static readonly DependencyProperty TextBoxClearProperty =
            DependencyProperty.RegisterAttached(
                "TextBoxClear",
                typeof(bool),
                typeof(TextBoxClearBehavior),
                new UIPropertyMetadata(false, OnTextBoxClearPropertyChanged));

        public static bool GetTextBoxClear(DependencyObject obj)
        {
            return (bool)obj.GetValue(TextBoxClearProperty);
        }

        public static void SetTextBoxClear(DependencyObject obj, bool value)
        {
            obj.SetValue(TextBoxClearProperty, value);
        }

        private static void OnTextBoxClearPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue == false)
            {
                return;
            }

            var textBox = (TextBox)d;
            textBox?.Clear();
        }
    }

    public interface ILogBoxViewModel
    {
        void CmdAppend(string toAppend);
        void CmdClear();

        bool AttachedPropertyClear { get; set; }

        string AttachedPropertyAppend { get; set; }
    }

    /*[Export(typeof(ILogBoxViewModel))]
    public class LogBoxViewModel : ILogBoxViewModel, INotifyPropertyChanged
    {
        private readonly ILog _log = LogManager.GetLogger<LogBoxViewModel>();

        private bool _attachedPropertyClear;
        private string _attachedPropertyAppend;

        public void CmdAppend(string toAppend)
        {
            string toLog = $"{DateTime.Now:HH:mm:ss} - {toAppend}\n";

            // Attached properties only fire on a change. This means it will still work if we publish the same message twice.
            AttachedPropertyAppend = "";
            AttachedPropertyAppend = toLog;

            _log.Info($"Appended to log box: {toAppend}.");
        }

        public void CmdClear()
        {
            AttachedPropertyClear = false;
            AttachedPropertyClear = true;

            _log.Info($"Cleared the GUI log box.");
        }

        public bool AttachedPropertyClear
        {
            get { return _attachedPropertyClear; }
            set { _attachedPropertyClear = value; OnPropertyChanged(); }
        }

        public string AttachedPropertyAppend
        {
            get { return _attachedPropertyAppend; }
            set { _attachedPropertyAppend = value; OnPropertyChanged(); }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }*/
}

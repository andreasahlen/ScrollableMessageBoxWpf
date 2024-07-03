using ScrollableMessageBoxLib.Converter;
using ScrollableMessageBoxLib.Core;
using ScrollableMessageBoxLib.Enums;
using ScrollableMessageBoxLib.I18N;
using ScrollableMessageBoxLib.Properties;
using ScrollableMessageBoxLib.Utils;
using ScrollableMessageBoxLib.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScrollableMessageBoxLib.Viewmodels
{
    public sealed class ScrollableMessageBoxViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private Window _Owner = null;

        private Dictionary<MessageBoxResultEx, char> _HotKeyMapping = new Dictionary<MessageBoxResultEx, char>();

        private MessageBoxButtonEx _Buttons = MessageBoxButtonEx.OK;

        public MessageBoxButtonEx Buttons => this._Buttons;

        private MessageBoxImageEx _Icon = MessageBoxImageEx.Information;

        private MessageBoxResultEx _DialogDefaultResult = MessageBoxResultEx.None;

        public MessageBoxResultEx DialogDefaultResult => this._DialogDefaultResult;

        private MessageBoxResultEx _DialogResult = MessageBoxResultEx.None;

        public MessageBoxResultEx DialogResult
        {
            get => this._DialogResult;
            set
            {
                if (this._DialogResult != value)
                {
                    this._DialogResult = value;
                    this.RaisePropertyChanged(nameof(DialogResult));
                }
            }
        }

        private CultureInfo _Locales = CultureInfo.CurrentUICulture;

        public CultureInfo Locales => this._Locales;

        public Window Owner => this._Owner;

        public string Title { get; }

        public string MessageText { get; }

        private ImageSource _MessageIcon = null;

        private string _Text = null;

        public ImageSource MessageIcon
        {
            get => this._MessageIcon;
            set
            {
                if (this._MessageIcon != value)
                {
                    this._MessageIcon = value;
                    this.RaisePropertyChanged(nameof(MessageIcon));
                }
            }
        }

        public Visibility OkButtonVisibility => this._OkButtonVisibility;
        public Visibility CancelButtonVisibility => _CancelButtonVisibility;
        public Visibility YesButtonVisibility => _YesButtonVisibility;
        public Visibility NoButtonVisibility => _NoButtonVisibility;
        public Visibility AbortButtonVisibility => _AbortButtonVisibility;
        public Visibility RetryButtonVisibility => _RetryButtonVisibility;
        public Visibility IgnoreButtonVisibility => _IgnoreButtonVisibility;

        #region commands

        // OnClickCommand

        private ICommand _OnClickCommand = null;
        
        private Visibility _OkButtonVisibility = Visibility.Collapsed;
        
        private Visibility _CancelButtonVisibility = Visibility.Collapsed;
        
        private Visibility _YesButtonVisibility = Visibility.Collapsed;
        
        private Visibility _NoButtonVisibility = Visibility.Collapsed;
        
        private Visibility _AbortButtonVisibility = Visibility.Collapsed;
        
        private Visibility _RetryButtonVisibility = Visibility.Collapsed;
        
        private Visibility _IgnoreButtonVisibility = Visibility.Collapsed;

        private string _OkButtonContent;
        
        private string _CancelButtonContent;
        
        private string _YesButtonContent;
        
        private string _NoButtonContent;
        
        private string _AbortButtonContent;
        
        private string _RetryButtonContent;
        
        private string _IgnoreButtonContent;

        public string OkButtonContent => this._OkButtonContent;
        public string CancelButtonContent => this._CancelButtonContent;
        public string YesButtonContent => this._YesButtonContent;
        public string NoButtonContent => this._NoButtonContent;
        public string AbortButtonContent => this._AbortButtonContent;
        public string RetryButtonContent => this._RetryButtonContent;
        public string IgnoreButtonContent => this._IgnoreButtonContent;


        public ICommand OnClickCommand
        {
            get => this._OnClickCommand;
        }

        #endregion commands

        public ScrollableMessageBoxViewModel(Window owner, string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon, CultureInfo culture = null)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = owner;
            this._Buttons = button;
            this._Icon = icon;
            this.SetCulture(culture);
            this.SetIcon();
            this.BindCommands(true);
            this.SetButtons();
        }

        public ScrollableMessageBoxViewModel(string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon, CultureInfo culture = null)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = null;
            this._Buttons = button;
            this._Icon = icon;
            this.SetCulture(culture);
            this.SetIcon();
            this.BindCommands(true);
            this.SetButtons();
        }

        public ScrollableMessageBoxViewModel(Window owner, string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon, MessageBoxResultEx defaultResult, CultureInfo culture = null)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = Application.Current.MainWindow;
            this._Buttons = button;
            this._Icon = icon;
            this._DialogDefaultResult = defaultResult;
            this.SetCulture(culture);
            this.SetIcon();
            this.BindCommands(true);
            this.SetButtons();
        }

        private void BindCommands(bool attach)
        {
            if (attach)
            {
                this._OnClickCommand = new CommandHandler(ClickedEventHandler, CanClick);
            }
        }

        private bool CanClick()
        {
            return true;
        }

        private void ClickedEventHandler(object obj)
        {
            if (obj != null)
            {
                if (obj is MessageBoxResultEx)
                {
                    this.DialogResult = (MessageBoxResultEx)obj;
                }
                else
                {
                    this.DialogResult = MessageBoxResultEx.None;
                }
            }
            
        }

        private void SetCulture(CultureInfo culture)
        {
            if (culture != null)
            {
                this._Locales = culture;
            }
        }

        public MessageBoxResultEx ShowDialog()
        {
            this._DialogDefaultResult = MessageBoxResultEx.None;
            using (ScrollableMessageBoxView msgBox = new ScrollableMessageBoxView(this))
            {
                return new ScrollableMessageBoxView(this).ShowDialog();
            }
        }

        private void SetIcon()
        {
            this.MessageIcon = this.ToImageSource(IconFromSystemIcons());
        }

        private Icon IconFromSystemIcons()
        {
            switch (this._Icon)
            {
                case MessageBoxImageEx.None:
                    return SystemIcons.Information;
                    
                case MessageBoxImageEx.Hand:
                    return SystemIcons.Hand;
                case MessageBoxImageEx.Question:
                    return SystemIcons.Question;
                case MessageBoxImageEx.Exclamation:
                    return SystemIcons.Exclamation;
                case MessageBoxImageEx.Asterisk:
                    return SystemIcons.Asterisk;
                case MessageBoxImageEx.Stop:
                    return SystemIcons.Hand;
                case MessageBoxImageEx.Error:
                    return SystemIcons.Error;
                case MessageBoxImageEx.Warning:
                    return SystemIcons.Warning;
                case MessageBoxImageEx.Information:
                    return SystemIcons.Hand;
                default:
                    return SystemIcons.Information;
            }
        }

        public ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        public void Dispose()
        {
            this.BindCommands(false);
        }

        public void ScrollableMessageBoxView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == MessageBoxResultEx.None)
            {
                if (this._Buttons != MessageBoxButtonEx.YesNo)
                {
                    this.CancelEvent();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public void ScrollableMessageBoxView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (this.OkButtonVisibility == Visibility.Visible)
                {
                    this.DialogResult = this._DialogDefaultResult != MessageBoxResultEx.None ? this._DialogDefaultResult : MessageBoxResultEx.OK;
                    this.CloseDialog(sender);
                }

                //if (this.YesButton.Visibility == Visibility.Visible)
                //{
                //    this._DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.Yes;
                //}

                if (this.RetryButtonVisibility == Visibility.Visible)
                {
                    this.DialogResult = this._DialogDefaultResult != MessageBoxResultEx.None ? this._DialogDefaultResult : MessageBoxResultEx.Retry;
                    this.CloseDialog(sender);
                }

                if (this._Buttons == MessageBoxButtonEx.YesNoCancel)
                {
                    this.DialogResult = this._DialogDefaultResult != MessageBoxResultEx.None ? this._DialogDefaultResult : MessageBoxResultEx.Yes;
                    this.CloseDialog(sender);
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.CancelEvent();
                if (this._Buttons != MessageBoxButtonEx.YesNo)
                {
                    this.CloseDialog(sender);
                    e.Handled = true; // IMPORTANT: sign event as handled, do not pass to calling window
                }
            }
            else
            {
                this.ProcessHotKey(e.Key.ToString());
                this.CloseDialog(sender);
            }
        }

        private void CancelEvent()
        {
            if (this._Buttons == MessageBoxButtonEx.OK && this.OkButtonVisibility == Visibility.Visible)
            {
                this.DialogResult = MessageBoxResultEx.OK;
            }
            else if (this.CancelButtonVisibility == Visibility.Visible)
            {
                this.DialogResult = this._DialogDefaultResult != MessageBoxResultEx.None ? this._DialogDefaultResult : MessageBoxResultEx.Cancel;
            }
            else if (this.AbortButtonVisibility == Visibility.Visible)
            {
                this.DialogResult = this._DialogDefaultResult != MessageBoxResultEx.None ? this._DialogDefaultResult : MessageBoxResultEx.Abort;
            }
            //else if (NoButton.Visibility == Visibility.Visible)
            //{
            //    this._DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.No;
            //}
            else
            {
                if (this._Buttons != MessageBoxButtonEx.YesNo)
                {
                    this.DialogResult = this._DialogDefaultResult != MessageBoxResultEx.None ? this._DialogDefaultResult : MessageBoxResultEx.Cancel;
                }
            }
        }

        private char GetChar(string value)
        {
            return value[0];
        }

        private void ProcessHotKey(string value)
        {
            if (value.Length == 1)
            {
                char hotkey = this.GetChar(value);
                if (this._HotKeyMapping.Any(v => v.Value == hotkey))
                {
                    KeyValuePair<MessageBoxResultEx, char> mapping = this._HotKeyMapping.FirstOrDefault(v => v.Value == hotkey);

                    if (mapping.Key == MessageBoxResultEx.OK && OkButtonVisibility == Visibility.Visible)
                    {
                        this.DialogResult = MessageBoxResultEx.OK;
                        return;
                    }
                    else if (mapping.Key == MessageBoxResultEx.Cancel && CancelButtonVisibility == Visibility.Visible)
                    {
                        this.DialogResult = MessageBoxResultEx.Cancel;
                        return;
                    }
                    else if (mapping.Key == MessageBoxResultEx.Yes && YesButtonVisibility == Visibility.Visible)
                    {
                        this.DialogResult = MessageBoxResultEx.Yes;
                        return;
                    }
                    else if (mapping.Key == MessageBoxResultEx.No && NoButtonVisibility == Visibility.Visible)
                    {
                        this.DialogResult = MessageBoxResultEx.No;
                        return;
                    }
                    else if (mapping.Key == MessageBoxResultEx.Abort && AbortButtonVisibility == Visibility.Visible)
                    {
                        this.DialogResult = MessageBoxResultEx.Abort;
                        return;
                    }
                    else if (mapping.Key == MessageBoxResultEx.Retry && RetryButtonVisibility == Visibility.Visible)
                    {
                        this.DialogResult = MessageBoxResultEx.Retry;
                        return;
                    }
                    else if (mapping.Key == MessageBoxResultEx.Ignore && IgnoreButtonVisibility == Visibility.Visible)
                    {
                        this.DialogResult = MessageBoxResultEx.Ignore;
                        return;
                    }
                }
            }
        }

        public void SetButtons()
        {
            this.SetButtonLocales();
            this.SetButtonVisibility();

        }

        public void SetButtonVisibility()
        {
            switch (this._Buttons)
            {
                case MessageBoxButtonEx.OK:
                    this._OkButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.OK, this._OkButtonContent);
                    break;
                case MessageBoxButtonEx.OKCancel:
                    this._OkButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.OK, this._OkButtonContent);
                    this._CancelButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.Cancel, this._CancelButtonContent);
                    break;
                case MessageBoxButtonEx.YesNo:
                    this._YesButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.Yes, this._YesButtonContent);
                    this._NoButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.No, this._NoButtonContent);
                    break;
                case MessageBoxButtonEx.YesNoCancel:
                    this._YesButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.Yes, this._YesButtonContent);
                    this._NoButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.No, this._NoButtonContent);
                    this._CancelButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.Cancel, this._CancelButtonContent);
                    break;
                case MessageBoxButtonEx.AbortRetryIgnore:
                    this._AbortButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.Abort, this._AbortButtonContent);
                    this._RetryButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.Retry, this._RetryButtonContent);
                    this._IgnoreButtonVisibility = Visibility.Visible;
                    this.SetButtonHotKyes(MessageBoxResultEx.Ignore, this._IgnoreButtonContent);
                    break;
                default:
                    break;
            }
        }

        public void SetButtonHotKyes(MessageBoxResultEx button, string content)
        {
            char hotKey = HotKeyHelper.GetHotKeyFromString(content.ToString());
            this._HotKeyMapping.Add(button, hotKey);
        }

        private void CloseDialog(object sender)
        {
            ((Window)sender).Close();
        }

        private void SetButtonLocales()
        {
            this.OverrideCultureInfo();

            this._OkButtonContent = Properties.Resources.ButtonOKText;
            this._CancelButtonContent = Properties.Resources.ButtonCancelText;
            this._YesButtonContent = Properties.Resources.ButtonYesText;
            this._NoButtonContent = Properties.Resources.ButtonNoText;
            this._RetryButtonContent = Properties.Resources.ButtonRetryText;
            this._AbortButtonContent = Properties.Resources.ButtonAbortText;
            this._IgnoreButtonContent = Properties.Resources.ButtonIgnoreText;
        }

        private void OverrideCultureInfo()
        {
            List<string> avail = new CultureInfoEnumerator()?.GetAvailableLanguages();
            if (avail.Any(v => v == this._Locales.IetfLanguageTag))
            {
                this._Locales = new CultureInfo(avail.FirstOrDefault(v => v == this._Locales.IetfLanguageTag));
            }
            else
            {
                // except "en-US" (default resources.resx culture)
                if (this._Locales.IetfLanguageTag == "en-US")
                {
                    this._Locales = new CultureInfo("en-US"); // pass this
                }
                else
                {
                    this._Locales = new CultureInfo(Settings.Default.FallbackIetfLanguageTag);
                }
            }

            CultureInfo.CurrentUICulture = this._Locales;
        }

        public void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, false);
            }
        }

        public void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, true);
            }
        }

        public void SetBold(Button button, bool bold)
        {
            button.FontWeight = !bold ? FontWeights.Normal : FontWeights.Bold;
        }

        public void OkButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, false);
            }
        }

        public void OkButton_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, true);
            }
        }
    }
}

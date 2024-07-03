using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScrollableMessageBoxLib.Core;
using ScrollableMessageBoxLib.Enums;
using ScrollableMessageBoxLib.I18N;
using ScrollableMessageBoxLib.Properties;
using ScrollableMessageBoxLib.Utils;
using ScrollableMessageBoxLib.Viewmodels;

namespace ScrollableMessageBoxLib.Views
{
    /// <summary>
    /// Interaction logic for ScrollableMeesageBoxView.xaml
    /// </summary>
    public partial class ScrollableMessageBoxView : Window, IDisposable
    {
        private MessageBoxResultEx _DefaultDialogResult = MessageBoxResultEx.None;

        private MessageBoxButtonEx _Buttons = MessageBoxButtonEx.OK;

        private Dictionary<string, char> _HotKeyMapping = new Dictionary<string, char>();

        private CultureInfo _Locales;

        public ScrollableMessageBoxView(ScrollableMessageBoxViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            InitializeDialog();
            
        }

        public bool CanButtonCommandHandlerExecute { get => true; }

        public void Dispose()
        {
            this.EventHandlers(false);
        }

        private void InitializeDialog()
        {
            this.SetWindowProperties();
            this.SetTextBoxProperties();
            this.SetButtonLocales();
            
        }

        private void SetTextBoxProperties()
        {
            this.TextBoxMessageText.IsReadOnly = true;
            this.TextBoxMessageText.TextWrapping = TextWrapping.Wrap;
            this.TextBoxMessageText.Background = Brushes.Transparent;
        }

        private void SetWindowProperties()
        {
            ScreenResolution res = new ScreenResolution();

            this.Owner = ((ScrollableMessageBoxViewModel)this.DataContext).Owner;
            this.Title = ((ScrollableMessageBoxViewModel)this.DataContext).Title;

            this.ShowInTaskbar = false;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowStyle = WindowStyle.ToolWindow;
            this.SetDefaultDialogResult(((ScrollableMessageBoxViewModel)this.DataContext).DialogDefaultResult);

            this.SetButtonVisibility(((ScrollableMessageBoxViewModel)this.DataContext).Buttons);

            this.MinWidth = 300;
            this.MinHeight = 200;
            this.Top = res.BorderSize;
            this.Left = res.BorderSize;
            this.MaxHeight = res.Height;
            this.MaxWidth = res.Width;
            this.ResizeMode = ResizeMode.CanResize;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void SetButtonLocales()
        {
            this.OverrideCultureInfo();

            this.OkButton.Content = Properties.Resources.ButtonOKText;
            this.CancelButton.Content = Properties.Resources.ButtonCancelText;
            this.YesButton.Content = Properties.Resources.ButtonYesText;
            this.NoButton.Content = Properties.Resources.ButtonNoText;
            this.RetryButton.Content = Properties.Resources.ButtonRetryText;
            this.AbortButton.Content = Properties.Resources.ButtonAbortText;
            this.IgnoreButton.Content = Properties.Resources.ButtonIgnoreText;
        }

        private void OverrideCultureInfo()
        {
            this._Locales = ((ScrollableMessageBoxViewModel)this.DataContext).Locales;
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

        public void SetButtonVisibility(MessageBoxButtonEx buttons)
        {
            this._Buttons = buttons;
            switch (this._Buttons)
            {
                case MessageBoxButtonEx.OK:
                    this.OkButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButtonEx.OKCancel:
                    this.OkButton.Visibility = Visibility.Visible;
                    this.CancelButton.Visibility = Visibility.Visible;

                    break;

                case MessageBoxButtonEx.YesNoCancel:
                    this.YesButton.Visibility = Visibility.Visible;
                    this.NoButton.Visibility = Visibility.Visible;
                    this.CancelButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButtonEx.YesNo:
                    this.YesButton.Visibility = Visibility.Visible;
                    this.NoButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButtonEx.AbortRetryIgnore:
                    this.AbortButton.Visibility = Visibility.Visible;
                    this.RetryButton.Visibility = Visibility.Visible;
                    this.IgnoreButton.Visibility = Visibility.Visible;
                    break;

                default:
                    break;
            }
        }

        private void EventHandlers(bool attach)
        {
            if (attach)
            {
                this.ButtonEvents(this.OkButton, true);
                this.ButtonEvents(this.CancelButton, true);
                this.ButtonEvents(this.YesButton, true);
                this.ButtonEvents(this.NoButton, true);
                this.ButtonEvents(this.AbortButton, true);
                this.ButtonEvents(this.RetryButton, true);
                this.ButtonEvents(this.IgnoreButton, true);

                this.Closing += ScrollableMessageBoxView_Closing;
                this.KeyDown += ScrollableMessageBoxView_KeyDown;
            }
            else
            {
                this.ButtonEvents(this.OkButton, false);
                this.ButtonEvents(this.CancelButton, false);
                this.ButtonEvents(this.YesButton, false);
                this.ButtonEvents(this.NoButton, false);
                this.ButtonEvents(this.AbortButton, false);
                this.ButtonEvents(this.RetryButton, false);
                this.ButtonEvents(this.IgnoreButton, false);

                this.Closing -= ScrollableMessageBoxView_Closing;
                this.KeyDown -= ScrollableMessageBoxView_KeyDown;
            }
        }

        private void SetDefaultDialogResult(MessageBoxResultEx dialogDefaultResult)
        {
            this._DefaultDialogResult = dialogDefaultResult;
        }

        private void ButtonEvents(Button button, bool attach)
        {
            if (attach)
            {
                this.SetButtonHotKyes(button);
                button.Click += ButtonClickedHandler;
                button.GotFocus += OkButton_GotFocus;
                button.LostFocus += OkButton_LostFocus;
                button.MouseEnter += Button_MouseEnter;
                button.MouseLeave += Button_MouseLeave;
            }
            else
            {
                button.Click -= ButtonClickedHandler;
                button.GotFocus -= OkButton_GotFocus;
                button.LostFocus -= OkButton_LostFocus;
                button.MouseEnter -= Button_MouseEnter;
                button.MouseLeave -= Button_MouseLeave;
            }
        }

        private void SetButtonHotKyes(Button button)
        {
            char hotKey = HotKeyHelper.GetHotKeyFromString(button.Content.ToString());
            this._HotKeyMapping.Add(button.Name, hotKey);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, false);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, true);
            }
        }

        private void SetBold(Button button, bool bold)
        {
            button.FontWeight = !bold ? FontWeights.Normal : FontWeights.Bold;
        }

        private void OkButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, false);
            }
        }

        private void OkButton_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                this.SetBold(sender as Button, true);
            }
        }

        private void ScrollableMessageBoxView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                if (this.OkButton.Visibility == Visibility.Visible)
                {
                    ((ScrollableMessageBoxViewModel)DataContext).DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.OK;
                    this.Deactivate();
                }

                //if (this.YesButton.Visibility == Visibility.Visible)
                //{
                //    this._DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.Yes;
                //}

                if (this.RetryButton.Visibility == Visibility.Visible)
                {
                    ((ScrollableMessageBoxViewModel)DataContext).DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.Retry;
                    this.Deactivate();
                }

                if (this._Buttons == MessageBoxButtonEx.YesNoCancel)
                {
                    ((ScrollableMessageBoxViewModel)DataContext).DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.Yes;
                    this.Deactivate();
                }
                
            }
            else if (e.Key == Key.Escape)
            {
                this.CancelEvent();
                if (this._Buttons != MessageBoxButtonEx.YesNo)
                {
                    this.Deactivate();
                    e.Handled = true; // IMPORTANT: sign event as handled, do not pass to calling window
                }
            }
            else
            {
                this.ProcessHotKey(e.Key.ToString());
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
                    KeyValuePair<string, char> mapping = this._HotKeyMapping.FirstOrDefault(v => v.Value == hotkey);

                    if (mapping.Key == nameof(OkButton) && OkButton.Visibility == Visibility.Visible)
                    {
                        ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.OK;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(CancelButton) && CancelButton.Visibility == Visibility.Visible)
                    {
                        ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Cancel;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(YesButton) && YesButton.Visibility == Visibility.Visible)
                    {
                        ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Yes;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(NoButton) && NoButton.Visibility == Visibility.Visible)
                    {
                        ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.No;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(AbortButton) && AbortButton.Visibility == Visibility.Visible)
                    {
                        ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Abort;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(RetryButton) && RetryButton.Visibility == Visibility.Visible)
                    {
                        ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Retry;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(IgnoreButton) && IgnoreButton.Visibility == Visibility.Visible)
                    {
                        ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Ignore;
                        this.Deactivate();
                    }
                }
            }
        }

        private void Deactivate()
        {
            this.Focusable = false;
            this.Close();
        }

        public MessageBoxResultEx ShowDialog()
        {
            this.EventHandlers(true);
            base.ShowDialog();
            return ((ScrollableMessageBoxViewModel)DataContext).DialogResult;
        }

        private void ScrollableMessageBoxView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this._Buttons == MessageBoxButtonEx.YesNo)
            {
                if (((ScrollableMessageBoxViewModel)DataContext).DialogResult != MessageBoxResultEx.No)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                if (((ScrollableMessageBoxViewModel)DataContext).DialogResult == MessageBoxResultEx.None)
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
        }

        private void CancelEvent()
        {
            if (this._Buttons == MessageBoxButtonEx.OK && this.OkButton.Visibility == Visibility.Visible)
            {
                ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.OK;
            }
            else if (this.CancelButton.Visibility == Visibility.Visible)
            {
                ((ScrollableMessageBoxViewModel)DataContext).DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.Cancel;
            }
            else if (this.AbortButton.Visibility == Visibility.Visible)
            {
                ((ScrollableMessageBoxViewModel)DataContext).DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.Abort;
            }
            //else if (NoButton.Visibility == Visibility.Visible)
            //{
            //    this._DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.No;
            //}
            else
            {
                if (this._Buttons != MessageBoxButtonEx.YesNo)
                {
                    ((ScrollableMessageBoxViewModel)DataContext).DialogResult = this._DefaultDialogResult != MessageBoxResultEx.None ? this._DefaultDialogResult : MessageBoxResultEx.Cancel;
                }
                
            }
        }

        private void ButtonClickedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                Button btn = e.OriginalSource as Button;
                if (btn != null)
                {
                    switch (btn.Name)
                    {
                        case nameof(OkButton):
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.OK;
                            break;

                        case nameof(CancelButton):
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Cancel;
                            break;

                        case nameof(YesButton):
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Yes;
                            break;

                        case nameof(NoButton):
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.No;
                            break;

                        case nameof(RetryButton):
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Retry;
                            break;

                        case nameof(AbortButton):
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Abort;
                            break;

                        case nameof(IgnoreButton):
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.Ignore;
                            break;

                        default:
                            ((ScrollableMessageBoxViewModel)DataContext).DialogResult = MessageBoxResultEx.None;
                            break;
                    }
                }
                this.Deactivate();
            }
        }
    }
}
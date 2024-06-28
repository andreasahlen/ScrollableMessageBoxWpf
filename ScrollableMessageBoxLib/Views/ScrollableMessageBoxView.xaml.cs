using ScrollableMessageBoxLib.Core;
using ScrollableMessageBoxLib.Enums;
using ScrollableMessageBoxLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScrollableMessageBoxLib.Views
{
    /// <summary>
    /// Interaction logic for ScrollableMeesageBoxView.xaml
    /// </summary>
    public partial class ScrollableMessageBoxView : Window, IDisposable
    {
        private MessageBoxResultEx _DialogResult = MessageBoxResultEx.None;

        private MessageBoxButtonEx _Buttons = MessageBoxButtonEx.OK;

        private Dictionary<string, char> _HotKeyMapping = new Dictionary<string, char>();

        public ScrollableMessageBoxView()
        {
            InitializeComponent();
            InitializeDialog();
        }

        public MessageBoxResultEx DialogResult
        {
            get => this._DialogResult;
        }
        public bool CanButtonCommandHandlerExecute { get => true; }

        public void Dispose()
        {
            this.EventHandlers(false);
        }

        private void InitializeDialog()
        {
            this.SetWindow();
            this.TextBoxMessageText.IsReadOnly = true;
        }

        private void SetWindow()
        {
            this.MinWidth = 400;
            this.MinHeight = 100;
            this.MaxHeight = 600;
            this.MaxWidth = 600;
            this.ResizeMode = ResizeMode.NoResize;
            this.SizeToContent = SizeToContent.WidthAndHeight;
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
                    this._DialogResult = MessageBoxResultEx.OK;
                }

                if (this.YesButton.Visibility == Visibility.Visible)
                {
                    this._DialogResult = MessageBoxResultEx.Yes;
                }

                if (this.RetryButton.Visibility == Visibility.Visible)
                {
                    this._DialogResult = MessageBoxResultEx.Retry;
                }
                this.Deactivate();
            }
            else if (e.Key == Key.Escape)
            {
                this.CancelEvent();
                this.Deactivate();
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
                        this._DialogResult = MessageBoxResultEx.OK;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(CancelButton) && CancelButton.Visibility == Visibility.Visible)
                    {
                        this._DialogResult = MessageBoxResultEx.Cancel;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(YesButton) && YesButton.Visibility == Visibility.Visible)
                    {
                        this._DialogResult = MessageBoxResultEx.Yes;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(NoButton) && NoButton.Visibility == Visibility.Visible)
                    {
                        this._DialogResult = MessageBoxResultEx.No;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(AbortButton) && AbortButton.Visibility == Visibility.Visible)
                    {
                        this._DialogResult = MessageBoxResultEx.Abort;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(RetryButton) && RetryButton.Visibility == Visibility.Visible)
                    {
                        this._DialogResult = MessageBoxResultEx.Retry;
                        this.Deactivate();
                    }
                    else if (mapping.Key == nameof(IgnoreButton) && IgnoreButton.Visibility == Visibility.Visible)
                    {
                        this._DialogResult = MessageBoxResultEx.Ignore;
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

        public void ShowDialog()
        {
            this.EventHandlers(true);
            base.ShowDialog();
        }

        private void ScrollableMessageBoxView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this._DialogResult == MessageBoxResultEx.None)
            {
                this.CancelEvent();
            }
        }

        private void CancelEvent()
        {
            if (this._Buttons == MessageBoxButtonEx.OK && this.OkButton.Visibility == Visibility.Visible)
            {
                this._DialogResult = MessageBoxResultEx.OK;
            }
            else if (this.CancelButton.Visibility == Visibility.Visible)
            {
                this._DialogResult = MessageBoxResultEx.Cancel;
            }
            else if (this.AbortButton.Visibility == Visibility.Visible)
            {
                this._DialogResult = MessageBoxResultEx.Abort;
            }
            else if (NoButton.Visibility == Visibility.Visible)
            {
                this._DialogResult = MessageBoxResultEx.No;
            }
            else
            {
                this._DialogResult = MessageBoxResultEx.Cancel;
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
                            this._DialogResult = MessageBoxResultEx.OK;
                            break;

                        case nameof(CancelButton):
                            this._DialogResult = MessageBoxResultEx.Cancel;
                            break;

                        case nameof(YesButton):
                            this._DialogResult = MessageBoxResultEx.Yes;
                            break;

                        case nameof(NoButton):
                            this._DialogResult = MessageBoxResultEx.No;
                            break;

                        case nameof(RetryButton):
                            this._DialogResult = MessageBoxResultEx.Retry;
                            break;

                        case nameof(AbortButton):
                            this._DialogResult = MessageBoxResultEx.Abort;
                            break;

                        case nameof(IgnoreButton):
                            this._DialogResult = MessageBoxResultEx.Ignore;
                            break;

                        default:
                            this._DialogResult = MessageBoxResultEx.None;
                            break;
                    }
                }
                this.Deactivate();
            }
        }
    }
}
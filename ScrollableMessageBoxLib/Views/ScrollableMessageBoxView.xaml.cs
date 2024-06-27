using ScrollableMessageBoxLib.Enums;
using System;
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

        public ScrollableMessageBoxView()
        {
            InitializeComponent();
            InitializeDialog();
        }

        public MessageBoxResultEx DialogResult
        {
            get => this._DialogResult;
        }

        public void Dispose()
        {
            this.EventHandlers(false);
        }

        private void InitializeDialog()
        {
            this.TextBoxMessageText.IsReadOnly = true;
            this.EventHandlers(true);
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
                this.OkButton.Click += ButtonClickedHandler;
                this.CancelButton.Click += ButtonClickedHandler;
                this.YesButton.Click += ButtonClickedHandler;
                this.NoButton.Click += ButtonClickedHandler;
                this.AbortButton.Click += ButtonClickedHandler;
                this.RetryButton.Click += ButtonClickedHandler;
                this.IgnoreButton.Click += ButtonClickedHandler;
                this.Closing += ScrollableMessageBoxView_Closing;
                this.KeyUp += ScrollableMessageBoxView_KeyUp;
            }
            else
            {
                this.OkButton.Click -= ButtonClickedHandler;
                this.CancelButton.Click -= ButtonClickedHandler;
                this.YesButton.Click -= ButtonClickedHandler;
                this.NoButton.Click -= ButtonClickedHandler;
                this.AbortButton.Click -= ButtonClickedHandler;
                this.RetryButton.Click -= ButtonClickedHandler;
                this.IgnoreButton.Click -= ButtonClickedHandler;
                this.Closing -= ScrollableMessageBoxView_Closing;
                this.KeyUp -= ScrollableMessageBoxView_KeyUp;
            }
        }

        private void ScrollableMessageBoxView_KeyUp(object sender, KeyEventArgs e)
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
                this.Hide();
            }
            else if (e.Key == Key.Escape)
            {
                this.CancelEvent();
                this.Hide();
            }
            else
            {
                this._DialogResult = MessageBoxResultEx.Cancel;
                this.Hide();
            }
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
                this.Hide();
            }
        }
    }
}
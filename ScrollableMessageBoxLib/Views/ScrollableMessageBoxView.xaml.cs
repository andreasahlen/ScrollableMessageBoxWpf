using ScrollableMessageBoxLib.Core;
using ScrollableMessageBoxLib.Enums;
using ScrollableMessageBoxLib.I18N;
using ScrollableMessageBoxLib.Properties;
using ScrollableMessageBoxLib.Utils;
using ScrollableMessageBoxLib.Viewmodels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScrollableMessageBoxLib.Views
{
    /// <summary>
    /// Interaction logic for ScrollableMeesageBoxView.xaml
    /// </summary>
    public partial class ScrollableMessageBoxView : Window, IDisposable
    {
        private MessageBoxResultEx _DefaultDialogResult = MessageBoxResultEx.None;

        private MessageBoxButtonEx _Buttons = MessageBoxButtonEx.OK;

        private bool _Resizable = true;

        // private Dictionary<string, char> _HotKeyMapping = new Dictionary<string, char>();

        // private CultureInfo _Locales;

        public ScrollableMessageBoxView(ScrollableMessageBoxViewModel vm, bool resizable = true)
        {
            InitializeComponent();
            this.DataContext = vm;
            this._Resizable = resizable;
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
            // this.SetButtonLocales();
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
            
            this.WindowStyle = WindowStyle.ToolWindow;
            this.SetDefaultDialogResult(((ScrollableMessageBoxViewModel)this.DataContext).DialogDefaultResult);

            this.SetButtonVisibility(((ScrollableMessageBoxViewModel)this.DataContext).Buttons);

            this.MinWidth = 300;
            this.MinHeight = 200;
            this.Top = res.BorderSize;
            this.Left = res.BorderSize;
            if (this._Resizable)
            {
                this.MaxHeight = res.Height;
                this.MaxWidth = res.Width;
                this.ResizeMode = ResizeMode.CanResize;
            }
            else
            {
                this.MaxHeight = 600;
                this.MaxWidth = 800;
                this.ResizeMode = ResizeMode.NoResize;
            }

            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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

                this.Closing += ((ScrollableMessageBoxViewModel)DataContext).ScrollableMessageBoxView_Closing;
                this.KeyDown += ((ScrollableMessageBoxViewModel)DataContext).ScrollableMessageBoxView_KeyDown;
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

                this.Closing -= ((ScrollableMessageBoxViewModel)DataContext).ScrollableMessageBoxView_Closing;
                this.KeyDown -= ((ScrollableMessageBoxViewModel)DataContext).ScrollableMessageBoxView_KeyDown;
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
                button.Click += Button_Click;
                button.GotFocus += ((ScrollableMessageBoxViewModel)DataContext).OkButton_GotFocus;
                button.LostFocus += ((ScrollableMessageBoxViewModel)DataContext).OkButton_LostFocus;
                button.MouseEnter += ((ScrollableMessageBoxViewModel)DataContext).Button_MouseEnter;
                button.MouseLeave += ((ScrollableMessageBoxViewModel)DataContext).Button_MouseLeave;
            }
            else
            {
                button.GotFocus -= ((ScrollableMessageBoxViewModel)DataContext).OkButton_GotFocus;
                button.LostFocus -= ((ScrollableMessageBoxViewModel)DataContext).OkButton_LostFocus;
                button.MouseEnter -= ((ScrollableMessageBoxViewModel)DataContext).Button_MouseEnter;
                button.MouseLeave -= ((ScrollableMessageBoxViewModel)DataContext).Button_MouseLeave;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public MessageBoxResultEx ShowDialog()
        {
            this.EventHandlers(true);
            base.ShowDialog();
            //this.Close();
            return ((ScrollableMessageBoxViewModel)DataContext).DialogResult;
        }
    }
}
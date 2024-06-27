using ScrollableMessageBoxLib.Converter;
using ScrollableMessageBoxLib.Enums;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Media.Imaging;

namespace ScrollableMessageBoxLib.Viewmodels
{
    public sealed class ScrollableMessageBoxViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private ScrollableMessageBoxView _View = null;

        private Window _Owner = null;

        private MessageBoxButtonEx _Button = MessageBoxButtonEx.OK;

        private MessageBoxImageEx _Icon = MessageBoxImageEx.Information;

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

        private Dictionary<MessageBoxButtonTypeEx, string> _DefaultLocales = new Dictionary<MessageBoxButtonTypeEx, string>
        {
            { MessageBoxButtonTypeEx.Abort, "_Abort" },
            { MessageBoxButtonTypeEx.Cancel, "_Cancel" },
            { MessageBoxButtonTypeEx.Ignore, "_Ignore" },
            { MessageBoxButtonTypeEx.No, "_No" },
            { MessageBoxButtonTypeEx.OK, "_OK" },
            { MessageBoxButtonTypeEx.Retry, "_Retry" },
            { MessageBoxButtonTypeEx.Yes, "_Yes" }
        };
            
        public ScrollableMessageBoxViewModel(Window owner, string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon, Dictionary<MessageBoxButtonTypeEx, string> locales = null)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = owner;
            this._Button = button;
            this._Icon = icon;
            if (locales != null)
            {
                this._DefaultLocales = locales;
            }
        }

        public ScrollableMessageBoxViewModel(string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = null;
            this._Button = button;
            this._Icon = icon;
        }

        public MessageBoxResultEx ShowDialog()
        {
            CultureInfo uiCulture = CultureInfo.CurrentUICulture;
            this.InitDialog();
            this.SetButtonLocales();
            this._View.ShowDialog();
            return this._View.DialogResult;
        }

        private void InitDialog()
        {
            this._View = new ScrollableMessageBoxView();
            this._View.Owner = this._Owner;
            this._View.DataContext = this;
            this._View.ShowInTaskbar = false;
            this._View.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this._View.WindowStyle = WindowStyle.ToolWindow;
            this.MessageIcon = this.ToImageSource(IconFromSystemIcons());
            this._View.SetButtonVisibility(this._Button);
        }

        private void SetButtonLocales()
        {
            this._View.OkButton.Content = this._DefaultLocales.FirstOrDefault(v => v.Key == MessageBoxButtonTypeEx.OK).Value;
            this._View.CancelButton.Content = this._DefaultLocales.FirstOrDefault(v => v.Key == MessageBoxButtonTypeEx.Cancel).Value;
            this._View.YesButton.Content = this._DefaultLocales.FirstOrDefault(v => v.Key == MessageBoxButtonTypeEx.Yes).Value;
            this._View.NoButton.Content = this._DefaultLocales.FirstOrDefault(v => v.Key == MessageBoxButtonTypeEx.No).Value;
            this._View.RetryButton.Content = this._DefaultLocales.FirstOrDefault(v => v.Key == MessageBoxButtonTypeEx.Retry).Value;
            this._View.AbortButton.Content = this._DefaultLocales.FirstOrDefault(v => v.Key == MessageBoxButtonTypeEx.Abort).Value;
            this._View.IgnoreButton.Content = this._DefaultLocales.FirstOrDefault(v => v.Key == MessageBoxButtonTypeEx.Ignore).Value;
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
            this?._View?.Dispose();
        }
    }
}

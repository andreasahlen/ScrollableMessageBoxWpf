using ScrollableMessageBoxLib.Converter;
using ScrollableMessageBoxLib.Enums;
using ScrollableMessageBoxLib.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

        public ScrollableMessageBoxViewModel(Window owner, string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = owner;
            this._Button = button;
            this._Icon = icon;
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
            this._View = new ScrollableMessageBoxView();
            this._View.Owner = this._Owner;
            this._View.DataContext = this;
            this._View.ShowInTaskbar = false;
            this._View.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this._View.WindowStyle = WindowStyle.ToolWindow;
            this.MessageIcon = this.ToImageSource(IconFromSystemIcons());
            this._View.SetButtonVisibility(this._Button);
            this._View.ShowDialog();
            return this._View.DialogResult;
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

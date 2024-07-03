using ScrollableMessageBoxLib.Converter;
using ScrollableMessageBoxLib.Enums;
using ScrollableMessageBoxLib.I18N;
using ScrollableMessageBoxLib.Properties;
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

namespace ScrollableMessageBoxLib.Viewmodels
{
    public sealed class ScrollableMessageBoxViewModel : ViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private Window _Owner = null;

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

        public ScrollableMessageBoxViewModel(Window owner, string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon, CultureInfo culture = null)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = owner;
            this._Buttons = button;
            this._Icon = icon;
            this.SetCulture(culture);
            this.SetIcon();
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
            // TODO
        }
    }
}

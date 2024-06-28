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
        private ScrollableMessageBoxView _View = null;

        private Window _Owner = null;

        private MessageBoxButtonEx _Button = MessageBoxButtonEx.OK;

        private MessageBoxImageEx _Icon = MessageBoxImageEx.Information;

        private CultureInfo _Locales = CultureInfo.CurrentUICulture;

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
            this._Button = button;
            this._Icon = icon;
            this.SetCulture(culture);
        }

        public ScrollableMessageBoxViewModel(string content, string title, MessageBoxButtonEx button, MessageBoxImageEx icon, CultureInfo culture = null)
        {
            this.Title = title;
            this.MessageText = content;
            this._Owner = null;
            this._Button = button;
            this._Icon = icon;
            this.SetCulture(culture);
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
            this._View.SetButtonVisibility(this._Button);
            this.MessageIcon = this.ToImageSource(this.IconFromSystemIcons());
            
        }

        private void SetButtonLocales()
        {
            this.OverrideCultureInfo();

            this._View.OkButton.Content = ScrollableMessageBoxLib.Properties.Resources.ButtonOKText;
            this._View.CancelButton.Content = ScrollableMessageBoxLib.Properties.Resources.ButtonCancelText;
            this._View.YesButton.Content = ScrollableMessageBoxLib.Properties.Resources.ButtonYesText;
            this._View.NoButton.Content = ScrollableMessageBoxLib.Properties.Resources.ButtonNoText;
            this._View.RetryButton.Content = ScrollableMessageBoxLib.Properties.Resources.ButtonRetryText;
            this._View.AbortButton.Content = ScrollableMessageBoxLib.Properties.Resources.ButtonAbortText;
            this._View.IgnoreButton.Content = ScrollableMessageBoxLib.Properties.Resources.ButtonIgnoreText;
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

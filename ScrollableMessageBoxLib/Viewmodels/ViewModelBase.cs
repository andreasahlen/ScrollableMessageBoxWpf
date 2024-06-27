using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScrollableMessageBoxLib.Viewmodels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Is raised if property changed
        /// </summary>
        /// <param name="propertyName">If no "propertyName" is given, it will be automatically filled by the [CallerMemberName] Attribute</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
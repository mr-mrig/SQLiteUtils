using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.ViewModel
{

    public class BaseViewModel : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;


        public void RaisePropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion


        #region Public Methods
        public bool SetProperty<T>(ref T prop, T value, [CallerMemberName]string propName = "")
        {
            // Check if property really changed
            if(!EqualityComparer<T>.Default.Equals(prop, value))
            {
                prop = value;
                RaisePropertyChanged(propName);
                return true;
            }

            return false;
        }
        #endregion
    }
}

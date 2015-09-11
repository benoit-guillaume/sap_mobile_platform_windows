using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UsageWindows.Contexts
{
    public class UsageContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string deviceInformation = string.Empty;
        public string DeviceInformation 
        { 
            get 
            { 
                return this.deviceInformation; 
            } 

            set 
            { 
                this.deviceInformation = value; 
                this.NotifyPropertyChanged(); 
            } 
        }
    }

    public class SharedContext
    {
        public static UsageContext Context { get; set; }
    }
}

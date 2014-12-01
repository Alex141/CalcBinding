using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExample
{
    public class ExampleViewModel:INotifyPropertyChanged
    {
        private decimal a;
        public decimal A
        {
            get { return a; }
            set
            {
                a = value;
                PropertyChanged(this, new PropertyChangedEventArgs("A"));
            }
        }

        private decimal b;
        public decimal B
        {
            get { return b; }
            set
            {
                b = value;
                PropertyChanged(this, new PropertyChangedEventArgs("B"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

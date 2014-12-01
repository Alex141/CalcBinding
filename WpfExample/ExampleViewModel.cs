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
        private int a = 10;
        public int A
        {
            get { return a; }
            set
            {
                a = value;
                PropertyChanged(this, new PropertyChangedEventArgs("A"));
            }
        }

        private int b = 4;
        public int B
        {
            get { return b; }
            set
            {
                b = value;
                PropertyChanged(this, new PropertyChangedEventArgs("B"));
            }
        }

        private int c;
        public int C
        {
            get { return c; }
            set
            {
                c = value;
                PropertyChanged(this, new PropertyChangedEventArgs("C"));
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }

        private bool isFull;
        public bool IsFull
        {
            get { return isFull; }
            set
            {
                isFull = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsFull"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

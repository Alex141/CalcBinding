using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExample
{
    public class ExampleViewModel:object, INotifyPropertyChanged
    {
        private double a = 10;
        public double A
        {
            get { return a; }
            set
            {
                a = value;

                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("A"))
                );
            }
        }

        private int b = 4;
        public int B
        {
            get { return b; }
            set
            {
                b = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("B"))
                );
            }
        }

        private int c;
        public int C
        {
            get { return c; }
            set
            {
                c = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("C"))
                );
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"))
                );
            }
        }

        private bool isFull;
        public bool IsFull
        {
            get { return isFull; }
            set
            {
                isFull = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("IsFull"))
                );
            }
        }

        private String name;
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("Name"))
                );
            }
        }

        private String surname;
        public String Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("Surname"))
                );
            }
        }

        private bool isMan;
        public bool IsMan
        {
            get { return isMan; }
            set
            {
                isMan = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("IsMan"))
                );
            }
        }

        private bool hasPrivileges;
        public bool HasPrivileges
        {
            get { return hasPrivileges; }
            set
            {
                hasPrivileges = value;
                new object().TraceTime(
                    () => PropertyChanged(this, new PropertyChangedEventArgs("HasPrivileges"))
                );
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public static class TimeExtension
    {
        public static void TraceTime(this object obj, Action proc)
        {
            var startTime = DateTime.Now;
            proc();
            var time = DateTime.Now - startTime;
            Trace.WriteLine(String.Format("timeSpan: {0}", time));
        }
    }
}

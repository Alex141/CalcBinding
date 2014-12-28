using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WpfExample
{
    /// <summary>
    /// Example view model
    /// </summary>
    public class ExampleViewModel : INotifyPropertyChanged
    {
        private double a = 10;
        public double A
        {
            get { return a; }
            set
            {
                a = value;

                new object().TraceTime(
                    () => RaisePropertyChanged(() => A)
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
                    () => RaisePropertyChanged(() => B)
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
                    () => RaisePropertyChanged(() => C)
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
                    () => RaisePropertyChanged(() => IsChecked)
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
                    () => RaisePropertyChanged(() => IsFull)
                );
            }
        }

        private String name = "Willy";
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => Name)
                );
            }
        }

        private String surname = "Williams";
        public String Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => Surname)
                );
            }
        }

        private bool isMan = true;
        public bool IsMan
        {
            get { return isMan; }
            set
            {
                isMan = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => IsMan)
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
                    () => RaisePropertyChanged(() => HasPrivileges)
                );
            }
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (PropertyChanged != null)
            {
                var propertyName = (propertyExpression.Body as MemberExpression).Member.Name;

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

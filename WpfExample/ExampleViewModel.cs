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
    public class BaseViewModel: INotifyPropertyChanged
    {
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

    public class DoubleNestedViewModel : BaseViewModel
    {
        private double a = 300;
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

        private int b = 340;
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
    }

    public class NestedViewModel : BaseViewModel
    {
        private double a = 100;
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

        private int b = 34;
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

        private int c = -4;
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

        private bool isChecked = false;
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

        private DoubleNestedViewModel doubleNestedViewModel = new DoubleNestedViewModel();
        public DoubleNestedViewModel DoubleNestedViewModel
        {
            get { return doubleNestedViewModel; }
            set
            {
                doubleNestedViewModel = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => DoubleNestedViewModel)
                );
            }
        }    
    }

    /// <summary>
    /// Example view model
    /// </summary>
    public class ExampleViewModel : BaseViewModel
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

        private NestedViewModel nestedViewModel = new NestedViewModel();
        public NestedViewModel NestedViewModel
        {
            get { return nestedViewModel; }
            set
            {
                nestedViewModel = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => NestedViewModel)
                );
            }
        }

        private int porksCount = 1;
        public int PorksCount
        {
            get { return porksCount; }
            set
            {
                porksCount = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => PorksCount)
                );
            }
        }

        private int pandus = 1;
        public int Pandus
        {
            get { return pandus; }
            set
            {
                pandus = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => Pandus)
                );
            }
        }

        private int fairlessly = 1;
        public int Fairlessly
        {
            get { return fairlessly; }
            set
            {
                fairlessly = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => Fairlessly)
                );
            }
        }

        private int fairless = 1;
        public int Fairless
        {
            get { return fairless; }
            set
            {
                fairless = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => Fairless)
                );
            }
        }

        private double readonlyA = 4;
        public double ReadonlyA
        {
            get { return readonlyA; }
        }
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

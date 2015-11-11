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

    public class AbstractViewModel:BaseViewModel
    {

    }

    public class ConcreteViewModel1:AbstractViewModel
    {
        private double a;
        public double A
        {
            get { return a; }
            set
            {
                a = value;
                RaisePropertyChanged(() => A);
            }
        }

        public ConcreteViewModel1(int a)
        {
            this.a = a;
        }
    }

    public class ConcreteViewModel2 : AbstractViewModel
    {
        private double a;
        public double A
        {
            get { return a; }
            set
            {
                a = value;
                RaisePropertyChanged(() => A);
            }
        }

        public ConcreteViewModel2(int a)
        {
            this.a = a;
        }
    }

    /// <summary>
    /// Example view model
    /// </summary>
    public class ExampleViewModel : BaseViewModel
    {
		private bool clickMethodInvoked;
		public bool ClickMethodInvoked
		{
			get { return clickMethodInvoked; }
			set
			{
				clickMethodInvoked = value;

				new object().TraceTime(
					() => RaisePropertyChanged(() => ClickMethodInvoked)
				);
			}
		}
		public void ClickMethod()
		{
			ClickMethodInvoked = !ClickMethodInvoked;
		}

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

        private double m = 10;
        public double M
        {
            get { return m; }
            set
            {
                m = value;

                new object().TraceTime(
                    () => RaisePropertyChanged(() => M)
                );
            }
        }

        // test for properties, containing numbers
        private double n1 = 10;
        public double N1
        {
            get { return n1; }
            set
            {
                n1 = value;

                new object().TraceTime(
                    () => RaisePropertyChanged(() => N1)
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

        private NestedViewModel nestedViewModelOther = new NestedViewModel();
        public NestedViewModel NestedViewModelOther
        {
            get { return nestedViewModelOther; }
            set
            {
                nestedViewModelOther = value;
                new object().TraceTime(
                    () => RaisePropertyChanged(() => NestedViewModelOther)
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

        private double? nullableA;
        public double? NullableA
        {
            get
            {
                return nullableA;
            }
            set
            {
                nullableA = value;
                RaisePropertyChanged(() => NullableA);
            }
        }

        private AbstractViewModel flickeringViewModel;
        public AbstractViewModel FlickeringViewModel
        {
            get { return flickeringViewModel; }
            set
            {
                flickeringViewModel = value;

                RaisePropertyChanged(() => FlickeringViewModel);
            }
        }

        public bool NameIsNull
        {
            get
            {
                return name == null;
            }
            set
            {
                Name = value ? null : "";

                RaisePropertyChanged(() => NameIsNull);
            }
        }
    }

    public static class TimeExtension
    {
        public static void TraceTime(this object obj, Action proc)
        {
            var timer = new Stopwatch();
            timer.Start();
            proc();
            Trace.WriteLine(String.Format("timeSpan: {0}", timer.ElapsedMilliseconds));
        }
    }
}

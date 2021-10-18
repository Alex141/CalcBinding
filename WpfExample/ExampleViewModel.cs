using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private int b = 15;
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

        private char symbol = 'S';
        public char Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                symbol = value;
                RaisePropertyChanged(() => Symbol);
            }
        }

        private Enum2 enumValue;
        public Enum2 EnumValue
        {
            get
            {
                return enumValue;
            }
            set
            {
                enumValue = value;
                RaisePropertyChanged(() => EnumValue);
            }
        }

        private Enum3 enum3Value;
        public Enum3 Enum3Value
        {
            get
            {
                return enum3Value;
            }
            set
            {
                enum3Value = value;
                RaisePropertyChanged(() => Enum3Value);
            }
        }

        private Visibility visibility;
        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                RaisePropertyChanged(() => Visibility);
            }
        }

        public bool IsEnumEqualsValue1
        {
            get
            {
                return EnumValue == Enum2.Value1;
            }
            set
            {
                if (value)
                {
                    EnumValue = Enum2.Value1;
                    RaisePropertyChanged(() => IsEnumEqualsValue1);
                }
            }
        }

        public bool IsEnumEqualsValue2
        {
            get
            {
                return EnumValue == Enum2.Value2;
            }
            set
            {
                if (value)
                {
                    EnumValue = Enum2.Value2;
                    RaisePropertyChanged(() => IsEnumEqualsValue2);
                }
            }
        }
    }

    public class StaticExampleClass
    {
        private static double staticA = 15;

        public static double StaticA
        {
            get
            {
                return staticA;
            }
            set
            {
                staticA = value;
                RaiseStaticPropertyChanged(() => StaticA);
            }
        }

        private static int staticB = -4;

        public static int StaticB
        {
            get
            {
                return staticB;
            }
            set
            {
                staticB = value;
                RaiseStaticPropertyChanged(() => StaticB);
            }
        }

        private static string staticString = "";

        public static string StaticString
        {
            get
            {
                return staticString;
            }
            set
            {
                staticString = value;
                RaiseStaticPropertyChanged(() => StaticString);
            }
        }


        private static Char staticChar = 'A';

        public static Char StaticChar
        {
            get
            {
                return staticChar;
            }
            set
            {
                staticChar = value;
                RaiseStaticPropertyChanged(() => StaticChar);
            }
        }

        private static String name = "A";

        public static String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaiseStaticPropertyChanged(() => Name);
            }
        }

        private static Enum2 enumValue;
        public static Enum2 EnumValue
        {
            get
            {
                return enumValue;
            }
            set
            {
                enumValue = value;
                RaiseStaticPropertyChanged(() => EnumValue);
            }
        }        

        private static Visibility visibility;
        public static Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                RaiseStaticPropertyChanged(() => Visibility);
            }
        }

        private static bool staticBool = false;

        public static bool StaticBool
        {
            get
            {
                return staticBool;
            }
            set
            {
                staticBool = value;
                RaiseStaticPropertyChanged(() => StaticBool);
            }
        }

        private static String readOnlyName = "ReadonlyName";

        public static String ReadOnlyName
        {
            get
            {
                return readOnlyName;
            }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (StaticPropertyChanged != null)
            {
                var propertyName = (propertyExpression.Body as MemberExpression).Member.Name;
                StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static double staticAWithPersonalEvent = 15;

        public static double StaticAWithPersonalEvent
        {
            get
            {
                return staticAWithPersonalEvent;
            }
            set
            {
                staticAWithPersonalEvent = value;

                if (StaticAWithPersonalEventChanged != null)
                    StaticAWithPersonalEventChanged(null, EventArgs.Empty);
            }
        }

        public static event EventHandler StaticAWithPersonalEventChanged;
    }

    public static class StaticStaticClass
    {
        private static double staticA = 15;

        public static double StaticA
        {
            get
            {
                return staticA;
            }
            set
            {
                staticA = value;
                RaiseStaticPropertyChanged(() => StaticA);
            }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (StaticPropertyChanged != null)
            {
                var propertyName = (propertyExpression.Body as MemberExpression).Member.Name;
                StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
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

    public enum Enum1
    {
        Value1,
        Value2
    }

    public enum Enum2
    {
        Value1,
        Value2
    }

    public enum Enum3
    {
        Value11,
        Value22
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

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

        private char symbol;
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

        ObservableCollection<GridItemViewModel> items;
        public ObservableCollection<GridItemViewModel> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                RaisePropertyChanged(() => Items);
            }
        }

        ObservableCollection<GridItemViewModel> items2;
        public ObservableCollection<GridItemViewModel> Items2
        {
            get
            {
                return items2;
            }
            set
            {
                items2 = value;
                RaisePropertyChanged(() => Items2);
            }
        }

        public ObservableCollection<GridItemViewModel> FillItems(int count)
        {
            var random = new Random();

            var newItems = new List<GridItemViewModel>(count);

            for (int i = 0; i < count; i++)
            {
                var newItem = new GridItemViewModel
                {
                    A = random.Next(10000),
                    B = random.NextDouble() * 10000,
                    Name = GetRandomString(random, 10),
                    EnumValue = GetRandomEnum<Enum1>(random),
                    B1 = GetRandomBool(random),
                    B2 = GetRandomBool(random),

                };

                newItems.Add(newItem);
            }

            return new ObservableCollection<GridItemViewModel>(newItems);
        }

        private T GetRandomEnum<T>(Random random)
        {
            var values = Enum.GetValues(typeof(T));

            var value = values.GetValue(random.Next(values.Length));

            return (T)value;
        }

        private bool GetRandomBool(Random random)
        {
            return random.Next(2) == 1;
        }

        private string GetRandomString(Random random, int length)
        {
            var str = "";

            for (int i = 0; i < length; i++)
            {
                var c = (char)('a' + random.Next(26));

                str += c;
            }
            return str;
        }
    }

    public class GridItemViewModel:BaseViewModel
    {
        private int a = 10;
        public int A
        {
            get { return a; }
            set
            {
                a = value;
                RaisePropertyChanged(() => A);
            }
        }

        private double b = 10;
        public double B
        {
            get { return b; }
            set
            {
                b = value;
                RaisePropertyChanged(() => B);
            }
        }

        private String name = "";
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private Enum1 enumValue;
        public Enum1 EnumValue
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

        private bool b1;
        public bool B1
        {
            get { return b1; }
            set
            {
                b1 = value;
                RaisePropertyChanged(() => B1);
            }
        }

        private bool b2;
        public bool B2
        {
            get { return b2; }
            set
            {
                b2 = value;
                RaisePropertyChanged(() => B2);
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

    public class StringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() + "1234";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

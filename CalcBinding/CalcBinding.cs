using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Linq.Expressions;
using System.Globalization;

using OldBinding = System.Windows.Data.Binding;

namespace CalcBinding
{
    public class Binding : MarkupExtension
    {
        public String Path { get; set; }

        public Binding()
        {

        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 1.) Разберем выражение, нужно создать multiBinding по всем свойствам (для примера - искать будем
            // только в dataContext

            //var znak = @"(  \( | \) | \+ | \- | \* | \/ | \% | \^ | \! | \&\& | \|\| | \d)";
            
            // это просто проверка регулярки
            //var matches = Regex.Matches(Path.Replace(" ", ""), String.Format(@"{0}*?(?<path>\w+){0}*?", znak));

            // это уже получить данные
            var matches = Regex.Matches(Path.Replace(" ", ""), @"[a-zA-Z]+(\.[a-zA-Z]+)*");
            var pathsList = new List<String>();
            foreach (var match in matches.OfType<Match>())
            {
                pathsList.Add(match.Value);
            }
            pathsList = pathsList.Distinct().ToList();

            var mathPath = Path; 
            
            foreach (var p in pathsList)
                mathPath = mathPath.Replace(p, pathsList.IndexOf(p).ToString("{0}"));

            var mBinding = new MultiBinding();
            var mathConverter = new CalcConverter();
            mBinding.Converter = mathConverter;
            mBinding.ConverterParameter = mathPath;

            foreach (var path in pathsList)
            {
                var binding = new OldBinding(path);
                mBinding.Bindings.Add(binding);
            }

            //Получим провайдер, с информацией об объекте и привязках
            var providerValuetarget = (IProvideValueTarget)serviceProvider
              .GetService(typeof(IProvideValueTarget));

            //Получим объект, вызвавший привязку
            FrameworkElement _targetObject = (FrameworkElement)providerValuetarget.TargetObject;

            //Получим свойство для привязки
            DependencyProperty _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty; 
            
            //_targetObject.GetType().GetProperty(Path);

            // прикрепим биндинг
            //_targetObject.SetBinding(_targetProperty, mBinding);

            ////Вернем значение свойства
            //var context = _targetObject.DataContext;
            //var contextType = context.GetType();
            //var curValues = pathsList.Select(p => contextType.GetProperty(p).GetValue(context)).ToList();
            //var defaultValue = mathConverter.Convert(curValues, _targetProperty.PropertyType, mathPath, CultureInfo.CurrentCulture);
            //// не знаю, что возвращать
            //return defaultValue;//new MultiBindingExpression();
            //return _sourceProperty.GetValue(_targetObject, null);

            return mBinding.ProvideValue(serviceProvider);
        }
    }
}

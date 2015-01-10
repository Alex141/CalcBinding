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
using System.ComponentModel;
using DynamicExpresso;

namespace CalcBinding
{
    /// <summary>
    /// Binding with advantages
    /// </summary>
    public class Binding : MarkupExtension
    {
        public String Path { get; set; }

        /// <summary>
        /// False to visibility. Default: False = Collapsed
        /// </summary>
        //[DefaultValue(FalseToVisibility.Collapsed)]
        public FalseToVisibility FalseToVisibility 
        {
            get { return falseToVisibility; }
            set { falseToVisibility = value; }
        }
        private FalseToVisibility falseToVisibility = FalseToVisibility.Collapsed;

        public Binding()
        {

        }

        public Binding(String path)
        {
            Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //provider with reflection of binding and target object
            var providerValuetarget = (IProvideValueTarget)serviceProvider
              .GetService(typeof(IProvideValueTarget));

            FrameworkElement _targetObject = (FrameworkElement)providerValuetarget.TargetObject;
            DependencyProperty _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty; 

            var normPath = normalizePath(Path);
            var pathsList = getPathes(normPath);
            
            var uniquePathList = pathsList
                .Select(path => path.Item1)
                .ToList();

            // logic critical code: order of params must be only {0}..{1}..{2} etc, 
            // and not another. We can't unsynchronize order of parameters[] and params occure
            // in expression template (bug 3).
            // There is an example in description of bug 3 in bug tracker
            var orderedPathes = uniquePathList
                .Select((path, index) => new Tuple<string, int>(path, index))
                .OrderByDescending(path => path.Item1.Length).ToList();

            var exprTemplate = normPath;

            //bug detected: Math.Abs(A) => replace A -> a => Math.abs(A), 
            // not resolved
            exprTemplate = getNormExprTemplate(exprTemplate, orderedPathes, pathsList);

            // end of critical code
            //exprTemplate = "{1} {2} {0}" (AA C BBB)
            var mathConverter = new CalcConverter
            {
                FalseToVisibility = FalseToVisibility
            };

            BindingBase resBinding;

            // possibility of twoway mode. Out of the box it is only 
            // one variable or negative from bool variable
            if (uniquePathList.Count() == 1)
            {
                var binding = new System.Windows.Data.Binding(uniquePathList.Single())
                {
                    Mode = Mode,
                    NotifyOnSourceUpdated = NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = NotifyOnTargetUpdated,
                    NotifyOnValidationError = NotifyOnValidationError,
                    UpdateSourceExceptionFilter = UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = UpdateSourceTrigger,
                    ValidatesOnDataErrors = ValidatesOnDataErrors,
                    ValidatesOnExceptions = ValidatesOnExceptions,
                    ValidatesOnNotifyDataErrors = ValidatesOnNotifyDataErrors,
                };

                if (Source != null)
                    binding.Source = Source;

                if (ElementName != null)
                    binding.ElementName = ElementName;

                if (RelativeSource != null)
                    binding.RelativeSource = RelativeSource;

                if (StringFormat != null)
                    binding.StringFormat = StringFormat;

                // we don't use converter if binding is trivial - {0}, except type convertion from bool to visibility
                if (exprTemplate != "{0}" || _targetProperty.PropertyType == typeof(Visibility))
                {
                    binding.Converter = mathConverter;
                    binding.ConverterParameter = exprTemplate;
                    binding.ConverterCulture = ConverterCulture;
                }
                resBinding = binding;
            }
            else
            {
                var mBinding = new MultiBinding
                {
                    Converter = mathConverter,
                    ConverterParameter = exprTemplate,
                    ConverterCulture = ConverterCulture,
                    Mode = BindingMode.OneWay,
                    NotifyOnSourceUpdated = NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = NotifyOnTargetUpdated,
                    NotifyOnValidationError = NotifyOnValidationError,
                    UpdateSourceExceptionFilter = UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = UpdateSourceTrigger,
                    ValidatesOnDataErrors = ValidatesOnDataErrors,
                    ValidatesOnExceptions = ValidatesOnExceptions,
                    ValidatesOnNotifyDataErrors = ValidatesOnNotifyDataErrors,
                };

                if (StringFormat != null)
                    mBinding.StringFormat = StringFormat;

                mathConverter.StringFormatDefined = StringFormat != null;
                foreach (var path in uniquePathList)
                {
                    var binding = new System.Windows.Data.Binding(path);

                    if (Source != null)
                        binding.Source = Source;

                    if (ElementName != null)
                        binding.ElementName = ElementName;

                    mBinding.Bindings.Add(binding);
                }

                resBinding = mBinding;
            }
            
            return resBinding.ProvideValue(serviceProvider);
        }

        private string getNormExprTemplate(string source, List<Tuple<string, int>> orderedPathes, List<Tuple<string, List<int>>> pathsList)
        {
            var result = "";
            var sourceIndex = 0;

            while (sourceIndex < source.Length)
            {
                var replaced = false;
                foreach (var path in orderedPathes)
                {
                    var replace = path.Item2.ToString("{0}");
                    var indexes = pathsList.First(p => p.Item1 == path.Item1).Item2;

                    foreach (var index in indexes)
                    {
                        if (sourceIndex == index)
                        {
                            result += replace;
                            sourceIndex += replace.Length;
                            replaced = true;
                            break;
                        }
                    }
                    if (replaced) break;
                }
            }

            return result;
            //foreach (var path in orderedPathes)
            //    foreach (var index in pathsList.First(p => p.Item1 == path.Item1).Item2)
            //        exprTemplate = exprTemplate.Substring(0, index) +
            //            path.Item2.ToString("{0}") +
            //            exprTemplate.Substring(index + path.Item1.Length, exprTemplate.Length - index - path.Item1.Length);

        }

        /// <summary>
        /// Find and return all pathes in Path string
        /// </summary>
        /// <param name="normPath"></param>
        /// <returns>List of value and it start positions</returns>
        private List<Tuple<String, List<int>>> getPathes(string normPath)
        {
            var operators = new List<String>() 
            { 
                "(", ")", "+", "-", "*", "/", "%", "^", "!", "&&", "||", 
                "&", "|", "?", ":", "<", ">", "<=", ">=", "==", "!=", "," 
            };

            var matches = normPath.Split(operators.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var pathsList = new List<string>();

            foreach (var match in matches)
            {
                if (!Regex.IsMatch(match, @"\d") && !match.Contains("\""))
                {
                    // math detection
                    if (!Regex.IsMatch(match, @"Math.\w+\(\w+\)") && !Regex.IsMatch(match, @"Math.\w+"))
                        pathsList.Add(match);
                }
            }

            var pathIndexList = pathsList
                .Distinct()
                .Select(path => new Tuple<string, List<int>>(path, new List<int>()))
                .ToList();

            foreach (var path in pathIndexList)
            {
                var indexes = Regex.Matches(normPath, path.Item1).Cast<Match>().Select(m => m.Index).ToList();

                foreach (var index in indexes)
                {
                    var startPosIsOperator = false;
                    if (index == 0)
                        startPosIsOperator = true;

                    foreach (var op in operators)
                        if (index >= op.Length && normPath.Substring(index-op.Length, op.Length) == op)
                            startPosIsOperator = true;

                    var endPosIsOperator = false;

                    if (index + path.Item1.Length == normPath.Length)
                        endPosIsOperator = true;

                    foreach (var op in operators)
                        if (index + path.Item1.Length <= normPath.Length - op.Length && normPath.Substring(index + path.Item1.Length, op.Length) == op)
                            endPosIsOperator = true;

                    if (startPosIsOperator && endPosIsOperator)
                        path.Item2.Add(index);
                }
            }
            return pathIndexList;
        }

        /// <summary>
        /// Replace operators labels to operators names (ex. and -> &&), remove excess spaces
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string normalizePath(string path)
        {
            var replaceDict = new Dictionary<String, String>
            {
                {"and",     "&&"},
                {"or",      "||"},
                {"less=",   "<="},
                {"\'",      "\""}
            };

            var normPath = path;
            foreach (var pair in replaceDict)
                normPath = normPath.Replace(pair.Key, pair.Value);

            // delete all spaces out of user string
            var i = 0;
            var canDelete = true;
            var res = "";
            do
            {
                if (normPath[i] == '\"')
                    canDelete = !canDelete;

                if (normPath[i] != ' ' || !canDelete)
                    res += normPath[i];
            }
            while (++i < normPath.Length);

            normPath = res;

            return normPath;
        }

        #region Binding Properties

        //
        // Summary:
        //     Gets or sets the converter to use to convert the source values to or from
        //     the target value.
        //
        // Returns:
        //     A value of type System.Windows.Data.IMultiValueConverter that indicates the
        //     converter to use. The default value is null.
        [DefaultValue("")]
        public IMultiValueConverter Converter { get; set; }
        //
        // Summary:
        //     Gets or sets the System.Globalization.CultureInfo object that applies to
        //     any converter assigned to bindings wrapped by the System.Windows.Data.MultiBinding
        //     or on the System.Windows.Data.MultiBinding itself.
        //
        // Returns:
        //     A valid System.Globalization.CultureInfo.
        [DefaultValue("")]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }
        //
        // Summary:
        //     Gets or sets an optional parameter to pass to a converter as additional information.
        //
        // Returns:
        //     A parameter to pass to a converter. The default value is null.
        [DefaultValue("")]
        public object ConverterParameter { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates the direction of the data flow of this
        //     binding.
        //
        // Returns:
        //     One of the System.Windows.Data.BindingMode values. The default value is System.Windows.Data.BindingMode.Default,
        //     which returns the default binding mode value of the target dependency property.
        //     However, the default value varies for each dependency property. In general,
        //     user-editable control properties, such as System.Windows.Controls.TextBox.Text,
        //     default to two-way bindings, whereas most other properties default to one-way
        //     bindings.A programmatic way to determine whether a dependency property binds
        //     one-way or two-way by default is to get the property metadata of the property
        //     using System.Windows.DependencyProperty.GetMetadata(System.Type) and then
        //     check the Boolean value of the System.Windows.FrameworkPropertyMetadata.BindsTwoWayByDefault
        //     property.
        public BindingMode Mode { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether to raise the System.Windows.FrameworkElement.SourceUpdated
        //     event when a value is transferred from the binding target to the binding
        //     source.
        //
        // Returns:
        //     true if the System.Windows.FrameworkElement.SourceUpdated event will be raised
        //     when the binding source value is updated; otherwise, false. The default value
        //     is false.
        [DefaultValue(false)]
        public bool NotifyOnSourceUpdated { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether to raise the System.Windows.FrameworkElement.TargetUpdated
        //     event when a value is transferred from the binding source to the binding
        //     target.
        //
        // Returns:
        //     true if the System.Windows.FrameworkElement.TargetUpdated event will be raised
        //     when the binding target value is updated; otherwise, false. The default value
        //     is false.
        [DefaultValue(false)]
        public bool NotifyOnTargetUpdated { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether to raise the System.Windows.Controls.Validation.Error attached
        //     event on the bound element.
        //
        // Returns:
        //     true if the System.Windows.Controls.Validation.Error attached event will
        //     be raised on the bound element when there is a validation error during source
        //     updates; otherwise, false. The default value is false.
        [DefaultValue(false)]
        public bool NotifyOnValidationError { get; set; }
        //
        // Summary:
        //     Gets or sets a handler you can use to provide custom logic for handling exceptions
        //     that the binding engine encounters during the update of the binding source
        //     value. This is only applicable if you have associated the System.Windows.Controls.ExceptionValidationRule
        //     with your System.Windows.Data.MultiBinding object.
        //
        // Returns:
        //     A method that provides custom logic for handling exceptions that the binding
        //     engine encounters during the update of the binding source value.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter { get; set; }
        //
        // Summary:
        //     Gets or sets a value that determines the timing of binding source updates.
        //
        // Returns:
        //     One of the System.Windows.Data.UpdateSourceTrigger values. The default value
        //     is System.Windows.Data.UpdateSourceTrigger.Default, which returns the default
        //     System.Windows.Data.UpdateSourceTrigger value of the target dependency property.
        //     However, the default value for most dependency properties is System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
        //     while the System.Windows.Controls.TextBox.Text property has a default value
        //     of System.Windows.Data.UpdateSourceTrigger.LostFocus.A programmatic way to
        //     determine the default System.Windows.Data.Binding.UpdateSourceTrigger value
        //     of a dependency property is to get the property metadata of the property
        //     using System.Windows.DependencyProperty.GetMetadata(System.Type) and then
        //     check the value of the System.Windows.FrameworkPropertyMetadata.DefaultUpdateSourceTrigger
        //     property.
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether to include the System.Windows.Controls.DataErrorValidationRule.
        //
        // Returns:
        //     true to include the System.Windows.Controls.DataErrorValidationRule; otherwise,
        //     false.
        [DefaultValue(false)]
        public bool ValidatesOnDataErrors { get; set; }
        
        // Summary:
        //     Gets or sets a value that indicates whether to include the System.Windows.Controls.ExceptionValidationRule.
        //
        // Returns:
        //     true to include the System.Windows.Controls.ExceptionValidationRule; otherwise,
        //     false.
        [DefaultValue(false)]
        public bool ValidatesOnExceptions { get; set; }
 
        //
        // Summary:
        //     Gets or sets a value that indicates whether to include the System.Windows.Controls.NotifyDataErrorValidationRule.
        //
        // Returns:
        //     true to include the System.Windows.Controls.NotifyDataErrorValidationRule;
        //     otherwise, false. The default is true.
        [DefaultValue(true)]
        public bool ValidatesOnNotifyDataErrors { get; set; }

        //
        // Summary:
        //     Gets or sets the binding source by specifying its location relative to the
        //     position of the binding target.
        //
        // Returns:
        //     A System.Windows.Data.RelativeSource object specifying the relative location
        //     of the binding source to use. The default is null.
        [DefaultValue("")]
        public RelativeSource RelativeSource { get; set; }
        //
        // Summary:
        //     Gets or sets the object to use as the binding source.
        //
        // Returns:
        //     The object to use as the binding source.
        public object Source { get; set; }

        //
        // Summary:
        //     Gets or sets the name of the element to use as the binding source object.
        //
        // Returns:
        //     The value of the Name property or x:Name Directive of the element of interest.
        //     You can refer to elements in code only if they are registered to the appropriate
        //     System.Windows.NameScope through RegisterName. For more information, see
        //     WPF XAML Namescopes.The default is null.
        [DefaultValue("")]
        public string ElementName { get; set; }

        //
        // Summary:
        //     Gets or sets a string that specifies how to format the binding if it displays
        //     the bound value as a string.
        //
        // Returns:
        //     A string that specifies how to format the binding if it displays the bound
        //     value as a string.
        [DefaultValue("")]
        public string StringFormat { get; set; }

	    #endregion    
    }
}

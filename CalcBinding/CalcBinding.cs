using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

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
        public FalseToVisibility FalseToVisibility 
        {
            get { return falseToVisibility; }
            set { falseToVisibility = value; }
        }
        private FalseToVisibility falseToVisibility = FalseToVisibility.Collapsed;

        public Binding() { }

        public Binding(String path)
        {
            Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var targetPropertyType = GetPropertyType(serviceProvider);
            var normalizedPath = NormalizePath(Path);
            var sourcePropertiesPathesWithPositions = GetSourcePropertiesPathes(normalizedPath);
            var expressionTemplate = GetExpressionTemplate(normalizedPath, sourcePropertiesPathesWithPositions);

            var mathConverter = new CalcConverter
            {
                FalseToVisibility = FalseToVisibility,
                StringFormatDefined = StringFormat != null
            };

            BindingBase resBinding;

            if (sourcePropertiesPathesWithPositions.Count() == 1)
            {
                var binding = new System.Windows.Data.Binding(sourcePropertiesPathesWithPositions.Single().LocalPath)
                {
                    Mode = Mode,
                    NotifyOnSourceUpdated = NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = NotifyOnTargetUpdated,
                    NotifyOnValidationError = NotifyOnValidationError,
                    UpdateSourceExceptionFilter = UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = UpdateSourceTrigger,
                    ValidatesOnDataErrors = ValidatesOnDataErrors,
                    ValidatesOnExceptions = ValidatesOnExceptions,
#if NET45
                    ValidatesOnNotifyDataErrors = ValidatesOnNotifyDataErrors,
#endif
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
                if (expressionTemplate != "{0}" || targetPropertyType == typeof(Visibility))
                {
                    binding.Converter = mathConverter;
                    binding.ConverterParameter = expressionTemplate;
                    binding.ConverterCulture = ConverterCulture;
                }
                resBinding = binding;
            }
            else
            {
                var mBinding = new MultiBinding
                {
                    Converter = mathConverter,
                    ConverterParameter = expressionTemplate,
                    ConverterCulture = ConverterCulture,
                    Mode = BindingMode.OneWay,
                    NotifyOnSourceUpdated = NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = NotifyOnTargetUpdated,
                    NotifyOnValidationError = NotifyOnValidationError,
                    UpdateSourceExceptionFilter = UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = UpdateSourceTrigger,
                    ValidatesOnDataErrors = ValidatesOnDataErrors,
                    ValidatesOnExceptions = ValidatesOnExceptions,
#if NET45
                    ValidatesOnNotifyDataErrors = ValidatesOnNotifyDataErrors,
#endif
                };

                if (StringFormat != null)
                    mBinding.StringFormat = StringFormat;

                foreach (var sourcePropertyPathWithPositions in sourcePropertiesPathesWithPositions)
                {
                    var binding = new System.Windows.Data.Binding(sourcePropertyPathWithPositions.LocalPath);

                    if (Source != null)
                        binding.Source = Source;

                    if (ElementName != null)
                        binding.ElementName = ElementName;

                    if (RelativeSource != null)
                        binding.RelativeSource = RelativeSource;

                    mBinding.Bindings.Add(binding);
                }

                resBinding = mBinding;
            }
            
            return resBinding.ProvideValue(serviceProvider);
        }

        private Type GetPropertyType(IServiceProvider serviceProvider)
        {
            //provider of target object and it's property
            var targetProvider = (IProvideValueTarget) serviceProvider
                .GetService(typeof (IProvideValueTarget));

            if (targetProvider.TargetProperty is DependencyProperty)
            {
                return ((DependencyProperty) targetProvider.TargetProperty).PropertyType;
            }

            return targetProvider.TargetProperty.GetType();
        }

        /// <summary>
        /// Replace source properties pathes by its numbers
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathes"></param>
        /// <returns></returns>
        private string GetExpressionTemplate(string fullpath, List<ParsedPath> pathes)
        {
            //Flatten parsed paths, get all merged indexes and corresponding localpath keys
            var pathmap = new List<Tuple<string, int, int>>(); //Item1 = LocalPath, Item2 = order of localpath, Item3 = position of localpath in fullpath
            
            for (int i = 0; i < pathes.Count; i++)
            {
                foreach (var index in pathes[i].MergedIndexes)
                {
                    var t = new Tuple<string, int, int>(pathes[i].LocalPath, i, index);
                    pathmap.Add(t);
                }
            }
            pathmap = pathmap.OrderBy(x => x.Item2).ToList();

            //Iterate through ascending index values and rebuild the fullpath with replaced values
            StringBuilder sb = new StringBuilder();
            int currentindex = 0;
            foreach (var m in pathmap)
            {
                var index = m.Item3;
                var localpath = m.Item1;
                var pathorder = m.Item2;

                //Either we are appending the local path, or we are appending what is between the consecutive map values
                if (currentindex < index)
                {
                    var s = fullpath.Substring(currentindex, index - currentindex);
                    sb.Append(s);
                    currentindex += s.Length;
                }
                if (currentindex == index)
                {
                    sb.AppendFormat("{{{0}}}", pathorder);
                    currentindex += localpath.Length;
                }
            }
            //Fill in the remaining characters
            var remaininglength = fullpath.Length - currentindex;
            var substr = fullpath.Substring(currentindex, remaininglength);
            sb.Append(substr);

            var result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Find and return all sourceProperties pathes in Path string
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns>List of pathes and its start positions</returns>
        private List<ParsedPath> GetSourcePropertiesPathes(string fullPath)
        {
            // temporary solution of problem: all string content shouldn't be parsed. Solution - remove strings from sourcePath.
            //todo: better solution is to use parser PARSER!!


            //detect all start positions
            
            //what problem this code solves:
            //for examle, we have Path = Math.Abs(M) + M, where M - source property.
            //We found that M - source property name, but we don't know positions of M.
            // If we call Path.Replace("M", "{0}"), we obtain expressionTemlate = "{0}ath.Abs({0}) + {0}"
            // which is invalid, because we souldn't replace M when it is the part of other property name.
            // So, foreach founded source property name we need to found all positions and foreaech of 
            // these positions check - near source property path at founded positions must be OPERATORS
            // not other symbols. So, following code perform this check 

            //may be that task solved by using PARSER!

            //Get the set of valid paths
            var pathsList = GetPathes(fullPath)
                .Where(p => p.ValidPath)
                .ToList();

            //Merge the groups. 
            //We are combining each distinct value of the LocalPath, merging the differing indexes into the MergedIndexes list
            //This is so that we don't have duplicate property evaluations for the same data, improving performance
            var pathGroups = pathsList.GroupBy(p => p.LocalPath);
            foreach (var g in pathGroups)
            {
                var first = g.First();
                first.MergedIndexes = g.Select(x => x.StartIndex).ToList();
            }

            pathsList = pathsList.Where(p => p.MergedIndexes?.Count > 0).ToList();

            return pathsList;
        }

        class ParsedPath
        {
            public static string[] operators = new[]
            {
                "(", ")", "+", "-", "*", "/", "%", "^", "&&", "||",
                "&", "|", "?", ":", "<=", ">=", "<", ">", "==", "!=", "!", ","
            };

            static string[] opsWithoutParenthesis = operators.Except(new[] { "(", ")" }).ToArray();

            public string FullPath { get; set; }
            public string LocalPath { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get { return StartIndex + LocalPath.Length - 1; } }
            public List<int> MergedIndexes { get; set; }

            public bool ValidPath
            {
                get
                {
                    return StartPosIsOperator && EndPosIsOperator;
                }
            }

            public bool StartPosIsOperator
            {
                get
                {
                    if (StartIndex == 0)
                        return true;
                    return operators.Any(op =>
                        op.Length <= StartIndex &&
                        op == FullPath.Substring(StartIndex - op.Length, op.Length));
                }
            } 

            public bool EndPosIsOperator
            {
                get
                {
                    if (StartIndex + LocalPath.Length == FullPath.Length)
                        return true;
                    return operators.Any(op =>
                        FullPath.Length - op.Length >= StartIndex + LocalPath.Length &&
                        op == FullPath.Substring(StartIndex + LocalPath.Length, op.Length));
                }
            }
            
            public bool LocalPathIsNotAtEdge
            {
                get
                {
                    return (StartIndex > 0 && EndIndex < FullPath.Length - 1);
                }
            }

            public bool SurroundedByParenthesis
            {
                get
                {
                    if (!LocalPathIsNotAtEdge)
                        return false;
                    char start = FullPath[StartIndex - 1];
                    char end = FullPath[EndIndex + 1];
                    return start == '(' && end == ')';
                }
            }

            public override string ToString()
            {
                return LocalPath;
            }
        }

        /// <summary>
        /// Returns all strings that are pathes
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private List<ParsedPath> GetPathes(string fullPath)
        {
            var originalPath = fullPath;
            var operators = ParsedPath.operators;

            var substrings = originalPath.Split(new[] { "\"" }, StringSplitOptions.None);

            //Keep track of quoted string lengths in order to preserve token indexes
            var stringlengths = new Queue<int>();

            if (substrings.Length > 0)
            {
                var pathWithoutStringsBuilder = new StringBuilder();
                for (int i = 0; i < substrings.Length; i++)
                {
                    if (i % 2 == 0)
                        pathWithoutStringsBuilder.Append(substrings[i]);
                    else
                    {
                        pathWithoutStringsBuilder.Append("\"\"");
                        stringlengths.Enqueue(substrings[i].Length);
                    }
                }

                fullPath = pathWithoutStringsBuilder.ToString();
            }

            

            var matches = fullPath.Split(operators, StringSplitOptions.RemoveEmptyEntries);

            //If containing text has no operators, but surrounded by ( ), treat the enclosing parenthesis 
            //as part of the string

            // detect all pathes
            var pathsList = new List<ParsedPath>();

            //Scan through the full path to locate the indexes of the matches
            int currentIndex = 0;
            int totalStringLengths = 0;
            foreach (var match in matches)
            {
                if (match == "\"\"")
                {
                    //Keep track of the current number of characters removed up to this point
                    //So that an offset can be calculated
                    totalStringLengths += stringlengths.Dequeue();
                }

                currentIndex = fullPath.IndexOf(match, currentIndex);
                var parsed = new ParsedPath()
                {
                    FullPath = originalPath,
                    LocalPath = match,
                    StartIndex = currentIndex + totalStringLengths
                };
                currentIndex++;

                //Since (Canvas.Left) is a valid path but Canvas.Left is not, we need to account for this.
                if (parsed.SurroundedByParenthesis)
                {
                    //Expand local path by one character in each direction, to include the surrounding parenthesis
                    parsed.LocalPath = parsed.FullPath.Substring(parsed.StartIndex - 1, parsed.LocalPath.Length + 2);
                    parsed.StartIndex--;
                }

                if (!isDouble(match) && !match.Contains("\""))
                {
                    // math detection
                    if (!Regex.IsMatch(match, @"Math.\w+\(\w+\)") && !Regex.IsMatch(match, @"Math.\w+"))
                        if (match != "null")
                            pathsList.Add(parsed);
                }
            }
            return pathsList;
        }

        /// <summary>
        /// Return true, is string can be converted to Double type, and false otherwise
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private bool isDouble(string match)
        {
            double result;
            return Double.TryParse(match, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        /// <summary>
        /// Replace operators labels to operators names (ex. and -> &&), remove excess spaces
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string NormalizePath(string path)
        {
            var replaceDict = new Dictionary<String, String>
            {
                {" and ",     " && "},
                {")and ",     ")&& "},
                {" and(",     " &&("},
                {")and(",     ")&&("},

                {" or ",      " || "},
                {")or ",      ")|| "},
                {" or(",      " ||("},
                {")or(",      ")||("},

                {" less ",    " < "},
                {")less ",    ")< "},
                {" less(",    " <("},
                {")less(",    ")<("},

                {" less=",   " <="}, 
                {")less=",   ")<="}, 
                
                {"\'",    "\""},

                {"not ",    "!"}
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
 
#if NET45
        //
        // Summary:
        //     Gets or sets a value that indicates whether to include the System.Windows.Controls.NotifyDataErrorValidationRule.
        //
        // Returns:
        //     true to include the System.Windows.Controls.NotifyDataErrorValidationRule;
        //     otherwise, false. The default is true.
        [DefaultValue(true)]
        public bool ValidatesOnNotifyDataErrors { get; set; }
#endif
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

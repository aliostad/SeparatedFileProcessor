using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SeparatedFileProcessor
{

    public class Schema
    {
        public const string Rule_Required = "Required";
        public const string Rule_Int32 = "Int32";
        public const string Rule_Long = "Int64";
        public const string Rule_DateTime = "DateTime";

        private List<string> _fields = new List<string>(); 

        public Schema(string schema)
        {
            var strings = schema.Split(';');
            int position = 0;
            foreach (var s in strings)
            {
                var def = s.Split('=');
                int pos = def.Length == 2 ? (int) s[0] : position;
                position++;


            }

           

        }

        private Func<string, string> GetValidatory(string descriptor, string option)
        {
            switch (descriptor)
            {
                case Schema.Rule_Required:
                    return ((s) => s.Trim().Length == 0 ? "Field is required" : string.Empty);
                case Schema.Rule_Int32:
                    int i;
                    return ((s) => !Int32.TryParse(s, out i) ? "Field is not Int32" : string.Empty);
                case Schema.Rule_Long:
                    long l;
                    return ((s) => !Int64.TryParse(s, out l) ? "Field is not Int64" : string.Empty);
                case Schema.Rule_DateTime:
                    DateTime d;
                    return ((s) => !DateTime.TryParse(s, out d) ? "Field is not DateTime" : string.Empty);
                default:
                    throw new ArgumentException("Unknown descriptor: " + descriptor);
            }
        }

        public string Validate(string[] fields)
        {
            
        }

    }

    public class CommandLineOptions : CommandOption
    {
        [Option('i', "inputFileName", Required = true, HelpText = "path to the input file")]
        public string InputFileName { get; set; }

        [Option('l', "LogFileName", Required = false, HelpText = "path to the log file")]
        public string LogFileName { get; set; }

        [Option('s', "separator", Required = false, HelpText = "Separator character", DefaultValue = ",")]
        public string Separator { get; set; }

        [Option('q', "Schema", Required = false, HelpText = "Schema, for example 0=Int32;4=Required", DefaultValue = "")]
        public string Schema { get; set; }

        [Option('n', "count", Required = false, HelpText = "expected number of fields")]
        public int Count { get; set; }

        [HelpOption('?', "help")]
        public string GetTheHelp()
        {
            return GetHelp();
        }
    }


    public abstract class CommandOption
    {
        protected virtual string GetHelp()
        {
            var options = new StringBuilder();
            var parameters = new StringBuilder();
            options.AppendLine("Usage:");

            foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<OptionAttribute>() != null)
                .OrderByDescending(x => x.GetCustomAttribute<OptionAttribute>().Required))
            {
                var attribute = property.GetCustomAttribute<OptionAttribute>();
                if (attribute != null)
                {
                    options.Append(property.PropertyType == typeof(bool)
                                       ? " [-" + attribute.ShortName + "] "
                                       : attribute.Required
                                             ? " -" + attribute.ShortName + " " + attribute.LongName
                                             : " [-" + attribute.ShortName + " " + attribute.LongName + "]");
                    parameters.Append(" -");
                    parameters.Append(attribute.ShortName);
                    parameters.Append("\t");
                    parameters.Append(attribute.Required ? "Required. " : "Optional. ");
                    parameters.Append(attribute.HelpText);
                    parameters.AppendLine(property.PropertyType == typeof(bool) ? " (boolean switch)" :
                        (!attribute.Required && attribute.DefaultValue != null) ? " (default=" + attribute.DefaultValue + ")" : "");
                }

            }

            options.AppendLine();
            options.AppendLine("Parameters: ");
            options.AppendLine(parameters.ToString());

            var method = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                  .FirstOrDefault(x => x.GetCustomAttributes<ExampleAttribute>().Any());
            if (method != null)
            {
                options.AppendLine();
                options.AppendLine("Examples:");
                options.AppendLine(method.Invoke(this, new object[0]).ToString());
            }

            return options.ToString();
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class ExampleAttribute : Attribute
        {
        }
    }
}

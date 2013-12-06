using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SeparatedFileProcessor
{

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

        [Option('k', "skip", Required = false, HelpText = "number of lines to skip")]
        public int SkipCount { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "whether to output errors on console", DefaultValue = true)]
        public bool IsVerbose { get; set; }



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

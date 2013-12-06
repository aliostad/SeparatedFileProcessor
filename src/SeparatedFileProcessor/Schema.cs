using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeparatedFileProcessor
{
    public class Schema
    {
        public const string Rule_Required = "Required";
        public const string Rule_Int32 = "Int32";
        public const string Rule_Long = "Int64";
        public const string Rule_DateTime = "DateTime";

        private List<Tuple<int, Func<string, string>>> _fields = new List<Tuple<int, Func<string, string>>>();

        public Schema(string schema)
        {
            if (string.IsNullOrEmpty(schema))
                return;

            var strings = schema.Split(';');
            int position = 0;
            foreach (var s in strings)
            {
                var def = s.Split('=');
                int pos = def.Length == 2 ? Convert.ToInt32(def[0]) : position;
                string descriptor = def.Length == 2 ? def[1] : def[0];
                position++;

                _fields.Add(new Tuple<int, Func<string, string>>(
                    pos, GetValidator(descriptor, "")));
            }



        }

        private Func<string, string> GetValidator(string descriptor, string option)
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
            var builder = new StringBuilder();
            for (int i = 0; i < fields.Length; i++)
            {
                string field = fields[i];
                var validator = _fields.FirstOrDefault(x => x.Item1 == i);
                var result = validator == null
                    ? string.Empty
                    : validator.Item2(field);
                if (!string.IsNullOrEmpty(result))
                    builder.AppendLine("Field index " + i + " - " + result);
            }

            return builder.ToString();
        }

    }
}

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Streamflix.Api.Formatting
{
    /// <summary>
    /// Output formatter that serializes objects to CSV (text/csv).
    /// Designed primarily for IEnumerable&lt;T&gt; where T is a simple POCO,
    /// but will also handle single objects by emitting a one-row CSV.
    /// </summary>
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type? type)
        {
            if (type == null)
                return false;

            // Handle IEnumerable<T> (but ignore string which is IEnumerable<char>)
            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            {
                var elementType = GetElementType(type);
                return elementType != null && HasSerializableProperties(elementType);
            }

            // Handle single POCOs
            return HasSerializableProperties(type);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "text/csv; charset=" + selectedEncoding.WebName;

            var buffer = new StringBuilder();

            if (context.Object is null)
            {
                // Nothing to write
                await using var writer = new StreamWriter(response.Body, selectedEncoding, leaveOpen: true);
                await writer.FlushAsync();
                return;
            }

            var objectType = context.Object.GetType();

            if (context.Object is IEnumerable enumerable && objectType != typeof(string))
            {
                var elementType = GetElementType(objectType) ?? objectType;
                WriteCollection(buffer, enumerable, elementType);
            }
            else
            {
                WriteSingle(buffer, context.Object, objectType);
            }

            await using var outWriter = new StreamWriter(response.Body, selectedEncoding, leaveOpen: true);
            await outWriter.WriteAsync(buffer.ToString());
            await outWriter.FlushAsync();
        }

        private static void WriteCollection(StringBuilder buffer, IEnumerable values, Type elementType)
        {
            var props = GetSerializableProperties(elementType);

            // Header
            WriteHeaderLine(buffer, props);

            // Rows
            foreach (var value in values)
            {
                WriteObjectLine(buffer, value, props);
            }
        }

        private static void WriteSingle(StringBuilder buffer, object value, Type type)
        {
            var props = GetSerializableProperties(type);

            // Header
            WriteHeaderLine(buffer, props);

            // Single row
            WriteObjectLine(buffer, value, props);
        }

        private static void WriteHeaderLine(StringBuilder buffer, PropertyInfo[] props)
        {
            var header = string.Join(",", props.Select(p => EscapeForCsv(p.Name)));
            buffer.AppendLine(header);
        }

        private static void WriteObjectLine(StringBuilder buffer, object? value, PropertyInfo[] props)
        {
            if (value == null)
            {
                // Still write an empty line to preserve row count
                buffer.AppendLine();
                return;
            }

            var cells = props.Select(p =>
            {
                var raw = p.GetValue(value, null);
                string? str = raw switch
                {
                    null => string.Empty,
                    DateTime dt => dt.ToString("o"),          // ISO 8601
                    DateTimeOffset dto => dto.ToString("o"),  // ISO 8601
                    _ => Convert.ToString(raw, System.Globalization.CultureInfo.InvariantCulture)
                };
                return EscapeForCsv(str ?? string.Empty);
            });

            buffer.AppendLine(string.Join(",", cells));
        }

        private static string EscapeForCsv(string input)
        {
            bool mustQuote = input.Contains(',') ||
                             input.Contains('"') ||
                             input.Contains('\r') ||
                             input.Contains('\n');

            if (!mustQuote)
                return input;

            var escaped = input.Replace("\"", "\"\"");
            return "\"" + escaped + "\"";
        }

        private static bool HasSerializableProperties(Type type)
        {
            return GetSerializableProperties(type).Length > 0;
        }

        private static PropertyInfo[] GetSerializableProperties(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                .ToArray();
        }

        private static Type? GetElementType(Type type)
        {
            // Arrays
            if (type.IsArray)
                return type.GetElementType();

            // IEnumerable<T> on the type itself
            if (type.IsGenericType && type.GetGenericArguments().Length == 1)
                return type.GetGenericArguments()[0];

            // Or implemented interfaces
            var ienumerableInterface = type
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return ienumerableInterface?.GetGenericArguments().FirstOrDefault();
        }
    }
}

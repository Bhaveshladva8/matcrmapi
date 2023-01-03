using System.Globalization;
using Newtonsoft.Json.Converters;

namespace matcrm.data.JsonConverters {
    public class Iso8601DateTimeConverter : IsoDateTimeConverter
    {
        public Iso8601DateTimeConverter() {
            DateTimeStyles = DateTimeStyles.AdjustToUniversal;
            DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";
        }
    }
}
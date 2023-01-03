using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace matcrm.data.Models.MollieModel.Payment
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Resource
    {
        [EnumMember(Value = "orders")] Orders,
        [EnumMember(Value = "payments")] Payments
    }
}

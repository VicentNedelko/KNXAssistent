using Knx.Bus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Converters
{
    public class GAJsonConverter : JsonConverter<GroupAddress>
    {
        public override GroupAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var adr = reader.GetString();
            return new GroupAddress(adr);
        }

        public override void Write(Utf8JsonWriter writer, GroupAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("G3"));
        }
    }
}

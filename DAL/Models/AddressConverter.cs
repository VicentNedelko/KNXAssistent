using Knx.Bus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class AddressConverter : JsonConverter<GroupAddress>
    {
        public override GroupAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            GroupAddress gA = GroupAddress.Parse(reader.GetString());
            return gA;
        }

        public override void Write(Utf8JsonWriter writer, GroupAddress value, JsonSerializerOptions options)
        {
            writer.WriteString("Address", value.Address.ToString());
        }
    }
}

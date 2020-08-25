using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalEquipmentHostingSystem.Areas.App.Models
{
    /// <summary>
    /// 最小时间序列化转换器
    /// </summary>
    public class MinDateTimeConverter : DateTimeConverterBase
    {

        /// <summary>
        /// null反序列化为最小时间
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="objectType">Type</param>
        /// <param name="existingValue">object</param>
        /// <param name="serializer">序列化</param>
        /// <returns>时间</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return DateTime.MinValue;
            else
                return (DateTime)reader.Value;
        }

        /// <summary>
        /// 最小时间序列化为null
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">object</param>
        /// <param name="serializer">序列化</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dateTimeValue = (DateTime)value;
            if (dateTimeValue == DateTime.MinValue)
                writer.WriteNull();
            else
                writer.WriteValue(value);
        }
    }
}
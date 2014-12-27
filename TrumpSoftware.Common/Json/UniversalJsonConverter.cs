using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrumpSoftware.Common.Json
{
    public class UniversalJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jObject = new JObject();
            var fields = value.GetType().GetRuntimeFields();
            var properties = value.GetType().GetRuntimeProperties();
            foreach (var field in fields)
            {
                var propertyAttribute = field.GetCustomAttribute<UniversalJsonPropertyAttribute>();
                if (propertyAttribute == null)
                    continue;
                object fieldValue = field.GetValue(value);
                AddProperty(propertyAttribute, field, fieldValue, jObject);
            }
            foreach (var property in properties)
            {
                if (!property.CanRead)
                    continue;
                var propertyAttribute = property.GetCustomAttribute<UniversalJsonPropertyAttribute>();
                if (propertyAttribute == null)
                    continue;
                object propertyValue = property.GetValue(value);
                AddProperty(propertyAttribute, property, propertyValue, jObject);
            }
            jObject.WriteTo(writer);
        }

        private static void AddProperty(UniversalJsonPropertyAttribute propertyAttribute, MemberInfo property, object propertyValue, JObject jObject)
        {
            string propertyName = propertyAttribute.PropertyName ?? property.Name;
            JToken token = JToken.FromObject(propertyValue);
            jObject.Add(propertyName, token);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            ConstructorInfo defaultConstructor = objectType.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x => !x.GetParameters().Any());
            object result = defaultConstructor.Invoke(new object[0]);
            var fields = objectType.GetRuntimeFields();
            var properties = objectType.GetRuntimeProperties();
            foreach (var field in fields)
            {
                var propertyAttribute = field.GetCustomAttribute<UniversalJsonPropertyAttribute>();
                if (propertyAttribute == null)
                    continue;
                string fieldName = propertyAttribute.PropertyName ?? field.Name;
                JProperty jProperty = jObject.Property(fieldName);
                Type fieldType = propertyAttribute.PropertyType ?? field.FieldType;
                object value = jProperty.Value.ToObject(fieldType);
                field.SetValue(result, value);
            }
            foreach (var property in properties)
            {
                if (!property.CanWrite)
                    continue;
                var propertyAttribute = property.GetCustomAttribute<UniversalJsonPropertyAttribute>();
                if (propertyAttribute == null)
                    continue;
                string fieldName = propertyAttribute.PropertyName ?? property.Name;
                JProperty jProperty = jObject.Property(fieldName);
                Type propertyType = propertyAttribute.PropertyType ?? property.PropertyType;
                object value = jProperty.Value.ToObject(propertyType);
                property.SetValue(result, value);
            }
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
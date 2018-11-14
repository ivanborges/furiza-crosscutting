using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.Serialization
{
    public class PropertyRenameAndIgnoreCamelCaseContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> ignores;
        private readonly Dictionary<Type, Dictionary<string, string>> renames;

        public PropertyRenameAndIgnoreCamelCaseContractResolver()
        {
            ignores = new Dictionary<Type, HashSet<string>>();
            renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        public PropertyRenameAndIgnoreCamelCaseContractResolver IgnoreProperties(Type type, IEnumerable<string> jsonPropertyNames)
        {
            if (jsonPropertyNames == null || !jsonPropertyNames.Any())
                return this;

            if (!ignores.ContainsKey(type))
                ignores[type] = new HashSet<string>();

            foreach (var prop in jsonPropertyNames)
                ignores[type].Add(prop);

            return this;
        }

        public PropertyRenameAndIgnoreCamelCaseContractResolver RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!renames.ContainsKey(type))
                renames[type] = new Dictionary<string, string>();

            renames[type][propertyName] = newJsonPropertyName;

            return this;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
                property.PropertyName = newJsonPropertyName;

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!ignores.ContainsKey(type))
                return false;

            return ignores[type].Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            Dictionary<string, string> _renames;

            if (!renames.TryGetValue(type, out _renames) || !_renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
            {
                newJsonPropertyName = null;
                return false;
            }

            return true;
        }
    }
}
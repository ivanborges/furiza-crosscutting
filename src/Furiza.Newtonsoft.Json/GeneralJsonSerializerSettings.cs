using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Newtonsoft.Json
{
    public class GeneralJsonSerializerSettings : JsonSerializerSettings
    {
        private readonly PropertyRenameAndIgnoreCamelCaseContractResolver contractResolver = new PropertyRenameAndIgnoreCamelCaseContractResolver();

        public GeneralJsonSerializerSettings() : base()
        {
            ContractResolver = contractResolver;
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            NullValueHandling = NullValueHandling.Ignore;
            Formatting = Formatting.Indented;
        }

        public GeneralJsonSerializerSettings IgnoreProperties(Type type, IEnumerable<string> jsonPropertyNames)
        {
            contractResolver.IgnoreProperties(type, jsonPropertyNames);
            return this;
        }

        public GeneralJsonSerializerSettings RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            contractResolver.RenameProperty(type, propertyName, newJsonPropertyName);
            return this;
        }
    }
}
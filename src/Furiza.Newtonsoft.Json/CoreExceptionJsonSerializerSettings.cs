using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace Newtonsoft.Json
{
    public class CoreExceptionJsonSerializerSettings : JsonSerializerSettings
    {
        public CoreExceptionJsonSerializerSettings() : base()
        {
            ContractResolver = new PropertyRenameAndIgnoreCamelCaseContractResolver() { IgnoreSerializableInterface = true }
                .IgnoreProperties(typeof(Exception), typeof(Exception)
                    .GetProperties()
                    .Where(p => p.Name != nameof(Exception.Message))
                    .Select(p => p.Name));
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            NullValueHandling = NullValueHandling.Ignore;
            Formatting = Formatting.Indented;
        }
    }
}
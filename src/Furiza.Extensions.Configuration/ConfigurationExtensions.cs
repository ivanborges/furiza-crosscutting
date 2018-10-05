using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Try to deserialize a configuration section into a predefined settings class; throws an exception if it fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static T TryGet<T>(this IConfiguration configuration) where T : class
        {
            var model = configuration.Get<T>() ??
                throw new InvalidOperationException($"Configuration section '{typeof(T).FullName}' is missing.");

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
                throw new InvalidOperationException($"One or more attributes are missing from the configuration section '{typeof(T).FullName}'. " +
                    $"---> {string.Join(" | ", validationResults.Select(vr => vr.ErrorMessage))}");

            return model;
        }
    }
}
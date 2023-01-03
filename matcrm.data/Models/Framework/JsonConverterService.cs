﻿using System.Collections.Generic;
using matcrm.data.Extensions;
using matcrm.data.Framework.Factories;
using matcrm.data.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace matcrm.data.Framework {
    public class JsonConverterService {
        private readonly JsonSerializerSettings _defaultJsonDeserializerSettings;

        public JsonConverterService() {
            this._defaultJsonDeserializerSettings = this.CreateDefaultJsonDeserializerSettings();
        }

        public string Serialize(object objectToSerialize) {
            return JsonConvert.SerializeObject(objectToSerialize,
                new JsonSerializerSettings {
                    DateFormatString = "yyyy-MM-dd",
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public T Deserialize<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json, this._defaultJsonDeserializerSettings);
        }

        /// <summary>
        ///     Creates the default Json serial settings for the JSON.NET parsing.
        /// </summary>
        /// <returns></returns>
        private JsonSerializerSettings CreateDefaultJsonDeserializerSettings() {
            return new JsonSerializerSettings {
                DateFormatString = "yyyy-MM-dd",
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> {
                    // Add a special converter for payment responses, because we need to create specific classes based on the payment method
                    new PaymentResponseConverter(new PaymentResponseFactory())
                }
            };
        }
    }
}
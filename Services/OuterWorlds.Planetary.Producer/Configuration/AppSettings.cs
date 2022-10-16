using Configuration.AppSettingsModels;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Configuration
{
    public class AppSettings
    {
        public static AppSettings Settings => ReadSettings<AppSettings>($"{AppDomain.CurrentDomain.BaseDirectory}appsettings.json");


        [JsonProperty("applicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty("rabbitMQSettings")]
        public RabbitMQSettings RabbitMQSettings { get; set; }

        #region Reader

        private static T ReadSettings<T>(string filename)
        {
            T settingsObject;

            if (!File.Exists(filename))
                throw new Exception($"{filename} not found");

            using(var reader = new StreamReader(filename))
            {
                var json = reader.ReadToEnd();
                settingsObject = JsonConvert.DeserializeObject<T>(json);
            }

            return settingsObject;
        }

        #endregion
    }
}

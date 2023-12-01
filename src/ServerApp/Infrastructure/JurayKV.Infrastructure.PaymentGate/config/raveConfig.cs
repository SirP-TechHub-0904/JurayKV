using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rave
{

    public class RaveConfig
    {
        private readonly IConfiguration _configuration;

        public RaveConfig(IConfiguration configuration)
        {
            _configuration = configuration;

            // Retrieve values from appsettings.json
            var raveConfigSection = _configuration.GetSection("RaveConfig");

            // Default values or handle missing/invalid configurations as needed
            IsLive = GetValueOrDefault<bool>(raveConfigSection, "IsLive", defaultValue: false);
            PbfPubKey = GetValueOrDefault<string>(raveConfigSection, "PbfPubKey", defaultValue: string.Empty);
            SecretKey = GetValueOrDefault<string>(raveConfigSection, "SecretKey", defaultValue: string.Empty);
        }

        public bool IsLive { get; set; }
        public string PbfPubKey { get; set; }
        public string SecretKey { get; set; }

        private T GetValueOrDefault<T>(IConfigurationSection section, string key, T defaultValue)
        {
            var value = section[key];
            return string.IsNullOrEmpty(value) ? defaultValue : ConvertValue<T>(value);
        }

        private T ConvertValue<T>(string value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                // Handle the conversion error as needed, e.g., log the error
                // and return a default value or throw an exception
                throw new InvalidOperationException($"Failed to convert value '{value}' to type '{typeof(T).Name}' for configuration key.");
            }
        }
    }




    //public class RaveConfig
    //{

    //    private readonly IConfiguration _configuration;

    //    public RaveConfig(IConfiguration configuration)
    //    {
    //        _configuration = configuration;

    //        // Retrieve values from appsettings.json
    //        var raveConfigSection = _configuration.GetSection("RaveConfig");

    //        IsLive = Convert.ToBoolean(raveConfigSection["IsLive"]);
    //        PbfPubKey = raveConfigSection["PbfPubKey"];
    //        SecretKey = raveConfigSection["SecretKey"];
    //    }

    //    public bool IsLive { get; set; }
    //    public string PbfPubKey { get; set; }
    //    public string SecretKey { get; set; }


    //    //public RaveConfig(string publicKey, string secretKey, bool isLive)
    //    //{
    //    //    IsLive = isLive;
    //    //    PbfPubKey = publicKey;
    //    //    SecretKey = secretKey;
    //    //}

    //    //public RaveConfig(string publicKey, bool isLive)
    //    //{
    //    //    IsLive = isLive;
    //    //    PbfPubKey = publicKey;
    //    //}

    //    //public RaveConfig(string publicKey, string secretKey)
    //    //{
    //    //    IsLive = false;
    //    //    PbfPubKey = publicKey;
    //    //    SecretKey = secretKey;
    //    //}

    //    //public bool IsLive { get; set; }
    //    //public string PbfPubKey { get; set; }
    //    //public string SecretKey { get; set; }




    //}
}
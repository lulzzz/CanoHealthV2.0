
using System;
using System.Configuration;

namespace CanoHealth.WindowsService.Infrastructure
{
    public class ConfigureSettings : ConfigurationDAL { }

    public class ConfigurationDAL
    {
        private readonly static string emailSettingsHost = GetSettingsValue("EmailSettings.host", "smtp.office365.com");
        public static string EmailSettingsHost
        {
            get
            {
                return emailSettingsHost;
            }
        }

        private readonly static int emailSettingsPort = GetSettingsValue("EmailSettings.port", 587);
        public static int EmailSettingsPort
        {
            get
            {
                return emailSettingsPort;
            }
        }

        private readonly static bool emailSettingsSsl = GetSettingsValue("EmailSettings.ssl", true);
        public static bool EmailSettingsSsl
        {
            get
            {
                return emailSettingsSsl;
            }
        }

        private readonly static string emailSettingsFrom = GetSettingsValue("EmailSettings.from", "do_not_reply@medprosystems.net");
        public static string EmailSettingsFrom
        {
            get
            {
                return emailSettingsFrom;
            }
        }

        private readonly static string emailSettingsPassword = GetSettingsValue("EmailSettings.password", "MEDpro15!");
        public static string EmailSettingsPassword
        {
            get
            {
                return emailSettingsPassword;
            }
        }

        private readonly static string credentialingContact = GetSettingsValue("CredentialingContact", "afernandez@canohealth.com");
        public static string GetCredentialingContact
        {
            get
            {
                return credentialingContact;
            }
        }

        public static string GetSetting(string name)
        {
            return GetSettingsValue<string>(name, null);
        }


        private static readonly string medicalLicense = GetSettingsValue("Source.MedicalLicense", "DoctorMedicalLicense");
        public static string GetMedicalLicense
        {
            get { return medicalLicense; }
        }

        private static readonly string locationLicense = GetSettingsValue("Source.LocationLicense", "LocationLicense");
        public static string GetLocationLicense
        {
            get { return locationLicense; }
        }


        public static T GetSettingsValue<T>(string key, T defaultValue)
        {
            object result = null;
            if (ConfigurationManager.AppSettings[key] != null)
            {
                var configValue = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(configValue))
                    return defaultValue;

                return (T)Convert.ChangeType(configValue, typeof(T));
            }

            if (result == null)
                return defaultValue;

            return (T)result;
        }
    }
}

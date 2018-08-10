
using System;
using System.Configuration;

namespace CanoHealth.WebPortal.Infraestructure
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

        public static string GetSetting(string name)
        {
            return GetSettingsValue<string>(name, null);
        }

        /*IMPERSONATION SECTION*/
        private static readonly string impUserName = GetSettingsValue("Impersonation.username", "MMDFiles");
        public static string ImpersonationUsrName
        {
            get { return impUserName; }
        }

        private static readonly string impPassword = GetSettingsValue("Impersonation.password", "Medpro123!");
        public static string ImpersonationPassword
        {
            get { return impPassword; }
        }
        private static readonly string impDomain = GetSettingsValue("Impersonation.domain", "MEDPROBILL");
        public static string ImpersonationDomain
        {
            get { return impDomain; }
        }

        private static readonly string srvLicFiles = GetSettingsValue("License.Files", @"\\fl-nas02\TestShares\LicenseFiles\");
        public static string ServerLicFiles
        {
            get { return srvLicFiles; }
        }

        private static readonly string contractAddendums = GetSettingsValue("Contract.Addendums", @"D:\2016-2017 Projects\ContractAddendums\");
        public static string GetContractAddendumsDirectory
        {
            get { return contractAddendums; }
        }

        private static readonly string medicalLicenses = GetSettingsValue("Doctor.MedicalLicense", @"D:\2016-2017 Projects\DoctorMedicalLicenseFiles\");
        public static string GetMedicalLicensesDirectory
        {
            get { return medicalLicenses; }
        }

        private static readonly string personalFiles = GetSettingsValue("Doctor.PersonalFiles", @"D:\2016-2017 Projects\DoctorMedicalLicenseFiles\");
        public static string GetPersonalFilesDirectory
        {
            get { return personalFiles; }
        }

        /*SENDGRID SECTION*/
        private readonly static string sendgridHost = GetSettingsValue("SendGrid.server", "smtp.sendgrid.net");
        public static string GetSendGridHost
        {
            get
            {
                return sendgridHost;
            }
        }

        private readonly static int sendgridPort = GetSettingsValue("SendGrid.port", 587);
        public static int GetSendGridPort
        {
            get
            {
                return sendgridPort;
            }
        }

        private readonly static string sendgridFrom = GetSettingsValue("SendGrid.sentfrom", "emedservicecorp@gmail.com");
        public static string GetSendGridFromAddress
        {
            get
            {
                return sendgridFrom;
            }
        }

        private readonly static string sendgridUser = GetSettingsValue("SendGrid.username", "tikikon11252014");
        public static string GetSendGridUser
        {
            get
            {
                return sendgridUser;
            }
        }

        private readonly static string sendgridPassword = GetSettingsValue("SendGrid.password", "/*harnier123");
        public static string GetSendGridPassword
        {
            get
            {
                return sendgridPassword;
            }
        }

        /*AZURE STORAGE ACCOUNT*/
        private readonly static string share = GetSettingsValue("StorageShare", "canohealth");
        public static string GetShare
        {
            get
            {
                return share;
            }
        }

        private readonly static string locationContainer = GetSettingsValue("LocationContainer", "PlaceOfServiceLicenses");
        public static string GetLocationContainer
        {
            get
            {
                return locationContainer;
            }
        }

        private readonly static string localFileSystem = GetSettingsValue("LocalFileSystem", @"Documents\");
        public static string GetLocalFileSystem
        {
            get
            {
                return localFileSystem;
            }
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
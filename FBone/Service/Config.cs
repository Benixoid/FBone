namespace FBone.Service
{
    public class Config
    {
        public static string ConnectionString { get; set; }
        public static string SupportContactName { get; set; }
        public static string SupportContactPhone { get; set; }
        public static string SupportContactEmail { get; set; }
        public static string SupportContactAddr { get; set; }
        public static string EncryptedDBpassword { get; set; }
        public static string MailSMTPserver { get; set; }
        public static string MailSmtpPort { get; set; }
        public static string MailLoginName { get; set; }
        public static string MailLoginPassword { get; set; }
        public static string ServerDateFormat { get; set; }
        public static string SQLServerDateFormat { get; set; }
        public static string PICollectiveName { get; set; }
        public static bool isProduction { get; set; }
        public static bool emailEnabled { get; set; }
        public static int ReportingDefaultEventType { get; set; }
        public static int ReportingDefaultReportIt { get; set; }
        public static int AuditApproverPosition1 { get; set; }
        public static int AuditApproverPosition2 { get; set; }
        public static string AuditRejectMode { get; set; }
        public static string GetConnectionString()
        {
            string conn_string = "";
            if (Config.EncryptedDBpassword.Equals(""))
            {
                conn_string = Config.ConnectionString;
            }
            else
            {
                conn_string = Config.ConnectionString + "Password='" + MyEncryption.DecryptString(Config.EncryptedDBpassword) + "';";
            }
            return conn_string;
        }
    }
}

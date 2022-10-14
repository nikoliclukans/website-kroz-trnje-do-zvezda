using MailKit.Security;

namespace PerAspera.Infrastructure.Configuration
{
    public sealed class SmtpConfiguration
    {
        public SmtpConfiguration(IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            From = configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:From");
            FromName = configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:FromName");
            Host = configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:Host");
            Username = configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:Username");
            Password = configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:Password");
            Port = configuration.GetValue<int>("Umbraco:CMS:Global:Smtp:Port");
            SecureSocketOptions = configuration.GetValue<SecureSocketOptions>("Umbraco:CMS:Global:Smtp:SecureSocketOptions");
        }

        public string From { get; }
        public string FromName { get; }
        public string Host { get; }
        public string Username { get; }
        public string Password { get; }
        public int Port { get; }
        public SecureSocketOptions SecureSocketOptions { get; }
    }
}

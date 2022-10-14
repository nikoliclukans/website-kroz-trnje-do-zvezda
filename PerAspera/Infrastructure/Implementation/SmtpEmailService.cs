using MimeKit.Text;
using MimeKit;
using PerAspera.Infrastructure.Builders.Email;
using PerAspera.Infrastructure.Interfaces;
using Umbraco.Cms.Core.Models.Email;
using MailKit.Net.Smtp;
using PerAspera.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace PerAspera.Infrastructure.Implementation
{
    public sealed class SmtpEmailService : IEmailService
    {
        private readonly SmtpConfiguration _smtpConfiguration;

    
        public SmtpEmailService(SmtpConfiguration smtpConfiguration)
        {
            _smtpConfiguration = smtpConfiguration ?? throw new ArgumentNullException(nameof(smtpConfiguration));
        }

        public void Send(EmailMessage emailMessage, IEmailTemplate emailTemplate)
        {

            if (emailMessage is null) throw new ArgumentNullException(nameof(emailMessage));

            using var client = CreateClient(_smtpConfiguration);

            client.Send(CreateMimeMessage(emailMessage, emailTemplate));

        }

        public async Task SendAsync(EmailMessage emailMessage, IEmailTemplate emailTemplate)
        {
            if (emailMessage is null) throw new ArgumentNullException(nameof(emailMessage));

            using var client = CreateClient(_smtpConfiguration);

            await client.SendAsync(CreateMimeMessage(emailMessage, emailTemplate));
        }

        private static IReadOnlyList<InternetAddress> CreateAddresses(string[] emails)
            => emails
                .Select(CreateAddress)
                .ToList();

        private static InternetAddress CreateAddress(string email)
            => new MailboxAddress(email, email);

        private static MimeEntity CreateEmailBody(EmailMessage emailMessage, IEmailTemplate emailTemplate)
        {
            var textPart = new TextPart(emailMessage.IsBodyHtml ? TextFormat.Html : TextFormat.Plain)
            {
                Text = emailTemplate
                        .LoadTemplate()
                        .LoadData()
                        .Build()
            };

            if (!emailMessage.HasAttachments) return textPart;

            var multipart = new Multipart("mixed")
            {
                textPart
            };

            foreach (var attachment in emailMessage.Attachments)
            {
                multipart.Add(new MimePart()
                {
                    FileName = attachment.FileName,
                    Content = new MimeContent(attachment.Stream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64
                });
            }

            return multipart;
        }

        private static MimeMessage CreateMimeMessage(EmailMessage emailMessage, IEmailTemplate emailTemplate)
        {
            MimeMessage mimeMessage = new();
            mimeMessage.From.Add(CreateAddress(emailMessage.From));
            mimeMessage.To.AddRange(CreateAddresses(emailMessage.To));
            mimeMessage.Subject = emailMessage.Subject;
            mimeMessage.Body = CreateEmailBody(emailMessage, emailTemplate);

            return mimeMessage;

        }

        private static SmtpClient CreateClient(SmtpConfiguration smtpConfiguration)
        {
            var client = new SmtpClient();
            client.Connect(smtpConfiguration.Host, smtpConfiguration.Port, smtpConfiguration.SecureSocketOptions);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(smtpConfiguration.Username, smtpConfiguration.Password);

            return client;
        }
    }
}

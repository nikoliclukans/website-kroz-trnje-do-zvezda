using PerAspera.Infrastructure.Builders.Email;
using Umbraco.Cms.Core.Models.Email;

namespace PerAspera.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage, IEmailTemplate emailTemplate);
        Task SendAsync(EmailMessage emailMessage, IEmailTemplate emailTemplate);
    }
}

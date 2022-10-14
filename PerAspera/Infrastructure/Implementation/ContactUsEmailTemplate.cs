using PerAspera.Infrastructure.Builders.Email;

namespace PerAspera.Infrastructure.Implementation
{
	public class ContactUsEmailTemplate : IEmailTemplate, IEmailTemplateBuilder, IEmailTemplateWithData
    {
		private string _message;
		public ContactUsEmailTemplate(string message)
		{
			_message = message;
		}

		public string Build()
		{
			return _message;
		}

		public IEmailTemplateBuilder LoadData()
		{
			return this;
		}

		public IEmailTemplateWithData LoadTemplate()
		{
			return this;
		}
	}
}

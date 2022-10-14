namespace PerAspera.Infrastructure.Builders.Email
{
    public interface IEmailTemplate
    {
        IEmailTemplateWithData LoadTemplate();

    }

    public interface IEmailTemplateWithData
    {
        IEmailTemplateBuilder LoadData();
    }

    public interface IEmailTemplateBuilder
    {
        string Build();
    }
}

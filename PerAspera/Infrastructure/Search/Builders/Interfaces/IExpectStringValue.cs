namespace PerAspera.Infrastructure.Search.Builders.Interfaces
{
	internal interface IExpectStringValue
	{
		IStringValue WithValue(string value);
		IStringValue WithValues(string[] values);
	}
}

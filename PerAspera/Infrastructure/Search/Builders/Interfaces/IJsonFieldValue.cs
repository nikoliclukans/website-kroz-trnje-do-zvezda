using PerAspera.Infrastructure.Search.Builders.Interfaces;

namespace PerAspera.Infrastructure.Search.Builders.Interfaces
{
    internal interface IJsonFieldValue : IExpectJsonFieldValue
	{
		IBuilder<string> NoMoreValues();
	}
}

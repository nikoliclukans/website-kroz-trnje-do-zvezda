using PerAspera.Infrastructure.Search.Builders.Interfaces;

namespace PerAspera.Infrastructure.Search.Builders.Interfaces
{
    internal interface IStringValue : IExpectStringValue
	{
		IBuilder<string> NoMoreValues();
	}
}

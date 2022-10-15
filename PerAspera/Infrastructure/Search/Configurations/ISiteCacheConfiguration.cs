using System;

namespace PerAspera.Infrastructure.Search.Configurations
{
	public interface ISiteCacheConfiguration
	{
		TimeSpan ExpirationTime { get; }
	}
}

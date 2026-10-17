using System;

namespace JurayKV.Api;

public static class ConfigurationHelper
{
	public static string GetDbConnectionString(this WebApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
		if (string.IsNullOrWhiteSpace(connectionString))
		{
			connectionString = builder.Configuration.GetConnectionString("DockerDbConnection");
		}

		return connectionString ?? string.Empty;
	}
}

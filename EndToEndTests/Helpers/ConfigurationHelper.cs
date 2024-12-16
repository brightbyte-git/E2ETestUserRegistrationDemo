using Microsoft.Extensions.Configuration;

namespace EndToEndTests.Helpers;

public class ConfigurationHelper
{
    public static string GetConnectionString(string name)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.EndToEndTest.json", optional: false, reloadOnChange: true)
            .Build();

        return configuration.GetConnectionString(name);
    }
}
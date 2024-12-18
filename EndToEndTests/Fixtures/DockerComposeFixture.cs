using EndToEndTests.Services.Docker;

namespace EndToEndTests.Fixtures;

public class DockerComposeFixture 
{
    private readonly ContainerHealthCheck _containerHealthCheck;
    public DockerComposeFixture()
    {
        _containerHealthCheck = new ContainerHealthCheck();
        StartDockerCompose();
        WaitForSqlServer();
    }
    
    private void StartDockerCompose()
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "docker-compose",
            Arguments = "up -d",
            WorkingDirectory = "../", // Set the correct path
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new System.Diagnostics.Process
        {
            StartInfo = startInfo
        };

        process.Start();
        Thread.Sleep(5000); // Wait 1 second between retries

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception("Docker Compose failed:\n" + process.StandardError.ReadToEnd());
        }
    }
    
    private void WaitForSqlServer()
    {
        // Wait for the "TestE2EDemoDatabase created successfully." log message
        bool isDatabaseReady = _containerHealthCheck.WaitForLogContainerMessage("sql-cmd-container", "TestE2EDemoDatabase created successfully.");
    
        if (!isDatabaseReady)
        {
            throw new Exception("Database initialization failed: 'TestE2EDemoDatabase created successfully.' log not found.");
        }
    }

    public void StopDockerCompose()
    {
        var stopInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "docker-compose",
            Arguments = "down",
            WorkingDirectory = "../", // Set the correct path
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new System.Diagnostics.Process
        {
            StartInfo = stopInfo
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception("Failed to stop Docker Compose:\n" + process.StandardError.ReadToEnd());
        }
    }
}
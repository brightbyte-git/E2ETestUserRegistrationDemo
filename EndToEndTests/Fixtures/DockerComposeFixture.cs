using EndToEndTests.Services.Docker;

namespace EndToEndTests.Fixtures;

public class DockerComposeFixture 
{
    private readonly ContainerHealthCheck _containerHealthCheck;
    public DockerComposeFixture()
    {
        _containerHealthCheck = new ContainerHealthCheck();
        StartDockerCompose();
        WaitForBackend();
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

    private void WaitForBackend()
    {
        using var httpClient = new HttpClient();
        var maxRetries = 10;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var response = httpClient.GetAsync("http://localhost:5182/swagger/index.html").Result;
                if (response.IsSuccessStatusCode) return;
            }
            catch(Exception ex)
            {
                // Ignore exceptions during retries
                throw new Exception($"Docker Compose failed:\n{ex.Message}");
            }

            retryCount++;
            Thread.Sleep(1000); // Wait 1 second between retries
        }

        throw new Exception("Backend service did not become healthy within the timeout period.");
    }
    
    private void WaitForSqlServer()
    {
        //TODO: research why localhost 1432 needs to be used
        // TODO: Ensure connection string is read from environment variable.
        
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
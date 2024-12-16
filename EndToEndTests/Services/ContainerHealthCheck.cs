namespace EndToEndTests.Services.Docker;

public class ContainerHealthCheck
{
    public bool WaitForLogContainerMessage(string containerName, string logMessage, int timeoutSeconds = 30)
    {
        var startTime = DateTime.UtcNow;
        var processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"logs {containerName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        while ((DateTime.UtcNow - startTime).TotalSeconds < timeoutSeconds)
        {
            using var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
            process.Start();
            string logs = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (logs.Contains(logMessage))
            {
                Console.WriteLine($"Found log message: '{logMessage}' in container: '{containerName}'");
                return true;
            }

            Thread.Sleep(1000); // Wait 1 second before retrying
        }

        Console.WriteLine($"Timeout reached while waiting for log message: '{logMessage}' in container: '{containerName}'");
        return false;
    }
}
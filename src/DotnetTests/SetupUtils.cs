namespace DotnetTest.SetupUtils;

public class SetupUtils
{
    public static void LoadEnvironmentVariables(string pathToEnvFile)
    {
        var envFilePath = Directory.GetCurrentDirectory() + pathToEnvFile;
        if (System.IO.File.Exists(envFilePath))
        {
            using (StreamReader reader = new StreamReader(envFilePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (
                        line.Count() == 0
                        || line.TrimStart().FirstOrDefault() == '#'
                    )
                    {
                        continue;
                    }
                    int i = line.IndexOf("=");
                    if (i == -1)
                    {
                        throw new InvalidOperationException(
                            "Invalid .env file format"
                        );
                    }
                    Environment.SetEnvironmentVariable(
                        line.Substring(0, i),
                        line.Substring(i + 1)
                    );
                }
            }
        }
    }
}

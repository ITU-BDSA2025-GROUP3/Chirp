namespace Chirp.cli.Tests;

using System.Diagnotics;
using System.Text;
using Xunit;

public class e2eTest
{
    [Fact]
    public void TestReadCheep()
    {
        // Arrange
        private static void ArrangeTestDatabase();
        {
            var src = "./Chirp.cli.Tests/e2eTest.cs/Chirp.cli/chirp_cli_db.csv";
            var dst = "./Chirp.cli.db.csv";
            
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            File.Copy(src, dst, overwrite: true);
        }
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "/usr/bin/dotnet";
            process.StartInfo.Arguments = "./src/Chirp.CLI.Client/bin/Debug/net8.0/chirp.dll read 10";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        string fstCheep = output.Split("\n")[0];
        // Assert
        Assert.StartsWith("ropf", fstCheep);
        Assert.EndsWith("Hello, World!", fstCheep);
    }
}
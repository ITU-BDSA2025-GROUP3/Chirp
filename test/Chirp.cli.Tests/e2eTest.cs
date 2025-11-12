using System.Diagnostics;
namespace Chirp.cli.Tests;

public class e2eTest {
    [Fact]
    public void TestRead10Cheep()
    {
        // Arrange
        string projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../../"));
        ArrangeTestDatabase(projectRoot);
        
        // Act 
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"run --project \"{Path.Combine(projectRoot, "src/Chirp.cli/Chirp.cli.csproj")}\" -- read 10";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = projectRoot;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            StreamReader reader = process.StandardOutput;
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        
        string[] fstCheep = output.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        
        // Assert
        Assert.True(fstCheep.Length >= 4, "Expected to find 4 cheep lines");
        
        Assert.Contains("ropf", fstCheep[0]);
        Assert.Contains("Hello, BDSA students!", fstCheep[0]);
        
        Assert.Contains("adho", fstCheep[1]);
        Assert.Contains("Welcome to the course!", fstCheep[1]);
        
        Assert.Contains("adho", fstCheep[2]);
        Assert.Contains("I hope you had a good summer.", fstCheep[2]);
        
        Assert.Contains("ropf", fstCheep[3]);
        Assert.Contains("Cheeping cheeps on Chirp", fstCheep[3]);
        
    }

    private void ArrangeTestDatabase(string projectRoot)
    {
        string sourceDb = Path.Combine(projectRoot, "src/Chirp.cli/chirp_cli_db.csv");
        string targetDb = Path.Combine(projectRoot, "chirp_cli_db.csv");
        
        if (!File.Exists(sourceDb))
        {
            throw new FileNotFoundException("Test database not found at:", sourceDb);
        }
        
        File.Copy(sourceDb, targetDb, overwrite: true);
    }
}
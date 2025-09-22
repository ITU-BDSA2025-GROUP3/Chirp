using System;
using System.IO;
using System.Linq;
using Xunit;
using SimpleDB;
using static Chirp;

public class CsvDatabaseIntegrationTests : IDisposable
{
    private readonly string tempFilePath;
    
    public CsvDatabaseIntegrationTests() 
    {
        tempFilePath = Path.GetTempFileName();
    }

    [Fact]
    public void StoreAndReadCheep_ReturnsStoredCheep()
    {
        var db = CSVDatabase<Cheep>.Instance;
        db.setFilePath(tempFilePath);
        var cheep = new Cheep { Author = "testuser", Message = "test cheep", Timestamp = 1234567890 };
        
        db.Store(cheep);
        var result = db.Read(1).First();

        Assert.Equal(cheep.Author, result.Author);
        Assert.Equal(cheep.Message, result.Message);
        Assert.Equal(cheep.Timestamp, result.Timestamp);
    }
    
    [Fact]
    public void ReadEmptyData_ReturnsEmptyCheep()
    {
        var db = CSVDatabase<Cheep>.Instance;
        db.setFilePath(tempFilePath);

        var result = db.Read(3);

        Assert.Empty(result);
    }

    public void Dispose()
    {
        if (File.Exists(tempFilePath))
            File.Delete(tempFilePath);
    }
}
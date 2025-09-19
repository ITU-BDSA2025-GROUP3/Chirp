using System.Globalization;

using CsvHelper;

namespace SimpleDB.Tests;

public class CsvDatabaseTestRead
{
    
    public class TestCheepRecord
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
    
    [Fact]
    public void Read_AllRowsFromCsvFile()
    {
        var tempFilePath = Path.GetTempFileName();
        try
        {
            var db =  CSVDatabase<TestCheepRecord>.Instance.UseFile(tempFilePath);
            db.Store(new TestCheepRecord { Author = "alice", Message = "first", Timestamp = 1 });
            db.Store(new TestCheepRecord { Author = "malice", Message = "second", Timestamp = 2 });
            var results = db.Read().ToList();
            Assert.Equal(2, results.Count);
            Assert.Collection(results,
                r => { Assert.Equal("alice",  r.Author); Assert.Equal("first",  r.Message); Assert.Equal(1, r.Timestamp); },
                r => { Assert.Equal("malice", r.Author); Assert.Equal("second", r.Message); Assert.Equal(2, r.Timestamp); }
            );
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }
    
    [Fact]
    public void Read_ReturnsEmptyOnHeaderOnlyFile()
    {
        var tempFilePath = Path.GetTempFileName();
        try
        {
            using var stream = File.Open(tempFilePath, FileMode.Append);
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<TestCheepRecord>();
                csv.NextRecord();
            }

            var db =  CSVDatabase<TestCheepRecord>.Instance.UseFile(tempFilePath);
            
            var results = db.Read().ToList();
            
            // header exists, but has no data rows
            Assert.Empty(results); 
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }
}
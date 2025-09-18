using System.Globalization;

using Xunit.Abstractions;

namespace SimpleDB.Tests;

using Xunit;
using SimpleDB;
using CsvHelper;

public class CsvDatabaseTest
{
    
    public class TestCheepRecord
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
    
    
    [Fact]
    public void Store_WritesHeaderAndRowsWhenFileEmpty()
    {
        var tempFilePath = Path.GetTempFileName();
        try
        {
            var db = new CSVDatabase<TestCheepRecord>(tempFilePath);
            db.Store(new TestCheepRecord { Author = "alice", Message = "first", Timestamp = 1 });
            db.Store(new TestCheepRecord { Author = "malice", Message = "second", Timestamp = 2 });
            
            var lines = File.ReadAllLines(tempFilePath);
            // 3 rows: header row, plus two cheep rows
            Assert.Equal(3, lines.Length);
            
            // check header contains correct values
            Assert.Contains("Author", lines[0]);
            Assert.Contains("Message", lines[0]);
            Assert.Contains("Timestamp", lines[0]);
            
            // verify cheep row values are correct
            using var reader = new StreamReader(tempFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<TestCheepRecord>().ToList();
            
            // ensure num of records (cheep msgs), plus content are correct
            Assert.Equal(2, records.Count);
            Assert.Collection(
                records, 
                record => { Assert.Equal("alice",  record.Author); Assert.Equal("first",  record.Message); Assert.Equal(1, record.Timestamp); },
                record => { Assert.Equal("malice",  record.Author); Assert.Equal("second",  record.Message); Assert.Equal(2, record.Timestamp); }
            );
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }
    [Fact]
    public void Store_NoHeaderRewrite()
    {
        var tempFilePath = Path.GetTempFileName(); // empty file header will be written on first store
        try
        {
            // two seperate database instances, writing to same file
            // empty file, write header and row
            var db1 = new CSVDatabase<TestCheepRecord>(tempFilePath);
            db1.Store(new TestCheepRecord { Author = "alice", Message = "first", Timestamp = 1 });
            // non empty file, only write row
            var db2 = new CSVDatabase<TestCheepRecord>(tempFilePath);
            db2.Store(new TestCheepRecord { Author = "malice", Message = "second", Timestamp = 2 });

            var lines = File.ReadAllLines(tempFilePath);
            // assert header doesn't get written again when storing from new instance
            Assert.Equal(3, lines.Length); // header + 2 rows
            // assert the second row doesn't have a header row
            Assert.DoesNotContain(lines.Skip(1), l => l.Contains("Author") && l.Contains("Message") && l.Contains("Timestamp"));
        }
        finally
        {
            File.Delete(tempFilePath);
        }
    }
}

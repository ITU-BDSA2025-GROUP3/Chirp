using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T> 
{
    private readonly string _filePath; // common naming convention, avoid using .this
    
    public CSVDatabase(string? filePath = null)
    {
        _filePath = string.IsNullOrWhiteSpace(filePath) ? "chirp_cli_db.csv" : filePath;
    }
    
    public IEnumerable<T> Read(int? limit = null)
    {
        var records = new List<T>();
        using var reader = new StreamReader(_filePath);
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = csv.GetRecord<T>();
                records.Add(record);
                
                if (limit.HasValue && records.Count >= limit.Value)
                    break;
            }
        }
        return records;
    }

    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
            Delimiter = ",",
            NewLine = Environment.NewLine
        };
        // APPEND CHIRP MESSAGES TO CSV FILE
        using var stream = File.Open(_filePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);
        if (stream.Length == 0)
        {
            csv.WriteHeader<T>();
            csv.NextRecord();
        }
        csv.WriteRecord(record);
        csv.NextRecord();
    }
}
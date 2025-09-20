using System.Globalization;
using Microsoft.AspNetCore.Builder;
using CsvHelper;
using CsvHelper.Configuration;

using CSVDBService;

namespace CSVDBService;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static CSVDatabase<T>? s_instance;
    private readonly object _gate = new();
    private string _filePath; // common naming convention, avoid using .this
    
    private CSVDatabase()
    {
        _filePath = "chirp_cli_db.csv";
    }
    public static CSVDatabase<T> Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new CSVDatabase<T>();
            }
            
            return s_instance;
        }
    }
    
    public CSVDatabase<T> UseFile(string? filePath)
    {
        lock (_gate)
        {
            _filePath = string.IsNullOrWhiteSpace(filePath) ? "chirp_cli_db.csv" : filePath!;
        }
        return this;
    }
    
    public IEnumerable<T> Read(int? limit = null)
    {
        //Refactor this entire thing to now just be reading from a webrequest
        string path;
        lock (_gate)
        {
            path = _filePath;
        }
        var records = new List<T>();
        using var reader = new StreamReader(path);
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
        string path;
        lock (_gate)
        {
            path = _filePath;
        }
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
            Delimiter = ",",
            NewLine = Environment.NewLine
        };
        // APPEND CHIRP MESSAGES TO CSV FILE
        using var stream = File.Open(path, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);
        if (stream.Length == 0)
        {
            csv.WriteHeader<T>();
            csv.NextRecord();
        }
        csv.WriteRecord(record);
        csv.NextRecord();
        writer.Flush();
    }
}
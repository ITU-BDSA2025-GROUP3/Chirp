using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T> 
{
    
    static string filePath = "chirp_cli_db.csv";
    private static CSVDatabase<T> instance = null;

    private CSVDatabase() { }
    public static CSVDatabase<T> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CSVDatabase<T>();
            }
            
            return instance;
        }
    }
    
    public IEnumerable<T> Read(int? limit = null)
    {
        List<T> records = new List<T>();
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = csv.GetRecord<T>();
                records.Add(record);
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
        };
        // APPEND CHIRP MESSAGES TO CSV FILE
        using (var stream = File.Open(filePath, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecord(record);
            writer.Write("\n");
        }
    }
    
}
// See https://aka.ms/new-console-template for more information

using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

internal class Chirp
{
    static string filePath = "chirp_cli_db.csv";
    public static void Main(string[] args)
    {
        if (args[0] == "cheep")
        {
            // EXIT IF NO MSG AFTER '-- CHEEP'
            if (args.Length < 2)
            {
                Console.WriteLine("incorrect usage, write a message");
                Environment.Exit(1);
            }
            string msg = args[1];
            
            // STORE NEW MSG IN CSV FILE
            StoreCheep(msg);
            // PRINT CONTENTS FROM CSV FILE
            // PrintCheep();
        }

        if (args[0] == "read")
        {
            // IF READ JUST PRINT FROM CSV FILE
            // PrintCheep();
        }
    }
    
    // APPEND CHIRP MESSAGES TO CSV FILE
    private static void StoreCheep(string msg)
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
            var record = new Cheep
            {
                Author = Environment.UserName,
                Message = msg,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };
            
            csv.WriteRecord(record);
            writer.Write("\n");
        }
    }

    private static List<Cheep> ReadCheep()
    {
        List<Cheep> records = new List<Cheep>();
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = csv.GetRecord<Cheep>();
                records.Add(record);
            }
        }
        return records;
    }

    public record Cheep
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
}
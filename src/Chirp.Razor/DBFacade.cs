using System.Reflection;

using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;
using Microsoft.Data.Sqlite;

//Responsible for all database access
public class DBFacade
{
    private readonly string _connectionString;
    private const int PAGE_SIZE = 32; // Fixed page size
    private readonly string _queryPagesSql, _queryPagesFromAuthorSql, _queryTotalPagesSql, _queryTotalPagesFromAuthorSql;

    public DBFacade(string connectionString)
    {
        // find db file path (for now fixed to /tmp/chirp.db) Data Source=/tmp/chirp.db
        _connectionString = connectionString;
        //read embedded resources to form SQL queries as strings with parameter placeholders
        _queryPagesSql = ReadEmbeddedResource("Data/SQLiteQueries/QueryPages.sql");
        _queryPagesFromAuthorSql =  ReadEmbeddedResource("Data/SQLiteQueries/QueryPagesFromAuthor.sql");
        _queryTotalPagesSql = ReadEmbeddedResource("Data/SQLiteQueries/QueryTotalPages.sql");
        _queryTotalPagesFromAuthorSql = ReadEmbeddedResource("Data/SQLiteQueries/QueryTotalPagesFromAuthor.sql");
    }

    private static string ReadEmbeddedResource(string filepath)
    {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        if (!embeddedProvider.GetFileInfo(filepath).Exists)
        {
            throw new FileNotFoundException($"Embedded file {filepath} not found.");
        }
        
        using var reader = embeddedProvider.GetFileInfo(filepath).CreateReadStream();
        using var sr = new StreamReader(reader);
        return sr.ReadToEnd();
    }
    
    public int GetTotalPagesFromAuthor(string author)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = new SqliteCommand(_queryTotalPagesFromAuthorSql, connection);
        command.Parameters.AddWithValue("@author", author);
        
        int totalMessages = Convert.ToInt32(command.ExecuteScalar());
        int totalPages = (totalMessages + PAGE_SIZE - 1) / PAGE_SIZE;
        return totalPages;
    }
    
    public int GetTotalPages()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = new SqliteCommand(_queryTotalPagesSql, connection);
        
        int totalMessages = Convert.ToInt32(command.ExecuteScalar());
        int totalPages = (totalMessages + PAGE_SIZE - 1) / PAGE_SIZE;
        return totalPages;
    }
    public List<CheepViewModel> GetAllCheeps(int page = 1)
    {
        var cheeps = new List<CheepViewModel>();
        var offset = (page - 1) * PAGE_SIZE; // this calculates how many records to skip
       
        //connection to sqlite database
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = new SqliteCommand(_queryPagesSql, connection);
        command.Parameters.AddWithValue("@pageSize", PAGE_SIZE);
        command.Parameters.AddWithValue("@offset", offset);
        
        using var reader = command.ExecuteReader();

        // loop through all rows returned by the query
        while (reader.Read())
        {
            // it also handles null values now
            var author = reader.IsDBNull(0) ? String.Empty : reader.GetString(0);
            var message = reader.IsDBNull(1) ? String.Empty : reader.GetString(1);
            var timestamp = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);
            
            // add to list as CheepViewModel
            cheeps.Add(new CheepViewModel(author, message, UnixTimeStampToDateTimeString(timestamp)));
        }
        return cheeps;
    }
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1)
    {
        var cheeps = new List<CheepViewModel>();
        var offset = (page - 1) * PAGE_SIZE;
        
        // open sqlite connection to the database file
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = new SqliteCommand(_queryPagesFromAuthorSql, connection);
        command.Parameters.AddWithValue("@username", author);
        command.Parameters.AddWithValue("@pageSize", PAGE_SIZE);
        command.Parameters.AddWithValue("@offset", offset);

        using var reader = command.ExecuteReader();

        // read each row and add it to cheeps
        while (reader.Read())
        {
            // it also handles null values now
            var name = reader.IsDBNull(0) ? String.Empty : reader.GetString(0);
            var message = reader.IsDBNull(1) ? String.Empty : reader.GetString(1);
            var timestamp = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);

            cheeps.Add(new CheepViewModel(name, message, UnixTimeStampToDateTimeString(timestamp)));
        }

        return cheeps;
    }
    private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        // convert unix timestamp to human readable date
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp)
            .ToLocalTime()
            .ToString("MM/dd/yy H:mm:ss");
    }

}
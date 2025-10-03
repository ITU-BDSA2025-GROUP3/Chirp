using System.Reflection;

using Chirp.Razor.DomainModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;
using Microsoft.Data.Sqlite;

//Responsible for all database access
public class ChirpDbContext : DbContext
{
    private readonly string _connectionString;
    private const int PAGE_SIZE = 32; // Fixed page size
    private DbSet<Cheep> cheeps { get; set; }
    private DbSet<Author> authors { get; set; }
    
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {
    }
   
    
    public int GetTotalPagesFromAuthor(string author)
    {
        /*using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = new SqliteCommand(_queryTotalPagesFromAuthorSql, connection);
        command.Parameters.AddWithValue("@author", author);
        
        int totalMessages = Convert.ToInt32(command.ExecuteScalar());
        int totalPages = (totalMessages + PAGE_SIZE - 1) / PAGE_SIZE;
        return totalPages;
        */

        throw new NotImplementedException();
    }
    
    public int GetTotalPages()
    {
        /*using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var command = new SqliteCommand(_queryTotalPagesSql, connection);
        
        int totalMessages = Convert.ToInt32(command.ExecuteScalar());
        int totalPages = (totalMessages + PAGE_SIZE - 1) / PAGE_SIZE;
        return totalPages;*/
        throw new NotImplementedException();
    }
    public List<CheepViewModel> GetAllCheeps(int page = 1)
    {
        /*var cheeps = new List<CheepViewModel>();
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
        */
        throw new NotImplementedException();
    }
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1)
    {
        /*var cheeps = new List<CheepViewModel>();
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
        */
        throw new NotImplementedException();
    }
    private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        // convert unix timestamp to human readable date
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp)
            .ToLocalTime()
            .ToString("MM/dd/yy H:mm:ss");
    }

}
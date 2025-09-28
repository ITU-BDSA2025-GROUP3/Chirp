namespace Chirp.Razor;
using Microsoft.Data.Sqlite;

//Responsible for all database access
public class DBFacade
{
    private readonly string _connectionString;

    public DBFacade(string connectionString)
    {
        // find db file path (for now fixed to /tmp/chirp.db) Data Source=/tmp/chirp.db"
        _connectionString = connectionString;
    }
    public List<CheepViewModel> GetAllCheeps()
    {
        var cheeps = new List<CheepViewModel>();

        //connection to sqlite database
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // SQL query to join user and message
        var sql = @"SELECT u.username, m.text, m.pub_date
                FROM message m
                JOIN user u ON m.author_id = u.user_id
                ORDER BY m.pub_date DESC;";

        using var command = new SqliteCommand(sql, connection);
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
    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        var cheeps = new List<CheepViewModel>();

        // open sqlite connection to the database file
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // sql query: filter by given username
        var sql = @"SELECT u.username, m.text, m.pub_date
                    FROM message m
                    JOIN user u ON m.author_id = u.user_id
                    WHERE u.username = @username
                    ORDER BY m.pub_date DESC;";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@username", author);

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
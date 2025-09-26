namespace Chirp.Razor;
using Microsoft.Data.Sqlite;

//Responsible for all database access
public class DBFacade
{
    private readonly string _connectionString;

    public DBFacade()
    {
        // find db file path (for now fixed to /tmp/chirp.db)
        _connectionString = "Data Source=/tmp/chirp.db";
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
            var author = reader.GetString(0);
            var message = reader.GetString(1);
            var timestamp = reader.GetInt64(2);

            // add to list as CheepViewModel
            cheeps.Add(new CheepViewModel(author, message, UnixTimeStampToDateTimeString(timestamp)));
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
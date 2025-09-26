using Microsoft.Data.Sqlite;


public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
   

    public List<CheepViewModel> GetCheeps()
    {
        var cheeps = new List<CheepViewModel>();

        // open sqlite connection to the database file
        using var connection = new SqliteConnection("Data Source=/tmp/chirp.db");
        connection.Open();

        // sql query: join user and message table
        var sql = @"SELECT u.username, m.text, m.pub_date
                    FROM message m
                    JOIN user u ON m.author_id = u.user_id
                    ORDER BY m.pub_date DESC;";

        using var command = new SqliteCommand(sql, connection);
        using var reader = command.ExecuteReader();

        // read each row and add it to cheeps
        while (reader.Read())
        {
            var author = reader.GetString(0);
            var message = reader.GetString(1);
            var timestamp = reader.GetInt64(2);

            cheeps.Add(new CheepViewModel(author, message, UnixTimeStampToDateTimeString(timestamp)));
        }

        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        var cheeps = new List<CheepViewModel>();

        // open sqlite connection to the database file
        using var connection = new SqliteConnection("Data Source=/tmp/chirp.db");
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
            var name = reader.GetString(0);
            var message = reader.GetString(1);
            var timestamp = reader.GetInt64(2);

            cheeps.Add(new CheepViewModel(name, message, UnixTimeStampToDateTimeString(timestamp)));
        }

        return cheeps;
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}

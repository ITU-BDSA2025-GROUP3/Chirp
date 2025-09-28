using System.Data;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        try
        {
            int pageQuery = Convert.ToInt32(Request.Query["page"]);
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            
            //refactor below into a separate utility function with dependency injection for which script and what function
            //parameters to apply 
            
            //this part reads an embedded resource
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using var reader = embeddedProvider.GetFileInfo("./data/SQLiteQueries/PaginatedCheep.sql").CreateReadStream();
            using var sr = new StreamReader(reader);
            var query = sr.ReadToEnd();

            //this establishes connection to database but right now it is just a placeholder string to stop compiler from
            //complaining 
            using var connection = new SqliteConnection("<connectionString>");
            
            //this part runs a sql command and replaces placeholder values programatically. 
            //this also prevents sql injections if done right, but is a secondary optional concern for us
            var command = new SqliteCommand(query, connection);
            command.Parameters.Add("@offset", SqliteType.Integer);
            command.Parameters["@offset"].Value = (pageQuery-1)*32; //todo make 32 a global constant
            command.Parameters.AddWithValue("@limit", pageQuery);
            connection.Open();
            using var sqliteDataReader = command.ExecuteReader(); //executes the sql command and returns a reader to loop over
            Cheeps = new List<CheepViewModel>(); //reset stored cheeps
            //reads each row retrieved and adds them to the list. There doesn't seem to be a better way to do this
            while (sqliteDataReader.Read())
            {
                Cheeps.Add(new CheepViewModel(sqliteDataReader.GetString(0), sqliteDataReader.GetString(1), sqliteDataReader.GetString(2)));
            }
            //after they have been added the method can just return the page and they should automatically be displayed
            //in the provided cshtml
        }
        catch (FormatException e)
        {
            return BadRequest($"Invalid page query. Page query provided: {Request.Query["page"]}");
        }
        catch (Exception e) when (e is ArgumentOutOfRangeException or OverflowException)
        {
            return BadRequest($"Page query '{Request.Query["page"]}' is out of range: 1:{Int32.MaxValue}.");
        }
        return Page();
    }
}

using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Database;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests;

public static class Utility
{
    public static ChirpDbContext CreateFakeChirpDbContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var contextOptions = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlite(connection).Options;
        
        var context = new ChirpDbContext(contextOptions);
        context.Database.EnsureDeletedAsync(); 
        context.Database.EnsureCreatedAsync(); 
        return context;
    }

    /// <summary>
    /// Seeds the database for use in testing.
    /// DO NOT CHANGE CONTENTS AS TESTS DEPENDS ON THE EXACT CONTENTS AS IS
    /// </summary>
    /// <param name="context"></param>
    public static void SeedDatabase(ChirpDbContext context)
    {
        //create a dictionary of how many cheeps to create for each author - ease of access
        var cheepsPerAuthor = new Dictionary<String, int>
        {
            { "Alice", 10 }, { "Bob", 5 }, { "Charlie", 2 }, { "David", 0 }
        };

        List<Author> authorList = [];
        List<Cheep> cheepList = [];
        var idCounter = 1; //must be at least "1" as EF-Core expects this, lest breaking the system
        var timestampCounter = 0;
        foreach (var name in cheepsPerAuthor)
        {
            var author = new Author { Id = idCounter++, UserName = name.Key, Email = $"{name.Key}@{name.Key}.com", Cheeps = new  List<Cheep>(), Follows = new  List<Author>() };
            authorList.Add(author);
            for (int i = 0; i < name.Value; i++)
            {
                var cheep = new Cheep
                {
                    Text = "test",
                    TimeStamp = new DateTime(timestampCounter++),
                    IdOfAuthor = author.Id,
                    Author = author
                };
                author.Cheeps.Add(cheep);
                cheepList.Add(cheep);
            }
        }
        context.Cheeps.AddRange(cheepList);
        context.Authors.AddRange(authorList);
        context.SaveChanges();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Example
{
    public class CharacterData
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Species { get; set; }
        public string Photo { get; set; }
    }

    public class Series
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public ICollection<Character> Characters { get; set; }
    }

    public class Character
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Gender { get; set; }

        [Required]
        public string Species { get; set; }

        [Required]
        public Series Series { get; set; }

        public CharacterPhoto Photo { get; set; }
    }

    public class CharacterPhoto
    {
        public int Id { get; set; }

        [Required]
        public Character Character { get; set; }

        [Required]
        public byte[] Photo { get; set; }
    }

    public class StarTrekContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Series> Series { get; set; }
        public DbSet<Character> Characters { get; set; }

        public StarTrekContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NB while upgrading to ef-core 2.1 we noticed that this one-to-one mapping
            //    was not really ideal, but for data backward-compatibility, we leave it
            //    as it was originally.
            modelBuilder.Entity<CharacterPhoto>()
                .HasOne(cp => cp.Character)
                .WithOne(c => c.Photo)
                .HasForeignKey<Character>("PhotoId");
        }
    }

    // this is used when you run a design-time command, e.g.:
    //      dotnet ef migrations add InitialCreate
    //      dotnet ef migrations add AddCharacterPhoto
    //      dotnet ef database update # -- [args]
    // see https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
    // see https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#bundles
    public class StarTrekContextFactory : IDesignTimeDbContextFactory<StarTrekContext>
    {
        public StarTrekContext CreateDbContext(string[] args)
        {
            var connectionString = Program.GetEnvironmentConnectionString();

            return new StarTrekContext(connectionString);
        }
    }

    class Program
    {
        internal static string GetEnvironmentConnectionString()
        {
            // see https://docs.microsoft.com/en-us/ef/core/
            // see http://www.npgsql.org/efcore/
            // see http://www.npgsql.org/doc/connection-string-parameters.html
            // see http://www.npgsql.org/doc/security.html
            var pgHost = Environment.GetEnvironmentVariable("PGHOST");
            var pgPort = Environment.GetEnvironmentVariable("PGPORT");
            var pgUser = Environment.GetEnvironmentVariable("PGUSER");
            var pgPassword = Environment.GetEnvironmentVariable("PGPASSWORD");
            var pgDatabase = Environment.GetEnvironmentVariable("PGDATABASE");
            var connectionString = $"Host={pgHost}; Port={pgPort}; SSL Mode=Disable; Username={pgUser}; Password={pgPassword}; Database={pgDatabase}";
            return connectionString;
        }

        static void Main(string[] args)
        {
            var connectionString = GetEnvironmentConnectionString();

            // TODO somehow integrate this in the StarTrekContextFactory migrate process.
            if (args.Length == 1 && args[0] == "seed")
            {
                Seed(connectionString);
            }

            using (var db = new StarTrekContext(connectionString))
            {
                Console.WriteLine("Star Trek Characters:");

                foreach (var c in db.Characters.Include(c => c.Series).OrderBy(c => c.Name))
                {
                    Console.WriteLine("    {0} ({1}; {2})", c.Name, (c.Gender ?? "").Replace("\n", "; "), c.Series.Name);
                }
            }
        }

        private static void Seed(string connectionString)
        {
            using (var db = new StarTrekContext(connectionString))
            {
                if (db.Series.Any())
                {
                    return;
                }

                Console.WriteLine("Populating the database");

                var data = JsonConvert.DeserializeObject<Dictionary<string, CharacterData[]>>(File.ReadAllText("star-trek-scraper/data.json"));

                foreach (var kp in data)
                {
                    var series = new Series
                    {
                        Name = kp.Key,
                        Characters = new List<Character>(),
                    };
                    db.Series.Add(series);

                    foreach (var c in kp.Value.OrderBy(d => d.Name))
                    {
                        Console.WriteLine("Adding {0} ({1}) to the database", c.Name, series.Name);

                        series.Characters.Add(
                            new Character
                            {
                                Name = c.Name,
                                Gender = c.Gender,
                                Species = c.Species,
                                Photo = new CharacterPhoto { Photo = Convert.FromBase64String(c.Photo) },
                            }
                        );
                    }
                }

                var count = db.SaveChanges();

                Console.WriteLine("{0} records saved to the database", count);
            }
        }
    }
}

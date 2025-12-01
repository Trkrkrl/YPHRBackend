using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Entities.Concrete;
using Core.Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext()
        {
        }

        // [AUTO-DBSETS-START]
        // Auto-generated DbSet<T> entries will be inserted here by the console tool.
        public DbSet<OperationClaim> OperationClaims { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; } = null!;

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<Title> Titles { get; set; }

        // [AUTO-DBSETS-END]

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            // Read database provider & connection strings from appsettings.json
            var configuration = new ConfigurationManager();
            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            var provider = configuration["Database:Provider"] ?? "Sqlite";

            // Connection strings
            var sqliteConn =
                configuration.GetConnectionString("Sqlite") ??
                configuration["ConnectionStrings:Sqlite"] ??
                "Data Source=personnel.db";

            var sqlServerConn =
                configuration.GetConnectionString("SqlServer") ??
                configuration["ConnectionStrings:SqlServer"];

            var postgreConn =
                configuration.GetConnectionString("PostgreSql") ??
                configuration["ConnectionStrings:PostgreSql"];

            var mySqlConn =
                configuration.GetConnectionString("MySql") ??
                configuration["ConnectionStrings:MySql"];

            var mongoConn =
                configuration.GetConnectionString("MongoDb") ??
                configuration["ConnectionStrings:MongoDb"];

            // Select provider
            switch (provider.Trim().ToLowerInvariant())
            {
                case "sqlserver":
                case "mssql":
                    if (!string.IsNullOrWhiteSpace(sqlServerConn))
                    {
                        optionsBuilder.UseSqlServer(sqlServerConn);
                        break;
                    }
                    goto default; // fallback to Sqlite

                case "postgres":
                case "postgresql":
                case "npgsql":
                    if (!string.IsNullOrWhiteSpace(postgreConn))
                    {
                        optionsBuilder.UseNpgsql(postgreConn);
                        break;
                    }
                    goto default;

                case "mysql":
                    if (!string.IsNullOrWhiteSpace(mySqlConn))
                    {
                        // Requires Pomelo.EntityFrameworkCore.MySql
                        optionsBuilder.UseMySql(mySqlConn, ServerVersion.AutoDetect(mySqlConn));
                        break;
                    }
                    goto default;

                case "mongodb":
                    // MongoDb is not supported via EF Core DbContext.
                    // Use the official MongoDB driver and a separate context abstraction.
                    throw new NotSupportedException("MongoDb is not supported through EF Core DbContext. Use MongoDB driver instead.");

                default:
                    // Default / fallback: Sqlite
                    optionsBuilder.UseSqlite(sqliteConn);
                    break;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API configurations will be added here later.
        }
    }
}

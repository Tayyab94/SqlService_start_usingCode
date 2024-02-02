using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartSqlServiceFromCode
{
    public class YourDbContext:DbContext
    {
        private readonly string _connectionString;

        public YourDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Student> Students { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Students");
            base.OnModelCreating(modelBuilder);
        }
    }
}

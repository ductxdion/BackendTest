using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Matrix> Matrices { get; set; }
    }
}

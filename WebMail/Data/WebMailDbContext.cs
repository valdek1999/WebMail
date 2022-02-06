using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMail.Models;

namespace WebMail.Data
{
    /// <summary>
    /// Database context
    /// </summary>
    public class WebMailDbContext: DbContext
    {
        /// <summary>
        /// The Mail Database table
        /// </summary>
        public DbSet<Mail> Mail { get; set; }

        /// <summary>
        /// The Recipient Database table
        /// </summary>
        public DbSet<Mail> Recipient { get; set; }
        public WebMailDbContext(DbContextOptions<WebMailDbContext> options): base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // base.OnModelCreating(modelBuilder);
        }
    }
}

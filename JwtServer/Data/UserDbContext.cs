using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JwtServer.Data
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> t_users { get; set; }
        public DbSet<ValueModel> t_values { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=app.db");
        }
    }

    [Table(name: "users")]
    public class User
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Uid { get; set; }
        public string Pwd { get; set; }

    }

    [Table(name: "values")]
    public class ValueModel
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string value { get; set; }
    }
}

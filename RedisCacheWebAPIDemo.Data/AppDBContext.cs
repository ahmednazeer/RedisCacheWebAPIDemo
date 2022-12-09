using Microsoft.EntityFrameworkCore;
using RedisCacheWebAPIDemo.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheWebAPIDemo.Data
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) 
        {

        }
        public DbSet<Driver> Drivers { get; set; }
    }
}

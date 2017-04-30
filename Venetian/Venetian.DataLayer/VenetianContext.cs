using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venetian.DomainClasses;

namespace Venetian.DataLayer
{
    public class VenetianContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}

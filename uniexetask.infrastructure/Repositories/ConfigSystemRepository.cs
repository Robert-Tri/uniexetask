using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    public class ConfigSystemRepository : GenericRepository<ConfigSystem>, IConfigSystemRepository
    {
        public ConfigSystemRepository(UniExetaskContext context) : base(context)
        {
        }

        public ConfigSystem? GetConfigSystemByID(int id)
        {
            return dbSet.FirstOrDefault(c => c.ConfigId == id);
        }
    }
}

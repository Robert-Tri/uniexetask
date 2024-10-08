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
    class WorkShopRepository : GenericRepository<Workshop>, IWorkShopRepository
    {
        public WorkShopRepository(UniExetaskContext dbContext) : base(dbContext) { }
    }
}

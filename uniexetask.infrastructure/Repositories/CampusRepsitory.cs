using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    class CampusRepsitory : GenericRepository<Campus>, ICampusRepository
    {
        public CampusRepsitory(UniExetaskContext dbContext) : base(dbContext)
        {
        }
    }
}

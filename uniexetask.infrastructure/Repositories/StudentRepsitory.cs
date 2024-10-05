using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    class StudentRepsitory : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepsitory(UniExetaskContext dbContext) : base(dbContext)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IMentorService
    {
        Task<Mentor?> GetMentorWithGroupAsync(int userId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uniexetask.shared.Models.Request
{
    public class UpdateMentorModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string Specialty { get; set; } = null!;
    }
}

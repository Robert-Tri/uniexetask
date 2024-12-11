    namespace uniexetask.shared.Models.Request
    {
        public class ExcelModel
    {
            public int UserId { get; set; }

            public int LecturerId { get; set; }

            public string StudentCode { get; set; } = null!;

            public string Major { get; set; } = null!;

            public int SubjectId { get; set; }

            public bool IsCurrentPeriod { get; set; }

            public string FullName { get; set; } = null!;

        public string? Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }
        public string? Avatar { get; set; }

        public int CampusId { get; set; }

        public bool IsDeleted { get; set; }

        public int RoleId { get; set; }
    }
    }

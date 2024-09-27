using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using uniexetask.core.Models;

namespace uniexetask.infrastructure;

public partial class UniExetaskContext : DbContext
{
    public UniExetaskContext()
    {
    }

    public UniExetaskContext(DbContextOptions<UniExetaskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<ChatGroup> ChatGroups { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupInvite> GroupInvites { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<MeetingSchedule> MeetingSchedules { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<Nofitication> Nofitications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Requirement> Requirements { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sponsor> Sponsors { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<uniexetask.core.Models.Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

/*    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=STHAO\\SONG_THAO;Database=UniEXETask;Trusted_Connection=True;TrustServerCertificate=True;");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__CAMPUS__01989FD1D20DE96D");

            entity.ToTable("CAMPUS");

            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.CampusCode)
                .HasMaxLength(50)
                .HasColumnName("campus_code");
            entity.Property(e => e.CampusName)
                .HasMaxLength(100)
                .HasColumnName("campus_name");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
        });

        modelBuilder.Entity<ChatGroup>(entity =>
        {
            entity.HasKey(e => e.ChatGroupId).HasName("PK__CHAT_GRO__F18D35793DC5074C");

            entity.ToTable("CHAT_GROUP");

            entity.Property(e => e.ChatGroupId).HasColumnName("chat_group_id");
            entity.Property(e => e.ChatboxName)
                .HasMaxLength(50)
                .HasColumnName("chatbox_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChatGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__creat__48CFD27E");

            entity.HasOne(d => d.Owner).WithMany(p => p.ChatGroupOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__owner__49C3F6B7");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__CHAT_MES__0BBF6EE6642C1C94");

            entity.ToTable("CHAT_MESSAGE");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.ChatGroupId).HasColumnName("chat_group_id");
            entity.Property(e => e.MessageContent)
                .HasMaxLength(4000)
                .HasColumnName("message_content");
            entity.Property(e => e.SendDatetime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("send_datetime");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ChatGroup).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.ChatGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_MESS__chat___4D94879B");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_MESS__user___4E88ABD4");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__EVENT__2370F7279EC27DDB");

            entity.ToTable("EVENT");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Location)
                .HasMaxLength(250)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.RegUrl)
                .HasMaxLength(250)
                .HasColumnName("reg_url");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GROUP__D57795A0961D119E");

            entity.ToTable("GROUP");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupName)
                .HasMaxLength(250)
                .HasColumnName("group_name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Project).WithMany(p => p.Groups)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP__project_i__7B5B524B");
        });

        modelBuilder.Entity<GroupInvite>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.NotificationId }).HasName("PK__GROUP_IN__3B720DE21B8DF914");

            entity.ToTable("GROUP_INVITE");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.InviteeId).HasColumnName("invitee_id");
            entity.Property(e => e.InviterId).HasColumnName("inviter_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__group__14270015");

            entity.HasOne(d => d.Notification).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__notif__151B244E");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.StudentId }).HasName("PK__GROUP_ME__67D4A5C909236B5D");

            entity.ToTable("GROUP_MEMBER");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__group__08B54D69");

            entity.HasOne(d => d.Student).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__stude__09A971A2");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__LABEL__E44FFA584979913C");

            entity.ToTable("LABEL");

            entity.Property(e => e.LabelId).HasColumnName("label_id");
            entity.Property(e => e.LabelName)
                .HasMaxLength(50)
                .HasColumnName("label_name");
        });

        modelBuilder.Entity<MeetingSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__MEETING___C46A8A6FE5A602ED");

            entity.ToTable("MEETING_SCHEDULE");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("content");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.MeetingDate)
                .HasColumnType("datetime")
                .HasColumnName("meeting_date");
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Group).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__group__00200768");

            entity.HasOne(d => d.Mentor).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__mento__01142BA1");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("PK__MENTOR__E5D27EF3F0FC653A");

            entity.ToTable("MENTOR");

            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Specialty)
                .HasMaxLength(250)
                .HasColumnName("specialty");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(250)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MENTOR__user_id__73BA3083");
        });

        modelBuilder.Entity<Nofitication>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__NOFITICA__E059842FB5BB2374");

            entity.ToTable("NOFITICATION");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Receiver).WithMany(p => p.NofiticationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOFITICAT__recei__0F624AF8");

            entity.HasOne(d => d.Sender).WithMany(p => p.NofiticationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOFITICAT__sende__0E6E26BF");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__PERMISSI__E5331AFAA7F0158F");

            entity.ToTable("PERMISSION");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__PROJECT__BC799E1FC9EB9923");

            entity.ToTable("PROJECT");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(10)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(50)
                .HasColumnName("topic_name");

            entity.HasOne(d => d.Subject).WithMany(p => p.Projects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__subject__5629CD9C");

            entity.HasMany(d => d.Labels).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__label__5FB337D6"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__proje__5EBF139D"),
                    j =>
                    {
                        j.HasKey("ProjectId", "LabelId").HasName("PK__PROJECT___223D61BAFF76C671");
                        j.ToTable("PROJECT_LABEL");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                        j.IndexerProperty<int>("LabelId").HasColumnName("label_id");
                    });

            entity.HasMany(d => d.Mentors).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectMentor",
                    r => r.HasOne<Mentor>().WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_M__mento__778AC167"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_M__proje__76969D2E"),
                    j =>
                    {
                        j.HasKey("ProjectId", "MentorId").HasName("PK__PROJECT___9224B9F0FC6D694F");
                        j.ToTable("PROJECT_MENTOR");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                        j.IndexerProperty<int>("MentorId").HasColumnName("mentor_id");
                    });

            entity.HasMany(d => d.Sponsors).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectSponsor",
                    r => r.HasOne<Sponsor>().WithMany()
                        .HasForeignKey("SponsorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_S__spons__6FE99F9F"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_S__proje__6EF57B66"),
                    j =>
                    {
                        j.HasKey("ProjectId", "SponsorId").HasName("PK__PROJECT___E79AE35A7A565ABE");
                        j.ToTable("PROJECT_SPONSOR");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                        j.IndexerProperty<int>("SponsorId").HasColumnName("sponsor_id");
                    });
        });

        modelBuilder.Entity<Requirement>(entity =>
        {
            entity.HasKey(e => e.RequirementId).HasName("PK__REQUIREM__2A73C1AD5181B1DB");

            entity.ToTable("REQUIREMENT");

            entity.Property(e => e.RequirementId).HasColumnName("requirement_id");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("content");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RequestorId).HasColumnName("requestor_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Project).WithMany(p => p.Requirements)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REQUIREME__proje__6383C8BA");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__RESOURCE__4985FC733E56EC39");

            entity.ToTable("RESOURCE");

            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UploadBy).HasColumnName("upload_by");
            entity.Property(e => e.Url)
                .HasMaxLength(250)
                .HasColumnName("url");

            entity.HasOne(d => d.Project).WithMany(p => p.Resources)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RESOURCE__projec__6754599E");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLE__760965CC64BDA0EE");

            entity.ToTable("ROLE");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ROLE_PERM__permi__440B1D61"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ROLE_PERM__role___4316F928"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__ROLE_PER__C85A546393D53628");
                        j.ToTable("ROLE_PERMISSION");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<Sponsor>(entity =>
        {
            entity.HasKey(e => e.SponsorId).HasName("PK__SPONSOR__BE37D454EF7E9B7D");

            entity.ToTable("SPONSOR");

            entity.Property(e => e.SponsorId).HasColumnName("sponsor_id");
            entity.Property(e => e.InvestmentField)
                .HasMaxLength(250)
                .HasColumnName("investment_field");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sponsors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPONSOR__user_id__6C190EBB");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__STUDENT__2A33069AD32336FB");

            entity.ToTable("STUDENT");

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Major)
                .HasMaxLength(250)
                .HasColumnName("major");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.StudentCode)
                .HasMaxLength(10)
                .HasColumnName("student_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__user_id__04E4BC85");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__SUBJECT__5004F6608A050065");

            entity.ToTable("SUBJECT");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(50)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectName)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("subject_name");
        });

        modelBuilder.Entity<uniexetask.core.Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TASK__0492148D926959F6");

            entity.ToTable("TASK");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK__project_id__59FA5E80");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__B9BE370FA12F3B22");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USER__AB6E6164B010E360").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");

            entity.HasOne(d => d.Campus).WithMany(p => p.Users)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__campus_id__3F466844");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__role_id__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

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

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupInvite> GroupInvites { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<MeetingSchedule> MeetingSchedules { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<Nofitication> Nofitications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RegMemberForm> RegMemberForms { get; set; }

    public virtual DbSet<RegTopicForm> RegTopicForms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ScoreCriterion> ScoreCriteria { get; set; }

    public virtual DbSet<Sponsorship> Sponsorships { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<core.Models.Task> Tasks { get; set; }

    public virtual DbSet<TaskAssign> TaskAssigns { get; set; }

    public virtual DbSet<Timeline> Timelines { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Workshop> Workshops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__CAMPUS__01989FD11448CEC7");

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
            entity.HasKey(e => e.ChatGroupId).HasName("PK__CHAT_GRO__F18D35795A124D66");

            entity.ToTable("CHAT_GROUP");

            entity.Property(e => e.ChatGroupId).HasColumnName("chat_group_id");
            entity.Property(e => e.ChatGroupAvatar)
                .HasMaxLength(255)
                .HasColumnName("chat_group_avatar");
            entity.Property(e => e.ChatGroupName)
                .HasMaxLength(50)
                .HasColumnName("chat_group_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.LatestActivity)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("latest_activity");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChatGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__creat__5629CD9C");

            entity.HasOne(d => d.Owner).WithMany(p => p.ChatGroupOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__owner__571DF1D5");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__CHAT_MES__0BBF6EE65045B724");

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
                .HasConstraintName("FK__CHAT_MESS__chat___5AEE82B9");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_MESS__user___5BE2A6F2");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.CriteriaId).HasName("PK__CRITERIA__401F949D9D36CEB2");

            entity.ToTable("CRITERIA");

            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.CriteriaName)
                .HasMaxLength(250)
                .HasColumnName("criteria_name");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.MilestoneName)
                .HasMaxLength(100)
                .HasColumnName("milestone_name");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__DOCUMENT__9666E8ACC0B8A3E2");

            entity.ToTable("DOCUMENT");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.IsFinancialReport).HasColumnName("is_financial_report");
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

            entity.HasOne(d => d.Project).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DOCUMENT__projec__7F2BE32F");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__FEATURE__7906CBD7C57AB6C6");

            entity.ToTable("FEATURE");

            entity.Property(e => e.FeatureId).HasColumnName("feature_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GROUP__D57795A0E9388722");

            entity.ToTable("GROUP");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupName)
                .HasMaxLength(250)
                .HasColumnName("group_name");
            entity.Property(e => e.HasMentor).HasColumnName("hasMentor");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Subject).WithMany(p => p.Groups)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP__subject_i__68487DD7");

            entity.HasMany(d => d.Mentors).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorGroup",
                    r => r.HasOne<Mentor>().WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__mento__06CD04F7"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__group__05D8E0BE"),
                    j =>
                    {
                        j.HasKey("GroupId", "MentorId").HasName("PK__MENTOR_G__FB2AB24FAE232F36");
                        j.ToTable("MENTOR_GROUP");
                        j.IndexerProperty<int>("GroupId").HasColumnName("group_id");
                        j.IndexerProperty<int>("MentorId").HasColumnName("mentor_id");
                    });
        });

        modelBuilder.Entity<GroupInvite>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.NotificationId }).HasName("PK__GROUP_IN__3B720DE2EA61C176");

            entity.ToTable("GROUP_INVITE");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.InviteeId).HasColumnName("invitee_id");
            entity.Property(e => e.InviterId).HasColumnName("inviter_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__group__1BC821DD");

            entity.HasOne(d => d.Notification).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__notif__1CBC4616");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.StudentId }).HasName("PK__GROUP_ME__67D4A5C96C73F217");

            entity.ToTable("GROUP_MEMBER");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__group__10566F31");

            entity.HasOne(d => d.Student).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__stude__114A936A");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__LABEL__E44FFA583D5970DB");

            entity.ToTable("LABEL");

            entity.Property(e => e.LabelId).HasColumnName("label_id");
            entity.Property(e => e.LabelName)
                .HasMaxLength(50)
                .HasColumnName("label_name");
        });

        modelBuilder.Entity<MeetingSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__MEETING___C46A8A6FD93D7947");

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
                .HasConstraintName("FK__MEETING_S__group__0B91BA14");

            entity.HasOne(d => d.Mentor).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__mento__0C85DE4D");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("PK__MENTOR__E5D27EF3CFFCBEE4");

            entity.ToTable("MENTOR");

            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Specialty)
                .HasMaxLength(250)
                .HasColumnName("specialty");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MENTOR__user_id__4BAC3F29");
        });

        modelBuilder.Entity<Nofitication>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__NOFITICA__E059842F090A5186");

            entity.ToTable("NOFITICATION");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message)
                .HasMaxLength(250)
                .HasColumnName("message");
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
                .HasConstraintName("FK__NOFITICAT__recei__17036CC0");

            entity.HasOne(d => d.Sender).WithMany(p => p.NofiticationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOFITICAT__sende__160F4887");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__PERMISSI__E5331AFA0A2D6E8D");

            entity.ToTable("PERMISSION");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FeatureId).HasColumnName("feature_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Feature).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PERMISSIO__descr__3F466844");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__PROJECT__BC799E1FCB6B6587");

            entity.ToTable("PROJECT");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TopicId).HasColumnName("topic_id");

            entity.HasOne(d => d.Group).WithMany(p => p.Projects)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__group_i__6E01572D");

            entity.HasOne(d => d.Subject).WithMany(p => p.Projects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__subject__6C190EBB");

            entity.HasOne(d => d.Topic).WithMany(p => p.Projects)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__topic_i__6D0D32F4");

            entity.HasMany(d => d.Labels).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__label__7B5B524B"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PROJECT_L__proje__7A672E12"),
                    j =>
                    {
                        j.HasKey("ProjectId", "LabelId").HasName("PK__PROJECT___223D61BA6BEAB92F");
                        j.ToTable("PROJECT_LABEL");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                        j.IndexerProperty<int>("LabelId").HasColumnName("label_id");
                    });
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__REFRESH___CB3C9E173A53EABD");

            entity.ToTable("REFRESH_TOKEN");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.Expires)
                .HasColumnType("datetime")
                .HasColumnName("expires");
            entity.Property(e => e.Revoked)
                .HasColumnType("datetime")
                .HasColumnName("revoked");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REFRESH_T__user___2B0A656D");
        });

        modelBuilder.Entity<RegMemberForm>(entity =>
        {
            entity.HasKey(e => e.RegMemberId).HasName("PK__REG_MEMB__8BAC7116F1191D9B");

            entity.ToTable("REG_MEMBER_FORM");

            entity.Property(e => e.RegMemberId).HasColumnName("reg_member_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Group).WithMany(p => p.RegMemberForms)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REG_MEMBE__group__30C33EC3");
        });

        modelBuilder.Entity<RegTopicForm>(entity =>
        {
            entity.HasKey(e => e.RegTopicId).HasName("PK__REG_TOPI__E20C4927449A6E79");

            entity.ToTable("REG_TOPIC_FORM");

            entity.Property(e => e.RegTopicId).HasColumnName("reg_topic_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(50)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(100)
                .HasColumnName("topic_name");

            entity.HasOne(d => d.Group).WithMany(p => p.RegTopicForms)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__REG_TOPIC__group__2DE6D218");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLE__760965CC30B19E8B");

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
                        .HasConstraintName("FK__ROLE_PERM__permi__48CFD27E"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ROLE_PERM__role___47DBAE45"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__ROLE_PER__C85A546350FF9448");
                        j.ToTable("ROLE_PERMISSION");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<ScoreCriterion>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("PK__SCORE_CR__8CA190505161AC75");

            entity.ToTable("SCORE_CRITERIA");

            entity.Property(e => e.ScoreId).HasColumnName("score_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(250)
                .HasColumnName("comment");
            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RatingStatus).HasColumnName("rating_status");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.ScoredBy)
                .HasMaxLength(100)
                .HasColumnName("scored_by");

            entity.HasOne(d => d.Criteria).WithMany(p => p.ScoreCriteria)
                .HasForeignKey(d => d.CriteriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SCORE_CRI__crite__25518C17");

            entity.HasOne(d => d.Project).WithMany(p => p.ScoreCriteria)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SCORE_CRI__proje__2645B050");
        });

        modelBuilder.Entity<Sponsorship>(entity =>
        {
            entity.HasKey(e => e.SponsorshipDetailId).HasName("PK__SPONSORS__A62594A8FF007161");

            entity.ToTable("SPONSORSHIP");

            entity.Property(e => e.SponsorshipDetailId).HasColumnName("sponsorship_detail_id");
            entity.Property(e => e.AmountMoney)
                .HasColumnType("money")
                .HasColumnName("amount_money");
            entity.Property(e => e.IsInvesting).HasColumnName("isInvesting");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Sponsorships)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPONSORSH__proje__02084FDA");

            entity.HasOne(d => d.User).WithMany(p => p.Sponsorships)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPONSORSH__user___02FC7413");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__STUDENT__2A33069AA368D65A");

            entity.ToTable("STUDENT");

            entity.HasIndex(e => e.StudentCode, "UQ__STUDENT__6DF33C45DC8A85EF").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.IsEligible).HasColumnName("is_eligible");
            entity.Property(e => e.LecturerId).HasColumnName("lecturer_id");
            entity.Property(e => e.Major)
                .HasMaxLength(250)
                .HasColumnName("major");
            entity.Property(e => e.StudentCode)
                .HasMaxLength(10)
                .HasColumnName("student_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Students)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__lecture__5070F446");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__user_id__4F7CD00D");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__SUBJECT__5004F6609D001783");

            entity.ToTable("SUBJECT");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(50)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(50)
                .HasColumnName("subject_name");
        });

        modelBuilder.Entity<core.Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TASK__0492148DD93BA97B");

            entity.ToTable("TASK");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TaskName)
                .HasMaxLength(50)
                .HasColumnName("task_name");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK__project_id__71D1E811");
        });

        modelBuilder.Entity<TaskAssign>(entity =>
        {
            entity.HasKey(e => e.TaskAssignId).HasName("PK__TASK_ASS__8736D568E3878538");

            entity.ToTable("TASK_ASSIGN");

            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.AsignedDate)
                .HasColumnType("datetime")
                .HasColumnName("asigned_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Student).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__stude__75A278F5");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__task___74AE54BC");
        });

        modelBuilder.Entity<Timeline>(entity =>
        {
            entity.HasKey(e => e.TimelineId).HasName("PK__TIMELINE__DC6F55B007274CD6");

            entity.ToTable("TIMELINE");

            entity.Property(e => e.TimelineId).HasColumnName("timeline_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.TimelineName)
                .HasMaxLength(100)
                .HasColumnName("timeline_name");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__TOPIC__D5DAA3E94A9FB5C0");

            entity.ToTable("TOPIC");

            entity.Property(e => e.TopicId).HasColumnName("topic_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(50)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(100)
                .HasColumnName("topic_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__B9BE370FE625C41F");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USER__AB6E61644F3F600A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
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
                .HasConstraintName("FK__USER__campus_id__440B1D61");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__role_id__44FF419A");

            entity.HasMany(d => d.ChatGroups).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserChatGroup",
                    r => r.HasOne<ChatGroup>().WithMany()
                        .HasForeignKey("ChatGroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__USER_CHAT__chat___5FB337D6"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__USER_CHAT__user___5EBF139D"),
                    j =>
                    {
                        j.HasKey("UserId", "ChatGroupId").HasName("PK__USER_CHA__36A6E458A709C0A1");
                        j.ToTable("USER_CHAT_GROUP");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("ChatGroupId").HasColumnName("chat_group_id");
                    });
        });

        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.WorkshopId).HasName("PK__WORKSHOP__EA6B0559B82F9918");

            entity.ToTable("WORKSHOP");

            entity.Property(e => e.WorkshopId).HasColumnName("workshop_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.RegUrl).HasColumnName("reg_url");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using uniexetask.core.Models;
using Task = uniexetask.core.Models.Task;

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

    public virtual DbSet<ConfigSystem> ConfigSystems { get; set; }

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupInvite> GroupInvites { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<MeetingSchedule> MeetingSchedules { get; set; }

    public virtual DbSet<MemberScore> MemberScores { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<Milestone> Milestones { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectProgress> ProjectProgresses { get; set; }

    public virtual DbSet<ProjectScore> ProjectScores { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RegMemberForm> RegMemberForms { get; set; }

    public virtual DbSet<RegTopicForm> RegTopicForms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskAssign> TaskAssigns { get; set; }

    public virtual DbSet<TaskDetail> TaskDetails { get; set; }

    public virtual DbSet<TaskProgress> TaskProgresses { get; set; }

    public virtual DbSet<Timeline> Timelines { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<TopicForMentor> TopicForMentors { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Workshop> Workshops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__CAMPUS__01989FD13E4FAB4F");

            entity.ToTable("CAMPUS");

            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.CampusCode)
                .HasMaxLength(50)
                .HasColumnName("campus_code");
            entity.Property(e => e.CampusName)
                .HasMaxLength(100)
                .HasColumnName("campus_name");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
        });

        modelBuilder.Entity<ChatGroup>(entity =>
        {
            entity.HasKey(e => e.ChatGroupId).HasName("PK__CHAT_GRO__F18D3579CE1A3630");

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
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
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
                .HasConstraintName("FK__CHAT_GROU__creat__60A75C0F");

            entity.HasOne(d => d.Group).WithMany(p => p.ChatGroups)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__CHAT_GROU__group__5FB337D6");

            entity.HasOne(d => d.Owner).WithMany(p => p.ChatGroupOwners)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_GROU__owner__619B8048");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__CHAT_MES__0BBF6EE68D0F2833");

            entity.ToTable("CHAT_MESSAGE");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.ChatGroupId).HasColumnName("chat_group_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.IsFile).HasColumnName("isFile");
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
                .HasConstraintName("FK__CHAT_MESS__chat___6754599E");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHAT_MESS__user___68487DD7");
        });

        modelBuilder.Entity<ConfigSystem>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__CONFIG_S__4AD1BFF1443DF6F3");

            entity.ToTable("CONFIG_SYSTEM");

            entity.Property(e => e.ConfigId).HasColumnName("config_id");
            entity.Property(e => e.ConfigName)
                .HasMaxLength(250)
                .HasColumnName("config_name");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.CriteriaId).HasName("PK__CRITERIA__401F949DD4A630F7");

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
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Milestone).WithMany(p => p.Criteria)
                .HasForeignKey(d => d.MilestoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CRITERIA__milest__40F9A68C");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__DOCUMENT__9666E8AC1774B7F9");

            entity.ToTable("DOCUMENT");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modified_date");
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

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.DocumentModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK__DOCUMENT__modifi__17F790F9");

            entity.HasOne(d => d.Project).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DOCUMENT__projec__17036CC0");

            entity.HasOne(d => d.UploadByNavigation).WithMany(p => p.DocumentUploadByNavigations)
                .HasForeignKey(d => d.UploadBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DOCUMENT__upload__18EBB532");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__FEATURE__7906CBD7011AE6B9");

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
            entity.HasKey(e => e.GroupId).HasName("PK__GROUP__D57795A064F33A14");

            entity.ToTable("GROUP");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupName)
                .HasMaxLength(250)
                .HasColumnName("group_name");
            entity.Property(e => e.HasMentor).HasColumnName("hasMentor");
            entity.Property(e => e.IsCurrentPeriod)
                .HasDefaultValue(true)
                .HasColumnName("isCurrentPeriod");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Subject).WithMany(p => p.Groups)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP__subject_i__59063A47");

            entity.HasMany(d => d.Mentors).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorGroup",
                    r => r.HasOne<Mentor>().WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__mento__1DB06A4F"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MENTOR_GR__group__1CBC4616"),
                    j =>
                    {
                        j.HasKey("GroupId", "MentorId").HasName("PK__MENTOR_G__FB2AB24FE88878A5");
                        j.ToTable("MENTOR_GROUP");
                        j.IndexerProperty<int>("GroupId").HasColumnName("group_id");
                        j.IndexerProperty<int>("MentorId").HasColumnName("mentor_id");
                    });
        });

        modelBuilder.Entity<GroupInvite>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.NotificationId }).HasName("PK__GROUP_IN__3B720DE23BCA1E04");

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
                .HasConstraintName("FK__GROUP_INV__group__32AB8735");

            entity.HasOne(d => d.Notification).WithMany(p => p.GroupInvites)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_INV__notif__339FAB6E");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.StudentId }).HasName("PK__GROUP_ME__67D4A5C94E9A8F95");

            entity.ToTable("GROUP_MEMBER");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__group__2739D489");

            entity.HasOne(d => d.Student).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GROUP_MEM__stude__282DF8C2");
        });

        modelBuilder.Entity<MeetingSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__MEETING___C46A8A6F66E956F4");

            entity.ToTable("MEETING_SCHEDULE");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.Content)
                .HasMaxLength(250)
                .HasColumnName("content");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.MeetingDate)
                .HasColumnType("datetime")
                .HasColumnName("meeting_date");
            entity.Property(e => e.MeetingScheduleName)
                .HasMaxLength(250)
                .HasColumnName("meeting_schedule_name");
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.Url)
                .HasMaxLength(250)
                .HasColumnName("url");

            entity.HasOne(d => d.Group).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__group__22751F6C");

            entity.HasOne(d => d.Mentor).WithMany(p => p.MeetingSchedules)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEETING_S__mento__236943A5");
        });

        modelBuilder.Entity<MemberScore>(entity =>
        {
            entity.HasKey(e => e.MemberScoreId).HasName("PK__MEMBER_S__A2363F57F6F1C726");

            entity.ToTable("MEMBER_SCORE");

            entity.Property(e => e.MemberScoreId).HasColumnName("member_score_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(250)
                .HasColumnName("comment");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.ScoredBy).HasColumnName("scored_by");
            entity.Property(e => e.ScoringDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("scoring_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Milestone).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.MilestoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__miles__4D5F7D71");

            entity.HasOne(d => d.Project).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__proje__4C6B5938");

            entity.HasOne(d => d.ScoredByNavigation).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.ScoredBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__score__4E53A1AA");

            entity.HasOne(d => d.Student).WithMany(p => p.MemberScores)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MEMBER_SC__stude__4B7734FF");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("PK__MENTOR__E5D27EF3075ECF26");

            entity.ToTable("MENTOR");

            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.Specialty)
                .HasMaxLength(250)
                .HasColumnName("specialty");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MENTOR__user_id__4D94879B");
        });

        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.HasKey(e => e.MilestoneId).HasName("PK__MILESTON__67592EB750D2E3E0");

            entity.ToTable("MILESTONE");

            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MilestoneName)
                .HasMaxLength(100)
                .HasColumnName("milestone_name");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Subject).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MILESTONE__subje__3C34F16F");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__NOTIFICA__E059842F3D66FF8E");

            entity.ToTable("NOTIFICATION");

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

            entity.HasOne(d => d.Receiver).WithMany(p => p.NotificationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOTIFICAT__recei__2DE6D218");

            entity.HasOne(d => d.Sender).WithMany(p => p.NotificationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOTIFICAT__sende__2CF2ADDF");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__PERMISSI__E5331AFA29297CF3");

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
                .HasConstraintName("FK__PERMISSIO__descr__412EB0B6");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__PROJECT__BC799E1FAFB1482A");

            entity.ToTable("PROJECT");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsCurrentPeriod)
                .HasDefaultValue(true)
                .HasColumnName("isCurrentPeriod");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
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
                .HasConstraintName("FK__PROJECT__group_i__787EE5A0");

            entity.HasOne(d => d.Subject).WithMany(p => p.Projects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__subject__76969D2E");

            entity.HasOne(d => d.Topic).WithMany(p => p.Projects)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__topic_i__778AC167");
        });

        modelBuilder.Entity<ProjectProgress>(entity =>
        {
            entity.HasKey(e => e.ProjectProgressId).HasName("PK__PROJECT___A8484F6AE28D0831");

            entity.ToTable("PROJECT_PROGRESS");

            entity.Property(e => e.ProjectProgressId).HasColumnName("project_progress_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Note)
                .HasMaxLength(250)
                .HasColumnName("note");
            entity.Property(e => e.ProgressPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectProgresses)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_P__proje__7E37BEF6");
        });

        modelBuilder.Entity<ProjectScore>(entity =>
        {
            entity.HasKey(e => e.ProjectScoreId).HasName("PK__PROJECT___636E77820CFAA381");

            entity.ToTable("PROJECT_SCORE");

            entity.Property(e => e.ProjectScoreId).HasColumnName("project_score_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(250)
                .HasColumnName("comment");
            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.ScoredBy).HasColumnName("scored_by");
            entity.Property(e => e.ScoringDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("scoring_date");

            entity.HasOne(d => d.Criteria).WithMany(p => p.ProjectScores)
                .HasForeignKey(d => d.CriteriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_S__crite__44CA3770");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectScores)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_S__proje__45BE5BA9");

            entity.HasOne(d => d.ScoredByNavigation).WithMany(p => p.ProjectScores)
                .HasForeignKey(d => d.ScoredBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT_S__score__46B27FE2");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__REFRESH___CB3C9E17F4902E58");

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
                .HasConstraintName("FK__REFRESH_T__user___540C7B00");
        });

        modelBuilder.Entity<RegMemberForm>(entity =>
        {
            entity.HasKey(e => e.RegMemberId).HasName("PK__REG_MEMB__8BAC7116AEC4771A");

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
                .HasConstraintName("FK__REG_MEMBE__group__59C55456");
        });

        modelBuilder.Entity<RegTopicForm>(entity =>
        {
            entity.HasKey(e => e.RegTopicId).HasName("PK__REG_TOPI__E20C492720F44955");

            entity.ToTable("REG_TOPIC_FORM");

            entity.Property(e => e.RegTopicId).HasColumnName("reg_topic_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.RejectionReason).HasColumnName("rejection_reason");
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
                .HasConstraintName("FK__REG_TOPIC__group__56E8E7AB");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLE__760965CCB0261B00");

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
                        .HasConstraintName("FK__ROLE_PERM__permi__4AB81AF0"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ROLE_PERM__role___49C3F6B7"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__ROLE_PER__C85A546390B8826F");
                        j.ToTable("ROLE_PERMISSION");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__STUDENT__2A33069A9CC2369F");

            entity.ToTable("STUDENT");

            entity.HasIndex(e => e.StudentCode, "UQ__STUDENT__6DF33C455ED47522").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.IsCurrentPeriod)
                .HasDefaultValue(true)
                .HasColumnName("isCurrentPeriod");
            entity.Property(e => e.LecturerId).HasColumnName("lecturer_id");
            entity.Property(e => e.Major)
                .HasMaxLength(250)
                .HasColumnName("major");
            entity.Property(e => e.StudentCode)
                .HasMaxLength(10)
                .HasColumnName("student_code");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Students)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__lecture__534D60F1");

            entity.HasOne(d => d.Subject).WithMany(p => p.Students)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__subject__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__STUDENT__user_id__52593CB8");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__SUBJECT__5004F6602D3241E1");

            entity.ToTable("SUBJECT");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(50)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(50)
                .HasColumnName("subject_name");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TASK__0492148DFD6E18D6");

            entity.ToTable("TASK");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
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
                .HasConstraintName("FK__TASK__project_id__02084FDA");
        });

        modelBuilder.Entity<TaskAssign>(entity =>
        {
            entity.HasKey(e => e.TaskAssignId).HasName("PK__TASK_ASS__8736D5688B7088F7");

            entity.ToTable("TASK_ASSIGN");

            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.AssignedDate)
                .HasColumnType("datetime")
                .HasColumnName("assigned_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Student).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__stude__0C85DE4D");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssigns)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_ASSI__task___0B91BA14");
        });

        modelBuilder.Entity<TaskDetail>(entity =>
        {
            entity.HasKey(e => e.TaskDetailId).HasName("PK__TASK_DET__2B12CFB9C9B84BBC");

            entity.ToTable("TASK_DETAIL");

            entity.Property(e => e.TaskDetailId).HasColumnName("task_detail_id");
            entity.Property(e => e.IsCompleted).HasColumnName("isCompleted");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.ProgressPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.TaskDetailName)
                .HasMaxLength(250)
                .HasColumnName("task_detail_name");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskDetails)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_DETA__task___1332DBDC");
        });

        modelBuilder.Entity<TaskProgress>(entity =>
        {
            entity.HasKey(e => e.TaskProgressId).HasName("PK__TASK_PRO__26A7535ED0EE0D28");

            entity.ToTable("TASK_PROGRESS");

            entity.Property(e => e.TaskProgressId).HasColumnName("task_progress_id");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Note)
                .HasMaxLength(250)
                .HasColumnName("note");
            entity.Property(e => e.ProgressPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskProgresses)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK_PROG__task___08B54D69");
        });

        modelBuilder.Entity<Timeline>(entity =>
        {
            entity.HasKey(e => e.TimelineId).HasName("PK__TIMELINE__DC6F55B08BED7877");

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
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TimelineName)
                .HasMaxLength(100)
                .HasColumnName("timeline_name");

            entity.HasOne(d => d.Subject).WithMany(p => p.Timelines)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TIMELINE__subjec__6EF57B66");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__TOPIC__D5DAA3E9087A25CD");

            entity.ToTable("TOPIC");

            entity.Property(e => e.TopicId).HasColumnName("topic_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(50)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(100)
                .HasColumnName("topic_name");
        });

        modelBuilder.Entity<TopicForMentor>(entity =>
        {
            entity.HasKey(e => e.TopicForMentorId).HasName("PK__TOPIC_FO__7C87B8D52A2B91E9");

            entity.ToTable("TOPIC_FOR_MENTOR");

            entity.Property(e => e.TopicForMentorId).HasColumnName("topic_for_mentor_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.IsRegistered).HasColumnName("isRegistered");
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.TopicCode)
                .HasMaxLength(50)
                .HasColumnName("topic_code");
            entity.Property(e => e.TopicName)
                .HasMaxLength(100)
                .HasColumnName("topic_name");

            entity.HasOne(d => d.Mentor).WithMany(p => p.TopicForMentors)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TOPIC_FOR__mento__5D95E53A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__B9BE370F7B9ABD6E");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USER__AB6E6164A7D1D350").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Campus).WithMany(p => p.Users)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__campus_id__44FF419A");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER__role_id__45F365D3");

            entity.HasMany(d => d.ChatGroups).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserChatGroup",
                    r => r.HasOne<ChatGroup>().WithMany()
                        .HasForeignKey("ChatGroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__USER_CHAT__chat___6C190EBB"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__USER_CHAT__user___6B24EA82"),
                    j =>
                    {
                        j.HasKey("UserId", "ChatGroupId").HasName("PK__USER_CHA__36A6E458AC2AE2A4");
                        j.ToTable("USER_CHAT_GROUP");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("ChatGroupId").HasColumnName("chat_group_id");
                    });
        });

        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.WorkshopId).HasName("PK__WORKSHOP__EA6B0559D4EF0953");

            entity.ToTable("WORKSHOP");

            entity.Property(e => e.WorkshopId).HasColumnName("workshop_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
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

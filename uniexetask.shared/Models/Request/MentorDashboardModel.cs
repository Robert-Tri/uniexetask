using System;
using System.Collections.Generic;

namespace uniexetask.shared.Models.Request
{
    public class MentorDashboardModel
    {
        public List<GroupDashboardModel> Groups { get; set; } = new List<GroupDashboardModel>();
        public List<ProjectDashboardModel> Projects { get; set; } = new List<ProjectDashboardModel>();
        public List<RegTopicDashboardModel> RegTopics { get; set; } = new List<RegTopicDashboardModel>();
    }

    public class GroupDashboardModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public int SubjectId { get; set; }
        public bool HasMentor { get; set; }
        public bool IsCurrentPeriod { get; set; }
        public string Status { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public List<GroupMemberModel> GroupMembers { get; set; }
        public List<MeetingScheduleDashboardModel> MeetingSchedules { get; set; }

    }

    public class ProjectDashboardModel
    {
        public int ProjectId { get; set; }
        public int TopicId { get; set; }
        public string TopicName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ProgressPercentage { get; set; }
        public int SubjectId { get; set; }
        public bool IsCurrentPeriod { get; set; }
        public string Status { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public List<TaskDashboardModel> TaskDashboards { get; set; }

    }

    public class RegTopicDashboardModel
    {
        public int RegTopicId { get; set; }
        public string TopicCode { get; set; } = null!;
        public string TopicName { get; set; } = null!;
        public bool Status { get; set; }
    }
    public class TaskDashboardModel
    {
        public int TaskId { get; set; }
        public string Status { get; set; }
        public decimal ProgressPercentage { get; set; }
    }

    public class MeetingScheduleDashboardModel
    {
        public int ScheduleId { get; set; }
        public string GroupName { get; set; } = null!;
        public string MeetingScheduleName { get; set; } = null!;

        public string Location { get; set; } = null!;

        public DateTime MeetingDate { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; } = null!;
        public string Content { get; set; } = null!;

        public string? Url { get; set; }

        public bool IsDeleted { get; set; }
    }

}

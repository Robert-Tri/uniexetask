using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using uniexetask.core.Interfaces;
using uniexetask.services.Interfaces;
using uniexetask.core.Models.Enums;

namespace uniexetask.api.BackgroundServices
{
    public class TimelineBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TimelineBackgroundService(IServiceScopeFactory scopeFactory) 
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                CheckTimelines();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
        private async Task CheckTimelines()
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var timeLineService = scope.ServiceProvider.GetRequiredService<ITimeLineService>();
                var groupService = scope.ServiceProvider.GetRequiredService<IGroupService>();
                var timelines = await timeLineService.GetTimeLines();
                foreach (var timeline in timelines) 
                {
                    if (timeline.EndDate.Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))
                    {
                        switch ((TimelineType)timeline.TimelineId)
                        {
                            case TimelineType.FinalizeGroupEXE101:
                                await groupService.UpdateAndAssignStudentsToGroups(SubjectType.EXE101);
                                break;
                            case TimelineType.FinalizeGroupEXE201:
                                await groupService.UpdateAndAssignStudentsToGroups(SubjectType.EXE201);
                                break;

                            case TimelineType.FinalizeMentorEXE101:
                                await groupService.AddMentorToGroupAutomatically();
                                break;
                            case TimelineType.FinalizeMentorEXE201:
                                await groupService.AddMentorToGroupAutomatically();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}

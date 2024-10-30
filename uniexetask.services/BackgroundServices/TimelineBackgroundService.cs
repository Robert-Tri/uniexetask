using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using uniexetask.core.Interfaces;
using uniexetask.services.Interfaces;
using uniexetask.core.Enums;

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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                CheckTimelines();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
        private async Task CheckTimelines()
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var timeLineService = scope.ServiceProvider.GetRequiredService<ITimeLineRepository>();
                var groupService = scope.ServiceProvider.GetRequiredService<IGroupService>();
                var timelines = await timeLineService.GetAsync();
                foreach (var timeline in timelines) 
                {
                    if(timeline.EndDate == DateTime.Today)
                    {
                        switch ((TimelineType)timeline.TimelineId)
                        {
                            case TimelineType.AssignMentor:
                                await groupService.AddMentorToGroupAutomatically();
                                break;
                            case TimelineType.SelectTopic:
                                // Logic for task 2
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

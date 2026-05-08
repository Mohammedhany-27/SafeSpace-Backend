using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using safespace.Data;
using safespace.Model;
namespace safespace.Services
{
    public class MessageService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MessageService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanMessages();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CleanMessages()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cutoff = Date.GetEgyptTime().AddHours(-24);

            var oldMessages = context.Message
                .Where(m => m.sendAt < cutoff && m.IsSaved == false);
                
            context.Message.RemoveRange(oldMessages);

            await context.SaveChangesAsync();
        }
    }
}

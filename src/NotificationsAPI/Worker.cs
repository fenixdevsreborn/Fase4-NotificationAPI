namespace NotificationsAPI
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationsAPI Worker iniciado.");

            try
            {
                // Mantém o processo vivo enquanto o host estiver rodando
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Shutdown gracioso
            }
            finally
            {
                _logger.LogInformation("NotificationsAPI Worker finalizado.");
            }
        }
    }
}

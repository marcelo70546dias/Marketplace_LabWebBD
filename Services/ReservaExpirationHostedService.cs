namespace Marketplace_LabWebBD.Services
{
    public class ReservaExpirationHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservaExpirationHostedService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

        public ReservaExpirationHostedService(
            IServiceProvider serviceProvider,
            ILogger<ReservaExpirationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReservaExpirationHostedService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Verificando reservas expiradas...");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reservaService = scope.ServiceProvider.GetRequiredService<IReservaService>();
                        await reservaService.ExpireReservasAsync();
                    }

                    _logger.LogInformation($"Próxima verificação em {_checkInterval.TotalMinutes} minutos.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao verificar reservas expiradas.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("ReservaExpirationHostedService parado.");
        }
    }
}

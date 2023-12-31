using KodeCrypto.Application.Common.Interfaces;

namespace KodeCrypto.Infrastructure.Jobs
{
    public class JobExecuter
    {
        private readonly IEnumerable<ISyncService> _syncServices;

        public JobExecuter(IEnumerable<ISyncService> syncServices)
        {
            _syncServices = syncServices;
        }

        public async Task RunSync()
		{
            var syncTasks = _syncServices.Select(syncService => syncService.SyncAll());
            await Task.WhenAll(syncTasks);
        }
    }
}


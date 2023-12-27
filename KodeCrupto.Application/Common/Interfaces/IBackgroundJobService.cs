public interface IBackgroundJobService
{
    public void SyncProtofolioUserDataJob();
}

public class BackgroundJobService : IBackgroundJobService
{
    public void SyncProtofolioUserDataJob()
    {
        Console.WriteLine("User data synced successfully");
    }
}

namespace Dashboard.Models.Utility;

interface IDatabaseWorker : IDisposable
{
    void OpenDatabase();
    void CloseDatabase();
}

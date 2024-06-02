namespace Dashboard.Models.Service;

interface IDatabaseWorker : IDisposable
{
    void OpenDatabase();
    void CloseDatabase();
}

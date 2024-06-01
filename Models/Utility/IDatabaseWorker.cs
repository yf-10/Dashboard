using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Text;

namespace Dashboard.Models.Utility;

interface IDatabaseWorker : IDisposable
{
    void OpenDatabase();
    void CloseDatabase();
}

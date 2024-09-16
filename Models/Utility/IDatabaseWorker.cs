using System.Data;
using System.Data.Common;

namespace Dashboard.Models.Utility;

interface IDatabaseWorker : IDisposable {
    class Parameter(string name, dynamic value, DbType type) {
        public string Name { get; private set; } = name;
        public dynamic Value { get; private set; } = value;
        public DbType Type { get; private set; } = type;
    }

    void OpenDatabase();
    void CloseDatabase();

    // --------------------------------------------------------------------
    // Execute SQL : Return DataReader (SELECT)
    // --------------------------------------------------------------------
    DbDataReader ExecuteSqlGetData(string sql, List<Parameter> prms);
    DbDataReader ExecuteSqlGetData(string sql);

    // --------------------------------------------------------------------
    // Execute SQL : Return Affected Data Count (INSERT/UPDATE/DELETE)
    // --------------------------------------------------------------------
    int ExecuteSql(string sql, List<Parameter> prms);
    int ExecuteSql(string sql);
}

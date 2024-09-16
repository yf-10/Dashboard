using System.Data.Common;

using Npgsql;

namespace Dashboard.Models.Utility;

class PostgresqlWorker : IDatabaseWorker {
    private readonly Logger? logger = Logger.GetInstance();
    private bool disposed = false;
    private readonly NpgsqlConnection? connection = null;

    // ------------------------------------------------
    // Constructor
    // ------------------------------------------------
    public PostgresqlWorker() {
        const string host = "localhost";
        const int port = 5432;
        const string user = "postgres";
        const string pass = "postgres";
        const string name = "postgres";
        connection ??= new(GenerateConnectionString(host, port, user, pass, name));
        OpenDatabase();
    }

    // ------------------------------------------------
    // Destructor/Dispose
    // ------------------------------------------------
    ~PostgresqlWorker() {
        Dispose(false);
    }
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) {
        if (disposed == false) {
            if (disposing) CloseDatabase();
            disposed = true;
        }
    }

    // ------------------------------------------------
    // Generate Connection String
    // ------------------------------------------------
    private static string GenerateConnectionString(string host, int port, string user, string pass, string name) {
        return $"Server={host};Port={port};User Id={user};Password={pass};Database={name};";
    }

    // ------------------------------------------------
    // Open Database
    // ------------------------------------------------
    public void OpenDatabase() {
        connection?.Open();
    }

    // ------------------------------------------------
    // Close Database
    // ------------------------------------------------
    public void CloseDatabase() {
        connection?.Close();
        connection?.Dispose();
    }

    // ------------------------------------------------
    // Execute SQL : Return DataReader
    // ------------------------------------------------
    public DbDataReader ExecuteSqlGetData(string sql, List<IDatabaseWorker.Parameter>? prms) {
        using NpgsqlCommand command = new(sql, connection);
        logger?.Debug("[Sql] " + Environment.NewLine + sql);
        if (prms is not null || prms?.Count > 0) {
            prms.ForEach(prm => {
                command.Parameters.Add(new NpgsqlParameter(prm.Name, prm.Type));
                command.Parameters[prm.Name].Value = prm.Value;
                logger?.Debug("[Prm] " + prm.Name + ":" + prm.Value + "(" + prm.Type.ToString() + ")");
            });
        }
        return command.ExecuteReader();
    }
    public DbDataReader ExecuteSqlGetData(string sql) {
        return ExecuteSqlGetData(sql, null);
    }

    // ------------------------------------------------
    // Execute SQL : NonQuery
    // ------------------------------------------------
    public int ExecuteSql(string sql, List<IDatabaseWorker.Parameter>? prms) {
        using NpgsqlCommand command = new(sql, connection);
        logger?.Debug("[Sql] " + Environment.NewLine + sql);
        if (prms is not null || prms?.Count > 0) {
            prms.ForEach(prm => {
                command.Parameters.Add(new NpgsqlParameter(prm.Name, prm.Type));
                command.Parameters[prm.Name].Value = prm.Value;
                logger?.Debug("[Prm] " + prm.Name + ":" + prm.Value + "(" + prm.Type.ToString() + ")");
            });
        }
        return command.ExecuteNonQuery();
    }
    public int ExecuteSql(string sql) {
        return ExecuteSql(sql, null);
    }

}

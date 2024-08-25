using System.Data;
using Npgsql;

namespace Dashboard.Models.Utility;

class PostgresqlWorker : IDatabaseWorker
{
    // ------------------------------------------------
    // Field
    // ------------------------------------------------
    private readonly Logger? _logger = Logger.GetInstance();
    private bool _disposed = false;
    private readonly NpgsqlConnection? _connection = null;

    // ------------------------------------------------
    // Constructor
    // ------------------------------------------------
    public PostgresqlWorker() {
        string host = "localhost";
        int port = 5432;
        string user = "postgres";
        string pass = "postgres";
        string name = "postgres";
        _connection ??= new(GenerateConnectionString(host, port, user, pass, name));
        OpenDatabase();
    }

    // ------------------------------------------------
    // Destructor
    // ------------------------------------------------
    ~PostgresqlWorker() {
        Dispose(false);
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
        _connection?.Open();
    }

    // ------------------------------------------------
    // Close Database
    // ------------------------------------------------
    public void CloseDatabase() {
        _connection?.Close();
        _connection?.Dispose();
    }

    // ------------------------------------------------
    // Dispose
    // ------------------------------------------------
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (disposing) {
                CloseDatabase();
            }
            _disposed = true;
        }
    }

    // ------------------------------------------------
    // Add Parameter
    // ------------------------------------------------
    public static void AddParameter(ref Dictionary<string, dynamic> prms, string prmName, DbType prmType, dynamic prmValue) {
        prms.Add(prmName, new KeyValuePair<DbType, dynamic>(prmType, prmValue));
        return;
    }

    // ------------------------------------------------
    // Execute SQL : Return DataReader
    // ------------------------------------------------
    public NpgsqlDataReader ExecuteSqlGetData(string sql, Dictionary<string, dynamic>? prms) {
        using NpgsqlCommand command = new(sql, _connection);
        _logger?.Debug("[Sql] " + Environment.NewLine + sql);
        if (prms is not null || prms?.Count > 0) {
            foreach (KeyValuePair<string, dynamic> pair in prms) {
                string prmName = pair.Key;
                KeyValuePair<DbType, dynamic> value = pair.Value;
                DbType prmType = value.Key;
                dynamic prmValue = value.Value;
                command.Parameters.Add(new NpgsqlParameter(prmName, prmType));
                command.Parameters[prmName].Value = prmValue;
                _logger?.Debug("[Prm] " + prmName + ":" + prmValue + "(" + prmType.ToString() + ")");
            }
        }
        return command.ExecuteReader();
    }
    public NpgsqlDataReader ExecuteSqlGetData(string sql) {
        return ExecuteSqlGetData(sql, null);
    }

    // ------------------------------------------------
    // Execute SQL : NonQuery
    // ------------------------------------------------
    public int ExecuteSql(string sql, Dictionary<string, dynamic>? prms) {
        using NpgsqlCommand command = new(sql, _connection);
        _logger?.Debug("[Sql] " + Environment.NewLine + sql);
        if (prms is not null || prms?.Count > 0) {
            foreach (KeyValuePair<string, dynamic> pair in prms) {
                string prmName = pair.Key;
                KeyValuePair<DbType, dynamic> value = pair.Value;
                DbType prmType = value.Key;
                dynamic prmValue = value.Value;
                command.Parameters.Add(new NpgsqlParameter(prmName, prmType));
                command.Parameters[prmName].Value = prmValue;
                _logger?.Debug("[Prm] " + prmName + ":" + prmValue + "(" + prmType.ToString() + ")");
            }
        }
        return command.ExecuteNonQuery();
    }
    public int ExecuteSql(string sql) {
        return ExecuteSql(sql, null);
    }

}

using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Text;
using Npgsql;
using System.Data;

namespace Dashboard.Models.Utility;

class PostgresqlWorker : IDatabaseWorker
{
    // ------------------------------------------------
    // Field
    // ------------------------------------------------
    private bool _disposed = false;
    private readonly NpgsqlConnection? _connection = null;


    // ------------------------------------------------
    // Constructor
    // ------------------------------------------------
    public PostgresqlWorker(string host, int port, string user, string pass, string name) {
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
    // Execute SQL : Return DataReader
    // ------------------------------------------------
    public NpgsqlDataReader ExecuteSqlGetData(string sql, Dictionary<string, dynamic>? prms) {
        using NpgsqlCommand command = new(sql, _connection);
        if (prms is not null || prms?.Count > 0) {
            foreach (KeyValuePair<string, dynamic> pair in prms) {
                string prmName = pair.Key;
                KeyValuePair<DbType, dynamic> value = pair.Value;
                DbType dbType = value.Key;
                dynamic prmValue = value.Value;
                command.Parameters.Add(new NpgsqlParameter(prmName, dbType));
                command.Parameters[prmName].Value = prmValue;
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
        if (prms is not null || prms?.Count > 0) {
            foreach (KeyValuePair<string, dynamic> pair in prms) {
                string prmName = pair.Key;
                KeyValuePair<DbType, dynamic> value = pair.Value;
                DbType dbType = value.Key;
                dynamic prmValue = value.Value;
                command.Parameters.Add(new NpgsqlParameter(prmName, dbType));
                command.Parameters[prmName].Value = prmValue;
            }
        }
        return command.ExecuteNonQuery();
    }
    public int ExecuteSql(string sql) {
        return ExecuteSql(sql, null);
    }


}

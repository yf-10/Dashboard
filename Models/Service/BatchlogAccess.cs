using System.Data;
using Npgsql;

using Dashboard.Models.Item;
using Dashboard.Models.Utility;

namespace Dashboard.Models.Service;

class BatchlogAccess(PostgresqlWorker worker)
{
    // ------------------------------------------------
    // Field
    // ------------------------------------------------
    private readonly Logger? _logger = Logger.GetInstance();
    private PostgresqlWorker Worker { get; set; } = worker;
    private string sql = string.Empty;
    private Dictionary<string, dynamic> prms = [];

    // ------------------------------------------------
    // Function : Select
    // ------------------------------------------------
    public List<BatchlogMain> Select(string? uuid, string? keyword, string? status) {
        List<BatchlogMain> ret = [];
        // ------------------------------------
        // Selec Batch log main data
        // ------------------------------------
        sql = string.Empty;
        prms = [];
        sql = """
            select
                uuid,
                status,
                program_id,
                program_name,
                to_char(start_time, 'YYYY/MM/DD HH24:MI:SS') as start_time,
                to_char(end_time, 'YYYY/MM/DD HH24:MI:SS') as end_time
            from
                public.batchlog_main
            where
                1 = 1
            """;
        if (uuid is not null) {
            sql += """
                and uuid = @uuid
            """;
            PostgresqlWorker.AddParameter(ref prms, "@uuid", DbType.String, uuid);
        }
        if (keyword is not null) {
            sql += """
                and (
                    program_id like @keyword
                    or program_name like @keyword
                )
            """;
            PostgresqlWorker.AddParameter(ref prms, "@keyword", DbType.String, keyword);
        }
        if (status is not null) {
            sql += """
                and status = @status
            """;
            PostgresqlWorker.AddParameter(ref prms, "@status", DbType.String, status);
        }
        // Execute SQL
        using (NpgsqlDataReader reader = Worker.ExecuteSqlGetData(sql, prms)) {
            while (reader.Read()) {
                BatchlogMain data = new(
                    reader["uuid"].ToString() ?? "",
                    reader["status"].ToString() ?? "",
                    reader["program_id"].ToString() ?? "",
                    reader["program_name"].ToString() ?? "",
                    reader["start_time"].ToString() ?? "",
                    reader["end_time"].ToString() ?? "",
                    []
                );
                ret.Add(data);
            }
        }
        // ------------------------------------
        // Select Batch log detail data
        // ------------------------------------
        sql = string.Empty;
        sql = """
            select
                uuid,
                log_no,
                log_msg,
                to_char(log_time, 'YYYY/MM/DD HH24:MI:SS') as log_time
            from
                public.batchlog_detail
            where
                uuid = @uuid
            order by
                uuid asc, log_no asc
            """;
        foreach (BatchlogMain main in ret) {
            prms = [];
            PostgresqlWorker.AddParameter(ref prms, "@uuid", DbType.String, main.Uuid);
            // Execute SQL
            using NpgsqlDataReader reader = Worker.ExecuteSqlGetData(sql, prms);
            while (reader.Read()) {
                BatchlogDetail data = new(
                    reader["uuid"].ToString() ?? "",
                    int.Parse(reader["log_no"].ToString() ?? "-1"),
                    reader["log_msg"].ToString() ?? "",
                    reader["log_time"].ToString() ?? ""
                );
                main.Details.Add(data);
            }
        };
        // ------------------------------------
        // Return Batch log data
        // ------------------------------------
        return ret;
    }

    // ------------------------------------------------
    // Function : Begin Batch
    // Return : UUID
    // ------------------------------------------------
    public string Begin(User user, string programId, string programName) {
        // ------------------------------------
        // Insert Batch log main data
        // ------------------------------------
        // Asign UUID
        Guid guid = Guid.NewGuid();
        // Status
        string status = "Running";
        // SQL
        sql = string.Empty;
        sql = """
            insert into 
                public.batchlog_main (
                    uuid,
                    status,
                    program_id,
                    program_name,
                    start_time,
                    created_by,
                    updated_by,
                    created_at,
                    updated_at
                ) values (
                    @uuid,
                    @status,
                    @program_id,
                    @program_name,
                    now(),
                    @created_by,
                    @updated_by,
                    now(),
                    now()
                )
            """;
        // Set Parameters
        prms = [];
        PostgresqlWorker.AddParameter(ref prms, "@uuid", DbType.String, guid.ToString());
        PostgresqlWorker.AddParameter(ref prms, "@status", DbType.String, status);
        PostgresqlWorker.AddParameter(ref prms, "@program_id", DbType.String, programId);
        PostgresqlWorker.AddParameter(ref prms, "@program_name", DbType.String, programName);
        PostgresqlWorker.AddParameter(ref prms, "@created_by", DbType.String, user.Name);
        PostgresqlWorker.AddParameter(ref prms, "@updated_by", DbType.String, user.Name);
        // Execute SQL
        Worker.ExecuteSql(sql, prms);
        // ------------------------------------
        // Add Batch log detail
        // ------------------------------------
        string logMsg = "バッチ処理を開始しました。";
        AddLog(user, guid.ToString(), logMsg);
        // ------------------------------------
        // Return UUID
        // ------------------------------------        
        return guid.ToString();
    }

    // ------------------------------------------------
    // Function : Complete Batch
    // Return : UUID
    // ------------------------------------------------
    public string Complete(User user, string uuid) {
        // Update status
        string status = "Complete";
        UpdateStatus(user, uuid, status);
        UpdateEndTime(user, uuid);
        // Add Batch log detail
        string logMsg = "バッチ処理を完了しました。";
        AddLog(user, uuid, logMsg);
        // Return UUID
        return uuid;
    }

    // ------------------------------------------------
    // Function : Abort Batch
    // Return : UUID
    // ------------------------------------------------
    public string Abort(User user, string uuid) {
        // Update status
        string status = "Abort";
        UpdateStatus(user, uuid, status);
        // Add Batch log detail
        string logMsg = "バッチ処理を中止しました。";
        AddLog(user, uuid, logMsg);
        // Return UUID
        return uuid;
    }

    // ------------------------------------------------
    // Function : Add Batch log detail
    // Return : UUID
    // ------------------------------------------------
    public string AddLog(User user, string uuid, string logMsg) {
        int logNo = GetMaxLogNo(uuid) + 1;
        // SQL
        sql = string.Empty;
        sql = """
            insert into 
                public.batchlog_detail (
                    uuid,
                    log_no,
                    log_msg,
                    log_time,
                    created_by,
                    updated_by,
                    created_at,
                    updated_at
                ) values (
                    @uuid,
                    @log_no,
                    @log_msg,
                    now(),
                    @created_by,
                    @updated_by,
                    now(),
                    now()
                )
            """;
        // Set Parameters
        prms = [];
        PostgresqlWorker.AddParameter(ref prms, "@uuid", DbType.String, uuid);
        PostgresqlWorker.AddParameter(ref prms, "@log_no", DbType.Int64, logNo);
        PostgresqlWorker.AddParameter(ref prms, "@log_msg", DbType.String, logMsg);
        PostgresqlWorker.AddParameter(ref prms, "@created_by", DbType.String, user.Name);
        PostgresqlWorker.AddParameter(ref prms, "@updated_by", DbType.String, user.Name);
        // Execute SQL
        Worker.ExecuteSql(sql, prms);
        return uuid;
    }

    // ------------------------------------------------
    // Function : Update status
    // ------------------------------------------------
    private void UpdateStatus(User user, string uuid, string status) {
        // SQL
        sql = string.Empty;
        sql = """
            update
                public.batchlog_main
            set
                status = @status,
                end_time = now(),
                updated_by = @updated_by,
                updated_at = now()
            where
                uuid = @uuid
            """;
        // Set Parameters
        prms = [];
        PostgresqlWorker.AddParameter(ref prms, "@uuid", DbType.String, uuid);
        PostgresqlWorker.AddParameter(ref prms, "@status", DbType.String, status);
        PostgresqlWorker.AddParameter(ref prms, "@updated_by", DbType.String, user.Name);
        // Execute SQL
        Worker.ExecuteSql(sql, prms);
        return;
    }

    // ------------------------------------------------
    // Function : Update end time
    // ------------------------------------------------
    private void UpdateEndTime(User user, string uuid) {
        // SQL
        sql = string.Empty;
        sql = """
            update
                public.batchlog_main
            set
                end_time = now(),
                updated_by = @updated_by,
                updated_at = now()
            where
                uuid = @uuid
            """;
        // Set Parameters
        prms = [];
        PostgresqlWorker.AddParameter(ref prms, "@uuid", DbType.String, uuid);
        PostgresqlWorker.AddParameter(ref prms, "@updated_by", DbType.String, user.Name);
        // Execute SQL
        Worker.ExecuteSql(sql, prms);
        return;
    }

    // ------------------------------------------------
    // Function : Get max log no
    // ------------------------------------------------
    private int GetMaxLogNo(string uuid) {
        int logNo = 0;
        // SQL
        sql = string.Empty;
        sql = """
            select
                uuid,
                coalesce(max(log_no), 0) as max_log_no
            from
                public.batchlog_detail
            where
                uuid = @uuid
            group by
                uuid
            """;
        prms = [];
        PostgresqlWorker.AddParameter(ref prms, "@uuid", DbType.String, uuid);
        // Execute SQL
        using NpgsqlDataReader reader = Worker.ExecuteSqlGetData(sql, prms);
        while (reader.Read()) {
            logNo = int.Parse(reader["max_log_no"].ToString() ?? "0");
            break;
        }
        return logNo;
    }


}

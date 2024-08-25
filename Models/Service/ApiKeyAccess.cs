using System.Data;
using Npgsql;

using Dashboard.Models.Data;
using Dashboard.Models.Utility;

namespace Dashboard.Models.Service;

class ApiKeyAccess(PostgresqlWorker worker)
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
    public List<ApiKey> Select(string? userId) {
        List<ApiKey> ret = [];
        sql = string.Empty;
        prms = [];
        // Construct Query
        sql = """
            select
                user_id,
                email,
                apikey,
                status,
                created_by,
                updated_by,
                to_char(created_at, 'YYYYMMDD HH24MISS') as created_at,
                to_char(updated_at, 'YYYYMMDD HH24MISS') as updated_at
            from
                public.dashboard_apikey
            where
                1 = 1
            """;
        // Add Parameters
        if (string.IsNullOrEmpty(userId) == false) {
            sql += """
                and user_id = @user_id
            """;
            PostgresqlWorker.AddParameter(ref prms, "@user_id", DbType.String, userId);
        }
        // Execute SQL
        using NpgsqlDataReader reader = Worker.ExecuteSqlGetData(sql, prms);
        while (reader.Read()) {
            ApiKey data = new(
                new User(
                    reader["user_id"].ToString() ?? "",
                    "",
                    reader["email"].ToString() ?? ""
                ),
                reader["apikey"].ToString() ?? "",
                reader["status"].ToString() ?? ""
            );
            ret.Add(data);
        }
        return ret;
    }

    // ------------------------------------------------
    // Function : Upsert
    // ------------------------------------------------
    public int Upsert(User user, ApiKey apiKey) {
        int count = 0;
        sql = string.Empty;
        prms = [];
        // Construct Query
        sql = """
            insert into 
                public.dashboard_apikey (
                    user_id,
                    email,
                    apikey,
                    status,
                    created_by,
                    updated_by,
                    created_at,
                    updated_at            
                ) values (
                    @user_id,
                    @email,
                    @apikey,
                    @status,
                    @created_by,
                    @updated_by,
                    now(),
                    now()
                )
                on conflict on constraint
                    dashboard_apikey_pkey
                do update set
                    email = @email,
                    apikey = @apikey,
                    status = @status,
                    updated_by = @updated_by,
                    updated_at = now()
            """;
        // Add Parameters
        PostgresqlWorker.AddParameter(ref prms, "@user_id", DbType.String, apiKey.User.Id);
        PostgresqlWorker.AddParameter(ref prms, "@email", DbType.String, apiKey.User.Email);
        PostgresqlWorker.AddParameter(ref prms, "@apikey", DbType.String, apiKey.Key);
        PostgresqlWorker.AddParameter(ref prms, "@status", DbType.String, apiKey.Status);
        PostgresqlWorker.AddParameter(ref prms, "@created_by", DbType.String, user.Name);
        PostgresqlWorker.AddParameter(ref prms, "@updated_by", DbType.String, user.Name);
        // Execute SQL
        int insertedCount = Worker.ExecuteSql(sql, prms);
        if (insertedCount > 0) count++;
        return count;
    }

    // ------------------------------------------------
    // Function : Generate API Key
    // ------------------------------------------------
    public string Generate(User user) {
        if (string.IsNullOrEmpty(user.Id)) throw new ArgumentException("Invalid user id");
        // Generate API Key
        Guid guid = Guid.NewGuid();
        // Register API Key
        ApiKey apiKey = new(user, guid.ToString(), "Available");
        Upsert(user, apiKey);
        return apiKey.Key;
    }

}

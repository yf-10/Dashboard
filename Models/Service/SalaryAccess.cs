using System.Data;
using Npgsql;

using Dashboard.Models.Item;
using Dashboard.Models.Utility;

namespace Dashboard.Models.Service;

class SalaryAccess(PostgresqlWorker worker)
{
    // ------------------------------------------------
    // Field
    // ------------------------------------------------
    private readonly Logger? _logger = Logger.GetInstance();
    private PostgresqlWorker Worker { get; set; } = worker;
    private string sql = string.Empty;
    private Dictionary<string, dynamic> prms = [];

    // ------------------------------------------------
    // Function : Get Salary
    // ------------------------------------------------
    public List<Salary> GetSalary(string? month, string? keyword) {
        List<Salary> ret = [];
        sql = string.Empty;
        prms = [];
        // Construct Query
        sql = """
            select 
                * 
            from 
                public.salary 
            where
                1 = 1 
            """;
        if (month is not null) {
            sql += """
                and month = @month 
            """;
            PostgresqlWorker.AddParameter(ref prms, "@month", DbType.String, month);
        }
        if (keyword is not null) {
            sql += """
                and payment_item = @keyword 
            """;
            PostgresqlWorker.AddParameter(ref prms, "@keyword", DbType.String, keyword);
        }
        // Execute SQL
        using NpgsqlDataReader reader = Worker.ExecuteSqlGetData(sql, prms);
        while (reader.Read()) {
            Salary data = new(
                reader["month"].ToString() ?? "",
                Boolean.Parse(reader["deduction"].ToString() ?? "false"),
                reader["month"].ToString() ?? "",
                new Money(
                    int.Parse(reader["amount"].ToString() ?? "0"),
                    reader["currency_code"].ToString() ?? ""
                )
            );
            ret.Add(data);
        }
        return ret;
    }



}

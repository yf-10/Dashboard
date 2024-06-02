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
    // Function : Select
    // ------------------------------------------------
    public List<Salary> Select(string? month, string? keyword) {
        List<Salary> ret = [];
        sql = string.Empty;
        prms = [];
        // Construct Query
        sql = """
            select
                month,
                deduction,
                payment_item,
                amount,
                currency_code
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

    // ------------------------------------------------
    // Function : Insert
    // ------------------------------------------------
    public int Insert(User user, List<Salary> salaries) {
        int count = 0;
        sql = string.Empty;
        sql = """
            insert into 
                public.salary ( 
                    created_at,
                    created_by,
                    updated_at,
                    updated_by,
                    exclusive_flag,
                    month,
                    deduction,
                    payment_item,
                    amount,
                    currency_code
                ) values (
                    now(),
                    @created_by,
                    now(),
                    @updated_by,
                    @exclusive_flag,
                    @month,
                    @deduction,
                    @payment_item,
                    @amount,
                    @currency_code
                )
                on conflict on constraint
                    salary_pkey
                do update set
                    updated_at = now(),
                    updated_by = @updated_by,
                    amount = @amount,
                    currency_code = @currency_code
            """;
        salaries.ForEach(salary => {
            prms = [];
            // Add Parameters
            PostgresqlWorker.AddParameter(ref prms, "@created_by", DbType.String, user.Name);
            PostgresqlWorker.AddParameter(ref prms, "@updated_by", DbType.String, user.Name);
            PostgresqlWorker.AddParameter(ref prms, "@exclusive_flag", DbType.Int64, 0);
            PostgresqlWorker.AddParameter(ref prms, "@month", DbType.String, salary.Month);
            PostgresqlWorker.AddParameter(ref prms, "@deduction", DbType.Boolean, salary.Deduction);
            PostgresqlWorker.AddParameter(ref prms, "@payment_item", DbType.String, salary.PaymentItem);
            PostgresqlWorker.AddParameter(ref prms, "@amount", DbType.Int64, salary.Money.Amount);
            PostgresqlWorker.AddParameter(ref prms, "@currency_code", DbType.String, salary.Money.CurrencyCode);
            // Execute SQL
            int insertedCount = Worker.ExecuteSql(sql, prms);
            if (insertedCount > 0) count++;
        });
        return count;
    }

}

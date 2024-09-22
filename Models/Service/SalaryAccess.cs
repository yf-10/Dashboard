using System.Data;

using Npgsql;

using Dashboard.Models.Data;
using Dashboard.Models.Utility;

namespace Dashboard.Models.Service;

class SalaryAccess(PostgresqlWorker worker) {
    // ------------------------------------------------
    // Field
    // ------------------------------------------------
    private PostgresqlWorker Worker { get; set; } = worker;
    private string sql = string.Empty;
    private List<IDatabaseWorker.Parameter> prms = [];

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
            prms.Add(new IDatabaseWorker.Parameter("@month", month, DbType.String));
        }
        if (keyword is not null) {
            sql += """
                and payment_item = @keyword
            """;
            prms.Add(new IDatabaseWorker.Parameter("@keyword", keyword, DbType.String));
        }
        // Execute SQL
        using NpgsqlDataReader reader = (NpgsqlDataReader)Worker.ExecuteSqlGetData(sql, prms);
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
            prms.Add(new IDatabaseWorker.Parameter("@created_by", user.Name, DbType.String));
            prms.Add(new IDatabaseWorker.Parameter("@updated_by", user.Name, DbType.String));
            prms.Add(new IDatabaseWorker.Parameter("@exclusive_flag", 0, DbType.Int64));
            prms.Add(new IDatabaseWorker.Parameter("@month", salary.Month, DbType.String));
            prms.Add(new IDatabaseWorker.Parameter("@deduction", salary.Deduction, DbType.Boolean));
            prms.Add(new IDatabaseWorker.Parameter("@payment_item", salary.PaymentItem, DbType.String));
            prms.Add(new IDatabaseWorker.Parameter("@amount", salary.Money.Amount, DbType.Int64));
            prms.Add(new IDatabaseWorker.Parameter("@currency_code", salary.Money.CurrencyCode, DbType.String));
            // Execute SQL
            int insertedCount = Worker.ExecuteSql(sql, prms);
            if (insertedCount > 0) count++;
        });
        return count;
    }

}

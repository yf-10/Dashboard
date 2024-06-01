using Microsoft.AspNetCore.Mvc;

using Dashboard.Models.Utility;
using Npgsql;
using System.Data;

namespace Dashboard.Controllers;
public class ApiController(IConfiguration conf) : BaseController(conf)
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // [GET] api/salary
    // ---------------------------------------------------------------------
    [Route("api/salary/{month?}")]
    public IActionResult Saraly(string? month) {
        using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
            string sql = string.Empty;
            Dictionary<string, dynamic> prms = [];
            sql = """
                select 
                    * 
                from 
                    public.salary 
                """;
            if (month is not null) {
                sql += """
                where 
                    month = @month 
                """;
                prms.Add("@month", new KeyValuePair<DbType, dynamic>(DbType.String, month));
            }
            using NpgsqlDataReader reader = worker.ExecuteSqlGetData(sql, prms);
            while (reader.Read()) {
                _logger.Debug("month: " + reader["month"].ToString() ?? "");
                _logger.Debug("item: " + reader["payment_item"].ToString() ?? "");
            }
        }

        return View();
    }

}

using Microsoft.AspNetCore.Mvc;

using Dashboard.Models.Utility;
using Npgsql;

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
    [Route("api/salary")]
    public IActionResult Saraly() {
        using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
            string sql = string.Empty;
            sql += "select * from public.salary ";
            using NpgsqlDataReader reader = worker.ExecuteSqlGetData(sql);
            while (reader.Read()) {
                _logger.Debug("month: " + reader["month"].ToString() ?? "");
                _logger.Debug("item: " + reader["payment_item"].ToString() ?? "");
            }
        }

        return View();
    }

}

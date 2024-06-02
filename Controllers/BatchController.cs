using Microsoft.AspNetCore.Mvc;

using Dashboard.Models.Utility;
using Dashboard.Models.Service;
using Dashboard.Models.Item;

namespace Dashboard.Controllers;
public class BatchController(IConfiguration conf) : BaseController(conf)
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // [GET] api/batchlogs
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("api/batchlogs/{uuid?}")]
    public IActionResult GetBatchLogs(string? uuid, string? keyword, string? status) {
        ApiResponseJson? response = null;
        try {
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                BatchlogAccess batchlogAccess = new(worker);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, keyword, status);
                response = new(1, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [GET] api/batchlogs/begin
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("api/batchlogs/begin")]
    public IActionResult BeginBatchlog(string programId, string programName, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                BatchlogAccess batchlogAccess = new(worker);
                string uuid = batchlogAccess.Begin(user, programId, programName);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(1, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [GET] api/batchlogs/complete
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("api/batchlogs/complete")]
    public IActionResult CompleteBatchLog(string uuid, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                BatchlogAccess batchlogAccess = new(worker);
                batchlogAccess.Complete(user, uuid);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(1, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [GET] api/batchlogs/abort
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("api/batchlogs/abort")]
    public IActionResult AbortBatchLog(string uuid, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                BatchlogAccess batchlogAccess = new(worker);
                batchlogAccess.Abort(user, uuid);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(1, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [POST] api/batchlogs
    // ---------------------------------------------------------------------
    [HttpPost]
    [Route("api/batchlogs")]
    public IActionResult AddBatchLog([FromBody] BatchlogDetail log, string? userName) {
        ApiResponseJson? response = null;
        try {
            if (string.IsNullOrEmpty(log.Uuid))
                throw new ArgumentNullException("UUID is necessary");
            if (string.IsNullOrEmpty(log.LogMsg))
                throw new ArgumentNullException("Log message is necessary");
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                BatchlogAccess batchlogAccess = new(worker);
                string uuid = batchlogAccess.AddLog(user, log.Uuid, log.LogMsg);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(1, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }



}

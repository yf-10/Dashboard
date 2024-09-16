using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Dashboard.Models.Utility;
using Dashboard.Models.Service;
using Dashboard.Models.Data;
using Dashboard.Models.ViewModel;

namespace Dashboard.Controllers;
public class BatchController(IConfiguration conf) : BaseController(conf) {
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // [GET] /batch
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("batch")]
    public IActionResult Batch() {
        try {
            List<BatchlogMain>? batchlogMainList = null;
            using (PostgresqlWorker worker = new()) {
                BatchlogAccess batchlogAccess = new(worker);
                batchlogMainList = batchlogAccess.Select(null, null, null);
            }
            BatchViewModel vm = new(batchlogMainList);
            return View(vm);
        } catch (Exception ex) {
            _logger.Error(ex);
            return RedirectToAction("Error", "Error", new { code = "500" });
        }
    }

    // ---------------------------------------------------------------------
    // [GET] /api/batchlogs
    // ---------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("api/batchlogs/{uuid?}")]
    public IActionResult GetBatchLogs(string? uuid, string? keyword, string? status) {
        ApiResponseJson? response = null;
        try {
            using (PostgresqlWorker worker = new()) {
                BatchlogAccess batchlogAccess = new(worker);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, keyword, status);
                response = new(0, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [GET] /api/batchlogs/begin
    // ---------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("api/batchlogs/begin")]
    public IActionResult BeginBatchlog(string programId, string programName, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new()) {
                BatchlogAccess batchlogAccess = new(worker);
                string uuid = batchlogAccess.Begin(user, programId, programName);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(0, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [GET] /api/batchlogs/complete
    // ---------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("api/batchlogs/{uuid}/complete")]
    public IActionResult CompleteBatchLog(string uuid, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new()) {
                BatchlogAccess batchlogAccess = new(worker);
                batchlogAccess.Complete(user, uuid);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(0, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [GET] /api/batchlogs/abort
    // ---------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("api/batchlogs/{uuid}/abort")]
    public IActionResult AbortBatchLog(string uuid, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new()) {
                BatchlogAccess batchlogAccess = new(worker);
                batchlogAccess.Abort(user, uuid);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(0, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [POST] /api/batchlogs
    // ---------------------------------------------------------------------
    [HttpPost]
    [Authorize]
    [Route("api/batchlogs")]
    public IActionResult AddBatchLog([FromBody] BatchlogDetail log, string? userName) {
        ApiResponseJson? response = null;
        try {
            if (string.IsNullOrEmpty(log.Uuid))
                throw new ArgumentException("UUID is necessary");
            if (string.IsNullOrEmpty(log.LogMsg))
                throw new ArgumentException("Log message is necessary");
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new()) {
                BatchlogAccess batchlogAccess = new(worker);
                string uuid = batchlogAccess.AddLog(user, log.Uuid, log.LogMsg);
                List<BatchlogMain> batchlogMainList = batchlogAccess.Select(uuid, null, null);
                response = new(0, "Success", batchlogMainList.Count, batchlogMainList);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }



}

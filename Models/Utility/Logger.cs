using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Text;

namespace Dashboard.Models.Utility;

public class Logger
{
    // ------------------------------------------------
    // Field
    // ------------------------------------------------
    private enum Level { ERROR, WARN, INFO, DEBUG }
    private static Logger? _singleton = null;
    private StreamWriter? _stream = null;
    private readonly object _lockObj = new();

    // ------------------------------------------------
    // Field : Configuration
    // ------------------------------------------------
    private static bool isFileOutput = true;
    private static int level;
    private static string dirPath = string.Empty;
    private static string fileName = string.Empty;
    private static string filePath = string.Empty;
    private static int fileMaxSize;
    private static int filePeriod;

    // ------------------------------------------------
    // Get Instance
    // ------------------------------------------------
    public static Logger GetInstance(IConfiguration conf) {
        if (_singleton is null) {
            // Load configurations
            var section = conf.GetSection("LoggerConfig");
            isFileOutput    = section.GetValue<bool>("isFileOutput", false);
            level           = section.GetValue<int>("level", 2);
            dirPath         = section.GetValue<string>("dirPath", "./logs") ?? "./logs";
            fileName        = section.GetValue<string>("fileName", "app") ?? "app";
            fileMaxSize     = section.GetValue<int>("fileMaxSize", 10485760);
            filePeriod      = section.GetValue<int>("filePeriod", 180);
            filePath        = Path.Combine(dirPath, fileName + ".log");
            // Create instance
            _singleton = new Logger();
        }
        return _singleton;
    }
    public static Logger? GetInstance() {
        return _singleton;
    }

    // ------------------------------------------------
    // Constructor
    // ------------------------------------------------
    private Logger() {
        if (isFileOutput) {
            CreateLogfile(new FileInfo(filePath));
        }
    }

    // ------------------------------------------------
    // 0 : ERROR
    // ------------------------------------------------
    public void Error(string msg) {
        if ((int)Level.ERROR <= level) {
            Out(Level.ERROR, msg);
        }
    }
    public void Error(Exception ex) {
        if ((int)Level.ERROR <= level) {
            Out(Level.ERROR, ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

    // ------------------------------------------------
    // 1 : WARN
    // ------------------------------------------------
    public void Warn(string msg) {
        if ((int)Level.WARN <= level) {
            Out(Level.WARN, msg);
        }
    }

    // ------------------------------------------------
    // 2 : INFO
    // ------------------------------------------------
    public void Info(string msg) {
        if ((int)Level.INFO <= level) {
            Out(Level.INFO, msg);
        }
    }

    // ------------------------------------------------
    // 3 : DEBUG
    // ------------------------------------------------
    public void Debug(string msg) {
        if ((int)Level.DEBUG <= level) {
            Out(Level.DEBUG, msg);
        }
    }

    // ------------------------------------------------
    // Output log
    // ------------------------------------------------
    private void Out(Level level, string msg) {
        int tid = Environment.CurrentManagedThreadId;
        string line = string.Format(
            "[{0}][{1}][{2}] {3}",
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            tid,
            level.ToString(),
            msg
        );
        if (isFileOutput) {
            // [File]
            lock (_lockObj) {
                _stream?.WriteLine(line);
                FileInfo logFile = new(filePath);
                if (fileMaxSize < logFile.Length) {
                    // Compress log file
                    CompressLogFile();
                    // Delete old log files
                    DeleteOldLogFile();
                }
            }
        } else {
            // [Console]
            Console.WriteLine(line);
        }
    }

    // ------------------------------------------------
    // Create log file
    // ------------------------------------------------
    private void CreateLogfile(FileInfo logFile) {
        // Create directory if not exists
        if (logFile.DirectoryName is not null) {
            if (!Directory.Exists(logFile.DirectoryName)) {
                Directory.CreateDirectory(logFile.DirectoryName);
            }
        }
        // Create log file (UTF-8)
        _stream = new StreamWriter(logFile.FullName, true, Encoding.UTF8) {
            AutoFlush = true
        };
    }

    // ------------------------------------------------
    // Compress log file
    // ------------------------------------------------
    private void CompressLogFile() {
        string oldFileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        string oldFilePath = Path.Combine(dirPath, oldFileName);
        File.Move(filePath, oldFilePath + ".log");
        // Compress with "GZip"
        FileStream inStream     = new FileStream(oldFilePath + ".log", FileMode.Open, FileAccess.Read);
        FileStream outStream    = new FileStream(oldFilePath + ".gz", FileMode.Create, FileAccess.Write);
        GZipStream gzStream     = new GZipStream(outStream, CompressionMode.Compress);
        byte[] buffer = new byte[fileMaxSize + 1000];
        int size;
        while (0 < (size = inStream.Read(buffer, 0, buffer.Length))) {
            gzStream.Write(buffer, 0, size);
        }
        // Close stream
        inStream.Close();
        outStream.Close();
        gzStream.Close();
        // Delete work file
        File.Delete(oldFilePath + ".log");
        // Create new log file
        CreateLogfile(new FileInfo(filePath));
    }

    // ------------------------------------------------
    // Delete old log file
    // ------------------------------------------------
    private void DeleteOldLogFile() {
        // Specified file name "filename_yyyyMMddHHmmss.gz"
        Regex regex = new(fileName + @"_(\d{14}).*\.gz");
        DateTime retentionDate = DateTime.Today.AddDays(-filePeriod);
        string[] filePathList = Directory.GetFiles(dirPath, fileName + "_*.gz", SearchOption.TopDirectoryOnly);
        foreach (string filePath in filePathList) {
            Match match = regex.Match(filePath);
            if (match.Success) {
                DateTime logCreatedDate = DateTime.ParseExact(match.Groups[1].Value.ToString(), "yyyyMMddHHmmss", null);
                if (logCreatedDate < retentionDate) {
                    Info("Delete old log file: " + filePath);
                    File.Delete(filePath);
                }
            }
        }
    }

}

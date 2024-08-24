using System.Text;
using System.Text.RegularExpressions;

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
            tid.ToString().PadLeft(5, ' '),
            level.ToString().PadRight(5, ' '),
            msg
        );
        if (isFileOutput) {
            // [File]
            lock (_lockObj) {
                _stream?.WriteLine(line);
                FileInfo logFile = new(filePath);
                if (fileMaxSize < logFile.Length) {
                    ArchiveLogFile();
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
    // Archive log file
    // ------------------------------------------------
    private void ArchiveLogFile() {
        string archiveFileName = string.Concat(fileName, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".log");
        string archiveFilePath = Path.Combine(dirPath, archiveFileName);
        File.Move(filePath, archiveFilePath);
        CreateLogfile(new FileInfo(filePath));
    }

    // ------------------------------------------------
    // Delete old log file
    // ------------------------------------------------
    private void DeleteOldLogFile() {
        // Specified file name "[filename]_yyyyMMddHHmmss.log"
        Regex regex = new(fileName + @"_(\d{14}).*\.log");
        DateTime retentionDate = DateTime.Today.AddDays(-filePeriod);
        string[] filePathList = Directory.GetFiles(dirPath, fileName + "_*.log", SearchOption.TopDirectoryOnly);
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

using System.Text;
using System.Text.RegularExpressions;

namespace Dashboard.Models.Utility;

public class Logger {
    private enum Level { ERROR, WARN, INFO, DEBUG }

    private static Logger?  singleton = null;
    private StreamWriter?   stream = null;
    private readonly object lockObj = new();

    private static bool     isFileOutput    = true;
    private static int      logLevel        = (int)Level.INFO;
    private static string   logDirPath      = "./logs";
    private static string   logFileName     = "app";
    private static string   logFilePath     = string.Empty;
    private static int      logFileMaxSize  = 10485760;
    private static int      logFilePeriod   = 180;

    public static Logger GetInstance(IConfiguration conf) {
        if (singleton is null) {
            var section = conf.GetSection("LoggerConfig");
            isFileOutput        = section.GetValue<bool>("isFileOutput", isFileOutput);
            logLevel            = section.GetValue<int>("logLevel", logLevel);
            logDirPath          = section.GetValue<string>("logDirPath", logDirPath) ?? logDirPath;
            logFileName         = section.GetValue<string>("logFileName", logFileName) ?? logFileName;
            logFileMaxSize      = section.GetValue<int>("logFileMaxSize", logFileMaxSize);
            logFilePeriod       = section.GetValue<int>("logFilePeriod", logFilePeriod);
            logFilePath         = Path.Combine(logDirPath, logFileName + ".log");
            singleton = new Logger();
        }
        return singleton;
    }
    public static Logger? GetInstance() {
        return singleton;
    }

    // ------------------------------------------------
    // Constructor
    // ------------------------------------------------
    private Logger() {
        if (isFileOutput) {
            CreateLogfile(new FileInfo(logFilePath));
        }
    }

    // ------------------------------------------------
    // 0 : ERROR
    // ------------------------------------------------
    public void Error(string msg) {
        if ((int)Level.ERROR <= logLevel) {
            Out(Level.ERROR, msg);
        }
    }
    public void Error(Exception ex) {
        if ((int)Level.ERROR <= logLevel) {
            Out(Level.ERROR, ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

    // ------------------------------------------------
    // 1 : WARN
    // ------------------------------------------------
    public void Warn(string msg) {
        if ((int)Level.WARN <= logLevel) {
            Out(Level.WARN, msg);
        }
    }

    // ------------------------------------------------
    // 2 : INFO
    // ------------------------------------------------
    public void Info(string msg) {
        if ((int)Level.INFO <= logLevel) {
            Out(Level.INFO, msg);
        }
    }

    // ------------------------------------------------
    // 3 : DEBUG
    // ------------------------------------------------
    public void Debug(string msg) {
        if ((int)Level.DEBUG <= logLevel) {
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
        if (isFileOutput) { // FILE
            lock (lockObj) {
                stream?.WriteLine(line);
                FileInfo logFile = new(logFilePath);
                if (logFileMaxSize < logFile.Length) {
                    ArchiveLogFile();
                    DeleteOldLogFile();
                }
            }
        } else { // CONSOLE
            Console.WriteLine(line);
        }
    }

    // ------------------------------------------------
    // Create log file
    // ------------------------------------------------
    private void CreateLogfile(FileInfo logFile) {
        if (logFile.DirectoryName is not null) {
            if (Directory.Exists(logFile.DirectoryName) == false) {
                Directory.CreateDirectory(logFile.DirectoryName);
            }
        }
        stream = new StreamWriter(logFile.FullName, true, Encoding.UTF8) {
            AutoFlush = true
        };
    }

    // ------------------------------------------------
    // Archive log file
    // ------------------------------------------------
    private void ArchiveLogFile() {
        string archiveFileName = string.Concat(logFileName, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".log");
        string archiveFilePath = Path.Combine(logDirPath, archiveFileName);
        File.Move(logFilePath, archiveFilePath);
        CreateLogfile(new FileInfo(logFilePath));
    }

    // ------------------------------------------------
    // Delete old log file
    // ------------------------------------------------
    private void DeleteOldLogFile() {
        // Specified file name "[filename]_yyyyMMddHHmmss.log"
        Regex regex = new(logFileName + @"_(\d{14}).*\.log");
        DateTime retentionDate = DateTime.Today.AddDays(-logFilePeriod);
        string[] filePathList = Directory.GetFiles(logDirPath, logFileName + "_*.log", SearchOption.TopDirectoryOnly);
        foreach (string filePath in filePathList) {
            Match match = regex.Match(filePath);
            if (match.Success) {
                DateTime logCreatedDate = DateTime.ParseExact(match.Groups[1].Value.ToString(), "yyyyMMddHHmmss", null);
                if (logCreatedDate < retentionDate) {
                    Info("Delete log file: " + filePath);
                    File.Delete(filePath);
                }
            }
        }
    }

}

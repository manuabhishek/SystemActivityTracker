using System.Drawing.Imaging;
using System.Drawing;
using System.Management; // For WMI
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowaService
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _config;
        //private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;

        //private static readonly string Iscapture = ConfigurationManager.AppSettings["Iscapture"];

        // Lock/Unlock tracking variables
        private static int lockCount = 0;
        private static int unlockCount = 0;
        private static DateTime? lockStartTime = null;
        private static TimeSpan totalLockDuration = TimeSpan.Zero;

        // Application usage tracking variables
        private static TimeSpan totalSystemUsage = TimeSpan.Zero;
        private static Dictionary<string, TimeSpan> appUsage = new Dictionary<string, TimeSpan>();
        private static string lastApp = string.Empty;
        private static DateTime lastActiveTime = DateTime.Now;

        // System information
        private static string logFilePath = "SystemUsageLog.txt";
        private static string systemName = Environment.MachineName;
        private static string systemId = GetSystemId();
        private static string userName = Environment.UserName;
        private static string systemIp = GetSystemIp();
        private static string screenCaptureFolder = "ScreenCaptures";

        //public Worker(ILogger<Worker> logger, IConfiguration config)
        //{
        //    _config = config;
        //    _logger = logger;
        //    _httpClient = new HttpClient();
        //}

        public Worker()
        {
           
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //if (_logger.IsEnabled(LogLevel.Information))
                //  _logger.LogWarning("Windows Service running at: {time}-Hi", DateTimeOffset.Now);

                //var appLog = new EventLog("Application");
                //appLog.Source = "CompanyLicenceVrifier";
                //appLog.WriteEntry("CompanyLicenceVrifier-Test log message");

                SystemActivityLogic.SendSystemActivity();

                await Task.Delay(1000 * 60 * 5, stoppingToken);
            }
        }

        //private async void InvokeApi(object state)
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync("https://api.example.com/endpoint");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Process the response
        //        }
        //        else
        //        {
        //            if (_logger.IsEnabled(LogLevel.Information))
        //                _logger.LogInformation($"API returned a non-success status code: {response.StatusCode}", DateTimeOffset.Now);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_logger.IsEnabled(LogLevel.Error))
        //            _logger.LogError($"Error while invoking API: {ex.Message}", DateTimeOffset.Now);
        //    }
        //}

        //public override Task StopAsync(CancellationToken cancellationToken)
        //{
        //    if (_logger.IsEnabled(LogLevel.Information))
        //        _logger.LogInformation("Windows Service stopped", DateTimeOffset.Now);

        //    CustomFunction();

        //    return base.StopAsync(cancellationToken);
        //}

        public override void Dispose()
        {
            _httpClient.Dispose();
            base.Dispose();
        }

        private void CustomFunction()
        {
            Console.WriteLine("System Usage Tracker");
            Console.WriteLine($"System Name: {systemName}");
            Console.WriteLine($"System ID: {systemId}");
            Console.WriteLine($"User Name: {userName}");
            Console.WriteLine($"System IP: {systemIp}");
            Console.WriteLine($"Log File: {Path.GetFullPath(logFilePath)}");
            Console.WriteLine("Tracking lock/unlock events and application usage. Press Ctrl+C to stop.\n");

            // Subscribe to lock/unlock events
            SystemEvents.SessionSwitch += OnSessionSwitch;

            // Start application usage tracking
            DateTime systemStartTime = DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount64);

            // Monitor application usage in a loop

            string currentApp = GetActiveWindowTitle();

            if (!string.IsNullOrEmpty(currentApp))
            {
                // Track time spent on the previous app
                if (currentApp != lastApp)
                {
                    if (!string.IsNullOrEmpty(lastApp))
                    {
                        TimeSpan usageTime = DateTime.Now - lastActiveTime;
                        if (appUsage.ContainsKey(lastApp))
                            appUsage[lastApp] += usageTime;
                        else
                            appUsage[lastApp] = usageTime;
                    }

                    lastApp = currentApp;
                    lastActiveTime = DateTime.Now;

                    // Capture the screen when the active application changes
                    //var isCapture = _config.GetValue<string>("Iscapture");
                    //if (isCapture == "Y")
                    //    CaptureScreen();
                }
            }

            // Calculate total system usage
            totalSystemUsage = DateTime.Now - systemStartTime;

            // Log every 10 seconds
            if (DateTime.Now.Second % 10 == 0)
            {
                Console.Clear();
                Console.WriteLine("=== System Usage Tracker ===");
                Console.WriteLine($"Total System Usage: {totalSystemUsage}");

                Console.WriteLine("\n=== Application Usage ===");
                foreach (var app in appUsage)
                {
                    Console.WriteLine($"{app.Key}: {app.Value}");
                }

                Console.WriteLine("\n=== Lock/Unlock Events ===");
                Console.WriteLine($"Lock Count: {lockCount}");
                Console.WriteLine($"Unlock Count: {unlockCount}");

                LogToFile();
            }
        }

        //private static void CaptureScreen()
        //{
        //    try
        //    {
        //        string fileName = Path.Combine(screenCaptureFolder, $"Capture_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        //        Rectangle bounds = Screen.PrimaryScreen.Bounds;
        //        using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
        //        {
        //            using (Graphics g = Graphics.FromImage(bitmap))
        //            {
        //                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
        //            }
        //            bitmap.Save(fileName, ImageFormat.Png);
        //        }
        //        Console.WriteLine($"Screen captured: {fileName}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error capturing screen: {ex.Message}");
        //    }
        //}

        //private static void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        //{
        //    if (e.Reason == SessionSwitchReason.SessionLock)
        //    {
        //        lockCount++;
        //        Console.WriteLine($"{DateTime.Now}: System Locked. Lock Count: {lockCount}");
        //    }
        //    else if (e.Reason == SessionSwitchReason.SessionUnlock)
        //    {
        //        unlockCount++;
        //        Console.WriteLine($"{DateTime.Now}: System Unlocked. Unlock Count: {unlockCount}");
        //    }
        //}

        private static void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                lockCount++;
                lockStartTime = DateTime.Now; // Record lock start time
                Console.WriteLine($"{DateTime.Now}: System Locked. Lock Count: {lockCount}");
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                unlockCount++;
                if (lockStartTime.HasValue)
                {
                    // Calculate the lock duration
                    TimeSpan lockDuration = DateTime.Now - lockStartTime.Value;
                    totalLockDuration += lockDuration; // Update total lock duration
                    lockStartTime = null; // Reset lock start time
                    Console.WriteLine($"{DateTime.Now}: System Unlocked. Unlock Count: {unlockCount}");
                    Console.WriteLine($"Last Lock Duration: {lockDuration}");
                    Console.WriteLine($"Total Lock Duration: {totalLockDuration}");
                }
            }
        }

        private static string GetSystemId()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    return queryObj["SerialNumber"]?.ToString().Trim() ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving system ID: {ex.Message}");
            }
            return "Unknown";
        }

        private static string GetSystemIp()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                return ip?.ToString() ?? "Unknown IP";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving system IP: {ex.Message}");
            }
            return "Unknown IP";
        }

        private static string GetActiveWindowTitle()
        {
            IntPtr handle = GetForegroundWindow();
            StringBuilder windowTitle = new StringBuilder(256);
            if (GetWindowText(handle, windowTitle, 256) > 0)
            {
                return windowTitle.ToString();
            }
            return null;
        }

        private static void LogToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"=== Log Time: {DateTime.Now} ===");
                    writer.WriteLine($"System Name: {systemName}");
                    writer.WriteLine($"System ID: {systemId}");
                    writer.WriteLine($"User Name: {userName}");
                    writer.WriteLine($"System IP: {systemIp}");
                    writer.WriteLine($"Total System Usage: {totalSystemUsage}");

                    writer.WriteLine("\n=== Application Usage ===");
                    foreach (var app in appUsage)
                    {
                        writer.WriteLine($"{app.Key}: {app.Value}");
                    }

                    writer.WriteLine("\n=== Lock/Unlock Events ===");
                    writer.WriteLine($"Lock Count: {lockCount}");
                    writer.WriteLine($"Unlock Count: {unlockCount}");
                    writer.WriteLine($"Total Lock Duration: {totalLockDuration}");
                    writer.WriteLine(new string('=', 50));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}

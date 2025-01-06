using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WindowaService
{
    internal class SystemActivityLogic
    {
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
        private static string systemMacId = GetSystemId();
        private static string userName = Environment.UserName;
        private static string systemIp = GetSystemIp();
        private static string screenCaptureFolder = "ScreenCaptures";
        private static List<string> list = new List<string>();

        public static void SendSystemActivity()
        {
            try
            {
                list = new List<string>();

                Console.WriteLine("System Usage Tracker");
                Console.WriteLine($"System Name: {systemName}");
                Console.WriteLine($"System Mac Id: {systemMacId}");
                Console.WriteLine($"User Name: {userName}");
                Console.WriteLine($"System IP: {systemIp}");
                Console.WriteLine($"Log File: {Path.GetFullPath(logFilePath)}");
                Console.WriteLine("Tracking lock/unlock events and application usage. Press Ctrl+C to stop.\n");

                // Subscribe to lock/unlock events
                SystemEvents.SessionSwitch += OnSessionSwitch;

                // Start application usage tracking
                DateTime systemStartTime = DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount64);

                // Monitor application usage in a loop
                while (true)
                {
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
                        }
                    }

                    // Calculate total system usage
                    totalSystemUsage = DateTime.Now - systemStartTime;

                    // Log every 10 seconds
                    if (DateTime.Now.Minute % 10 == 0)
                    {
                        if (lockCount == unlockCount)
                        {
                            //Console.WriteLine("=== System Usage Tracker ===");
                            //Console.WriteLine($"Total System Usage: {totalSystemUsage}");

                            //Console.WriteLine("\n=== Application Usage ===");
                            //foreach (var app in appUsage)
                            //{
                            //    Console.WriteLine($"{app.Key}: {app.Value}");
                            //}

                            //Console.WriteLine("\n=== Lock/Unlock Events ===");
                            //Console.WriteLine($"Lock Count: {lockCount}");
                            //Console.WriteLine($"Unlock Count: {unlockCount}");
                            LogToFile();
                        }
                    }
                    Thread.Sleep(1000); // Check every second
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

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
                    //Console.WriteLine($"{DateTime.Now}: System Unlocked. Unlock Count: {unlockCount}");
                    //Console.WriteLine($"Last Lock Duration: {lockDuration}");
                    //Console.WriteLine($"Total Lock Duration: {totalLockDuration}");
                }
            }
        }
        private static string GetSystemId()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)// only return MAC Address from first card
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                }
                return sMacAddress;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error retrieving system ID: {ex.Message}");
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
                //Console.WriteLine($"Error retrieving system IP: {ex.Message}");
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
            return String.Empty;
        }

        private static async Task LogToFile()
        {
            try
            {

                //using (StreamWriter writer = new StreamWriter(logFilePath, true))
                //{
                //writer.WriteLine($"=== Log Time: {DateTime.Now} ===");
                ////writer.WriteLine($"System Name: {systemName}");
                ////writer.WriteLine($"System ID: {systemMacId}");
                ////writer.WriteLine($"User Name: {userName}");
                ////writer.WriteLine($"System IP: {systemIp}");
                ////writer.WriteLine($"Total System Usage: {totalSystemUsage}");

                ////writer.WriteLine("\n=== Application Usage ===");
                EventDataInfo data = new EventDataInfo();
                data.SystemMacId = systemMacId;
                data.TotalSystemUsage = totalSystemUsage;
                data.SystemLockActivity = new SystemLockActivityInfo()
                {
                    LockCount = lockCount,
                    LockDuration = totalLockDuration
                };

                var appUsageList = new List<SystemAppActivityInfo>();
                foreach (var app in appUsage)
                {
                    //writer.WriteLine($"{app.Key}: {app.Value}");
                    appUsageList.Add(new SystemAppActivityInfo() { App = app.Key, Duration = app.Value }); 
                }
                data.SystemAppActivity = appUsageList;

                string jsonData = JsonSerializer.Serialize(data);

                using (HttpClient client = new HttpClient())
                {
                    // Set headers
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    string strURI = $"http://35.171.157.252/Test/api/SystemUsedActivity?compCode=Juggu00909&empCode=Emp0003&systemActivity={jsonData}";
                    HttpResponseMessage response = await client.PostAsync(strURI, null);
                    if (response.IsSuccessStatusCode)
                    {

                    }
                }

                ////writer.WriteLine("\n=== Lock/Unlock Events ===");

                //writer.WriteLine($"Lock Count: {lockCount} Unlock Count: {unlockCount} Total Lock Duration: {totalLockDuration}");

                //writer.WriteLine(new string('=', 50));


                appUsage.Clear();
                //}
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }

    public class EventDataInfo
    {
        public string SystemMacId { get; set; }
        public TimeSpan TotalSystemUsage { get; set; }
        public SystemLockActivityInfo SystemLockActivity { get; set; }
        public List<SystemAppActivityInfo> SystemAppActivity { get; set; }
    }

    public class SystemLockActivityInfo
    {
        public int LockCount { get; set; }
        public TimeSpan LockDuration { get; set; }       
    }

    public class SystemAppActivityInfo
    {
        public string App { get; set; }
        public TimeSpan Duration { get; set; }
    }
}

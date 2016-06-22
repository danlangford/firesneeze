using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class CrashSender
{
    [CompilerGenerated]
    private static AddCrashFileDelegate <>f__am$cache6;
    private string Arguments = string.Empty;
    public const string CONFIG_FILE_LOCATION = @"Assets\data\Debug\tennessee.crashbuddyconfig";
    public const string CONFIG_FILE_LOCATION_BACKUP = @"Tennesee_Data\Data\Debug\tennessee.crashbuddyconfig";
    public static string ConfigFilename = string.Empty;
    private string DumpDirectory = string.Empty;
    private bool IsCrashBuddyQueued;
    private static List<AddCrashFileDelegate> s_cAddCrashFileHandlers = new List<AddCrashFileDelegate>();
    private string ScreenShotFilename = string.Empty;

    public CrashSender()
    {
        if (s_cAddCrashFileHandlers.Count == 0)
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new AddCrashFileDelegate(CrashSender.<CrashSender>m__ED);
            }
            AddCrashFileHandler(<>f__am$cache6);
        }
    }

    [CompilerGenerated]
    private static void <CrashSender>m__ED(string sDumpDirectory)
    {
        string str = string.Empty;
        using (StreamWriter writer = new StreamWriter(Path.Combine(sDumpDirectory, "tool_console.txt")))
        {
            writer.Write(str);
        }
    }

    public static void AddCrashFileHandler(AddCrashFileDelegate cHandler)
    {
        s_cAddCrashFileHandlers.Add(cHandler);
    }

    private void BugHandling()
    {
        if (!IsCrashBuddyInstalled())
        {
            UnityEngine.Debug.LogWarning("Could not create a bug report because Crash Buddy is not installed.");
        }
        else
        {
            try
            {
                string str;
                string str2;
                this.GetCrashDumpPathAndCreateIfNecessary(out str, out str2);
                WriteBuildVersionToDirectory(str);
                try
                {
                    foreach (AddCrashFileDelegate delegate2 in s_cAddCrashFileHandlers)
                    {
                        try
                        {
                            delegate2(str);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
                if (!Application.isEditor || Application.isPlaying)
                {
                    this.TakeScreenshotsForBug(str);
                }
                this.DumpDirectory = str;
                this.IsCrashBuddyQueued = true;
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogWarning("Crash Buddy Setup Failed: " + exception.Message);
            }
        }
    }

    private static string GetCrashBuddyInstallKey() => 
        @"HKEY_CURRENT_USER\Software\Obsidian Entertainment\Crash Buddy\Environment";

    private static string GetCrashBuddyInstallValue() => 
        "InstallDir";

    private void GetCrashDumpPathAndCreateIfNecessary(out string sDumpDirectory, out string sDumpID)
    {
        string tempPath = Path.GetTempPath();
        string str2 = Path.ChangeExtension(Process.GetCurrentProcess().MainModule.ModuleName, null).Replace(" ", string.Empty);
        List<char> list = new List<char>();
        foreach (char ch in Path.GetInvalidPathChars())
        {
            list.Add(ch);
        }
        foreach (char ch2 in Path.GetInvalidFileNameChars())
        {
            if (!list.Contains(ch2))
            {
                list.Add(ch2);
            }
        }
        foreach (char ch3 in list)
        {
            str2 = str2.Replace(new string(ch3, 1), string.Empty);
        }
        string path = Path.ChangeExtension(Path.Combine(tempPath, str2), null);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        DateTime now = DateTime.Now;
        string userName = Environment.UserName;
        if (string.IsNullOrEmpty(userName))
        {
            userName = "unknown_user";
        }
        string str5 = now.Year.ToString("0000");
        string str6 = now.Month.ToString("00");
        string str7 = now.Day.ToString("00");
        string str8 = now.Hour.ToString("00");
        string str9 = now.Minute.ToString("00");
        string str10 = now.Second.ToString("00");
        string str11 = Process.GetCurrentProcess().Id.ToString();
        string str12 = Thread.CurrentThread.ManagedThreadId.ToString();
        sDumpID = $"{userName}-{str2}-{str5}{str6}{str7}-{str8}{str9}{str10}-{str11}-{str12}";
        sDumpDirectory = Path.Combine(path, sDumpID);
        if (!Directory.Exists(sDumpDirectory))
        {
            Directory.CreateDirectory(sDumpDirectory);
        }
    }

    private static string GetLagacyCrashBuddyInstallKey() => 
        @"HKEY_CURRENT_USER\Software\Obsidian Entertainment, Inc\Crash Buddy\Environment";

    public static bool IsCrashBuddyInstalled()
    {
        string str = Registry.GetValue(GetCrashBuddyInstallKey(), GetCrashBuddyInstallValue(), null) as string;
        if (string.IsNullOrEmpty(str))
        {
            str = Registry.GetValue(GetLagacyCrashBuddyInstallKey(), GetCrashBuddyInstallValue(), null) as string;
        }
        return (!string.IsNullOrEmpty(str) && Directory.Exists(str));
    }

    public void LaunchCrashBuddy(string sDumpDirectory, string arguments)
    {
        string str = string.Empty;
        string str2 = Registry.GetValue(GetCrashBuddyInstallKey(), GetCrashBuddyInstallValue(), null) as string;
        if (string.IsNullOrEmpty(str2))
        {
            str2 = Registry.GetValue(GetLagacyCrashBuddyInstallKey(), GetCrashBuddyInstallValue(), null) as string;
        }
        if (!string.IsNullOrEmpty(str2) && Directory.Exists(str2))
        {
            str = Path.Combine(str2, "CrashBuddy.exe");
        }
        else
        {
            UnityEngine.Debug.LogWarning("Crash Buddy Install Dir Does Not Exist Dir: " + str2);
            return;
        }
        if (!string.IsNullOrEmpty(str) && File.Exists(str))
        {
            try
            {
                Process process = new Process();
                ConfigFilename = Path.Combine(Environment.CurrentDirectory, @"Assets\data\Debug\tennessee.crashbuddyconfig");
                if (!File.Exists(ConfigFilename))
                {
                    ConfigFilename = Path.Combine(Environment.CurrentDirectory, @"Tennesee_Data\Data\Debug\tennessee.crashbuddyconfig");
                }
                string[] textArray1 = new string[] { "-configfile \"", ConfigFilename, "\" -dir \"", sDumpDirectory, "\" ", arguments };
                string str3 = string.Concat(textArray1);
                ProcessStartInfo info = new ProcessStartInfo(str, str3) {
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(str)
                };
                process.StartInfo = info;
                process.Start();
                if (!Screen.fullScreen)
                {
                    process.WaitForExit();
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogWarning("Crash Buddy Failed: " + exception.Message);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Crash Buddy Does Not Exist: " + str);
        }
    }

    public static void RemoveCrashFileHandler(AddCrashFileDelegate cHandler)
    {
        s_cAddCrashFileHandlers.Remove(cHandler);
    }

    private void TakeScreenshotsForBug(string sDumpDirectory)
    {
        string path = @"c:\Documents and Settings\" + Environment.UserName + @"\Local Settings\Application Data\Unity\Editor\Editor.log";
        string[] strArray = Directory.GetFiles(Environment.CurrentDirectory, "output_log.txt", SearchOption.AllDirectories);
        if (strArray.Length > 0)
        {
            try
            {
                File.Copy(strArray[0], Path.Combine(sDumpDirectory, "output_log.txt"));
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogWarning("TakeScreenshotsForBug - Error copying game log: " + exception.Message);
            }
        }
        path = @"tennesee_Data\output_log.txt";
        if (File.Exists(path))
        {
            try
            {
                File.Copy(path, Path.Combine(sDumpDirectory, "game_log.txt"));
            }
            catch (Exception exception2)
            {
                UnityEngine.Debug.LogWarning("TakeScreenshotsForBug - Error copying game log: " + exception2.Message);
            }
        }
        string filename = Path.Combine(sDumpDirectory, "assert_screenshot.png");
        try
        {
            File.Copy(GameDirectory.PathToFile(Game.SaveSlot, "game.sav"), Path.Combine(sDumpDirectory, "preturn.sav"), true);
        }
        catch (Exception exception3)
        {
            UnityEngine.Debug.LogWarning("TakeScreenshotsForBug - Error copying save file: " + exception3.Message);
        }
        try
        {
            Game.Save();
        }
        catch (Exception exception4)
        {
            UnityEngine.Debug.LogWarning("TakeScreenshotsForBug - Error creating save file: " + exception4.Message);
        }
        try
        {
            File.Copy(GameDirectory.PathToFile(Game.SaveSlot, "game.sav"), Path.Combine(sDumpDirectory, "bug.sav"), true);
        }
        catch (Exception exception5)
        {
            UnityEngine.Debug.LogWarning("TakeScreenshotsForBug - Error copying save file: " + exception5.Message);
        }
        try
        {
            Application.CaptureScreenshot(filename);
            this.ScreenShotFilename = filename;
        }
        catch (Exception exception6)
        {
            UnityEngine.Debug.LogWarning("CaptureScreenshot Failed: " + exception6.Message);
        }
    }

    public void TriggerBug(string arguments)
    {
        this.Arguments = arguments;
        this.BugHandling();
    }

    public void Update()
    {
        if (this.IsCrashBuddyQueued && (File.Exists(this.ScreenShotFilename) || (Application.isEditor && !Application.isPlaying)))
        {
            this.LaunchCrashBuddy(this.DumpDirectory, this.Arguments);
            this.IsCrashBuddyQueued = false;
            this.DumpDirectory = string.Empty;
            this.ScreenShotFilename = string.Empty;
            this.Arguments = string.Empty;
        }
    }

    public static void WriteBuildVersionToDirectory(string sDumpDirectory)
    {
        try
        {
            using (FileStream stream = new FileStream(Path.Combine(sDumpDirectory, "build_version.txt"), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                }
            }
        }
        catch
        {
        }
    }

    public delegate void AddCrashFileDelegate(string sCrashDumpDirectory);
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Prototype1
{
    public enum FocusUsageState
    {
        Active = 0,
        Idle = 1,
        Break = 2
    }

    public sealed class ForegroundAppInfo
    {
        public string ProcessName { get; set; } = "unknown";

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ProcessName))
                {
                    return ProcessName;
                }

                return "unknown";
            }
        }
    }

    public sealed class AppUsageSegment
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string ProcessName { get; set; } = "unknown";
        public FocusUsageState State { get; set; }

        public int DurationSeconds
        {
            get
            {
                double seconds = (EndAt - StartAt).TotalSeconds;
                return seconds <= 0 ? 0 : (int)Math.Round(seconds);
            }
        }
    }

    public sealed class FocusSessionRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Goal { get; set; } = "Focus session";
        public string Category { get; set; } = "Direct";
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public List<AppUsageSegment> Segments { get; set; } = new List<AppUsageSegment>();
        public int ActiveSeconds { get; set; }
        public int BreakSeconds { get; set; }
        public int AppSwitchCount { get; set; }

        public int TotalSeconds
        {
            get
            {
                double seconds = (EndedAt - StartedAt).TotalSeconds;
                return seconds <= 0 ? 0 : (int)Math.Round(seconds);
            }
        }
    }

    public sealed class AppUsageSummary
    {
        public string AppName { get; set; } = "unknown";
        public int ActiveSeconds { get; set; }
        public int BreakSeconds { get; set; }
        public int SwitchEntries { get; set; }
    }

    public static class FocusSessionStore
    {
        private const string StoreFileName = "FocusSessions.json";

        private static string StorePath
        {
            get
            {
                Directory.CreateDirectory(Application.UserAppDataPath);
                return Path.Combine(Application.UserAppDataPath, StoreFileName);
            }
        }

        public static List<FocusSessionRecord> LoadSessions()
        {
            try
            {
                if (!File.Exists(StorePath))
                {
                    return new List<FocusSessionRecord>();
                }

                string json = File.ReadAllText(StorePath);
                FocusSessionHistory history = JsonSerializer.Deserialize<FocusSessionHistory>(json);
                return history != null && history.Sessions != null
                    ? history.Sessions
                    : new List<FocusSessionRecord>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load focus session history: " + ex.Message);
                return new List<FocusSessionRecord>();
            }
        }

        public static void AppendSession(FocusSessionRecord session)
        {
            if (session == null)
            {
                return;
            }

            try
            {
                List<FocusSessionRecord> sessions = LoadSessions();
                sessions.Add(session);
                sessions = sessions
                    .OrderBy(s => s.StartedAt)
                    .TakeLastCompat(1000)
                    .ToList();

                FocusSessionHistory history = new FocusSessionHistory { Sessions = sessions };
                string json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(StorePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save focus session history: " + ex.Message);
            }
        }

        public sealed class FocusSessionHistory
        {
            public List<FocusSessionRecord> Sessions { get; set; } = new List<FocusSessionRecord>();
        }
    }

    public static class FocusSessionReportBuilder
    {
        public static List<AppUsageSummary> BuildAppUsage(FocusSessionRecord session)
        {
            if (session == null || session.Segments == null)
            {
                return new List<AppUsageSummary>();
            }

            Dictionary<string, AppUsageSummary> byApp = new Dictionary<string, AppUsageSummary>(StringComparer.OrdinalIgnoreCase);

            foreach (AppUsageSegment segment in session.Segments)
            {
                if (segment == null || segment.DurationSeconds <= 0 || segment.State == FocusUsageState.Idle)
                {
                    continue;
                }

                string appName = string.IsNullOrWhiteSpace(segment.ProcessName) ? "unknown" : segment.ProcessName;
                if (!byApp.TryGetValue(appName, out AppUsageSummary summary))
                {
                    summary = new AppUsageSummary { AppName = appName };
                    byApp.Add(appName, summary);
                }

                if (segment.State == FocusUsageState.Break)
                {
                    summary.BreakSeconds += segment.DurationSeconds;
                }
                else
                {
                    summary.ActiveSeconds += segment.DurationSeconds;
                }
            }

            foreach (AppUsageSegment segment in GetSwitchableSegments(session))
            {
                string appName = string.IsNullOrWhiteSpace(segment.ProcessName) ? "unknown" : segment.ProcessName;
                if (byApp.TryGetValue(appName, out AppUsageSummary summary))
                {
                    summary.SwitchEntries++;
                }
            }

            return byApp.Values
                .OrderByDescending(s => s.ActiveSeconds)
                .ThenByDescending(s => s.ActiveSeconds + s.BreakSeconds)
                .ToList();
        }

        public static string FormatDuration(int seconds)
        {
            if (seconds < 0)
            {
                seconds = 0;
            }

            TimeSpan value = TimeSpan.FromSeconds(seconds);
            if (value.TotalHours >= 1)
            {
                return string.Format("{0}시간 {1:D2}분", (int)value.TotalHours, value.Minutes);
            }

            if (value.TotalMinutes >= 1)
            {
                return string.Format("{0}분 {1:D2}초", (int)value.TotalMinutes, value.Seconds);
            }

            return string.Format("{0}초", value.Seconds);
        }

        public static string BuildSessionSummaryText(FocusSessionRecord session)
        {
            if (session == null)
            {
                return "선택된 세션이 없습니다.";
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("목표: " + DisplayText(session.Goal));
            builder.AppendLine("카테고리: " + DisplayText(session.Category));
            builder.AppendLine("시간: " + session.StartedAt.ToString("HH:mm") + " - " + session.EndedAt.ToString("HH:mm"));
            builder.AppendLine("활성 집중: " + FormatDuration(session.ActiveSeconds));
            builder.AppendLine("휴식: " + FormatDuration(session.BreakSeconds));
            builder.AppendLine("앱 전환: " + session.AppSwitchCount);
            return builder.ToString();
        }

        private static string DisplayText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }

        private static IEnumerable<AppUsageSegment> GetSwitchableSegments(FocusSessionRecord session)
        {
            if (session == null || session.Segments == null)
            {
                yield break;
            }

            foreach (AppUsageSegment segment in session.Segments)
            {
                if (segment == null)
                {
                    continue;
                }

                if (segment.State != FocusUsageState.Active || segment.DurationSeconds < 2)
                {
                    continue;
                }

                yield return segment;
            }
        }
    }

    public static class FocusSessionTelemetry
    {
        private static readonly TimeSpan IdleThreshold = TimeSpan.FromMinutes(2);
        private static FocusSessionRecord currentSession;
        private static AppUsageSegment openSegment;

        public static FocusSessionRecord LastCompletedSession { get; private set; }

        public static bool IsSessionActive
        {
            get { return currentSession != null; }
        }

        public static void StartSession(
            DateTime startedAt,
            string goal,
            string category)
        {
            if (currentSession != null)
            {
                CompleteSession(DateTime.Now);
            }

            LastCompletedSession = null;
            currentSession = new FocusSessionRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                Goal = string.IsNullOrWhiteSpace(goal) ? "Focus session" : goal.Trim(),
                Category = string.IsNullOrWhiteSpace(category) ? "Direct" : category.Trim(),
                StartedAt = startedAt,
                EndedAt = startedAt
            };

            openSegment = null;
            CaptureTick();
        }

        public static void CaptureTick()
        {
            if (currentSession == null)
            {
                return;
            }

            DateTime now = DateTime.Now;
            ForegroundAppInfo foreground = ForegroundWindowReader.GetForegroundAppInfo();
            TimeSpan idleTime = IdleDetector.GetIdleTime();
            FocusUsageState state = DataModel.IsBreakActive
                ? FocusUsageState.Break
                : idleTime >= IdleThreshold ? FocusUsageState.Idle : FocusUsageState.Active;

            if (openSegment == null || !IsSameSegment(openSegment, foreground, state))
            {
                CloseOpenSegment(now);
                openSegment = CreateSegment(now, foreground, state);
            }
            else
            {
                openSegment.EndAt = now;
            }

            currentSession.EndedAt = now;
        }

        public static FocusSessionRecord CompleteSession(DateTime endedAt)
        {
            if (currentSession == null)
            {
                LastCompletedSession = null;
                return null;
            }

            CloseOpenSegment(endedAt);
            currentSession.EndedAt = endedAt;
            RecalculateSummary(currentSession);

            FocusSessionRecord completed = currentSession;
            FocusSessionStore.AppendSession(completed);
            LastCompletedSession = completed;

            currentSession = null;
            openSegment = null;
            return completed;
        }

        private static AppUsageSegment CreateSegment(DateTime startedAt, ForegroundAppInfo foreground, FocusUsageState state)
        {
            return new AppUsageSegment
            {
                StartAt = startedAt,
                EndAt = startedAt,
                ProcessName = foreground.DisplayName,
                State = state
            };
        }

        private static void CloseOpenSegment(DateTime endAt)
        {
            if (currentSession == null || openSegment == null)
            {
                return;
            }

            openSegment.EndAt = endAt;
            if (openSegment.EndAt < openSegment.StartAt)
            {
                openSegment.EndAt = openSegment.StartAt;
            }

            if (openSegment.DurationSeconds > 0)
            {
                currentSession.Segments.Add(openSegment);
            }

            openSegment = null;
        }

        private static bool IsSameSegment(AppUsageSegment segment, ForegroundAppInfo foreground, FocusUsageState state)
        {
            if (segment.State != state)
            {
                return false;
            }

            string processName = foreground == null ? string.Empty : foreground.DisplayName;
            return string.Equals(segment.ProcessName, processName, StringComparison.OrdinalIgnoreCase);
        }

        private static void RecalculateSummary(FocusSessionRecord session)
        {
            session.ActiveSeconds = 0;
            session.BreakSeconds = 0;

            foreach (AppUsageSegment segment in session.Segments)
            {
                if (segment.State == FocusUsageState.Idle)
                {
                    continue;
                }
                else if (segment.State == FocusUsageState.Break)
                {
                    session.BreakSeconds += segment.DurationSeconds;
                }
                else
                {
                    session.ActiveSeconds += segment.DurationSeconds;
                }
            }

            session.AppSwitchCount = CalculateAppSwitchCount(session.Segments);
        }

        private static int CalculateAppSwitchCount(List<AppUsageSegment> segments)
        {
            int count = 0;
            string previousApp = null;

            foreach (AppUsageSegment segment in segments)
            {
                if (segment.State != FocusUsageState.Active || segment.DurationSeconds < 2)
                {
                    continue;
                }

                string appName = string.IsNullOrWhiteSpace(segment.ProcessName) ? "unknown" : segment.ProcessName;
                if (previousApp != null && !string.Equals(previousApp, appName, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }

                previousApp = appName;
            }

            return count;
        }

    }

    internal static class ForegroundWindowReader
    {
        public static ForegroundAppInfo GetForegroundAppInfo()
        {
            IntPtr handle = NativeMethods.GetForegroundWindow();
            if (handle == IntPtr.Zero)
            {
                return new ForegroundAppInfo();
            }

            uint processId;
            NativeMethods.GetWindowThreadProcessId(handle, out processId);

            ForegroundAppInfo info = new ForegroundAppInfo();

            try
            {
                using (Process process = Process.GetProcessById(unchecked((int)processId)))
                {
                    info.ProcessName = process.ProcessName;
                }
            }
            catch
            {
                info.ProcessName = "unknown";
            }

            return info;
        }
    }

    internal static class IdleDetector
    {
        public static TimeSpan GetIdleTime()
        {
            NativeMethods.LASTINPUTINFO info = new NativeMethods.LASTINPUTINFO();
            info.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.LASTINPUTINFO));

            if (!NativeMethods.GetLastInputInfo(ref info))
            {
                return TimeSpan.Zero;
            }

            uint idleTicks = unchecked((uint)Environment.TickCount - info.dwTime);
            return TimeSpan.FromMilliseconds(idleTicks);
        }
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> TakeLastCompat<T>(this IEnumerable<T> source, int count)
        {
            Queue<T> queue = new Queue<T>();

            foreach (T item in source)
            {
                queue.Enqueue(item);
                if (queue.Count > count)
                {
                    queue.Dequeue();
                }
            }

            return queue;
        }
    }
}

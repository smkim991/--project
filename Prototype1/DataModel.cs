using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Prototype1
{
    public static class DataModel
    {
        public const uint INITIAL_LIFE_COUNT = 3;
        public const int LIFE_BREAK_MINUTES = 5;
        public const int STOP_COUNTDOWN_SECONDS = 10;

        // 차단할 프로세스 명칭을 저장하는 전역 리스트
        public static List<string> SavedBlockList { get; set; } = new List<string>();

        // 차단할 웹사이트의 키워드 (ex. "youtube", "유튜브", "netflix", "넷플릭스" etc..)
        public static List<string> SavedWebBlockKeywordList { get; set; } = new List<string>();

        // 카테고리별 차단 항목 목록
        public static Dictionary<string, List<string>> BlockProfiles { get; set; } = CreateDefaultBlockProfiles();

        // 현재 차단 기능이 활성화(ON) 상태인지 확인하는 변수
        public static bool IsBlockingActive { get; set; } = false;

        // 라이프를 사용해 일시적으로 차단을 해제한 상태인지 확인하는 변수
        public static bool IsBreakActive { get; set; } = false;

        // 라이프 사용으로 얻은 자유시간 종료 시각
        public static DateTime BreakEndTime { get; set; } = DateTime.MinValue;

        // 긴급 종료 후 기존 집중세션 종료 시각까지 재시작을 막기 위한 시각
        public static DateTime EmergencyLockUntil { get; set; } = DateTime.MinValue;

        // 매 자정이 지나서 프로그램 실행할 시 3으로 초기화
        public static uint Life { get; set; } = INITIAL_LIFE_COUNT;

        // 마지막으로 리셋된 날짜보다 오늘의 날짜가 더 크다면 자정(00시) 라인을 통과한 것
        public static DateTime LastResetTime { get; set; } = DateTime.MinValue;

        // 다음 타이머 틱에서 FocusEndTime 검사를 건너뛸지 여부를 나타내는 플래그
        public static bool SkipNextFocusEndCheck { get; set; } = false;


        // 집중 종료 시각
        public static DateTime FocusEndTime = DateTime.MinValue;

        private static TimeSpan _pausedFocusRemainingTime = TimeSpan.Zero;

        public static TimeSpan PausedFocusRemainingTime
        {
            get { return _pausedFocusRemainingTime; }
        }

        public static string CurrentFocusGoal { get; set; } = string.Empty;

        public static string CurrentFocusCategory { get; set; } = string.Empty;

        public static bool IsEmergencyLockedOut
        {
            get { return DateTime.Now < EmergencyLockUntil; }
        }

        public static DateTime TodayMidnight
        {
            get { return DateTime.Today.AddDays(1); }
        }

        // JSON 파일 저장 경로: Prototype1 -> bin -> Debug
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static Dictionary<string, List<string>> CreateDefaultBlockProfiles()
        {
            return new Dictionary<string, List<string>>
            {
                { "대학생", new List<string> { "넷플릭스", "네이버웹툰" } },
                { "개발자", new List<string> { "유튜브", "메모장", "멜론" } },
                { "영상편집자", new List<string> { "인스타그램", "엑셀" } },
                { "수험생", new List<string> { "카카오톡", "인스타그램", "틱톡" } }
            };
        }

        public static Dictionary<string, List<string>> GetBlockProfilesCopy()
        {
            return CloneBlockProfiles(BlockProfiles);
        }

        public static void UpdateBlockProfiles(Dictionary<string, List<string>> profiles)
        {
            BlockProfiles = CloneBlockProfiles(profiles);
            SaveToJson();
        }

        public static void SetActiveBlockListForCategory(string categoryName)
        {
            SavedBlockList.Clear();

            if (BlockProfiles.TryGetValue(categoryName, out List<string> blockedItems))
            {
                SavedBlockList.AddRange(blockedItems);
            }

            SaveToJson();
        }

        public static void StartFocusSession(DateTime focusEndTime)
        {
            DateTime startedAt = DateTime.Now;
            FocusEndTime = focusEndTime;
            IsBlockingActive = true;
            IsBreakActive = false;
            BreakEndTime = DateTime.MinValue;
            _pausedFocusRemainingTime = TimeSpan.Zero;
            SkipNextFocusEndCheck = false;
            FocusSessionTelemetry.StartSession(
                startedAt,
                CurrentFocusGoal,
                CurrentFocusCategory);
            SaveToJson();
        }

        public static void CompleteFocusSession()
        {
            FocusSessionTelemetry.CompleteSession(DateTime.Now);
            IsBlockingActive = false;
            IsBreakActive = false;
            BreakEndTime = DateTime.MinValue;
            FocusEndTime = DateTime.MinValue;
            _pausedFocusRemainingTime = TimeSpan.Zero;
            SkipNextFocusEndCheck = false;
            CurrentFocusGoal = string.Empty;
            CurrentFocusCategory = string.Empty;
            SaveToJson();
        }

        public static bool StartLifeBreak()
        {
            if (Life == 0)
            {
                return false;
            }
            if (IsBreakActive) // 이미 자유시간 중이면 또 시작할 수 없음
            {
                return true;
            }

            Life--;
            IsBreakActive = true;
            BreakEndTime = DateTime.Now.AddMinutes(LIFE_BREAK_MINUTES);

            _pausedFocusRemainingTime = FocusEndTime - DateTime.Now;
            if (_pausedFocusRemainingTime < TimeSpan.Zero)
            {
                _pausedFocusRemainingTime = TimeSpan.Zero;
            }
            FocusEndTime = DateTime.MinValue;

            SkipNextFocusEndCheck = false;
            SaveToJson();
            return true;
        }

        public static void EndLifeBreak()
        {
            IsBreakActive = false;
            BreakEndTime = DateTime.MinValue;

            FocusEndTime = DateTime.Now + _pausedFocusRemainingTime;
            _pausedFocusRemainingTime = TimeSpan.Zero;

            SkipNextFocusEndCheck = true;
            SaveToJson();
        }

        public static bool EmergencyStopFocusSession()
        {
            if (Life == 0)
            {
                return false;
            }

            Life = 0;
            EmergencyLockUntil = TodayMidnight;

            IsBlockingActive = false;
            IsBreakActive = false;
            BreakEndTime = DateTime.MinValue;
            FocusEndTime = DateTime.MinValue;
            _pausedFocusRemainingTime = TimeSpan.Zero;
            SkipNextFocusEndCheck = false;
            FocusSessionTelemetry.CompleteSession(DateTime.Now);
            CurrentFocusGoal = string.Empty;
            CurrentFocusCategory = string.Empty;
            SaveToJson();
            return true;
        }

        private class AppData
        {
            public List<string> SavedBlockList { get; set; }
            public List<string> SavedWebBlockKeywordList { get; set; }
            public Dictionary<string, List<string>> BlockProfiles { get; set; }
            public bool IsBlockingActive { get; set; }
            public bool IsBreakActive { get; set; }
            public DateTime FocusEndTime { get; set; }
            public DateTime BreakEndTime { get; set; }
            public TimeSpan PausedFocusRemainingTime { get; set; }
            public uint Life { get; set; }
            public DateTime EmergencyLockUntil { get; set; }
            public DateTime LastResetTime { get; set; }
            public bool SkipNextFocusEndCheck { get; set; }
        }

        // 현재 DataModel 속성 값들을 JSON 파일에 저장
        public static void SaveToJson()
        {
            try
            {
                var saveData = new AppData
                {
                    SavedBlockList = SavedBlockList,
                    SavedWebBlockKeywordList = SavedWebBlockKeywordList,
                    BlockProfiles = BlockProfiles,
                    IsBlockingActive = IsBlockingActive,
                    IsBreakActive = IsBreakActive,
                    FocusEndTime = FocusEndTime,
                    BreakEndTime = BreakEndTime,
                    PausedFocusRemainingTime = _pausedFocusRemainingTime,
                    Life = Life,
                    EmergencyLockUntil = EmergencyLockUntil,
                    LastResetTime = LastResetTime,
                    SkipNextFocusEndCheck = SkipNextFocusEndCheck
                };

                string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON 저장 실패: {ex.Message}");
            }
        }

        // JSON 파일을 읽어 DataModel 속성 값들을 복원
        public static void LoadFromJson()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    ResetToInitialState();
                    SaveToJson();
                    return;
                }

                string jsonString = File.ReadAllText(FilePath);
                var loadedData = JsonSerializer.Deserialize<AppData>(jsonString);

                if (loadedData != null)
                {
                    SavedBlockList = loadedData.SavedBlockList ?? new List<string>();
                    SavedWebBlockKeywordList = loadedData.SavedWebBlockKeywordList ?? new List<string>();
                    BlockProfiles = CloneBlockProfiles(loadedData.BlockProfiles);
                    IsBlockingActive = loadedData.IsBlockingActive;
                    IsBreakActive = loadedData.IsBreakActive;
                    FocusEndTime = loadedData.FocusEndTime;
                    BreakEndTime = loadedData.BreakEndTime;
                    _pausedFocusRemainingTime = loadedData.PausedFocusRemainingTime;
                    Life = loadedData.Life;
                    EmergencyLockUntil = loadedData.EmergencyLockUntil;
                    LastResetTime = loadedData.LastResetTime == DateTime.MinValue
                        ? DateTime.Now
                        : loadedData.LastResetTime;
                    SkipNextFocusEndCheck = loadedData.SkipNextFocusEndCheck;
                }

                CheckMidnightReset();
                ClearExpiredEmergencyLock();

                if (IsBreakActive && DateTime.Now >= BreakEndTime)
                {
                    EndLifeBreak();
                }
                else if (IsBlockingActive && !IsBreakActive && DateTime.Now >= FocusEndTime)
                {
                    CompleteFocusSession();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON 불러오기 실패: {ex.Message}");
                ResetToInitialState();
            }
        }

        private static void CheckMidnightReset()
        {
            DateTime now = DateTime.Now;

            // 마지막으로 리셋된 날짜보다 오늘의 날짜가 더 크다면 자정(00시) 라인을 통과한 것
            if (LastResetTime.Date < now.Date)
            {
                Life = INITIAL_LIFE_COUNT;
                LastResetTime = now;
                EmergencyLockUntil = DateTime.MinValue;
                SaveToJson(); // 바뀐 상태를 JSON 파일에 즉시 저장
            }
        }

        private static void ClearExpiredEmergencyLock()
        {
            if (EmergencyLockUntil != DateTime.MinValue && DateTime.Now >= EmergencyLockUntil)
            {
                EmergencyLockUntil = DateTime.MinValue;
                SaveToJson();
            }
        }

        private static void ResetToInitialState()
        {
            SavedBlockList = new List<string>();
            SavedWebBlockKeywordList = new List<string>();
            BlockProfiles = CreateDefaultBlockProfiles();
            IsBlockingActive = false;
            IsBreakActive = false;
            FocusEndTime = DateTime.MinValue;
            BreakEndTime = DateTime.MinValue;
            _pausedFocusRemainingTime = TimeSpan.Zero;
            Life = INITIAL_LIFE_COUNT;
            EmergencyLockUntil = DateTime.MinValue;
            LastResetTime = DateTime.Now;
            SkipNextFocusEndCheck = false;
        }

        private static Dictionary<string, List<string>> CloneBlockProfiles(Dictionary<string, List<string>> source)
        {
            Dictionary<string, List<string>> clone = CreateDefaultBlockProfiles();

            if (source == null)
            {
                return clone;
            }

            foreach (KeyValuePair<string, List<string>> profile in source)
            {
                clone[profile.Key] = profile.Value == null
                    ? new List<string>()
                    : new List<string>(profile.Value);
            }

            return clone;
        }
    }
}

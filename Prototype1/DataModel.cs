using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Prototype1
{
    public static class DataModel
    {
        public const uint LIFE = 3;
        public const int LIFE_BREAK_MINUTES = 5;
        public const int STOP_COUNTDOWN_SECONDS = 10;

        // 차단할 프로세스 명칭을 저장하는 전역 리스트
        public static List<string> SavedBlockList { get; set; } = new List<string>();

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
        public static uint Life { get; set; } = LIFE;

        // 마지막으로 리셋된 날짜보다 오늘의 날짜가 더 크다면 자정(00시) 라인을 통과한 것
        public static DateTime LastResetTime { get; set; } = DateTime.MinValue;

        // 집중 종료 시각 (JSON 저장 X)
        public static DateTime FocusEndTime = DateTime.MinValue;

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
            CurrentFocusGoal = string.Empty;
            CurrentFocusCategory = string.Empty;
            SaveToJson();
        }

        public static bool StartLifeBreak()
        {
            if (!TryUseLife())
            {
                return false;
            }

            IsBreakActive = true;
            BreakEndTime = DateTime.Now.AddMinutes(LIFE_BREAK_MINUTES);
            FocusEndTime = FocusEndTime.AddMinutes(LIFE_BREAK_MINUTES);
            SaveToJson();
            return true;
        }

        public static void EndLifeBreak()
        {
            IsBreakActive = false;
            BreakEndTime = DateTime.MinValue;
            SaveToJson();
        }

        public static bool EmergencyStopFocusSession()
        {
            if (!TryUseLife())
            {
                return false;
            }

            IsBlockingActive = false;
            IsBreakActive = false;
            BreakEndTime = DateTime.MinValue;
            FocusEndTime = DateTime.MinValue;
            EmergencyLockUntil = Life == 0 ? TodayMidnight : EmergencyLockUntil;
            FocusSessionTelemetry.CompleteSession(DateTime.Now);
            CurrentFocusGoal = string.Empty;
            CurrentFocusCategory = string.Empty;
            SaveToJson();
            return true;
        }

        private static bool TryUseLife()
        {
            if (Life == 0)
            {
                return false;
            }

            Life--;

            if (Life == 0)
            {
                EmergencyLockUntil = TodayMidnight;
            }

            return true;
        }

        // 현재 DataModel 속성 값들을 JSON 파일에 저장
        public static void SaveToJson()
        {
            try
            {
                var saveData = new Dictionary<string, object>
                {
                    { "SavedBlockList", SavedBlockList },
                    { "BlockProfiles", BlockProfiles },
                    { "IsBlockingActive", IsBlockingActive },
                    { "IsBreakActive", IsBreakActive },
                    { "BreakEndTime", BreakEndTime },
                    { "EmergencyLockUntil", EmergencyLockUntil },
                    { "Life", Life },
                    { "LastResetTime", LastResetTime }
                };

                string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JSON 저장 실패: {0}", ex.Message);
            }
        }

        // JSON 파일을 읽어 DataModel 속성 값들을 복원
        public static void LoadFromJson()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    // 파일이 없으면 최초 실행이므로 현재 날짜만 저장
                    LastResetTime = DateTime.Now;
                    SaveToJson();
                    return;
                }

                string jsonString = File.ReadAllText(FilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

                if (data != null)
                {
                    if (data.TryGetValue("SavedBlockList", out var blockListEl))
                        SavedBlockList = JsonSerializer.Deserialize<List<string>>(blockListEl.GetRawText()) ?? new List<string>();

                    if (data.TryGetValue("BlockProfiles", out var blockProfilesEl))
                    {
                        Dictionary<string, List<string>> loadedProfiles =
                            JsonSerializer.Deserialize<Dictionary<string, List<string>>>(blockProfilesEl.GetRawText());
                        BlockProfiles = CloneBlockProfiles(loadedProfiles);
                    }
                    else
                    {
                        BlockProfiles = CreateDefaultBlockProfiles();
                    }

                    if (data.TryGetValue("IsBlockingActive", out var activeEl))
                        IsBlockingActive = activeEl.GetBoolean();

                    if (data.TryGetValue("IsBreakActive", out var breakActiveEl))
                        IsBreakActive = breakActiveEl.GetBoolean();

                    if (data.TryGetValue("BreakEndTime", out var breakEndEl))
                        BreakEndTime = breakEndEl.GetDateTime();

                    if (data.TryGetValue("EmergencyLockUntil", out var emergencyLockEl))
                        EmergencyLockUntil = emergencyLockEl.GetDateTime();

                    if (data.TryGetValue("Life", out var lifeEl))
                        Life = lifeEl.GetUInt32();

                    if (data.TryGetValue("LastResetTime", out var timeEl))
                        LastResetTime = timeEl.GetDateTime();
                }

                // FocusEndTime은 저장하지 않으므로 재시작 시 진행 중 세션은 안전하게 해제합니다.
                IsBlockingActive = false;
                IsBreakActive = false;
                BreakEndTime = DateTime.MinValue;

                CheckMidnightReset();
                ClearExpiredEmergencyLock();
                ApplyLifeExhaustionLock();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JSON 불러오기 실패: {0}", ex.Message);
            }
        }

        private static void CheckMidnightReset()
        {
            DateTime now = DateTime.Now;

            // 마지막으로 리셋된 날짜보다 오늘의 날짜가 더 크다면 자정(00시) 라인을 통과한 것
            if (LastResetTime.Date < now.Date)
            {
                Life = LIFE;
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

        private static void ApplyLifeExhaustionLock()
        {
            if (Life == 0 && EmergencyLockUntil < TodayMidnight)
            {
                EmergencyLockUntil = TodayMidnight;
                SaveToJson();
            }
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Prototype1
{
    public class FocusProfile
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPreset { get; set; }
        public string EnforcementMode { get; set; } = DataModel.EnforcementModeBlockList;
        public List<string> AllowedProcesses { get; set; } = new List<string>();
        public List<string> AllowedDomains { get; set; } = new List<string>();
        public List<string> BlockedProcesses { get; set; } = new List<string>();
        public List<string> BlockedDomains { get; set; } = new List<string>();
    }

    public static class DataModel
    {
        public const uint LIFE = 3;
        public const string CustomProfileId = "custom";
        public const string EnforcementModeBlockList = "BlockList";
        public const string EnforcementModeAllowList = "AllowList";

        // 차단할 프로세스 명칭을 저장하는 전역 리스트(기존 설정 호환용)
        public static List<string> SavedBlockList { get; set; } = new List<string>();

        // 프리셋 모드와 직접 설정을 같은 구조로 다루기 위한 프로필 목록
        public static List<FocusProfile> FocusProfiles { get; set; } = CreateDefaultFocusProfiles();

        // 현재 선택된 프로필 ID
        public static string SelectedProfileId { get; set; } = CustomProfileId;

        // 현재 차단 기능이 활성화(ON) 상태인지 확인하는 변수
        public static bool IsBlockingActive { get; set; } = false;

        // 매 자정이 지나서 프로그램 실행할 시 3으로 초기화
        public static uint Life { get; set; } = LIFE;

        // 마지막으로 리셋된 날짜보다 오늘의 날짜가 더 크다면 자정(00시) 라인을 통과한 것
        public static DateTime LastResetTime { get; set; } = DateTime.MinValue;

        // 집중 종료 시각 (JSON 저장 X)
        public static DateTime FocusEndTime = DateTime.MinValue;

        // JSON 파일 저장 경로: Prototype1 -> bin -> Debug
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        // 현재 DataModel 속성 값들을 JSON 파일에 저장
        public static void SaveToJson()
        {
            try
            {
                EnsureFocusProfiles();

                var saveData = new AppConfig
                {
                    SavedBlockList = SavedBlockList,
                    FocusProfiles = FocusProfiles,
                    SelectedProfileId = SelectedProfileId,
                    IsBlockingActive = IsBlockingActive,
                    Life = Life,
                    LastResetTime = LastResetTime
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
                    EnsureFocusProfiles();
                    SaveToJson();
                    return;
                }

                string jsonString = File.ReadAllText(FilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

                if (data != null)
                {
                    if (data.TryGetValue("SavedBlockList", out var blockListEl))
                        SavedBlockList = JsonSerializer.Deserialize<List<string>>(blockListEl.GetRawText()) ?? new List<string>();

                    if (data.TryGetValue("IsBlockingActive", out var activeEl))
                        IsBlockingActive = activeEl.GetBoolean();

                    if (data.TryGetValue("Life", out var lifeEl))
                        Life = lifeEl.GetUInt32();

                    if (data.TryGetValue("LastResetTime", out var timeEl))
                        LastResetTime = timeEl.GetDateTime();

                    if (data.TryGetValue("FocusProfiles", out var profilesEl))
                        FocusProfiles = JsonSerializer.Deserialize<List<FocusProfile>>(profilesEl.GetRawText()) ?? new List<FocusProfile>();

                    if (data.TryGetValue("SelectedProfileId", out var selectedProfileEl))
                        SelectedProfileId = selectedProfileEl.GetString() ?? CustomProfileId;
                }

                EnsureFocusProfiles();
                IsBlockingActive = false; // 실행 중 세션은 재시작 시 안전하게 새로 시작한다.

                // 데이터를 불러온 직후, 자정이 지났는지 체크
                CheckMidnightReset();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JSON 불러오기 실패: {0}", ex.Message);
                EnsureFocusProfiles();
            }
        }

        public static FocusProfile GetSelectedProfile()
        {
            EnsureFocusProfiles();
            return GetProfileById(SelectedProfileId) ?? GetProfileById(CustomProfileId) ?? FocusProfiles.First();
        }

        public static FocusProfile GetProfileById(string profileId)
        {
            if (FocusProfiles == null)
            {
                return null;
            }

            return FocusProfiles.FirstOrDefault(profile => string.Equals(profile.Id, profileId, StringComparison.OrdinalIgnoreCase));
        }

        public static void SetSelectedProfile(string profileId)
        {
            EnsureFocusProfiles();

            if (GetProfileById(profileId) == null)
            {
                SelectedProfileId = CustomProfileId;
                return;
            }

            SelectedProfileId = profileId;
        }

        public static bool UsesAllowList(FocusProfile profile)
        {
            return false;
        }

        public static List<string> GetEditableProcesses(string profileId)
        {
            FocusProfile profile = GetProfileById(profileId) ?? GetSelectedProfile();

            return NormalizeProcessNames(profile.BlockedProcesses);
        }

        public static void SaveEditableProcesses(string profileId, List<string> processNames)
        {
            FocusProfile profile = GetProfileById(profileId) ?? GetSelectedProfile();
            List<string> normalizedProcesses = NormalizeProcessNames(processNames);

            profile.EnforcementMode = EnforcementModeBlockList;
            profile.BlockedProcesses = normalizedProcesses;

            if (string.Equals(profile.Id, CustomProfileId, StringComparison.OrdinalIgnoreCase))
            {
                SavedBlockList = new List<string>(normalizedProcesses);
            }

            SaveToJson();
        }

        public static List<string> GetActiveBlockedProcesses()
        {
            FocusProfile profile = GetSelectedProfile();
            return NormalizeProcessNames(profile.BlockedProcesses);
        }

        public static List<string> NormalizeProcessNames(IEnumerable<string> processNames)
        {
            List<string> result = new List<string>();

            if (processNames == null)
            {
                return result;
            }

            foreach (string processName in processNames)
            {
                string normalizedName = NormalizeProcessName(processName);

                if (string.IsNullOrWhiteSpace(normalizedName))
                {
                    continue;
                }

                if (!result.Any(existing => string.Equals(existing, normalizedName, StringComparison.OrdinalIgnoreCase)))
                {
                    result.Add(normalizedName);
                }
            }

            return result;
        }

        public static string NormalizeProcessName(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                return string.Empty;
            }

            string normalizedName = processName.Trim();

            if (normalizedName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                normalizedName = Path.GetFileNameWithoutExtension(normalizedName);
            }

            return normalizedName;
        }

        private static void CheckMidnightReset()
        {
            DateTime now = DateTime.Now;

            // 마지막으로 리셋된 날짜보다 오늘의 날짜가 더 크다면 자정(00시) 라인을 통과한 것
            if (LastResetTime.Date < now.Date)
            {
                Life = LIFE;
                LastResetTime = now;
                SaveToJson(); // 바뀐 상태를 JSON 파일에 즉시 저장
            }
        }

        private static void EnsureFocusProfiles()
        {
            if (SavedBlockList == null)
            {
                SavedBlockList = new List<string>();
            }

            if (FocusProfiles == null)
            {
                FocusProfiles = new List<FocusProfile>();
            }

            FocusProfile customProfile = GetProfileById(CustomProfileId);
            if (customProfile == null)
            {
                customProfile = new FocusProfile
                {
                    Id = CustomProfileId,
                    Name = "직접 설정",
                    IsPreset = false,
                    EnforcementMode = EnforcementModeBlockList,
                    BlockedProcesses = NormalizeProcessNames(SavedBlockList)
                };

                FocusProfiles.Insert(0, customProfile);
            }

            if (customProfile.BlockedProcesses == null)
            {
                customProfile.BlockedProcesses = new List<string>();
            }

            if (SavedBlockList.Count > 0 && customProfile.BlockedProcesses.Count == 0)
            {
                customProfile.BlockedProcesses = NormalizeProcessNames(SavedBlockList);
            }

            AddOrUpdatePresetProfile(CreateExamProfile());
            AddOrUpdatePresetProfile(CreateDeveloperProfile());

            foreach (FocusProfile profile in FocusProfiles)
            {
                EnsureProfileLists(profile);
            }

            if (string.IsNullOrWhiteSpace(SelectedProfileId) || GetProfileById(SelectedProfileId) == null)
            {
                SelectedProfileId = CustomProfileId;
            }
        }

        private static void AddOrUpdatePresetProfile(FocusProfile defaultProfile)
        {
            FocusProfile existingProfile = GetProfileById(defaultProfile.Id);

            if (existingProfile == null)
            {
                FocusProfiles.Add(defaultProfile);
                return;
            }

            existingProfile.Name = defaultProfile.Name;
            existingProfile.IsPreset = true;
            existingProfile.EnforcementMode = EnforcementModeBlockList;
            existingProfile.BlockedProcesses = MergeProcessNames(existingProfile.BlockedProcesses, defaultProfile.BlockedProcesses);

            // AllowList was an early prototype. Keep the field for saved JSON compatibility, but do not execute it.
            existingProfile.AllowedProcesses = new List<string>();
        }

        private static List<string> MergeProcessNames(IEnumerable<string> first, IEnumerable<string> second)
        {
            List<string> merged = NormalizeProcessNames(first);

            foreach (string processName in NormalizeProcessNames(second))
            {
                if (!merged.Any(existing => string.Equals(existing, processName, StringComparison.OrdinalIgnoreCase)))
                {
                    merged.Add(processName);
                }
            }

            return merged;
        }

        private static void EnsureProfileLists(FocusProfile profile)
        {
            if (profile.AllowedProcesses == null)
                profile.AllowedProcesses = new List<string>();

            if (profile.AllowedDomains == null)
                profile.AllowedDomains = new List<string>();

            if (profile.BlockedProcesses == null)
                profile.BlockedProcesses = new List<string>();

            if (profile.BlockedDomains == null)
                profile.BlockedDomains = new List<string>();

            if (string.IsNullOrWhiteSpace(profile.EnforcementMode))
                profile.EnforcementMode = EnforcementModeBlockList;

            profile.AllowedProcesses = NormalizeProcessNames(profile.AllowedProcesses);
            profile.BlockedProcesses = NormalizeProcessNames(profile.BlockedProcesses);
        }

        private static List<FocusProfile> CreateDefaultFocusProfiles()
        {
            return new List<FocusProfile>
            {
                new FocusProfile
                {
                    Id = CustomProfileId,
                    Name = "직접 설정",
                    IsPreset = false,
                    EnforcementMode = EnforcementModeBlockList
                },
                CreateExamProfile(),
                CreateDeveloperProfile()
            };
        }

        private static FocusProfile CreateExamProfile()
        {
            return new FocusProfile
            {
                Id = "exam",
                Name = "시험 공부 모드",
                IsPreset = true,
                EnforcementMode = EnforcementModeBlockList,
                BlockedProcesses = new List<string>
                {
                    "KakaoTalk",
                    "Discord",
                    "Telegram",
                    "Steam",
                    "LeagueClient",
                    "Spotify",
                    "EpicGamesLauncher",
                    "Battle.net",
                    "RiotClientServices"
                }
            };
        }

        private static FocusProfile CreateDeveloperProfile()
        {
            return new FocusProfile
            {
                Id = "developer",
                Name = "개발자 모드",
                IsPreset = true,
                EnforcementMode = EnforcementModeBlockList,
                BlockedProcesses = new List<string>
                {
                    "KakaoTalk",
                    "Discord",
                    "Telegram",
                    "Steam",
                    "LeagueClient",
                    "Spotify",
                    "EpicGamesLauncher",
                    "Battle.net",
                    "RiotClientServices"
                }
            };
        }

        private class AppConfig
        {
            public List<string> SavedBlockList { get; set; } = new List<string>();
            public List<FocusProfile> FocusProfiles { get; set; } = new List<FocusProfile>();
            public string SelectedProfileId { get; set; } = CustomProfileId;
            public bool IsBlockingActive { get; set; }
            public uint Life { get; set; } = LIFE;
            public DateTime LastResetTime { get; set; } = DateTime.MinValue;
        }
    }
}

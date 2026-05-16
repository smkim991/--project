using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Prototype1
{
    public static class DataModel
    {
        public const uint LIFE = 3;
        
        // {get; set;} for Json

        // 차단할 프로세스 명칭을 저장하는 전역 리스트
        public static List<string> SavedBlockList { get; set; } = new List<string>();

        // 현재 차단 기능이 활성화(ON) 상태인지 확인하는 변수
        public static bool IsBlockingActive { get; set; } = false;

        // 매 자정이 지나서 프로그램 실행할 시 3으로 초기화
        public static uint Life { get; set; } = LIFE;

        // 마지막으로 리셋된 날짜보다 오늘의 날짜가 더 크다면 자정(00시) 라인을 통과한 것 -> 해당 로직을 통해 자정이 지나서 프로그램이 실행되었으면 Life를 3으로 초기화 구현
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
                var saveData = new Dictionary<string, object>
                {
                    { "SavedBlockList", SavedBlockList },
                    { "IsBlockingActive", IsBlockingActive },
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

                    if (data.TryGetValue("IsBlockingActive", out var activeEl))
                        IsBlockingActive = activeEl.GetBoolean();

                    if (data.TryGetValue("Life", out var lifeEl))
                        Life = lifeEl.GetUInt32();

                    if (data.TryGetValue("LastResetTime", out var timeEl))
                        LastResetTime = timeEl.GetDateTime();
                }

                // 데이터를 불러온 직후, 자정이 지났는지 체크
                CheckMidnightReset();
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
                SaveToJson(); // 바뀐 상태를 JSON 파일에 즉시 저장
            }
        }
    }
}
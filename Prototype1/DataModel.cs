using System;
using System.Collections.Generic;

namespace Prototype1
{
    public static class DataModel
    {
        // 차단할 프로세스 명칭을 저장하는 전역 리스트
        public static List<string> SavedBlockList = new List<string>();

        // 현재 차단 기능이 활성화(ON) 상태인지 확인하는 변수
        public static bool IsBlockingActive = false;
    }
}
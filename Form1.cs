using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp15
{
    public partial class Form1 : Form
    {
        private Dictionary<string, List<string>> currentBlockedItems;
        private string currentSelectedCategory = "대학생"; // 초기 선택 카테고리
        private Button lastClickedCategoryButton = null;

        public Form1()
        {
            InitializeComponent();
            InitializeDefaultBlockedItems(); // Form1 시작 시 기본 차단 목록 초기화
            UpdateCategorySettingsDisplay(currentSelectedCategory); // 초기 카테고리 설정 표시 (예: "대학생")
            btnConfirmSelection.Enabled = false;
        }

        private void InitializeDefaultBlockedItems()
        {
            currentBlockedItems = new Dictionary<string, List<string>>();
            currentBlockedItems.Add("대학생", new List<string> { "넷플릭스", "네이버웹툰" });
            currentBlockedItems.Add("개발자", new List<string> { "유튜브", "메모장", "멜론" });
            currentBlockedItems.Add("영상편집자", new List<string> { "인스타그램", "엑셀" });
            currentBlockedItems.Add("수험생", new List<string> { "카카오톡", "인스타그램", "틱톡" });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateCategorySettingsDisplay(currentSelectedCategory);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(this.currentBlockedItems); // <--- 여기서 'blockedItems'를 'currentBlockedItems'로 변경!

            if (form3.ShowDialog() == DialogResult.OK) // Form3가 DialogResult.OK로 닫혔을 경우
            {
                // Form3에서 업데이트된 데이터를 가져와 Form1의 currentBlockedItems에 저장
                this.currentBlockedItems = form3.GetUpdatedBlockedItems(); // <--- 여기서도 'blockedItems'를 'currentBlockedItems'로 변경!

                // Form1의 카테고리 설정 화면을 업데이트합니다.
                UpdateCategorySettingsDisplay(currentSelectedCategory);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 newStudyPlanForm = new Form4();
            newStudyPlanForm.Show();
        }
        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // 1. 이전에 선택된 버튼이 있다면 스타일 초기화 (기본 배경색으로 되돌리기)
                if (lastClickedCategoryButton != null)
                {
                    lastClickedCategoryButton.BackColor = SystemColors.Control;
                }

                // 2. 현재 선택된 카테고리 업데이트 및 버튼 스타일 변경 (시각적으로 선택 표시)
                currentSelectedCategory = clickedButton.Text;
                clickedButton.BackColor = Color.LightBlue; // 예시: 선택되면 하늘색으로 변경
                lastClickedCategoryButton = clickedButton; // 현재 클릭된 버튼을 저장

                // 3. Form1의 UI 업데이트 (lblBlockedItemsDisplay에 현재 선택된 카테고리 정보 표시)
                UpdateCategorySettingsDisplay(currentSelectedCategory);

                // 4. 오른쪽 아래의 '확인' 버튼 활성화
                btnConfirmSelection.Enabled = true;
            }
        }
        private void UpdateCategorySettingsDisplay(string categoryName)
        {
            if (currentBlockedItems.ContainsKey(categoryName))
            {
                List<string> blocked = currentBlockedItems[categoryName];
                // 예시: Form1에 'lblBlockedItemsDisplay' 라는 Label 컨트롤이 있다고 가정
                // Label 컨트롤에 차단된 항목들을 콤마로 구분하여 표시
                lblBlockedItemsDisplay.Text = $"'{categoryName}' 차단 항목: {string.Join(", ", blocked)}";
            }
            else
            {
                lblBlockedItemsDisplay.Text = $"'{categoryName}' 차단 항목: 없음";
            }
        }




        private void lblBlockedItemsDisplay_Click(object sender, EventArgs e)
        {

        }

        private void btnConfirmSelection_Click(object sender, EventArgs e)
        {
            using (Form5 messageInputForm = new Form5())
            {
                // Form5를 모달 다이얼로그로 띄우고 사용자의 '확인' 여부 확인
                if (messageInputForm.ShowDialog() == DialogResult.OK)
                {
                    string userMessage = messageInputForm.EnteredMessage;

                    // Form6 띄우기
                    Form6 nextForm = new Form6();
                    nextForm.ShowDialog(); // Form6를 모달 다이얼로그로 띄웁니다.

                    // Form6에서 돌아온 후, Form1의 카테고리 선택 UI 상태를 초기화합니다.
                    ResetCategorySelectionUI();
                }
                // else { // Form5에서 '취소'를 눌렀을 경우, 메시지 창만 닫히고 아무 일도 일어나지 않습니다. }
            }
        }

        private void ResetCategorySelectionUI()
        {
            if (lastClickedCategoryButton != null)
            {
                lastClickedCategoryButton.BackColor = SystemColors.Control;
                lastClickedCategoryButton = null; // 저장된 버튼 정보 초기화
            }
            btnConfirmSelection.Enabled = false; // '확인' 버튼 비활성화
            // lblBlockedItemsDisplay.Text = "카테고리를 선택하고 확인 버튼을 눌러주세요.";
            currentSelectedCategory = string.Empty; // 선택된 카테고리 초기화
        }
    }
}
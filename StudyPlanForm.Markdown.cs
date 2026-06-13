using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class StudyPlanForm
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.E))
            {
                ToggleMarkdownPreview();
                return true;
            }

            if (keyData == (Keys.Control | Keys.A))
            {
                InsertTaskCheckboxLine();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InsertTaskCheckboxLine()
        {
            if (!CanInsertTaskCheckbox())
            {
                return;
            }

            if (isMarkdownPreviewMode)
            {
                SetMarkdownPreviewMode(false);
            }

            txtMemo.Focus();

            string text = txtMemo.Text ?? string.Empty;
            int selectionStart = Math.Max(0, Math.Min(txtMemo.SelectionStart, text.Length));
            int lineStart = FindLineStart(text, selectionStart);
            int lineEnd = FindLineEnd(text, selectionStart);
            int lineContentEnd = lineEnd > lineStart && text[lineEnd - 1] == '\r' ? lineEnd - 1 : lineEnd;
            string currentLine = text.Substring(lineStart, Math.Max(0, lineContentEnd - lineStart));

            int insertIndex;
            string insertion;
            if (string.IsNullOrWhiteSpace(currentLine) || selectionStart == lineStart)
            {
                insertIndex = lineStart;
                insertion = "- [ ] ";
            }
            else
            {
                insertIndex = lineContentEnd;
                insertion = Environment.NewLine + "- [ ] ";
            }

            txtMemo.Select(insertIndex, 0);
            txtMemo.SelectedText = insertion;
            txtMemo.SelectionStart = insertIndex + insertion.Length;
        }

        private bool CanInsertTaskCheckbox()
        {
            return txtMemo != null &&
                   txtMemo.Enabled &&
                   !txtMemo.ReadOnly &&
                   !string.IsNullOrWhiteSpace(currentFilePath);
        }

        private int FindLineStart(string text, int selectionStart)
        {
            if (string.IsNullOrEmpty(text) || selectionStart <= 0)
            {
                return 0;
            }

            int searchStart = Math.Min(selectionStart - 1, text.Length - 1);
            int lineStart = text.LastIndexOf('\n', searchStart);
            return lineStart < 0 ? 0 : lineStart + 1;
        }

        private int FindLineEnd(string text, int selectionStart)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int lineEnd = text.IndexOf('\n', Math.Min(selectionStart, text.Length));
            return lineEnd < 0 ? text.Length : lineEnd;
        }

        private void ToggleMarkdownPreview()
        {
            if (!CanToggleMarkdownPreview())
            {
                return;
            }

            SetMarkdownPreviewMode(!isMarkdownPreviewMode);
        }

        private bool CanToggleMarkdownPreview()
        {
            return txtMemo != null &&
                   markdownPreviewBrowser != null &&
                   !string.IsNullOrWhiteSpace(currentFilePath) &&
                   txtMemo.Enabled;
        }

        private void SetMarkdownPreviewMode(bool previewMode)
        {
            isMarkdownPreviewMode = previewMode && CanToggleMarkdownPreview();

            if (markdownPreviewBrowser != null)
            {
                if (isMarkdownPreviewMode)
                {
                    markdownPreviewBrowser.DocumentText = BuildMarkdownHtml(txtMemo.Text);
                }

                markdownPreviewBrowser.Visible = isMarkdownPreviewMode;
            }

            if (txtMemo != null)
            {
                txtMemo.Visible = !isMarkdownPreviewMode;
            }

            if (btnToggleMarkdownPreview != null)
            {
                btnToggleMarkdownPreview.Text = isMarkdownPreviewMode ? "원문" : "미리보기";
                btnToggleMarkdownPreview.Enabled = CanToggleMarkdownPreview();
                btnToggleMarkdownPreview.BackColor = isMarkdownPreviewMode ? AccentColor : FieldColor;
            }
        }

        private void ClearMarkdownPreview()
        {
            if (markdownPreviewBrowser != null)
            {
                markdownPreviewBrowser.DocumentText = BuildMarkdownHtml(string.Empty);
                markdownPreviewBrowser.Visible = false;
            }

            isMarkdownPreviewMode = false;
            if (txtMemo != null)
            {
                txtMemo.Visible = true;
            }

            if (btnToggleMarkdownPreview != null)
            {
                btnToggleMarkdownPreview.Text = "미리보기";
                btnToggleMarkdownPreview.Enabled = false;
                btnToggleMarkdownPreview.BackColor = FieldColor;
            }
        }

        private string BuildMarkdownHtml(string markdown)
        {
            return "<!doctype html><html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />" +
                   "<meta charset=\"utf-8\" />" +
                   "<style>" +
                   "html,body{margin:0;padding:0;background:#232323;color:#fff;font-family:'Malgun Gothic','맑은 고딕',sans-serif;font-size:15px;line-height:1.65;}" +
                   "body{padding:22px 24px;box-sizing:border-box;}" +
                   "h1,h2,h3,h4,h5,h6{margin:0 0 12px;color:#fff;font-weight:700;line-height:1.28;}" +
                   "h1{font-size:30px;border-bottom:1px solid #3c3c3c;padding-bottom:10px;}h2{font-size:24px;}h3{font-size:20px;}h4,h5,h6{font-size:17px;}" +
                   "p{margin:0 0 14px;}ul,ol{margin:0 0 14px 24px;padding:0;}li{margin:4px 0;}" +
                   "blockquote{margin:0 0 14px;padding:10px 14px;border-left:4px solid #8b5cf6;background:#181818;color:#d8d8dc;}" +
                   "pre{margin:0 0 14px;padding:14px;overflow:auto;background:#181818;border:1px solid #3c3c3c;color:#f5f5f5;}" +
                   "code{font-family:Consolas,'Courier New',monospace;background:#181818;border:1px solid #3c3c3c;padding:2px 5px;color:#f5f5f5;}" +
                   "pre code{border:0;padding:0;background:transparent;}strong{color:#fff;}em{color:#e4e4e7;}a{color:#a78bfa;text-decoration:none;}" +
                   "hr{border:0;border-top:1px solid #3c3c3c;margin:18px 0;}.task{display:inline-block;width:16px;}" +
                   "</style></head><body>" +
                   RenderMarkdownBody(markdown) +
                   "</body></html>";
        }

        private string RenderMarkdownBody(string markdown)
        {
            string[] lines = (markdown ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            StringBuilder builder = new StringBuilder();
            bool inCodeBlock = false;
            string currentListTag = null;

            foreach (string rawLine in lines)
            {
                string line = rawLine ?? string.Empty;
                string trimmed = line.Trim();

                if (trimmed.StartsWith("```", StringComparison.Ordinal))
                {
                    if (inCodeBlock)
                    {
                        builder.AppendLine("</code></pre>");
                        inCodeBlock = false;
                    }
                    else
                    {
                        CloseList(builder, ref currentListTag);
                        builder.AppendLine("<pre><code>");
                        inCodeBlock = true;
                    }

                    continue;
                }

                if (inCodeBlock)
                {
                    builder.Append(WebUtility.HtmlEncode(line));
                    builder.Append('\n');
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    CloseList(builder, ref currentListTag);
                    continue;
                }

                Match heading = Regex.Match(trimmed, @"^(#{1,6})\s+(.+)$");
                if (heading.Success)
                {
                    CloseList(builder, ref currentListTag);
                    int level = heading.Groups[1].Value.Length;
                    builder.AppendFormat("<h{0}>{1}</h{0}>", level, RenderInlineMarkdown(heading.Groups[2].Value));
                    continue;
                }

                if (Regex.IsMatch(trimmed, @"^(-{3,}|\*{3,}|_{3,})$"))
                {
                    CloseList(builder, ref currentListTag);
                    builder.AppendLine("<hr />");
                    continue;
                }

                Match quote = Regex.Match(trimmed, @"^>\s?(.*)$");
                if (quote.Success)
                {
                    CloseList(builder, ref currentListTag);
                    builder.AppendFormat("<blockquote>{0}</blockquote>", RenderInlineMarkdown(quote.Groups[1].Value));
                    continue;
                }

                Match unordered = Regex.Match(line, @"^\s*[-*+]\s+(.+)$");
                if (unordered.Success)
                {
                    EnsureList(builder, ref currentListTag, "ul");
                    builder.AppendFormat("<li>{0}</li>", RenderTaskItem(unordered.Groups[1].Value));
                    continue;
                }

                Match ordered = Regex.Match(line, @"^\s*\d+[\.)]\s+(.+)$");
                if (ordered.Success)
                {
                    EnsureList(builder, ref currentListTag, "ol");
                    builder.AppendFormat("<li>{0}</li>", RenderInlineMarkdown(ordered.Groups[1].Value));
                    continue;
                }

                CloseList(builder, ref currentListTag);
                builder.AppendFormat("<p>{0}</p>", RenderInlineMarkdown(trimmed));
            }

            if (inCodeBlock)
            {
                builder.AppendLine("</code></pre>");
            }

            CloseList(builder, ref currentListTag);
            return builder.ToString();
        }

        private void EnsureList(StringBuilder builder, ref string currentListTag, string listTag)
        {
            if (currentListTag == listTag)
            {
                return;
            }

            CloseList(builder, ref currentListTag);
            currentListTag = listTag;
            builder.AppendFormat("<{0}>", currentListTag);
        }

        private void CloseList(StringBuilder builder, ref string currentListTag)
        {
            if (string.IsNullOrEmpty(currentListTag))
            {
                return;
            }

            builder.AppendFormat("</{0}>", currentListTag);
            currentListTag = null;
        }

        private string RenderTaskItem(string value)
        {
            Match task = Regex.Match(value ?? string.Empty, @"^\[( |x|X)\]\s+(.+)$");
            if (!task.Success)
            {
                return RenderInlineMarkdown(value);
            }

            string check = task.Groups[1].Value.Equals(" ", StringComparison.Ordinal) ? "☐" : "☑";
            return "<span class=\"task\">" + check + "</span>" + RenderInlineMarkdown(task.Groups[2].Value);
        }

        private string RenderInlineMarkdown(string value)
        {
            string encoded = WebUtility.HtmlEncode(value ?? string.Empty);

            encoded = Regex.Replace(
                encoded,
                @"\[(.+?)\]\((https?://[^\s)]+)\)",
                "<a href=\"$2\">$1</a>",
                RegexOptions.IgnoreCase);
            encoded = Regex.Replace(encoded, @"`([^`]+)`", "<code>$1</code>");
            encoded = Regex.Replace(encoded, @"\*\*([^*]+)\*\*", "<strong>$1</strong>");
            encoded = Regex.Replace(encoded, @"__([^_]+)__", "<strong>$1</strong>");
            encoded = Regex.Replace(encoded, @"(?<!\*)\*([^*\r\n]+)\*(?!\*)", "<em>$1</em>");
            encoded = Regex.Replace(encoded, @"(?<!_)_([^_\r\n]+)_(?!_)", "<em>$1</em>");
            return encoded;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Prototype1.UI
{
    internal sealed class SessionTimelinePanel : Panel
    {
        private readonly List<Tuple<Rectangle, FocusSessionRecord>> hitAreas = new List<Tuple<Rectangle, FocusSessionRecord>>();
        private DateTime day = DateTime.Today;
        private List<FocusSessionRecord> sessions = new List<FocusSessionRecord>();

        public event EventHandler<SessionSelectedEventArgs> SessionSelected;
        public string SelectedSessionId { get; set; } = string.Empty;

        public SessionTimelinePanel()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
        }

        public void SetSessions(DateTime selectedDay, List<FocusSessionRecord> selectedSessions)
        {
            day = selectedDay.Date;
            sessions = selectedSessions == null
                ? new List<FocusSessionRecord>()
                : selectedSessions.OrderBy(s => s.StartedAt).ToList();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (Tuple<Rectangle, FocusSessionRecord> area in hitAreas)
            {
                if (area.Item1.Contains(e.Location))
                {
                    SelectedSessionId = area.Item2.Id;
                    Invalidate();

                    EventHandler<SessionSelectedEventArgs> handler = SessionSelected;
                    if (handler != null)
                    {
                        handler(this, new SessionSelectedEventArgs(area.Item2));
                    }

                    return;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(Color.White);
            hitAreas.Clear();

            Rectangle bounds = ClientRectangle;
            int left = 190;
            int right = 24;
            int top = 34;
            int axisY = Math.Max(top + 36, bounds.Height - 42);
            int plotWidth = Math.Max(120, bounds.Width - left - right);

            using (Pen gridPen = new Pen(Color.FromArgb(220, 224, 230)))
            using (Pen axisPen = new Pen(Color.FromArgb(130, 140, 150)))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(45, 52, 60)))
            using (Font smallFont = new Font(Font.FontFamily, 8.5F))
            using (Font rowFont = new Font(Font.FontFamily, 9F))
            {
                g.DrawString(day.ToString("yyyy-MM-dd") + " 세션 타임라인", Font, textBrush, 12, 10);

                for (int hour = 0; hour <= 24; hour += 2)
                {
                    int x = left + (int)Math.Round(plotWidth * (hour / 24.0));
                    g.DrawLine(gridPen, x, top, x, axisY);
                    g.DrawString(hour.ToString("D2"), smallFont, textBrush, x - 9, axisY + 6);
                }

                g.DrawLine(axisPen, left, axisY, left + plotWidth, axisY);

                if (sessions.Count == 0)
                {
                    g.DrawString("선택한 날짜에 기록된 세션이 없습니다.", Font, textBrush, left, top + 35);
                    return;
                }

                DrawLegend(g, bounds, smallFont, textBrush);

                int rowY = top + 24;
                for (int i = 0; i < sessions.Count; i++)
                {
                    FocusSessionRecord session = sessions[i];
                    int y = rowY + (i * 44);
                    DrawSession(g, session, new Rectangle(left, y, plotWidth, 28), rowFont, textBrush);
                }
            }
        }

        private void DrawLegend(Graphics g, Rectangle bounds, Font font, Brush textBrush)
        {
            int x = Math.Max(260, bounds.Width - 200);
            DrawLegendItem(g, x, 10, Color.FromArgb(55, 158, 132), "활성", font, textBrush);
            DrawLegendItem(g, x + 78, 10, Color.FromArgb(238, 188, 84), "휴식", font, textBrush);
        }

        private void DrawLegendItem(Graphics g, int x, int y, Color color, string text, Font font, Brush textBrush)
        {
            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, x, y + 4, 14, 10);
            }

            g.DrawString(text, font, textBrush, x + 18, y);
        }

        private void DrawSession(Graphics g, FocusSessionRecord session, Rectangle track, Font rowFont, Brush textBrush)
        {
            DateTime dayStart = day;
            DateTime dayEnd = day.AddDays(1);
            DateTime sessionStart = session.StartedAt < dayStart ? dayStart : session.StartedAt;
            DateTime sessionEnd = session.EndedAt > dayEnd ? dayEnd : session.EndedAt;

            int startX = GetX(sessionStart, track);
            int endX = GetX(sessionEnd, track);
            Rectangle sessionRect = new Rectangle(startX, track.Y, Math.Max(4, endX - startX), track.Height);
            bool selected = string.Equals(SelectedSessionId, session.Id, StringComparison.OrdinalIgnoreCase);

            using (Brush background = new SolidBrush(Color.FromArgb(235, 239, 244)))
            using (Pen border = new Pen(selected ? Color.FromArgb(28, 99, 175) : Color.FromArgb(155, 163, 175), selected ? 2 : 1))
            {
                g.FillRectangle(background, sessionRect);
                g.DrawRectangle(border, sessionRect);
            }

            if (session.Segments != null && session.Segments.Count > 0)
            {
                foreach (AppUsageSegment segment in session.Segments)
                {
                    if (segment.State == FocusUsageState.Idle)
                    {
                        continue;
                    }

                    DateTime segStart = segment.StartAt < dayStart ? dayStart : segment.StartAt;
                    DateTime segEnd = segment.EndAt > dayEnd ? dayEnd : segment.EndAt;
                    if (segEnd <= dayStart || segStart >= dayEnd || segEnd <= segStart)
                    {
                        continue;
                    }

                    Rectangle segmentRect = new Rectangle(
                        GetX(segStart, track),
                        track.Y + 3,
                        Math.Max(2, GetX(segEnd, track) - GetX(segStart, track)),
                        track.Height - 6);

                    using (Brush brush = new SolidBrush(GetSegmentColor(segment)))
                    {
                        g.FillRectangle(brush, segmentRect);
                    }
                }
            }

            hitAreas.Add(Tuple.Create(sessionRect, session));

            string label = session.StartedAt.ToString("HH:mm") + "-" + session.EndedAt.ToString("HH:mm") +
                           " " + (string.IsNullOrWhiteSpace(session.Goal) ? "집중 세션" : session.Goal);

            using (StringFormat format = new StringFormat())
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                g.DrawString(label, rowFont, textBrush, new RectangleF(8, track.Y + 6, track.Left - 16, track.Height), format);
            }
        }

        private int GetX(DateTime time, Rectangle track)
        {
            double seconds = (time - day).TotalSeconds;
            seconds = Math.Max(0, Math.Min(86400, seconds));
            return track.Left + (int)Math.Round(track.Width * (seconds / 86400.0));
        }

        private Color GetSegmentColor(AppUsageSegment segment)
        {
            if (segment.State == FocusUsageState.Break)
            {
                return Color.FromArgb(238, 188, 84);
            }

            return Color.FromArgb(55, 158, 132);
        }
    }

    internal sealed class SessionSelectedEventArgs : EventArgs
    {
        public SessionSelectedEventArgs(FocusSessionRecord session)
        {
            Session = session;
        }

        public FocusSessionRecord Session { get; private set; }
    }
}

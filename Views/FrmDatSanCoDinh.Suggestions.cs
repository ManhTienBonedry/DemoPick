using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh
    {
        private sealed class SuggestedSlot
        {
            public DateTime Start { get; set; }
            public int DurationMinutes { get; set; }
            public string Reason { get; set; }
        }

        private void RefreshSmartSuggestionUi(int courtId, DateTime date, DateTime requestedStart, DateTime requestedEnd)
        {
            if (_lblSuggestions == null || _pnlSuggestions == null)
                return;

            if (courtId <= 0 || requestedEnd <= requestedStart)
            {
                SetSuggestionLinks(new List<SuggestedSlot>());
                return;
            }

            try
            {
                var suggestions = BuildSmartSuggestions(courtId, date, requestedStart, requestedEnd);
                SetSuggestionLinks(suggestions);
            }
            catch (Exception ex)
            {
                DemoPick.Services.DatabaseHelper.TryLogThrottled(
                    throttleKey: "FrmDatSanCoDinh.RefreshSmartSuggestionUi",
                    eventDesc: "Smart Suggestion Error",
                    ex: ex,
                    context: "FrmDatSanCoDinh.RefreshSmartSuggestionUi",
                    minSeconds: 120);
                SetSuggestionLinks(new List<SuggestedSlot>());
            }
        }

        private List<SuggestedSlot> BuildSmartSuggestions(int courtId, DateTime date, DateTime requestedStart, DateTime requestedEnd)
        {
            int durationMinutes = Math.Max(30, (int)Math.Round((requestedEnd - requestedStart).TotalMinutes));
            var dayBookings = (_controller.GetBookingsByDate(date) ?? new List<DemoPick.Models.BookingModel>())
                .Where(b =>
                    b != null &&
                    b.CourtID == courtId &&
                    !string.Equals(b.Status, DemoPick.Services.AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.StartTime)
                .ToList();

            var results = new List<SuggestedSlot>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            TryAddSuggestion(results, seen, requestedStart, durationMinutes, "Khung gio dang chon hop le.", dayBookings, date);

            foreach (var preferred in LoadHistoricalPreferredTimes(requestedStart.TimeOfDay))
            {
                DateTime candidate = date.Date.Add(preferred);
                string reason = "Theo lich su dat san cua khach.";
                TryAddSuggestion(results, seen, candidate, durationMinutes, reason, dayBookings, date);
            }

            foreach (var conflict in FindOverlaps(dayBookings, requestedStart, requestedEnd))
            {
                TryAddSuggestion(results, seen, conflict.EndTime, durationMinutes, "Ngay sau booking dang trung.", dayBookings, date);
                TryAddSuggestion(results, seen, conflict.StartTime.AddMinutes(-durationMinutes), durationMinutes, "Som hon booking dang trung.", dayBookings, date);
            }

            DateTime? nextFree = FindNearestFreeSlot(dayBookings, requestedStart, durationMinutes, date, searchForward: true);
            if (nextFree.HasValue)
            {
                TryAddSuggestion(results, seen, nextFree.Value, durationMinutes, "Khung gio trong gan nhat sau gio chon.", dayBookings, date);
            }

            DateTime? prevFree = FindNearestFreeSlot(dayBookings, requestedStart, durationMinutes, date, searchForward: false);
            if (prevFree.HasValue)
            {
                TryAddSuggestion(results, seen, prevFree.Value, durationMinutes, "Khung gio trong gan nhat truoc gio chon.", dayBookings, date);
            }

            return results
                .OrderBy(s => Math.Abs((s.Start - requestedStart).TotalMinutes))
                .ThenBy(s => s.Start)
                .Take(3)
                .ToList();
        }

        private void SetSuggestionLinks(List<SuggestedSlot> suggestions)
        {
            suggestions = suggestions ?? new List<SuggestedSlot>();

            for (int i = 0; i < _suggestionLinks.Count; i++)
            {
                var link = _suggestionLinks[i];
                if (i < suggestions.Count)
                {
                    var slot = suggestions[i];
                    link.Tag = slot;
                    link.Text = string.Format("{0:HH:mm} - {1:HH:mm} | {2}", slot.Start, slot.Start.AddMinutes(slot.DurationMinutes), slot.Reason);
                    link.Visible = true;
                }
                else
                {
                    link.Tag = null;
                    link.Text = string.Empty;
                    link.Visible = false;
                }
            }

            bool hasSuggestions = suggestions.Count > 0;
            _lblSuggestions.Visible = hasSuggestions;
            _pnlSuggestions.Visible = hasSuggestions;
            ApplySmartSuggestionLayout(hasSuggestions);
        }

        private void ApplySmartSuggestionLayout(bool showSuggestions)
        {
            if (btnCancel == null || btnConfirm == null || _lblSuggestions == null || _pnlSuggestions == null)
                return;

            if (!_smartSuggestionLayoutApplied && !showSuggestions)
                return;

            bool quickMode = CurrentMode == BookingMode.Quick;
            int formHeight = 540;
            int buttonTop = 460;

            if (showSuggestions)
            {
                if (quickMode)
                {
                    _lblSuggestions.Location = new System.Drawing.Point(30, 458);
                    _pnlSuggestions.Location = new System.Drawing.Point(30, 482);
                    _pnlSuggestions.Size = new System.Drawing.Size(700, 72);
                    formHeight = 660;
                    buttonTop = 575;
                }
                else
                {
                    _lblSuggestions.Location = new System.Drawing.Point(370, 428);
                    _pnlSuggestions.Location = new System.Drawing.Point(370, 452);
                    _pnlSuggestions.Size = new System.Drawing.Size(360, 78);
                    formHeight = 640;
                    buttonTop = 545;
                }
            }

            if (ClientSize.Height != formHeight)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, formHeight);
            }

            int linkWidth = Math.Max(220, _pnlSuggestions.Width - 14);
            for (int i = 0; i < _suggestionLinks.Count; i++)
            {
                _suggestionLinks[i].Width = linkWidth;
            }

            btnCancel.Top = buttonTop;
            btnConfirm.Top = buttonTop;
            _smartSuggestionLayoutApplied = showSuggestions;
        }

        private void SuggestionLink_Click(object sender, EventArgs e)
        {
            var link = sender as LinkLabel;
            var slot = link == null ? null : link.Tag as SuggestedSlot;
            if (slot == null || _dtStartClock == null)
                return;

            _dtStartClock.Value = DateTime.Today.Add(slot.Start.TimeOfDay);
            QueueConflictHintRefresh();
        }

        private void TryAddSuggestion(
            List<SuggestedSlot> results,
            HashSet<string> seen,
            DateTime candidateStart,
            int durationMinutes,
            string reason,
            List<DemoPick.Models.BookingModel> dayBookings,
            DateTime date)
        {
            if (candidateStart == DateTime.MinValue)
                return;

            candidateStart = NormalizeCandidate(candidateStart, date);
            DateTime candidateEnd = candidateStart.AddMinutes(durationMinutes);
            if (candidateEnd <= candidateStart)
                return;
            if (candidateStart.Date != date.Date)
                return;
            if (candidateEnd > date.Date.AddDays(1))
                return;
            if (!IsSlotFree(dayBookings, candidateStart, candidateEnd))
                return;

            string key = candidateStart.ToString("yyyyMMddHHmm");
            if (!seen.Add(key))
                return;

            results.Add(new SuggestedSlot
            {
                Start = candidateStart,
                DurationMinutes = durationMinutes,
                Reason = reason ?? string.Empty
            });
        }

        private static DateTime NormalizeCandidate(DateTime value, DateTime date)
        {
            if (value.Date != date.Date)
                return date.Date.Add(value.TimeOfDay);
            return value;
        }

        private static bool IsSlotFree(List<DemoPick.Models.BookingModel> bookings, DateTime start, DateTime end)
        {
            foreach (var booking in bookings)
            {
                if (start < booking.EndTime && end > booking.StartTime)
                    return false;
            }

            return true;
        }

        private static List<DemoPick.Models.BookingModel> FindOverlaps(List<DemoPick.Models.BookingModel> bookings, DateTime start, DateTime end)
        {
            return bookings
                .Where(b => start < b.EndTime && end > b.StartTime)
                .OrderBy(b => b.StartTime)
                .ToList();
        }

        private static DateTime? FindNearestFreeSlot(List<DemoPick.Models.BookingModel> bookings, DateTime requestedStart, int durationMinutes, DateTime date, bool searchForward)
        {
            var baseCandidates = new List<DateTime>();

            if (searchForward)
            {
                foreach (var booking in bookings.Where(b => b.EndTime >= requestedStart).OrderBy(b => b.EndTime))
                {
                    baseCandidates.Add(booking.EndTime);
                }
            }
            else
            {
                foreach (var booking in bookings.Where(b => b.StartTime <= requestedStart).OrderByDescending(b => b.StartTime))
                {
                    baseCandidates.Add(booking.StartTime.AddMinutes(-durationMinutes));
                }
            }

            for (int step = 15; step <= 240; step += 15)
            {
                baseCandidates.Add(requestedStart.AddMinutes(searchForward ? step : -step));
            }

            foreach (var candidate in baseCandidates)
            {
                DateTime normalized = NormalizeCandidate(candidate, date);
                DateTime end = normalized.AddMinutes(durationMinutes);
                if (normalized.Date != date.Date) continue;
                if (normalized < date.Date) continue;
                if (end > date.Date.AddDays(1)) continue;
                if (IsSlotFree(bookings, normalized, end))
                    return normalized;
            }

            return null;
        }

        private IEnumerable<TimeSpan> LoadHistoricalPreferredTimes(TimeSpan requestedTime)
        {
            string phoneDigits = DemoPick.Services.PhoneNumberValidator.NormalizeDigits(txtPhone == null ? string.Empty : txtPhone.Text);
            string customerName = (txtName == null ? string.Empty : txtName.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(phoneDigits) && string.IsNullOrWhiteSpace(customerName))
                yield break;

            var buckets = new Dictionary<int, int>();

            string query = @"
SELECT TOP 24 b.StartTime
FROM dbo.Bookings b
LEFT JOIN dbo.Members m ON m.MemberID = b.MemberID
WHERE b.Status <> @Cancelled
  AND (
        (@Phone = '' AND @Name <> '' AND (ISNULL(m.FullName, '') = @Name OR ISNULL(b.GuestName, '') LIKE @NameLike))
     OR (@Phone <> '' AND (ISNULL(m.Phone, '') = @Phone OR ISNULL(b.GuestName, '') LIKE @PhoneLike))
     OR (@Phone <> '' AND @Name <> '' AND ISNULL(m.FullName, '') = @Name)
  )
ORDER BY b.StartTime DESC";

            DataTable dt = DemoPick.Services.DatabaseHelper.ExecuteQuery(
                query,
                new SqlParameter("@Cancelled", DemoPick.Services.AppConstants.BookingStatus.Cancelled),
                new SqlParameter("@Phone", phoneDigits),
                new SqlParameter("@PhoneLike", "%" + phoneDigits + "%"),
                new SqlParameter("@Name", customerName),
                new SqlParameter("@NameLike", "%" + customerName + "%"));

            foreach (DataRow row in dt.Rows)
            {
                if (row["StartTime"] == DBNull.Value)
                    continue;

                var start = Convert.ToDateTime(row["StartTime"]);
                int minuteOfDay = (start.Hour * 60) + start.Minute;
                int bucket = ((minuteOfDay + 7) / 15) * 15;
                if (bucket >= 24 * 60)
                    bucket = (24 * 60) - 15;

                if (!buckets.ContainsKey(bucket))
                    buckets[bucket] = 0;
                buckets[bucket]++;
            }

            foreach (var bucket in buckets
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => Math.Abs(kv.Key - ((int)requestedTime.TotalMinutes)))
                .Take(3))
            {
                yield return TimeSpan.FromMinutes(bucket.Key);
            }
        }
    }
}

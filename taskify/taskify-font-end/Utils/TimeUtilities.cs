namespace taskify_font_end.Utils
{
    public static class TimeUtilities
    {
        public static string CalculateTimeStatus(DateTime startDateTime, DateTime endDateTime)
        {
            var currentDateTime = DateTime.Now;

            if (currentDateTime < startDateTime)
            {
                var timeSpan = startDateTime - currentDateTime;
                return $"Will start in {timeSpan.Days} days {timeSpan.Hours} hours {timeSpan.Minutes} minutes {timeSpan.Seconds} seconds";
            }
            else if (currentDateTime > endDateTime)
            {
                var timeSpan = currentDateTime - endDateTime;
                return $"Ended before {timeSpan.Days} days {timeSpan.Hours} hours {timeSpan.Minutes} minutes {timeSpan.Seconds} seconds";
            }
            else
            {
                var timeSpan = endDateTime - currentDateTime;
                return $"Ongoing, will end in {timeSpan.Days} days {timeSpan.Hours} hours {timeSpan.Minutes} minutes {timeSpan.Seconds} seconds";
            }
        }

    }
}

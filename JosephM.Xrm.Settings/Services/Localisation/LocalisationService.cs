using System;

namespace JosephM.Xrm.Settings.Services.Localisation
{
    public class LocalisationService
    {
        public LocalisationService(LocalisationSettings localisationSettings)
        {
            LocalisationSettings = localisationSettings;
        }

        private LocalisationSettings LocalisationSettings { get; set; }

        public DateTime TargetTodayUniversal
        {
            get
            {
                var localNow = DateTime.Now;
                var targetNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(localNow,
                    LocalisationSettings.TargetTimeZoneId);
                var difference = localNow - targetNow;
                return targetNow.Date.Add(difference).ToUniversalTime();
            }
        }

        public DateTime TargetToday
        {
            get { return ConvertToTargetTime(TargetTodayUniversal); }
        }

        public DateTime ConvertToTargetTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, LocalisationSettings.TargetTimeZoneId);
        }

        public DateTime ConvertToTargetDayUtc(DateTime day)
        {
            var sourceDate = day.Date;
            var targetDayTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(day,
                LocalisationSettings.TargetTimeZoneId);
            var difference = sourceDate - targetDayTime;
            return sourceDate.Add(difference).ToUniversalTime();
        }
    }
}
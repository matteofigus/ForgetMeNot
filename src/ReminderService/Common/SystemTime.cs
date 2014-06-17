using System;

namespace ReminderService.Common
{
    public static class SystemTime
    {
        private static DateTime _setTime = DateTime.MinValue;

        public static void Clear()
        {
            _setTime = DateTime.MinValue;
        }

        public static void Set(DateTime toSet)
        {
            _setTime = toSet;
        }
        public static DateTime Now()
        {
            if (_setTime == DateTime.MinValue)
                return DateTime.Now;
            return _setTime;
        }
    }
}

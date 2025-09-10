using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Goober.Base.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly Regex _regTemplateShift = new Regex("([+-])(\\d+)(mo|ms|[ywdhms])+", RegexOptions.Compiled);

        private static List<string> CustomDateFormats = new List<string> {
            "yyyy-MM-dd",
            "yyyy-MM-dd HH\\:mm\\:ss"
        };

        private class DateTimeParseResult
        {
            public DateTime? DateTime { get; set; }
        }

        private class DateTimeModelToParse
        {
            public string DateTime { get; set; }
        }

        public static DateTime? ToDateTimeByJsonSerialization(
            this string dateToParse)
        {
            var modelToParse = new DateTimeModelToParse { DateTime = dateToParse };
            var str = modelToParse.Serialize();

            try
            {
                var ret = str.Deserialize<DateTimeParseResult>();
                return ret?.DateTime;
            }
            catch
            {
                return null;
            }
        }

        public static string ToStringByJsonSerialization(this DateTime? dateTime)
        {
            var modelToParse = new DateTimeParseResult { DateTime = dateTime };
            var res = modelToParse.Serialize();

            return res;
        }

        public static DateTime? ToDateTime(
            this string dateToParse,
            List<string> formats = null,
            IFormatProvider provider = null,
            DateTimeStyles styles = DateTimeStyles.AssumeUniversal)
        {
            DateTime validDate;

            var dateFormats = formats ?? CustomDateFormats;

            foreach (var format in dateFormats)
            {
                if (format.EndsWith("Z"))
                {
                    if (DateTime.TryParseExact(dateToParse, format,
                             provider,
                             DateTimeStyles.AssumeLocal,
                             out validDate))
                    {
                        return validDate;
                    }
                }

                if (DateTime.TryParseExact(dateToParse, format,
                         provider, styles, out validDate))
                {
                    return validDate;
                }
            }

            return null;
        }

        public static DateTime? ToDateTimeApproximately(this string value, DateTime currentDateTime, int maxDeltaInHours, out bool wasOutOfDelta)
        {
            wasOutOfDelta = false;

            if (string.IsNullOrEmpty(value) == true)
                return null;

            foreach (var iStrCulture in Cultures)
            {
                var culture = new CultureInfo(iStrCulture);
                var formats = GetDateParseExactFormats(culture);

                foreach (var iDateFormat in formats)
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(value, iDateFormat, culture);

                        if (Math.Abs((currentDateTime - dt).TotalHours) < maxDeltaInHours)
                        {
                            return dt;
                        }
                        else
                        {
                            wasOutOfDelta = true;
                        }

                    }
                    catch { }
                }
            }

            return null;
        }

        /// <summary>
        /// Получить смещенное время
        /// </summary>
        /// <param name="time">Относительная метка времени</param>
        /// <param name="templateShift">Шаблон временного смещения</param>
        /// <remarks>
        /// Шаблон смещения состоит из знака операции и единицы измерения времени<br/><br/>
        /// Поддерживаемые знаки операций:<br/>
        /// + прибавить<br/>
        /// - вычесть<br/><br/>
        /// Поддерживаемые единицы измерения времени:<br/>
        /// y - год<br/>
        /// mo - месяц<br/>
        /// w - неделя<br/>
        /// d - день<br/>
        /// h - час<br/>
        /// m - минута<br/>
        /// s - секунда<br/>
        /// ms - миллисекунда<br/><br/>
        /// Примеры поддерживаемых шаблонов временного смещения:<br/>
        /// -1d<br/>
        /// -1mo+1d-2ms<br/>
        /// -1mo+1bad-1h шаблон с ошибочным выражением будет приведен к виду -1mo-1h<br/> 
        /// </remarks>
        /// <returns></returns>
        public static DateTime GetTimeShifted(this DateTime time, string? templateShift = null)
        {
            if(string.IsNullOrEmpty(templateShift))
                return time;

            var templateShiftLower = templateShift.ToLowerInvariant();

            var matches = _regTemplateShift.Matches(templateShiftLower);

            var res = time;

            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[2].Value, out var number) == false)
                    continue;

                var value = match.Groups[1].Value == "+" ? number : number * -1;
                var unit = match.Groups[3].Value;

                res = unit switch
                {
                    "d" => res.AddDays(value),
                    "h" => res.AddHours(value),
                    "m" => res.AddMinutes(value),
                    "s" => res.AddSeconds(value),
                    "mo" => res.AddMonths(value),
                    "w" => res.AddDays(value * 7),
                    "y" => res.AddYears(value),
                    "ms" => res.AddMilliseconds(value),
                    _ => throw new InvalidOperationException($"Не удалось определить единицу измерения времени '{unit}'")
                };
            }

            return res;
        }

        private static string[] Cultures = new string[] { "ru-RU", "en-US" };

        private static string[] GetDateParseExactFormats(CultureInfo ci)
        {
            return new[] { "MM-dd-yyyy", "M-d-yyyy", "MM-d-yyyy", "M-dd-yyyy", "MM-dd-yy", "M-dd-yy", "MM-d-yy", "d-M-yy",
               "dd-MM-yyyy", "d-M-yyyy", "d-MM-yyyy", "dd-M-yyyy", "dd-MM-yy", "d-MM-yy", "dd-M-yy", "M-d-yy",
               "yyyy-dd-MM", "yyyy-d-M", "yyyy-d-MM", "yyyy-dd-M", "yy-dd-MM", "yy-d-M", "yy-d-MM", "yy-dd-M",
               "yyyy-MM-dd", "yyyy-M-d", "yyyy-MM-d", "yyyy-M-dd", "yy-MM-dd", "yy-M-d", "yy-MM-d", "yy-M-dd",

               "MM/dd/yyyy", "M/d/yyyy", "MM/d/yyyy", "M/dd/yyyy", "MM/dd/yy", "M/dd/yy", "MM/d/yy", "d/M/yy",
               "dd/MM/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/M/yyyy", "dd/MM/yy", "d/MM/yy", "dd/M/yy", "M/d/yy",
               "yyyy/dd/MM", "yyyy/d/M", "yyyy/d/MM", "yyyy/dd/M", "yy/dd/MM", "yy/d/M", "yy/d/MM", "yy/dd/M",
               "yyyy/MM/dd", "yyyy/M/d", "yyyy/MM/d", "yyyy/M/dd", "yy/MM/dd", "yy/M/d", "yy/MM/d", "yy/M/dd",

               "MM.dd.yyyy", "M.d.yyyy", "MM.d.yyyy", "M.dd.yyyy", "MM.dd.yy", "M.dd.yy", "MM.d.yy", "d.M.yy",
               "dd.MM.yyyy", "d.M.yyyy", "d.MM.yyyy", "dd.M.yyyy", "dd.MM.yy", "d.MM.yy", "dd.M.yy", "M.d.yy",
               "yyyy.dd.MM", "yyyy.d.M", "yyyy.d.MM", "yyyy.dd.M", "yy.dd.MM", "yy.d.M", "yy.d.MM", "yy.dd.M",
               "yyyy.MM.dd", "yyyy.M.d", "yyyy.MM.d", "yyyy.M.dd", "yy.MM.dd", "yy.M.d", "yy.MM.d", "yy.M.dd",

               "dd MMM yyyy", "dd MMMM yyyy", "d MMM yyyy", "d MMMM yyyy", "dd MMM yy", "dd MMMM yy", "d MMM yy", "d MMMM yy",
               "yyyy dd MMM", "yyyy dd MMMM", "yyyy d MMM", "yyyy d MMMM", "yy dd MMM", "yy dd MMMM", "yy d MMM", "yy d MMMM",

               "MMM dd yyyy", "MMMM dd yyyy", "MMM d yyyy", "MMMM d yyyy", "MMM dd yy", "MMMM dd yy", "MMM d yy", "MMMM d yy",
               "yyyy MMM dd", "yyyy MMMM dd", "yyyy MMM d", "yyyy MMMM d", "yy MMM dd", "yy MMMM dd", "yy MMM d", "yy MMMM d",

               "dd-MMM-yyyy", "dd-MMMM-yyyy", "d-MMM-yyyy", "d-MMMM-yyyy", "dd-MMM-yy", "dd-MMMM-yy", "d-MMM-yy", "d-MMMM-yy",
               "yyyy-dd-MMM", "yyyy-dd-MMMM", "yyyy-d-MMM", "yyyy-d-MMMM", "yy-dd-MMM", "yy-dd-MMMM", "yy-d-MMM", "yy-d-MMMM",

               "MMM-dd-yyyy", "MMMM-dd-yyyy", "MMM-d-yyyy", "MMMM-d-yyyy", "MMM-dd-yy", "MMMM-dd-yy", "MMM-d-yy", "MMMM-d-yy",
               "yyyy-MMM-dd", "yyyy-MMMM-dd", "yyyy-MMM-d", "yyyy-MMMM-d", "yy-MMM-dd", "yy-MMMM-dd", "yy-MMM-d", "yy-MMMM-d",
           }.Union(ci.DateTimeFormat.GetAllDateTimePatterns()).ToArray();
        }
    }
}

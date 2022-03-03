using System;
using System.Collections.Generic;
using System.Text;

namespace tool_common
{
    public static class DataConvertHelper
    { /// <summary>
      /// 布尔转T或F的字符串
      /// </summary>
      /// <param name="source"></param>
      /// <returns></returns>
        public static String ConvertBoolToTOrF(bool source)
        {
            string result = "F";
            if (source)
            {
                result = "T";
            }
            return result;
        }

        public static int ConvertToWeek<T>(T source)
        {
            DateTime? date = DataConvertHelper.ConvertDateTimeOrNull(source);
            int result = 0;

            if (date.HasValue)
            {
                result = (int)date.Value.DayOfWeek;
                if (result == 0)
                {
                    result = 7;
                }
            }
            return result;
        }
        /// <summary>
        /// 时间，一星期的星期几
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public static int ConvertToWeek(DayOfWeek week)
        {
            int result = (int)week;

            if (result == 0)
            {
                result = 7;
            }
            return result;
        }
        /// <summary>
        /// 转时间，可为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime? ConvertDateTimeOrNull<T>(T source)
        {
            DateTime result;

            if (source == null)
            {
                return null;
            }
            else if (DateTime.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static bool ConvertToBool<T>(T source)
        {
            bool result;

            if (source == null)
            {
                return false;
            }
            if (source.ToString().ToUpper().Equals("T"))
            {
                return true;
            }
            if (source.ToString().ToUpper().Equals("F"))
            {
                return false;
            }
            if (Boolean.TryParse(source.ToString().ToLower(), out result))
            {
                return result;
            }
            else
            {
                return false;
            }
        }

        public static string FormatCantonName(string name)
        {
            if (name == null || name.Length == 0)
            {
                return string.Empty;
            }

            int index = name.Length;
            for (int i = 0; i < name.Length; i++)
            {
                if (Char.IsNumber(name[i]) || name[i] == '(' || name[i] == '*' || name[i] == 'N')
                {
                    index = i;
                    break;
                }
            }

            return name.Substring(0, index);
        }

        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(str);
            foreach (byte c in bytestr)
            {
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static V ConvertToEnum<T, V>(T source) where V : new()
        {
            Type type = typeof(V);
            V v = new V();

            if (source == null)
            {
                return v;
            }

            try
            {
                string sourceString = source.ToString();
                return (V)Enum.Parse(type, sourceString, true);
            }
            catch
            {
                return v;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte ConvertToByte<T>(T source)
        {
            byte result;

            if (source == null)
            {
                return default(byte);
            }
            else if (byte.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(byte);
            }
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int RoundToInt<T>(T source)
        {
            if (source == null)
            {
                return 0;
            }
            int ret;
            if (int.TryParse(source.ToString(), out ret))
            {
                return ret;
            }
            return
             ConvertObjectToInt32(
            Math.Floor(ConvertObjectToDecimalSingle(source) + 0.5M));
        }

        /// <summary>
        /// 转换数据为decimal?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal? ConvertObjectToDecimal<T>(T source)
        {
            decimal result;

            if (source == null)
            {
                return null;
            }
            else if (decimal.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal ConvertObjectToDecimalSingle<T>(T source)
        {
            decimal? result = ConvertObjectToDecimal(source);

            if (result != null)
            {
                return result.Value;
            }
            else
            {
                return 0M;
            }
        }

        /// <summary>
        /// 转换数据为int型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ConvertObjectToInt32<T>(T source)
        {
            int result;

            if (source == null)
            {
                return 0;
            }
            else if (int.TryParse(String.Format("{0:f0}", source), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换数据为long型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long ConvertObjectToInt64<T>(T source)
        {
            long result;

            if (source == null)
            {
                return 0;
            }
            else if (long.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换数据为short?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static short? ConvertObjectToInt16OrNull<T>(T source)
        {
            short result;

            if (source == null)
            {
                return null;
            }
            else if (short.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转换数据为short?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static short ConvertObjectToInt16<T>(T source)
        {
            short result;

            if (source == null)
            {
                return 0;
            }
            else if (short.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换数据为Double?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double ConvertObjectToDouble<T>(T source)
        {
            double result;

            if (source == null)
            {
                return 0;
            }
            else if (double.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换数据为int?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int? ConvertObjectToInt32OrNull<T>(T source)
        {
            int result;

            if (source == null)
            {
                return null;
            }
            else if (int.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转换数据为long?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long? ConvertObjectToInt64OrNull<T>(T source)
        {
            long result;

            if (source == null)
            {
                return null;
            }
            else if (long.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ConvertDateTime<T>(T source)
        {
            DateTime result;

            if (source == null)
            {
                return default(DateTime);
            }
            else if (DateTime.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// 将可空DateTime转换成字符串"yyyy-MM-dd hh:mm:ss"(如果为空则返回null)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToStr(DateTime? source)
        {
            string strReturn = null;
            if (source != null)
            {
                strReturn = ConvertDateTimeToStr(source.GetValueOrDefault());
            }
            return strReturn;
        }

        /// <summary>
        /// 将DateTime转换成字符串"yyyy-MM-dd hh:mm:ss"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToStr(DateTime source)
        {
            string strReturn = null;
            strReturn = source.ToString("yyyy-MM-dd HH:mm:ss");
            return strReturn;
        }

        public static TimeSpan ConvertTimeSpan<T>(T source)
        {
            TimeSpan result;

            if (source == null)
            {
                return default(TimeSpan);
            }
            else if (TimeSpan.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(TimeSpan);
            }
        }

        public static float ConvertToSingle<T>(T source)
        {
            float result;

            if (source == null)
            {
                return 0f;
            }
            else if (float.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return 0f;
            }
        }

        public static int ConvertToFormatInt<T>(T source, int provider)
        {
            try
            {
                return Convert.ToInt32(source.ToString(), provider);
            }
            catch
            {
                return 0;
            }
        }

        public static string ConvertToTrimString<T>(T source)
        {
            string result = null;
            if (source != null)
            {
                result = source.ToString();
            }
            if (!String.IsNullOrEmpty(result))
            {
                result = result.Trim();
            }

            return result;
        }

        /// <summary>
        /// 字符串分隔成数组
        /// </summary>
        /// <param name="tmpStr">分隔字符串</param>
        /// <param name="separator">分隔符</param>
        /// <returns>数组</returns>
        public static String[] StringSplit(String tmpStr, Char separator)
        {
            String[] rtnStr = tmpStr.Split(separator);

            return rtnStr;
        }

        public static T ToType<T>(object o)
        {
            if (o == null || o == DBNull.Value)
                return default(T);
            return (T)Convert.ChangeType(o, typeof(T));
        }

        public static T ToType<T>(object o, T defaultValue)
        {
            T value = default(T);
            try
            {
                value = ToType<T>(o);
            }
            catch
            {
                value = defaultValue;
            }
            return value;
        }

        public static T TryType<T>(this object obj, T defaultValue)
        {
            return DataConvertHelper.ToType<T>(obj, defaultValue);
        }

        public static T TryType<T>(this object obj)
        {
            return DataConvertHelper.ToType<T>(obj);
        }

        /// <summary>
        /// 判断字符串IsNullOrEmpty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断字符串NotIsNullEmpty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool NotIsNullEmpty(this string str)
        {
            return !(string.IsNullOrEmpty(str));
        }

        /// <summary>
        /// 格式化日期 到月份
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormateMonth(object time)
        {
            if (time != null && Convert.ToDateTime(time) != DateTime.MinValue)
            {
                return Convert.ToDateTime(time).ToString("yyyy-MM");
            }
            return string.Empty;
        }

        /// <summary>
        /// 格式化日期
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormateData(object time)
        {
            if (time != null && Convert.ToDateTime(time) != DateTime.MinValue)
            {
                return Convert.ToDateTime(time).ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormateTime(object time)
        {
            if (time != null && Convert.ToDateTime(time) != DateTime.MinValue)
            {
                return Convert.ToDateTime(time).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return string.Empty;
        }

        /// <summary>
        /// 截取字符串长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FormateString(object str, int len)
        {
            if (str != null && !string.IsNullOrEmpty(str.ToString()))
            {
                if (str.ToString().Length > len)
                {
                    return str.ToString().Substring(0, len) + "...";
                }
                else
                {
                    return str.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取中英文混排字符串的实际长度(字节数)
        /// </summary>
        /// <param name="str">要获取长度的字符串</param>
        /// <returns>字符串的实际长度值（字节数）</returns>
        public static int GetStringLength(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            int strlen = 0;
            ASCIIEncoding strData = new ASCIIEncoding();
            byte[] strBytes = strData.GetBytes(str);
            for (int i = 0; i <= strBytes.Length - 1; i++)
            {
                if (strBytes[i] == 63)
                {
                    strlen++;
                }
                strlen++;
            }
            return strlen;
        }

        /// <summary>
        /// 获取长期有效时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLongTimeExpirationDay()
        {
            DateTime dt = new DateTime(2099, 12, 31);
            return dt;
        }

        /// <summary>
        /// 获取小数价格
        /// </summary>
        /// <param name="originalPrice"></param>
        /// <returns></returns>
        public static decimal GetDecimalPrice(decimal originalPrice)
        {
            decimal price = 0;
            price = Math.Round(originalPrice, 2);

            return price > 0 ? price : 0;
        }

        /// <summary>
        /// 获取范围内的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        public static string GetStringLengthInRange(string str, int maxLength, int minLength)
        {
            string strBlank = "&nbsp;";
            if (str != null)
            {
                if (str.Length > maxLength)
                {
                    str = str.Substring(0, maxLength) + "...";
                }
                if (str.Length < minLength)
                {
                    int needAdd = minLength - str.Length;
                    for (int i = 0; i < needAdd; i++)
                    {
                        str = str + strBlank;
                    }
                }
            }
            else
            {
                str = strBlank;
            }
            return str;
        }
    }
}
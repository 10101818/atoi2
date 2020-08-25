using BusinessObjects.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace BusinessObjects.Util
{
    /// <summary>
    /// sql类型转换
    /// </summary>
    public static class SQLUtil
    {
        /// <summary>
        /// 内容置空
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static String TrimNull(Object obj)
        {
            if (obj == DBNull.Value)
                return "";
            else if (obj == null)
                return "";
            else
                return obj.ToString().Trim();
        }

        /// <summary>
        /// 值转int类型
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static int ConvertInt(Object obj)
        {
            if (obj == DBNull.Value)
                return 0;
            else if (obj == null || obj == "")
                return 0;
            else
                return Convert.ToInt32(obj.ToString().Replace(",", ""));
        }

        /// <summary>
        /// 值转long类型
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static long ConvertLong(Object obj)
        {
            if (obj == DBNull.Value)
                return 0;
            else if (obj == null)
                return 0;
            else
                return Convert.ToInt64(obj.ToString().Replace(",", ""));
        }

        /// <summary>
        /// 值转double类型
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static double ConvertDouble(Object obj, int accuracy=1)
        {
            if (obj == DBNull.Value)
                return 0;
            else if (obj == null)
                return 0;
            else if (TrimNull(obj) == "")
                return 0;
            else
            {
                double result = Convert.ToDouble(obj);
                return accuracy <= 0 ? result : Math.Round(result, accuracy, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// 值转DateTime类型
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static DateTime ConvertDateTime(Object obj)
        {
            DateTime result = new DateTime();

            if (obj == DBNull.Value)
                return DateTime.MinValue;
            else if (obj == null)
                return DateTime.MinValue;
            else if (TrimNull(obj) == "")
                return DateTime.MinValue;
            else
            {
                DateTime.TryParse(obj.ToString(), out result);
                return result;
            }
        }

        /// <summary>
        /// 值转TimeSpan类型
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static TimeSpan ConvertTime(Object obj)
        {
            if (obj == DBNull.Value)
                return TimeSpan.Zero;
            else if (obj == null)
                return TimeSpan.Zero;
            else
                return (TimeSpan)obj;
        }

        /// <summary>
        /// 值转byte[]类型
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static byte[] ConvertBytes(object obj)
        {
            if (obj == DBNull.Value)
                return new byte[0];
            else if (obj == null)
                return new byte[0];
            else
                return (byte[])obj;
        }

        /// <summary>
        /// 值转boolean类型
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>传出值</returns>
        public static Boolean ConvertBoolean(Object obj)
        {
            bool result = false;

            if (obj == DBNull.Value)
                return false;
            else if (obj == "")
                return false;
            else if (obj == null)
                return false;
            else if (obj.ToString() == "1")
                return true;
            else if (obj.ToString() == "0")
                return false;
            else
            {
                Boolean.TryParse(obj.ToString(), out result);
                return result;
            }
        }

        /// <summary>
        /// string数组转int泛型
        /// </summary>
        /// <param name="stringArray">string数组</param>
        /// <returns>int数组</returns>
        public static List<int> ConvertToIntList(string [] stringArray)
        {
            List<int> values = new List<int>();
            foreach (string s in stringArray)
            {
                values.Add(ConvertInt(s));
            }
            return values;
        }

        /// <summary>
        /// string泛型转string
        /// </summary>
        /// <param name="values">string泛型</param>
        /// <returns>string</returns>
        public static string ConvertToStr(List<string> values)
        {
            string inStr = "";
            foreach (string value in values)
            {
                if (inStr.Length > 0)
                    inStr += ",";
                inStr += value;
            }

            return inStr;
        }

        /// <summary>
        /// object泛型转string
        /// </summary>
        /// <param name="values">object泛型</param>
        /// <returns>string</returns>
        public static string ConvertToStr(List<Object> values)
        {
            string inStr = "";
            foreach (Object value in values)
            {
                if (inStr.Length > 0)
                    inStr += ",";
                inStr += TrimNull(value);
            }

            return inStr;
        }

        /// <summary>
        /// int泛型转string
        /// </summary>
        /// <param name="values">int泛型</param>
        /// <returns>string</returns>
        public static string ConvertToInStr(List<int> values)
        {
            string inStr = "";
            foreach (long value in values)
            {
                if (inStr.Length > 0)
                    inStr += ",";
                inStr += value.ToString();
            }

            return inStr;
        }

        /// <summary>
        /// string泛型转string
        /// </summary>
        /// <param name="values">string泛型</param>
        /// <returns>string</returns>
        public static string ConvertToInStr(List<string> values)
        {
            string inStr = "";
            foreach (string value in values)
            {
                if (inStr.Length > 0)
                    inStr += ",";
                inStr += "'" + value.Replace("'", "''") + "'";
            }

            return inStr;
        }

        #region "Return DBNull"
        /// <summary>
        /// 空字符串转null
        /// </summary>
        /// <param name="strData">传入值</param>
        /// <returns>传出值</returns>
        public static object EmptyStringToNull(string strData)
        {
            if (string.IsNullOrEmpty(strData))
                return DBNull.Value;
            else
                return strData.Trim();
        }

        /// <summary>
        /// 0转null
        /// </summary>
        /// <param name="intData">传入值</param>
        /// <returns>传出值</returns>
        public static object ZeroToNull(int intData)
        {
            if (intData == 0)
                return DBNull.Value;
            else
                return intData;
        }

        /// <summary>
        /// 年份转null
        /// </summary>
        /// <param name="daData">传入值</param>
        /// <returns>传出值</returns>
        public static object MinDateToNull(DateTime daData)
        {
            if (daData == DateTime.MinValue)
                return DBNull.Value;
            else
                return daData;
        }

        /// <summary>
        /// false转null
        /// </summary>
        /// <param name="boolData">传入值</param>
        /// <returns>传出值</returns>
        public static object FalseToNull(bool boolData)
        {
            if (boolData == false)
                return DBNull.Value;
            else
                return boolData;
        }

        /// <summary>
        /// null转DBNull
        /// </summary>
        /// <param name="objData">传入值</param>
        /// <returns>传出值</returns>
        public static object NullToDBNull(object objData)
        {
            if (objData == null)
                return DBNull.Value;
            else
                return objData;
        }
        #endregion

        /// <summary>
        /// 统计数据表中的id
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="idFeildName">字段名</param>
        /// <returns>id列表</returns>
        public static List<int> GetIDListFromDataTable(DataTable dt, string idFeildName = "ID")
        {
            List<int> ids = new List<int>();
            int id;
            foreach (DataRow dr in dt.Rows)
            {
                id = SQLUtil.ConvertInt(dr[idFeildName]);

                if (!ids.Contains(id)) ids.Add(id);
            }

            return ids;
        }

        /// <summary>
        /// 统计对象泛型中的id
        /// </summary>
        /// <param name="objects">对象泛型</param>
        /// <param name="idFeildName">字段名</param>
        /// <returns>id列表</returns>
        public static List<int> GetIDListFromObjectList(IList objects, string idFeildName = "ID")
        {
            List<int> ids = new List<int>();
            int id;
            foreach (Object obj in objects)
            {
                id = (int)obj.GetType().GetProperty(idFeildName).GetValue(obj, null);

                if (!ids.Contains(id)) ids.Add(id);
            }

            return ids;
        }

        /// <summary>
        /// 统计对象数组中的value
        /// </summary>
        /// <param name="objects">对象数组</param>
        /// <param name="valueFeildName">字段名</param>
        /// <returns>Object列表</returns>
        public static List<Object> GetValueListFromObjectList(IList objects, string valueFeildName = "Value")
        {
            List<Object> values = new List<Object>();

            foreach (Object obj in objects)
            {
                values.Add(obj.GetType().GetProperty(valueFeildName).GetValue(obj, null));
            }

            return values;
        }

        /// <summary>
        /// 统计对象数组中的value
        /// </summary>
        /// <param name="objects">对象数组</param>
        /// <param name="valueFeildName">字段名</param>
        /// <param name="includeEmpty">是否允许空值</param>
        /// <returns>string列表</returns>
        public static List<string> GetStringListFromObjectList(IList objects, string valueFeildName = "Value", bool includeEmpty = true)
        {
            List<string> values = new List<string>();

            string value = null;
            if (objects != null)
            {
                foreach (Object obj in objects)
                {
                    value = TrimNull(obj.GetType().GetProperty(valueFeildName).GetValue(obj, null));
                    if (includeEmpty == true || !string.IsNullOrEmpty(value))
                    {
                        if (!values.Contains(value)) values.Add(value);
                    }
                }
            }            

            return values;
        }

        /// <summary>
        /// 获取table的列名
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>列名list</returns>
        public static List<string> GetTableColumnNames(DataTable dt)
        {
            List<string> values = new List<string>();

            foreach (DataColumn col in dt.Columns)
            {
                values.Add(col.ColumnName);
            }

            return values;
        } 
        /// <summary>
        /// 转换枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertEnum<T>(Object obj)
        {
            int value = ConvertInt(obj);
            if (typeof(T).IsEnum && Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            else
                throw new ArgumentException(EnumUtil.ENUMTYPEERROR);
        }

        /// <summary>
        /// 列表每个值×比率
        /// </summary>
        /// <param name="amountList">list列表</param>
        /// <param name="rate">比率</param>
        /// <returns>计算后的比率</returns>
        public static List<double> GetPercentageAmount(List<double> amountList, double rate)
        {
            List<double> result = new List<double>();

            foreach(double amount in amountList)
            {
                result.Add(Math.Round(amount * rate / 100.0, 2));
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static List<double> MergeList(List<double> list1, List<double> list2, params List<double>[] lists)
        {
            List<double> result = new List<double>();

            int maxLength = list1.Count > list2.Count ? list1.Count : list2.Count;
            foreach (List<double> list in lists)
            {
                if (maxLength < list.Count) maxLength = list.Count;
            }

            double t = 0;
            for (int i = 0; i < maxLength; i++)
            {
                t = 0;

                if (list1.Count > i) t += list1[i];
                if (list2.Count > i) t += list2[i];

                foreach (List<double> list in lists)
                {
                    if (list.Count > i) t += list[i];
                }

                result.Add(Math.Round(t, 2));
            }

            return result;

        }
    }
}

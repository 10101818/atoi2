using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Util
{
    /// <summary>
    /// 枚举帮助类
    /// author-manyee
    /// </summary>
    public static class EnumUtil
    {  
        /// <summary>
        /// 错误描述
        /// </summary>
        public const string ENUMTYPEERROR = "传入参数或类型非可转换枚举类型！";
        /// <summary>
        /// 获取枚举项上设置的显示文字 Description 标注的 属性
        /// </summary>
        /// <param name="value">被扩展对象</param>
        public static string GetDescription(this IConvertible value)//若C#提高到7.3版本，可直接使用 Enum 替换 IConvertible
        {
            if (value.GetType().IsEnum)
            {
                string name = Enum.GetName(value.GetType(), value);
                if (string.IsNullOrEmpty(name))
                    return value.ToString();
                var attribute = value.GetType().GetField(name).GetCustomAttributes(
                        typeof(DescriptionAttribute), false)
                        .Cast<DescriptionAttribute>()
                        .FirstOrDefault();
                if (attribute != null)
                    return attribute.Description;

                return value.ToString();
            }
            else
            {
                throw new ArgumentException(ENUMTYPEERROR);
            }
        }
        /// <summary>
        /// 将枚举全部元素转换成 数字集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<int> ToIntList<T>() where T : struct, IConvertible //若C#提高到7.3版本，可直接使用 Enum 替换 struct, IConvertible,限定泛型参数类型
        {
            if (typeof(T).IsEnum)
            {
                List<int> list = new List<int>();

                foreach (var e in Enum.GetValues(typeof(T)))
                {
                    list.Add((int)e);
                }
                return list;
            }
            else
            {
                throw new ArgumentException(ENUMTYPEERROR);
            }
        }
        /// <summary>
        /// 获取枚举全部元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ToList<T>() where T : struct, IConvertible //若C#提高到7.3版本，可直接使用 Enum 替换 struct, IConvertible,限定泛型参数类型
        {
            if (typeof(T).IsEnum)
            {
                List<T> list = new List<T>();
                foreach (T info in Enum.GetValues(typeof(T)))
                {
                    list.Add(info);
                }
                return list;
            }
            else
            {
                throw new ArgumentException(ENUMTYPEERROR);
            }
        }
        /// <summary>
        /// 获取枚举转换键值对集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<KeyValueInfo<T>> ToKVList<T>() where T : struct, IConvertible //若C#提高到7.3版本，可直接使用 Enum 替换 struct, IConvertible,限定泛型参数类型
        {
            if (typeof(T).IsEnum)
            {
                List<KeyValueInfo<T>> list = new List<KeyValueInfo<T>>();
                foreach (T info in Enum.GetValues(typeof(T)))
                {
                    list.Add(new KeyValueInfo<T>(info));
                }
                return list;
            }
            else
            {
                throw new ArgumentException(ENUMTYPEERROR);
            }
        }
        /// <summary>
        /// 匹配DescriptionName与给定值相等的枚举元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetEnum<T>(string description) where T : struct, IConvertible //若C#提高到7.3版本，可直接使用 Enum 替换 struct, IConvertible,限定泛型参数类型
        {
            if (typeof(T).IsEnum)
            {
                return ToList<T>().Where(info => info.GetDescription().Equals(description.Trim())).FirstOrDefault();
            }
            else
            {
                throw new ArgumentException(ENUMTYPEERROR);
            }
        }
    }


    /// <summary>
    /// 枚举键值对
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyValueInfo<T> where T : struct, IConvertible //若C#提高到7.3版本，可直接使用 Enum 替换 struct, IConvertible,限定泛型参数类型
    {
        private T id;
        /// <summary>
        /// ID
        /// </summary>
        public T ID
        {
            get { return this.id; }
            set
            {
                if (typeof(T).IsEnum)
                {
                    this.id = value;
                }
                else
                {
                    throw new ArgumentException(EnumUtil.ENUMTYPEERROR);
                }
            }
        }
        private string name;
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            set { this.name = value; }
            get
            {
                if (typeof(T).IsEnum)
                {
                    if ((int)Convert.ChangeType(this.ID, typeof(int)) == 0)
                        return this.name;
                    else
                        return this.ID.GetDescription();
                }
                else
                {
                    throw new ArgumentException(EnumUtil.ENUMTYPEERROR);
                }
            }
        }
        /// <summary>
        /// KeyValueInfo
        /// </summary>
        public KeyValueInfo() { }
        /// <summary>
        /// KeyValueInfo
        /// </summary>
        /// <param name="t">t</param>
        public KeyValueInfo(T t)
        {
            if (typeof(T).IsEnum)
            {
                this.id = t;
            }
            else
            {
                throw new ArgumentException(EnumUtil.ENUMTYPEERROR);
            }
        }
    } 
}

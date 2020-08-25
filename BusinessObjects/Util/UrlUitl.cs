using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Util
{
    /// <summary>
    /// url
    /// </summary>
    public static class UrlUitl
    {
        /// <summary>
        /// 获取父页面url
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns>父页面url</returns>
        public static string GetParentUrl(Uri uri)
        {
            return uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length);
        }

        /// <summary>
        /// 获取最后一个Segment
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>最后一个Segment</returns>
        public static string GetLastSegment(string url)
        {
            Uri uri = new Uri(url);

            return uri.Segments.Last();
        }
    }
}

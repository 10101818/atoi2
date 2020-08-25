using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Models
{
    /// <summary>
    /// JsonNetResult
    /// </summary>
    public class JsonNetResult : JsonResult
    {
        /// <summary>
        /// 删除字段值为null的字段
        /// </summary>
        /// <param name="context">返回的参数信息</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType)  ? ContentType  : "application/json";

            if (ContentEncoding != null)  response.ContentEncoding = ContentEncoding;

            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            setting.Converters.Add(new MinDateTimeConverter());

            var serializedObject = JsonConvert.SerializeObject(Data, setting);

            response.Write(serializedObject);
        }
    }
}
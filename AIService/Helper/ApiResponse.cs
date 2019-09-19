using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AIService.Helper
{
    /// <summary>
    /// API返回数据的错误信息集合封装类型
    /// </summary>
    public class ResponseDataItem
    {
        /// <summary>
        /// 错误对应的属性（页面的表单元素）
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
    public static class Message
    {
        public static string Invalid = "数据填写有误！";
        public static string Unauthorized = "请登录后再操作！";
        public static string Success = "操作成功！";
        public static string Failure = "操作发生错误！";
        public static string GetDataError = "数据获取发生错误！";
        public static string QueryInitError = "搜索初始化错误！";
        //public static string Error = "服务器发生错误！";
        public static string NoAccess = "无操作权限！";
        public static string Conflict = "具有相同特征的数据已存在！";
        public static string NotFound = "指定数据不存在！";
        public static string CreateSuccess = "数据添加成功！";
        public static string CreateFailure = "数据添加错误，请检查数据，重新操作！";
        public static string NoDeleteItems = "请选择需要删除的数据";
        public static string DeletePartialSuccess = "部分指定的数据不存在，其余数据删除成功";
        public static string DeleteSuccess = "数据删除成功！";
        public static string DeleteDataNotFound = "需要删除的数据不存在！";
        public static string DeleteFailure = "数据删除错误，请刷新页面，重新操作！";
        public static string DeletesSuccess = "指定数据删除成功！";
        public static string DeletesFailure = "数据删除错误，请刷新页面，重新操作！";
        public static string EditSuccess = "数据更新成功！";
        public static string EditFailure = "数据更新错误，请检查数据，重新操作！";
        public static string OrderUpSuccess = "数据调整排序成功！";
        public static string OrderIsTop = "已经排至最前！";
        public static string OrderUpFailure = "数据调整排序错误，请刷新页面，重新操作！";
        public static string ExportFailure = "数据导出发生错误！";
    }
    /// <summary>
    /// API请求返回封装
    /// </summary>
    public class ApiResponse
    {
        enum ResponseCode
        {
            Ok = 200,
            NoContent = 204,
            BadRequest = 400,
            Unauthorized = 401,
            Invalid = 402,
            NoAccess = 403,
            NotFound = 404,
        }
        class ResponseData<T>
        {
            private ResponseCode code { get; set; }
            private string message { get; set; }
            private T data { get; set; }
            private IEnumerable<T> datas { get; set; }
            public ResponseData(ResponseCode code)
            {
                this.code = code;
                this.message = null;
                this.datas = null;
            }
            public ResponseData(ResponseCode code, string message)
            {
                this.code = code;
                this.message = message;
                this.datas = null;
            }
            public ResponseData(ResponseCode code, T data)
            {
                this.code = code;
                this.data = data;
                this.message = null;
            }
            public ResponseData(ResponseCode code, string message, T data)
            {
                this.code = code;
                this.message = message;
                this.data = data;
                this.datas = null;
            }
            public ResponseData(ResponseCode code, IEnumerable<T> datas)
            {
                this.code = code;
                this.datas = datas;
                this.message = null;
            }
            public ResponseData(ResponseCode code, string message, IEnumerable<T> datas)
            {
                this.code = code;
                this.message = message;
                this.datas = datas;
            }
            public string ToJsonString()
            {
                StringBuilder result = new StringBuilder("{");
                result.AppendFormat("\"code\":{0}", (int)this.code);
                if (message != null)
                    result.AppendFormat(",\"message\":\"{0}\"", this.message);
                if (data != null)
                    result.AppendFormat(",\"data\":{0}", JsonConvert.SerializeObject(this.data));
                else if (datas != null)
                    result.AppendFormat(",\"data\":{0}", this.datas.ToJsonString());
                result.Append("}");
                return result.ToString();
            }
        }
        /// <summary>
        /// 成功，并附带提示信息
        /// </summary>
        public static HttpResponseMessage Ok(string message)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<string>(ResponseCode.Ok, message).ToJsonString()) };
        }
        /// <summary>
        /// 成功，并附带对象数据
        /// </summary>
        public static HttpResponseMessage Ok<T>(T data)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<T>(ResponseCode.Ok, data).ToJsonString()) };
        }
        /// <summary>
        /// 成功，并附带提示信息和对象数据
        /// </summary>
        public static HttpResponseMessage Ok<T>(string message, T data)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<T>(ResponseCode.Ok, message, data).ToJsonString()) };
        }
        internal static HttpResponseMessage Invalid(string conflict)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 成功，并附带集合类型数据
        /// </summary>
        public static HttpResponseMessage Ok<T>(IEnumerable<T> datas)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<T>(ResponseCode.Ok, datas).ToJsonString()) };
        }
        /// <summary>
        /// 成功，并附带提示信息和集合类型数据
        /// </summary>
        public static HttpResponseMessage Ok<T>(string message, IEnumerable<T> datas)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<T>(ResponseCode.Ok, message, datas).ToJsonString()) };
        }
        /// <summary>
        /// 成功，不附带数据
        /// </summary>
        public static HttpResponseMessage NoContent()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<string>(ResponseCode.NoContent).ToJsonString()) };
        }
        /// <summary>
        /// 请求错误，附带字符串形式的错误信息
        /// </summary>
        public static HttpResponseMessage BadRequest(string message)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<string>(ResponseCode.BadRequest, message).ToJsonString()) };
        }
        /// <summary>
        /// 未登录，不附带数据
        /// </summary>
        public static HttpResponseMessage Unauthorized()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<string>(ResponseCode.Unauthorized).ToJsonString()) };
        }
        /// <summary>
        /// 数据验证错误，附带ResponseDataItem集合类型数据，序列化为json数组格式
        /// </summary>
        public static HttpResponseMessage Invalid(List<ResponseDataItem> errors)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<ResponseDataItem>(ResponseCode.Invalid, errors).ToJsonString()) };
        }
        /// <summary>
        /// 数据验证错误，附带单条验证错误信息，序列化为json数组格式
        /// </summary>
        /// <param name="target">发生错误的属性，对应于页面表单元素</param>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static HttpResponseMessage Invalid(string target, string message)
        {
            List<ResponseDataItem> errors = new List<ResponseDataItem>();
            errors.Add(new ResponseDataItem { Target = target, Message = message });
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<ResponseDataItem>(ResponseCode.Invalid, errors).ToJsonString()) };
        }
        /// <summary>
        /// 无权限访问，可附带提示信息，默认提示信息为“无操作权限！”
        /// </summary>
        public static HttpResponseMessage NoAccess(string message = null)
        {
            if (string.IsNullOrEmpty(message)) message = Message.NoAccess;
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<string>(ResponseCode.NoAccess, message).ToJsonString()) };
        }
        /// <summary>
        /// 请求不存在，可附带提示信息，默认提示信息为“指定数据不存在！”
        /// </summary>
        public static HttpResponseMessage NotFound(string message = null)
        {
            if (string.IsNullOrEmpty(message)) message = Message.NotFound;
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(new ResponseData<string>(ResponseCode.NotFound, message).ToJsonString()) };
        }
    }
}

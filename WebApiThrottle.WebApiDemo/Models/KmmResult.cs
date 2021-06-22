using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiThrottle.WebApiDemo.Models
{
    /// <summary>
    /// Class KmmResultBase.
    /// </summary>
    public class KmmResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KmmResultBase"/> class.
        /// </summary>
        public KmmResultBase()
        {
            this.code = KmHttpError.NetworkException;
            this.msg = "网络异常或系统错误";
        }
        /// <summary>
        /// 接口请求成功
        /// </summary>
        /// <returns></returns>
        public KmmResultBase Success()
        {
            this.code = KmHttpError.Success;
            this.msg = "OK";
            return this;
        }

        /// <summary>
        /// 接口请求失败
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public KmmResultBase Error(string code)
        {
            SetError(code);
            return this;
        }
        /// <summary>
        /// Errors the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns>KmmResultBase.</returns>
        public KmmResultBase Error(string code, string msg)
        {
            SetError(code, msg);
            return this;
        }
        /// 设置错误信息
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="msg">The MSG.</param>
        protected void SetError(string code, string msg = null)
        {
            this.code = code;
            this.msg =msg;
            if (string.IsNullOrEmpty(this.msg))
            {
                this.msg = "未知错误";
            }
        }
        /// <summary>
        /// 返回编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string msg { get; set; }

    }
    /// <summary>
    /// web api 请求结果
    /// </summary>
    /// <typeparam name="T">返回结果类型</typeparam>
    public class KmmResult<T> : KmmResultBase
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        public object extInfo { get; set; }

        /// <summary>
        /// 接口调用成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public KmmResult<T> Success(T data)
        {
            SetSuccess(data);
            return this;
        }
        /// <summary>
        /// Sets the success.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="extInfo">The ext information.</param>
        protected void SetSuccess(T data)
        {
            this.code = KmHttpError.Success;
            this.msg = "OK";
            this.data = data;
        }

        /// <summary>
        /// 接口调用失败
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <param name="msg"></param>
        /// <param name="extInfo"></param>
        /// <returns></returns>
        public KmmResult<T> Error(string code, string msg, object extInfo = null)
        {
            SetError(code, msg);
            this.extInfo = extInfo;
            return this;
        }
        /// <summary>
        /// Errors the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>KmmResult&lt;T&gt;.</returns>
        public new KmmResult<T> Error(string code)
        {
            SetError(code);
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KmmResult{T}"/> class.
        /// </summary>
        public KmmResult()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="KmmResult{T}"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="msg">The MSG.</param>
        public KmmResult(string code, string msg)
        {
            SetError(code, msg);
        }
    }

    /// <summary>
    /// Class KmHttpError.
    /// </summary>
    public class KmHttpError
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        public const string Success = "0000";

        /// <summary>
        /// 签名失败
        /// </summary>
        public const string SignFail = "0001";

        /// <summary>
        /// 错误
        /// </summary>
        public const string Error = "0002";

        /// <summary>
        /// 异常
        /// </summary>
        public const string Exception = "0003";

        /// <summary>
        /// 请求参数错误
        /// </summary>
        public const string ParameterError = "0004";

        /// <summary>
        /// 网络异常
        /// </summary>
        public const string NetworkException = "0007";

        /// <summary>
        /// 报头签名失败
        /// </summary>
        public const string HeaderSignFail = "0008";

        /// <summary>
        /// 验证码错误
        /// </summary>
        public const string CodeFail = "0009";
    }
}
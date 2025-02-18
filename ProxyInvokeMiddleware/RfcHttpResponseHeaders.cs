namespace ProxyInvokeMiddleware
{
    /// <summary>
    /// rfc2616
    /// https://www.w3.org/Protocols/rfc2616/rfc2616.html
    /// </summary>
    public static class RfcHttpResponseHeaders
    {
        /// <summary>
        /// 表明服务器是否支持指定范围请求及哪种类型的分段请求
        /// Accept-Ranges: bytes
        /// </summary>
        public const string AcceptRanges = "Accept-Ranges";
        /// <summary>
        /// 从原始服务器到代理缓存形成的估算时间（以秒计，非负）
        /// Age: 12
        /// </summary>
        public const string Age = "Age";
        /// <summary>
        /// 对某网络资源的有效的请求行为，不允许则返回405
        /// Allow: GET, HEAD
        /// </summary>
        public const string Allow = "Allow";
        /// <summary>
        /// 告诉所有的缓存机制是否可以缓存及哪种类型
        /// Cache-Control: no-cache
        /// </summary>
        public const string CacheControl = "Cache-Control";
        /// <summary>
        /// web服务器支持的返回内容压缩编码类型。
        /// Content-Encoding: gzip
        /// </summary>
        public const string ContentEncoding = "Content-Encoding";
        /// <summary>
        /// 响应体的语言
        /// Content-Language: en,zh
        /// </summary>
        public const string ContentLanguage = "Content-Language";
        /// <summary>
        /// 响应体的长度
        /// Content-Length: 348
        /// </summary>
        public const string ContentLength = "Content-Length";
        /// <summary>
        /// 请求资源可替代的备用的另一地址
        /// Content-Location: /index.htm
        /// </summary>
        public const string ContentLocation = "Content-Location";
        /// <summary>
        /// 返回资源的MD5校验值
        /// Content-MD5: Q2hlY2sgSW50ZWdyaXR5IQ==
        /// </summary>
        public const string ContentMD5 = "Content-MD5";
        /// <summary>
        /// 在整个返回体中本部分的字节位置
        /// Content-Range: bytes 21010-47021/47022
        /// </summary>
        public const string ContentRange = "Content-Range";
        /// <summary>
        /// 返回内容的MIME类型
        /// Content-Type: text/html; charset=utf-8
        /// </summary>
        public const string ContentType = "Content-Type";
        /// <summary>
        /// 原始服务器消息发出的时间
        /// Date: Tue, 15 Nov 2010 08:12:31 GMT
        /// </summary>
        public const string Date = "Date";
        /// <summary>
        /// 请求变量的实体标签的当前值
        /// ETag: “737060cd8c284d8af7ad3082f209582d”
        /// </summary>
        public const string ETag = "ETag";
        /// <summary>
        /// 响应过期的日期和时间
        /// Expires: Thu, 01 Dec 2010 16:00:00 GMT
        /// </summary>
        public const string Expires = "Expires";
        /// <summary>
        /// 请求资源的最后修改时间
        /// Last-Modified: Tue, 15 Nov 2010 12:45:26 GMT
        /// </summary>
        public const string LastModified = "Last-Modified";
        /// <summary>
        /// 用来重定向接收方到非请求URL的位置来完成请求或标识新的资源
        /// Location: http://www.zcmhi.com/archives/94.html
        /// </summary>
        public const string Location = "Location";
        /// <summary>
        /// 包括实现特定的指令，它可应用到响应链上的任何接收方
        /// Pragma: no-cache
        /// </summary>
        public const string Pragma = "Pragma";
        /// <summary>
        /// 它指出认证方案和可应用到代理的该URL上的参数
        /// Proxy-Authenticate: Basic
        /// </summary>
        public const string ProxyAuthenticate = "Proxy-Authenticate";
        /// <summary>
        /// 应用于重定向或一个新的资源被创造，在5秒之后重定向（由网景提出，被大部分浏览器支持）
        /// Refresh: 5; url=
        /// </summary>
        public const string Refresh = "refresh";
        /// <summary>
        /// 如果实体暂时不可取，通知客户端在指定时间之后再次尝试
        /// Retry-After: 120
        /// </summary>
        public const string RetryAfter = "Retry-After";
        /// <summary>
        /// web服务器软件名称
        /// Server: Apache/1.3.27 (Unix) (Red-Hat/Linux)
        /// </summary>
        public const string Server = "Server";
        /// <summary>
        /// 设置Http Cookie
        /// Set-Cookie: UserID=JohnDoe; Max-Age=3600; Version=1
        /// </summary>
        public const string SetCookie = "Set-Cookie";
        /// <summary>
        /// 指出头域在分块传输编码的尾部存在
        /// Trailer: Max-Forwards
        /// </summary>
        public const string Trailer = "Trailer";
        /// <summary>
        /// 文件传输编码
        /// Transfer-Encoding:chunked
        /// </summary>
        public const string TransferEncoding = "Transfer-Encoding";
        /// <summary>
        /// 告诉下游代理是使用缓存响应还是从原始服务器请求
        /// Vary: *
        /// </summary>
        public const string Vary = "Vary";
        /// <summary>
        /// 告知代理客户端响应是通过哪里发送的
        /// Via: 1.0 fred, 1.1 nowhere.com (Apache/1.1)
        /// </summary>
        public const string Via = "Via";
        /// <summary>
        /// 警告实体可能存在的问题
        /// Warning: 199 Miscellaneous warning
        /// </summary>
        public const string Warning = "Warning";
        /// <summary>
        /// 表明客户端请求实体应该使用的授权方案
        /// WWW-Authenticate: Basic
        /// </summary>
        public const string WWWAuthenticate = "WWW-Authenticate";
    }
}

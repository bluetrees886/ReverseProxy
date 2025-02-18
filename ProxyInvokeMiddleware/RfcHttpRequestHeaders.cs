namespace ProxyInvokeMiddleware
{
    /// <summary>
    /// rfc2616
    /// https://www.w3.org/Protocols/rfc2616/rfc2616.html
    /// </summary>
    public static class RfcHttpRequestHeaders
    {
        /// <summary>
        /// 指定客户端能够接收的内容类型
        /// Accept: text/plain, text/html
        /// </summary>
        public const string Accept = "Accept";
        /// <summary>
        /// 浏览器可以接受的字符编码集。
        /// Accept-Charset: iso-8859-5
        /// </summary>
        public const string AcceptCharset = "Accept-Charset";
        /// <summary>
        /// 指定浏览器可以支持的web服务器返回内容压缩编码类型。
        /// Accept-Encoding: compress, gzip
        /// </summary>
        public const string AcceptEncoding = "Accept-Encoding";
        /// <summary>
        /// 浏览器可接受的语言
        /// Accept-Language: en,zh
        /// </summary>
        public const string AcceptLanguage = "Accept-Language";
        /// <summary>
        /// 可以请求网页实体的一个或者多个子范围字段
        /// Accept-Ranges: bytes
        /// </summary>
        public const string AcceptRanges = "Accept-Ranges";
        /// <summary>
        /// HTTP授权的授权证书
        /// Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
        /// </summary>
        public const string Authorization = "Authorization";
        /// <summary>
        /// 指定请求和响应遵循的缓存机制
        /// Cache-Control: no-cache
        /// </summary>
        public const string CacheControl = "Cache-Control";
        /// <summary>
        /// 表示是否需要持久连接。（HTTP 1.1默认进行持久连接）
        /// Connection: close
        /// </summary>
        public const string Connection = "Connection";
        /// <summary>
        /// HTTP请求发送时，会把保存在该请求域名下的所有cookie值一起发送给web服务器。
        /// Cookie: $Version=1; Skin=new;
        /// </summary>
        public const string Cookie = "Cookie";
        /// <summary>
        /// 请求的内容长度
        /// Content-Length: 348
        /// </summary>
        public const string ContentLength = "Content-Length";
        /// <summary>
        /// 请求的与实体对应的MIME信息
        /// Content-Type: application/x-www-form-urlencoded
        /// </summary>
        public const string ContentType = "Content-Type";
        /// <summary>
        /// 请求发送的日期和时间
        /// Date: Tue, 15 Nov 2010 08:12:31 GMT
        /// </summary>
        public const string Date = "Date";
        /// <summary>
        /// 请求的特定的服务器行为
        /// Expect: 100-continue
        /// </summary>
        public const string Expect = "Expect";
        /// <summary>
        /// 发出请求的用户的Email
        /// From: user@email.com
        /// </summary>
        public const string From = "From";
        /// <summary>
        /// 指定请求的服务器的域名和端口号
        /// Host: www.zcmhi.com
        /// </summary>
        public const string Host = "Host";
        /// <summary>
        /// 只有请求内容与实体相匹配才有效
        /// If-Match: “737060cd8c284d8af7ad3082f209582d”
        /// </summary>
        public const string IfMatch = "If-Match";
        /// <summary>
        /// 如果请求的部分在指定时间之后被修改则请求成功，未被修改则返回304代码
        /// If-Modified-Since: Sat, 29 Oct 2010 19:43:31 GMT
        /// </summary>
        public const string IfModifiedSince = "If-Modified-Since";
        /// <summary>
        /// 如果内容未改变返回304代码，参数为服务器先前发送的Etag，与服务器回应的Etag比较判断是否改变
        /// If-None-Match: “737060cd8c284d8af7ad3082f209582d”
        /// </summary>
        public const string IfNoneMatch = "If-None-Match";
        /// <summary>
        /// 如果实体未改变，服务器发送客户端丢失的部分，否则发送整个实体。参数也为Etag
        /// If-Range: “737060cd8c284d8af7ad3082f209582d”
        /// </summary>
        public const string IfRange = "If-Range";
        /// <summary>
        /// 只在实体在指定时间之后未被修改才请求成功
        /// If-Unmodified-Since: Sat, 29 Oct 2010 19:43:31 GMT
        /// </summary>
        public const string IfUnmodifiedSince = "If-Unmodified-Since";
        /// <summary>
        /// 限制信息通过代理和网关传送的时间
        /// Max-Forwards: 10
        /// </summary>
        public const string MaxForwards = "Max-Forwards";
        /// <summary>
        /// 用来包含实现特定的指令
        /// Pragma: no-cache
        /// </summary>
        public const string Pragma = "Pragma";
        /// <summary>
        /// 连接到代理的授权证书
        /// Proxy-Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
        /// </summary>
        public const string ProxyAuthorization = "Proxy-Authorization";
        /// <summary>
        /// 只请求实体的一部分，指定范围
        /// Range: bytes=500-999/// </summary>
        public const string Range = "Range";
        /// <summary>
        /// 先前网页的地址，当前请求网页紧随其后,即来路
        /// Referer: http://www.zcmhi.com/archives/71.html
        /// </summary>
        public const string Referer = "Referer";
        /// <summary>
        /// 客户端愿意接受的传输编码，并通知服务器接受接受尾加头信息
        /// TE: trailers,deflate;q=0.5/// </summary>
        public const string TE = "TE";
        /// <summary>
        /// 向服务器指定某种传输协议以便服务器进行转换（如果支持）
        /// Upgrade: HTTP/2.0, SHTTP/1.3, IRC/6.9, RTA/x11
        /// </summary>
        public const string Upgrade = "Upgrade";
        /// <summary>
        /// User-Agent的内容包含发出请求的用户信息
        /// User-Agent: Mozilla/5.0 (Linux; X11)
        /// </summary>
        public const string UserAgent = "User-Agent";
        /// <summary>
        /// 通知中间网关或代理服务器地址，通信协议
        /// Via: 1.0 fred, 1.1 nowhere.com (Apache/1.1)
        /// </summary>
        public const string Via = "Via";
        /// <summary>
        /// 关于消息实体的警告信息
        /// Warn: 199 Miscellaneous warning
        /// </summary>
        public const string Warning = "Warning";
    }
}

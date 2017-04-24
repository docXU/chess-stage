namespace ChessMiddle.PublicTool
{
    /// <summary>
    /// 一个数据交换暗号的常量中心
    /// </summary>
    internal class PasswordCode
    {
        #region 普通文件部分 EncryptionDecrypt使用
        /// <summary>
        /// 发送普通信息的代码
        /// </summary>
        internal const byte _commonCode = 33;
        /// <summary>
        /// 发送的图片的代码
        /// </summary>
        internal const byte _photographCode = 35;
        /// <summary>
        /// 数据已经发送成功的代码;
        /// </summary>
        internal const byte _dateSuccess = 36;
        #endregion
        #region 小型代码验证处理中心如心跳等；EncryptionDecryptVerification使用
        /// <summary>
        /// 一般验证需要用的代号
        /// </summary>
        internal const byte _verificationCode = 41;
        /// <summary>
        /// 发送心跳的代码
        /// </summary>
        internal const byte _heartbeatCode = 42;
        /// <summary>
        /// 服务器发给客户端的登录确认码
        /// </summary>
        internal const byte _serverToClient = 43;
        /// <summary>
        /// 客户端回给服务器的登录确认码
        /// </summary>
        internal const byte _clientToServer = 44;
        /// <summary>
        /// 客户端收到这个信息就会不重连的关掉
        /// </summary>
        internal const byte _clientCloseCode = 45;
        #endregion
    }
}

namespace Milimoe.FunGame.Desktop.Library
{
    public class Config
    {
        /// <summary>
        /// 是否自动连接服务器
        /// </summary>
        public static bool FunGame_isAutoConnect
        {
            get => RunTime.Config.FunGame_isAutoConnect;
            set => RunTime.Config.FunGame_isAutoConnect = value;
        }

        /// <summary>
        /// 是否自动登录
        /// </summary>
        public static bool FunGame_isAutoLogin
        {
            get => RunTime.Config.FunGame_isAutoLogin;
            set => RunTime.Config.FunGame_isAutoLogin = value;
        }

        /// <summary>
        /// 是否在匹配中
        /// </summary>
        public static bool FunGame_isMatching
        {
            get => RunTime.Config.FunGame_isMatching;
            set => RunTime.Config.FunGame_isMatching = value;
        }

        /// <summary>
        /// 是否连接上服务器
        /// </summary>
        public static bool FunGame_isConnected
        {
            get => RunTime.Config.FunGame_isConnected;
            set => RunTime.Config.FunGame_isConnected = value;
        }

        /// <summary>
        /// 是否正在重连
        /// </summary>
        public static bool FunGame_isRetrying
        {
            get => RunTime.Config.FunGame_isRetrying;
            set => RunTime.Config.FunGame_isRetrying = value;
        }

        /// <summary>
        /// 是否自动重连
        /// </summary>
        public static bool FunGame_isAutoRetry
        {
            get => RunTime.Config.FunGame_isAutoRetry;
            set => RunTime.Config.FunGame_isAutoRetry = value;
        }

        /// <summary>
        /// 当前游戏模式
        /// </summary>
        public static string FunGame_GameMode
        {
            get => RunTime.Config.FunGame_GameMode;
            set => RunTime.Config.FunGame_GameMode = value;
        }

        /// <summary>
        /// 服务器名称
        /// </summary>
        public static string FunGame_ServerName
        {
            get => RunTime.Config.FunGame_ServerName;
            set => RunTime.Config.FunGame_ServerName = value;
        }

        /// <summary>
        /// 公告
        /// </summary>
        public static string FunGame_Notice
        {
            get => RunTime.Config.FunGame_Notice;
            set => RunTime.Config.FunGame_Notice = value;
        }

        /// <summary>
        /// 自动登录的账号
        /// </summary>
        public static string FunGame_AutoLoginUser
        {
            get => RunTime.Config.FunGame_AutoLoginUser;
            set => RunTime.Config.FunGame_AutoLoginUser = value;
        }

        /// <summary>
        /// 自动登录的密码
        /// </summary>
        public static string FunGame_AutoLoginPassword
        {
            get => RunTime.Config.FunGame_AutoLoginPassword;
            set => RunTime.Config.FunGame_AutoLoginPassword = value;
        }

        /// <summary>
        /// 自动登录的秘钥
        /// </summary>
        public static string FunGame_AutoLoginKey
        {
            get => RunTime.Config.FunGame_AutoLoginKey;
            set => RunTime.Config.FunGame_AutoLoginKey = value;
        }
    }
}

using System.Collections;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RunTimeController : Core.Controller.RunTimeController
    {
        private readonly Main Main;
        private readonly Core.Model.Session Usercfg = RunTime.Session;
        private readonly LoginController LoginController;

        public RunTimeController(Main main)
        {
            Main = main;
            LoginController = new(Main);
        }

        public void LoadPlugins()
        {
            try
            {
                // 构建AddonController
                Dictionary<string, object> delegates = [];
                delegates.Add("WriteLine", new Action<string, LogLevel, bool>(WritelnSystemInfo));
                delegates.Add("Error", new Action<Exception>(Error));
                delegates.Add("NewDataRequest", new Func<DataRequestType, DataRequest>(NewDataRequestForAddon));
                delegates.Add("NewLongRunningDataRequest", new Func<DataRequestType, DataRequest>(NewLongRunningDataRequestForAddon));
                RunTime.PluginLoader = PluginLoader.LoadPlugins(delegates, RunTime.Session, RunTime.Config);
                foreach (string name in RunTime.PluginLoader.Plugins.Keys)
                {
                    Main.GetMessage("[ Plugin ] Loaded: " + name);
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
        }

        public void LoadGameModules()
        {
            try
            {
                // 构建AddonController
                Dictionary<string, object> delegates = [];
                delegates.Add("WriteLine", new Action<string, string, LogLevel, bool>(WritelnSystemInfo));
                delegates.Add("Error", new Action<Exception>(Error));
                delegates.Add("NewGamingRequest", new Func<GamingType, DataRequest>(NewDataRequestForAddon));
                delegates.Add("NewLongRunningGamingRequest", new Func<GamingType, DataRequest>(NewLongRunningDataRequestForAddon));
                RunTime.GameModuleLoader = GameModuleLoader.LoadGameModules(Constant.FunGameType, delegates, RunTime.Session, RunTime.Config);
                foreach (string name in RunTime.GameModuleLoader.Modules.Keys)
                {
                    Main.GetMessage("[ GameModule ] Loaded: " + name);
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
        }

        public override void WritelnSystemInfo(string msg, LogLevel level = LogLevel.Info, bool useLevel = true)
        {
            Main.GetMessage(msg);
        }
        
        public void WritelnSystemInfo(string name, string msg, LogLevel level = LogLevel.Info, bool useLevel = true)
        {
            Main.GetMessage(msg);
        }

        public override void Error(Exception e)
        {
            Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            Main.UpdateUI(MainInvokeType.Disconnected);
            ConnectEventArgs args = new(RunTime.Session.Server_Address, RunTime.Session.Server_Port, ConnectResult.ConnectFailed);
            Main.OnFailedConnectEvent(Main, args);
            Close_Socket();
        }

        public override bool BeforeConnect(ref string ip, ref int port, ArrayList ConnectArgs)
        {
            if (Config.FunGame_isRetrying)
            {
                Main.GetMessage("正在连接服务器，请耐心等待。");
                return false;
            }
            string[] gamemodules = [];
            if (RunTime.GameModuleLoader != null)
            {
                gamemodules = [.. RunTime.GameModuleLoader.Modules.Keys];
            }
            ConnectArgs.Add(gamemodules); // 服务器检查是否拥有需要的模组
            ConnectArgs.Add(FunGameInfo.FunGame_DebugMode); // 是否开启了debug模式
            if (!Config.FunGame_isConnected)
            {
                Main.CurrentRetryTimes++;
                if (Main.CurrentRetryTimes == 0) Main.GetMessage("开始连接服务器...", TimeType.General);
                else Main.GetMessage("第" + Main.CurrentRetryTimes + "次重试连接服务器...");
                // 超过重连次数上限
                if (Main.CurrentRetryTimes + 1 > Main.MaxRetryTimes)
                {
                    throw new CanNotConnectException();
                }
                Config.FunGame_isRetrying = true;
                return true;
            }
            else
            {
                Main.GetMessage("已连接至服务器，请勿重复连接。");
                return false;
            }
        }

        public override void AfterConnect(ArrayList ConnectArgs)
        {
            Config.FunGame_isRetrying = false;

            ConnectResult result = (ConnectResult?)ConnectArgs[0] ?? ConnectResult.FindServerFailed;
            string msg = (string?)ConnectArgs[1] ?? "";
            string servername = (string?)ConnectArgs[2] ?? "";
            string notice = (string?)ConnectArgs[3] ?? "";

            if (msg != "")
            {
                Main.GetMessage(msg);
                if (result != ConnectResult.Success)
                {
                    // 有弹窗会关闭自动重连。
                    Config.FunGame_isAutoRetry = false;
                    if (Main.ShowMessage(ShowMessageType.RetryCancel, msg, "连接服务器失败") == MessageResult.Retry)
                    {
                        Config.FunGame_isAutoRetry = true;
                    }
                }
            }

            if (result == ConnectResult.Success)
            {
                // 设置可复用Socket
                RunTime.Socket = Socket;
                Config.FunGame_ServerName = servername;
                Config.FunGame_Notice = notice;
                Usercfg.SocketToken = Socket?.Token ?? Guid.Empty;
                Main.GetMessage($"已连接服务器：{servername}。\n\n********** 服务器公告 **********\n\n{notice}\n\n");
                // 设置等待登录的黄灯
                Main.UpdateUI(MainInvokeType.WaitLoginAndSetYellow);
                Main.GetMessage("连接服务器成功，请登录账号以体验FunGame。");
                Main.UpdateUI(MainInvokeType.Connected);
            }
            else
            {
                Main.UpdateUI(MainInvokeType.SetRed);
            }
        }

        public override void AutoLogin(string Username, string Password, string AutoKey)
        {
            try
            {
                TaskUtility.NewTask(async () => await LoginController.LoginAccountAsync(Username, Password, AutoKey));
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
        }

        protected override void SocketHandler_Disconnect(SocketObject ServerMessage)
        {
            // 断开与服务器的连接
            string msg = "";
            if (ServerMessage.Parameters.Length > 0) msg = ServerMessage.GetParam<string>(0) ?? "";
            Main.GetMessage(msg);
            Main.UpdateUI(MainInvokeType.Disconnect);
            Close_Socket();
        }

        protected override void SocketHandler_System(SocketObject ServerMessage)
        {
            // 收到系统消息，可输出消息或弹窗
            ShowMessageType type = ShowMessageType.None;
            string msg = "";
            string title = "系统消息";
            int autoclose = 60;
            if (ServerMessage.Parameters.Length > 0) type = ServerMessage.GetParam<ShowMessageType>(0);
            if (ServerMessage.Parameters.Length > 1) msg = ServerMessage.GetParam<string>(1) ?? "";
            if (ServerMessage.Parameters.Length > 2) title = ServerMessage.GetParam<string>(2) ?? title;
            if (ServerMessage.Parameters.Length > 3) autoclose = ServerMessage.GetParam<int>(3);
            Main.GetMessage(msg);
            if (type != ShowMessageType.None) Main.ShowMessage(type, msg, title, autoclose);
        }

        protected override void SocketHandler_HeartBeat(SocketObject ServerMessage)
        {
            // 收到心跳包时更新与服务器的连接延迟
            if (Socket != null && Socket.Connected && Usercfg.LoginUser.Id != 0)
            {
                Main.UpdateUI(MainInvokeType.SetGreenAndPing);
            }
        }

        protected override void SocketHandler_ForceLogout(SocketObject ServerMessage)
        {
            // 服务器强制下线登录
            string msg = "";
            if (ServerMessage.Length > 0) msg = ServerMessage.GetParam<string>(0) ?? "";
            Usercfg.LoginKey = Guid.Empty;
            Main.UpdateUI(MainInvokeType.LogOut, msg ?? "");
        }

        protected override void SocketHandler_Chat(SocketObject ServerMessage)
        {
            // 收到房间聊天信息
            string user = "", msg = "";
            if (ServerMessage.Length > 0) user = ServerMessage.GetParam<string>(0) ?? "";
            if (ServerMessage.Length > 1) msg = ServerMessage.GetParam<string>(1) ?? "";
            if (user != Usercfg.LoginUserName)
            {
                Main.GetMessage(msg, TimeType.None);
            }
        }

        protected override void SocketHandler_UpdateRoomMaster(SocketObject ServerMessage)
        {
            // 收到房间更换房主的信息
            Room room = General.HallInstance;
            if (ServerMessage.Length > 0) room = ServerMessage.GetParam<Room>(0) ?? General.HallInstance;
            if (room.Roomid != "-1" && room.Roomid == Usercfg.InRoom.Roomid) Main.UpdateUI(MainInvokeType.UpdateRoomMaster, room);
        }

        protected override void SocketHandler_MatchRoom(SocketObject ServerMessage)
        {
            // 匹配成功，询问是否加入房间
            Config.FunGame_isMatching = false;
            Room room = General.HallInstance;
            if (ServerMessage.Length > 0) room = ServerMessage.GetParam<Room>(0) ?? General.HallInstance;
            if (room.Roomid == "-1")
            {
                Main.ShowMessage(ShowMessageType.General, "暂时无法找到符合条件的房间。", "匹配房间", 10);
                Main.UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Success, room);
                Main.UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Cancel);
                return;
            }
            else if (Main.ShowMessage(ShowMessageType.YesNo, "已找到符合条件的房间，是否加入？", "匹配房间", 10) == MessageResult.Yes)
            {
                Main.UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Success, room);
                return;
            }
            Main.UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Cancel);
        }

        protected override void SocketHandler_StartGame(SocketObject ServerMessage)
        {
            // 游戏即将开始
            Room room = General.HallInstance;
            List<User> users = [];
            if (ServerMessage.Length > 0) room = ServerMessage.GetParam<Room>(0) ?? General.HallInstance;
            if (ServerMessage.Length > 1) users = ServerMessage.GetParam<List<User>>(1) ?? users;
            Main.UpdateUI(MainInvokeType.StartGame, room, users);
        }

        protected override void SocketHandler_EndGame(SocketObject ServerMessage)
        {
            Room room = General.HallInstance;
            List<User> users = [];
            if (ServerMessage.Length > 0) room = ServerMessage.GetParam<Room>(0) ?? General.HallInstance;
            if (ServerMessage.Length > 1) users = ServerMessage.GetParam<List<User>>(1) ?? users;
            Main.UpdateUI(MainInvokeType.EndGame, room, users);
        }

        protected override void SocketHandler_Gaming(SocketObject ServerMessage)
        {
            GamingType gamingtype = GamingType.None;
            Dictionary<string, object> data = [];
            if (ServerMessage.Length > 0) gamingtype = ServerMessage.GetParam<GamingType>(0);
            if (ServerMessage.Length > 1) data = ServerMessage.GetParam<Dictionary<string, object>>(1) ?? data;
            RunTime.Gaming?.GamingHandler(gamingtype, data);
        }
    }
}

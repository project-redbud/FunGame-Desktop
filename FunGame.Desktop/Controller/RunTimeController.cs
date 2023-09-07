using Milimoe.FunGame.Core.Api.Transmittal;
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

        public override void WritelnSystemInfo(string msg)
        {
            Main?.GetMessage(msg);
        }

        public override void Error(Exception e)
        {
            Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            Main.UpdateUI(MainInvokeType.Disconnected);
            Main.OnFailedConnectEvent(new ConnectEventArgs(RunTime.Session.Server_IP, RunTime.Session.Server_Port));
            Close();
        }

        public override bool BeforeConnect()
        {
            try
            {
                if (Config.FunGame_isRetrying)
                {
                    Main.GetMessage("正在连接服务器，请耐心等待。");
                    return false;
                }
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
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                Main.UpdateUI(MainInvokeType.SetRed);
                Config.FunGame_isRetrying = false;
                return false;
            }
        }

        public override void AfterConnect(object[] ConnectArgs)
        {
            Config.FunGame_isRetrying = false;

            ConnectResult result = (ConnectResult)ConnectArgs[0];
            string msg = (string)ConnectArgs[1];
            string servername = (string)ConnectArgs[2];
            string notice = (string)ConnectArgs[3];

            if (msg != "")
            {
                Main.GetMessage(msg);
                if (result != ConnectResult.Success) Main.ShowMessage(ShowMessageType.Error, msg);
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
        }

        public override void AutoLogin(string Username, string Password, string AutoKey)
        {
            try
            {
                Core.Api.Utility.TaskUtility.StartAndAwaitTask(async () => await LoginController.LoginAccountAsync(Username, Password, AutoKey));
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
            Main.OnSucceedDisconnectEvent(new GeneralEventArgs());
            Main.OnAfterDisconnectEvent(new GeneralEventArgs());
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
            Guid key = Guid.Empty;
            string msg = "";
            if (ServerMessage.Length > 0) key = ServerMessage.GetParam<Guid>(0);
            if (ServerMessage.Length > 1) msg = ServerMessage.GetParam<string>(1) ?? "";
            if (key == Usercfg.LoginKey)
            {
                Usercfg.LoginKey = Guid.Empty;
                Main.UpdateUI(MainInvokeType.LogOut, msg ?? "");
            }
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
    }
}

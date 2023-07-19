using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RunTimeController : Core.Controller.RunTimeController
    {
        private readonly Main Main;
        private readonly Core.Model.Session Usercfg = RunTime.Session;

        public RunTimeController(Main main)
        {
            Main = main;
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

        public override ConnectResult Connect()
        {
            if (RunTime.Session.Server_IP == "" || RunTime.Session.Server_Port <= 0)
            {
                (RunTime.Session.Server_IP, RunTime.Session.Server_Port) = GetServerAddress();
                if (RunTime.Session.Server_IP == "" || RunTime.Session.Server_Port <= 0)
                {
                    ShowMessage.ErrorMessage("查找可用的服务器失败！");
                    return ConnectResult.FindServerFailed;
                }
            }
            try
            {
                if (Config.FunGame_isRetrying)
                {
                    Main.GetMessage("正在连接服务器，请耐心等待。");
                    Config.FunGame_isRetrying = false;
                    return ConnectResult.CanNotConnect;
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
                    // 与服务器建立连接
                    Socket?.Close();
                    Config.FunGame_isRetrying = true;
                    _Socket = Socket.Connect(RunTime.Session.Server_IP, RunTime.Session.Server_Port);
                    if (Socket != null && Socket.Connected)
                    {
                        // 设置可复用Socket
                        RunTime.Socket = Socket;
                        // 发送连接请求
                        if (Socket.Send(SocketMessageType.RunTime_Connect) == SocketResult.Success)
                        {
                            SocketMessageType Result = Receiving();
                            if (Result == SocketMessageType.RunTime_Connect)
                            {
                                Main.GetMessage("连接服务器成功，请登录账号以体验FunGame。");
                                Main.UpdateUI(MainInvokeType.Connected);
                                StartReceiving();
                                Task t = Task.Run(() =>
                                {
                                    while (true)
                                    {
                                        if (_IsReceiving)
                                        {
                                            break;
                                        }
                                    }
                                });
                                Task.WhenAny(t);
                                return ConnectResult.Success;
                            }
                        }
                        Config.FunGame_isRetrying = false;
                        Socket.Close();
                        return ConnectResult.ConnectFailed;
                    }
                    Socket?.Close();
                    Config.FunGame_isRetrying = false;
                    throw new CanNotConnectException();
                }
                else
                {
                    Main.GetMessage("已连接至服务器，请勿重复连接。");
                    return ConnectResult.CanNotConnect;
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                Main.UpdateUI(MainInvokeType.SetRed);
                Config.FunGame_isRetrying = false;
                return ConnectResult.ConnectFailed;
            }
        }

        public override void AutoLogin(string Username, string Password, string AutoKey)
        {
            try
            {
                LoginController LoginController = new();
                Core.Api.Utility.TaskUtility.StartAndAwaitTask(() => LoginController.LoginAccount(Username, Password, AutoKey));
                LoginController.Dispose();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
        }

        protected override bool SocketHandler_Connect(SocketObject ServerMessage)
        {
            string msg = "";
            Guid token = Guid.Empty;
            if (ServerMessage.Parameters.Length > 0) msg = ServerMessage.GetParam<string>(0)!;
            string[] strings = msg.Split(';');
            if (strings.Length != 2)
            {
                // 服务器拒绝连接
                msg = strings[0];
                Main.GetMessage(msg);
                ShowMessage.ErrorMessage(msg);
                return false;
            }
            string ServerName = strings[0];
            string ServerNotice = strings[1];
            Config.FunGame_ServerName = ServerName;
            Config.FunGame_Notice = ServerNotice;
            if (ServerMessage.Parameters.Length > 1) token = ServerMessage.GetParam<Guid>(1);
            Socket!.Token = token;
            Usercfg.SocketToken = token;
            Main.GetMessage($"已连接服务器：{ServerName}。\n\n********** 服务器公告 **********\n\n{ServerNotice}\n\n");
            // 设置等待登录的黄灯
            Main.UpdateUI(MainInvokeType.WaitLoginAndSetYellow);
            return true;
        }

        protected override void SocketHandler_Disconnect(SocketObject ServerMessage)
        {
            string msg = "";
            if (ServerMessage.Parameters.Length > 0) msg = ServerMessage.GetParam<string>(0)!;
            Main.GetMessage(msg);
            Main.UpdateUI(MainInvokeType.Disconnect);
            Close();
            Main.OnSucceedDisconnectEvent(new GeneralEventArgs());
            Main.OnAfterDisconnectEvent(new GeneralEventArgs());
        }

        protected override void SocketHandler_HeartBeat(SocketObject ServerMessage)
        {
            if (Socket != null && Socket.Connected && Usercfg.LoginUser.Id != 0)
            {
                Main.UpdateUI(MainInvokeType.SetGreenAndPing);
            }
        }
    }
}

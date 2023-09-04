using Milimoe.FunGame.Core.Api.Transmittal;
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

        public override ConnectResult Connect()
        {
            if (RunTime.Session.Server_IP == "" || RunTime.Session.Server_Port <= 0)
            {
                (RunTime.Session.Server_IP, RunTime.Session.Server_Port) = GetServerAddress();
                if (RunTime.Session.Server_IP == "" || RunTime.Session.Server_Port <= 0)
                {
                    Main.ShowMessage(ShowMessageType.Error, "查找可用的服务器失败！");
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
                        DataRequest request = RunTime.NewDataRequest(DataRequestType.RunTime_Connect);
                        request.SendRequest();
                        if (request.Result == RequestResult.Success)
                        {
                            bool success = request.GetResult<bool>("success");
                            string msg = request.GetResult<string>("msg") ?? "";
                            if (!success)
                            {
                                // 服务器拒绝连接
                                if (msg != "")
                                {
                                    Main.GetMessage(msg);
                                    Main.ShowMessage(ShowMessageType.Error, msg);
                                }
                                return ConnectResult.ConnectFailed;
                            }
                            else
                            {
                                if (msg != "")
                                {
                                    Main.GetMessage(msg);
                                }
                                Guid token = request.GetResult<Guid>("token");
                                string servername = request.GetResult<string>("servername") ?? "";
                                string notice = request.GetResult<string>("notice") ?? "";
                                Config.FunGame_ServerName = servername;
                                Config.FunGame_Notice = notice;
                                Socket!.Token = token;
                                Usercfg.SocketToken = token;
                                Main.GetMessage($"已连接服务器：{servername}。\n\n********** 服务器公告 **********\n\n{notice}\n\n");
                                // 设置等待登录的黄灯
                                Main.UpdateUI(MainInvokeType.WaitLoginAndSetYellow);
                                Main.GetMessage("连接服务器成功，请登录账号以体验FunGame。");
                                Main.UpdateUI(MainInvokeType.Connected);
                                StartReceiving();
                                Task.Run(() =>
                                {
                                    while (true)
                                    {
                                        if (_IsReceiving)
                                        {
                                            break;
                                        }
                                    }
                                });
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
                Core.Api.Utility.TaskUtility.StartAndAwaitTask(async () => await LoginController.LoginAccountAsync(Username, Password, AutoKey));
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
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

using System.Diagnostics;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Base;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.Utility;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Main : BaseMain
    {

        #region 变量定义

        /**
         * 属性
         */
        public int MaxRetryTimes { get; } = SocketSet.MaxRetryTimes; // 最大重试连接次数
        public int CurrentRetryTimes { get; set; } = -1; // 当前重试连接次数

        /**
         * 变量
         */
        private Task? MatchFunGame = null; // 匹配线程（即将删除）
        private MainController? MainController = null;
        private readonly Core.Model.RoomList Rooms = RunTime.RoomList;
        private readonly Core.Model.Session Usercfg = RunTime.Session;

        /**
         * 委托【即将删除】
         */
        Action<int, object[]?>? StartMatch_Action = null;

        public Main()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// 所有自定义初始化的内容
        /// </summary>
        public void Init()
        {
            RunTime.Main = this;
            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
            SetRoomid(Usercfg.InRoom); // 房间号初始化
            ShowFunGameInfo(); // 显示FunGame信息
            GetFunGameConfig(); // 获取FunGame配置
            // 创建RunTime
            RunTime.Controller = new RunTimeController(this);
            // 窗口句柄创建后，进行委托
            TaskUtility.StartAndAwaitTask(() =>
            {
                while (true)
                {
                    if (IsHandleCreated)
                    {
                        break;
                    }
                }
                if (Config.FunGame_isAutoConnect) InvokeController_Connect();
            });
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        protected override void BindEvent()
        {
            base.BindEvent();
            Disposed += Main_Disposed;
            FailedConnect += FailedConnectEvent;
            SucceedConnect += SucceedConnectEvent;
            SucceedLogin += SucceedLoginEvent;
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 提供公共方法给Controller更新UI
        /// </summary>
        /// <param name="updatetype">string?</param>
        /// <param name="objs">object[]?</param>
        public void UpdateUI(MainInvokeType type, params object[]? objs)
        {
            void action()
            {
                try
                {
                    switch (type)
                    {
                        case MainInvokeType.SetGreen:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Green);
                            SetButtonEnableIfLogon(true, ClientState.Online);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetGreenAndPing:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Green, ping: NetworkUtility.GetServerPing(RunTime.Session.Server_IP));
                            SetButtonEnableIfLogon(true, ClientState.Online);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.WaitConnectAndSetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            if (MainController != null && Config.FunGame_isAutoConnect)
                            {
                                // 自动连接服务器
                                InvokeController_Connect();
                            }
                            break;

                        case MainInvokeType.WaitLoginAndSetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight((int)LightType.Yellow, true);
                            SetButtonEnableIfLogon(false, ClientState.WaitLogin);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetRed:
                            SetServerStatusLight((int)LightType.Red);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = false;
                            break;

                        case MainInvokeType.Disconnected:
                            Rooms.Clear();
                            RoomList.Items.Clear();
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isConnected = false;
                            SetServerStatusLight((int)LightType.Red);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            LogoutAccount();
                            MainController?.Dispose();
                            CloseConnectedWindows();
                            break;

                        case MainInvokeType.Disconnect:
                            Rooms.Clear();
                            RoomList.Items.Clear();
                            Config.FunGame_isAutoRetry = false;
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isAutoConnect = false;
                            Config.FunGame_isAutoLogin = false;
                            Config.FunGame_isConnected = false;
                            SetServerStatusLight((int)LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            LogoutAccount();
                            MainController?.Dispose();
                            break;

                        case MainInvokeType.LogIn:
                            break;

                        case MainInvokeType.LogOut:
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isAutoLogin = false;
                            SetServerStatusLight((int)LightType.Yellow, true);
                            SetButtonEnableIfLogon(false, ClientState.WaitLogin);
                            LogoutAccount();
                            if (objs != null && objs.Length > 0)
                            {
                                if (objs[0].GetType() == typeof(string))
                                {
                                    WritelnSystemInfo((string)objs[0]);
                                    ShowMessage(ShowMessageType.General, (string)objs[0], "退出登录", 5);
                                }
                            }
                            break;

                        case MainInvokeType.SetUser:
                            if (objs != null && objs.Length > 0)
                            {
                                LoginAccount(objs);
                            }
                            break;

                        case MainInvokeType.Connected:
                            NoticeText.Text = Config.FunGame_Notice;
                            break;

                        case MainInvokeType.UpdateRoom:
                            if (objs != null && objs.Length > 0)
                            {
                                RoomList.Items.Clear();
                                Rooms.Clear();
                                Rooms.AddRooms((List<Room>)objs[0]);
                                foreach (string roomid in Rooms.ListRoomID)
                                {
                                    if (roomid != "-1") RoomList.Items.Add(roomid);
                                }
                            }
                            break;

                        case MainInvokeType.UpdateRoomMaster:
                            if (objs != null && objs.Length > 0)
                            {
                                Room r = (Room)objs[0];
                                Usercfg.InRoom = r;
                                Rooms.RemoveRoom(r.Roomid);
                                Rooms.AddRoom(r);
                                if (r.RoomMaster != null)
                                {
                                    string msg = $"房间 [ {r.Roomid} ] 的房主已变更为" + (r.RoomMaster.Username != Usercfg.LoginUserName ? $" [ {r.RoomMaster.Username} ]" : "您") + "。";
                                    GetMessage(msg, TimeType.TimeOnly);
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    WritelnGameInfo(e.GetErrorInfo());
                    UpdateUI(MainInvokeType.SetRed);
                }
            }
            InvokeUpdateUI(action);
        }

        /// <summary>
        /// 提供公共方法给Controller发送系统信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timetype"></param>
        public void GetMessage(string msg, TimeType timetype = TimeType.TimeOnly)
        {
            void action()
            {
                try
                {
                    if (msg == "") return;
                    if (timetype != TimeType.None)
                    {
                        WritelnGameInfo(DateTimeUtility.GetDateTimeToString(timetype) + " >> " + msg);
                    }
                    else
                    {
                        WritelnGameInfo(msg);
                    }
                }
                catch (Exception e)
                {
                    WritelnGameInfo(e.GetErrorInfo());
                }
            };
            InvokeUpdateUI(action);
        }

        #endregion

        #region 实现

        /// <summary>
        /// 获取FunGame配置文件设定
        /// </summary>
        private void GetFunGameConfig()
        {
            try
            {
                if (INIHelper.ExistINIFile())
                {
                    string isAutoConnect = INIHelper.ReadINI("Config", "AutoConnect");
                    string isAutoLogin = INIHelper.ReadINI("Config", "AutoLogin");
                    string strUserName = INIHelper.ReadINI("Account", "UserName");
                    string strPassword = INIHelper.ReadINI("Account", "Password");
                    string strAutoKey = INIHelper.ReadINI("Account", "AutoKey");

                    if (isAutoConnect != null && isAutoConnect.Trim() != "" && (isAutoConnect.ToLower().Equals("false") || isAutoConnect.ToLower().Equals("true")))
                        Config.FunGame_isAutoConnect = Convert.ToBoolean(isAutoConnect);
                    else throw new ReadConfigException();

                    if (isAutoLogin != null && isAutoLogin.Trim() != "" && (isAutoLogin.ToLower().Equals("false") || isAutoLogin.ToLower().Equals("true")))
                        Config.FunGame_isAutoLogin = Convert.ToBoolean(isAutoLogin);
                    else throw new ReadConfigException();

                    if (strUserName != null && strUserName.Trim() != "")
                        Config.FunGame_AutoLoginUser = strUserName.Trim();

                    if (strPassword != null && strPassword.Trim() != "")
                        Config.FunGame_AutoLoginPassword = strPassword.Trim();

                    if (strAutoKey != null && strAutoKey.Trim() != "")
                        Config.FunGame_AutoLoginKey = strAutoKey.Trim();
                }
                else
                {
                    INIHelper.Init((FunGameInfo.FunGame)Constant.FunGameType);
                    WritelnGameInfo(">> 首次启动，已自动为你创建配置文件。");
                    GetFunGameConfig();
                }
            }
            catch (System.Exception e)
            {
                WritelnGameInfo(e.GetErrorInfo());
            }
        }

        /// <summary>
        /// 设置房间号和显示信息
        /// </summary>
        /// <param name="roomid"></param>
        private void SetRoomid(Room room)
        {
            Usercfg.InRoom = room;
            if (room.Roomid != "-1")
            {
                WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 加入房间");
                WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已加入房间 -> [ " + room.Roomid + " ]");
                Room.Text = "[ 当前房间 ]\n" + Convert.ToString(room.Roomid);
                NowRoomID.Text = room.Roomid;
            }
            else
            {
                NowRoomID.Text = "";
                Room.Text = "暂未进入房间";
            }
        }

        /// <summary>
        /// 向消息队列输出换行符
        /// </summary>
        private void WritelnGameInfo()
        {
            GameInfo.AppendText("\n");
            GameInfo.SelectionStart = GameInfo.Text.Length - 1;
            GameInfo.ScrollToCaret();
        }

        /// <summary>
        /// 向消息队列输出一行文字
        /// </summary>
        /// <param name="msg"></param>
        private void WritelnGameInfo(string msg)
        {
            if (msg.Trim() != "")
            {
                GameInfo.AppendText(msg + "\n");
                GameInfo.SelectionStart = GameInfo.Text.Length - 1;
                GameInfo.ScrollToCaret();
            }
        }

        /// <summary>
        /// 向消息队列输出文字
        /// </summary>
        /// <param name="msg"></param>
        private void WriteGameInfo(string msg)
        {
            if (msg.Trim() != "")
            {
                GameInfo.AppendText(msg);
                GameInfo.SelectionStart = GameInfo.Text.Length - 1;
                GameInfo.ScrollToCaret();
            }
        }

        /// <summary>
        /// 向消息队列输出一行系统信息
        /// </summary>
        /// <param name="msg"></param>
        private void WritelnSystemInfo(string msg)
        {
            msg = DateTimeUtility.GetDateTimeToString(TimeType.TimeOnly) + " >> " + msg;
            WritelnGameInfo(msg);
        }

        /// <summary>
        /// 在大厅中，设置按钮的显示和隐藏
        /// </summary>
        private void InMain()
        {
            // 显示：匹配、创建房间
            // 隐藏：退出房间、房间设定、当前房间号、复制房间号
            SetRoomid(Usercfg.InRoom);
            QuitRoom.Visible = false;
            StartMatch.Visible = true;
            RoomSetting.Visible = false;
            CreateRoom.Visible = true;
            NowRoomID.Visible = false;
            CopyRoomID.Visible = false;
        }

        /// <summary>
        /// 在房间中，设置按钮的显示和隐藏
        /// </summary>
        private void InRoom()
        {
            // 显示：退出房间、房间设置、当前房间号、复制房间号
            // 隐藏：停止匹配、创建房间
            StopMatch.Visible = false;
            QuitRoom.Visible = true;
            CreateRoom.Visible = false;
            RoomSetting.Visible = true;
            NowRoomID.Visible = true;
            CopyRoomID.Visible = true;
        }

        /// <summary>
        /// 未登录和离线时，停用按钮
        /// 登录的时候要激活按钮
        /// </summary>
        /// <param name="isLogon">是否登录</param>
        /// <param name="status">客户端状态</param>
        private void SetButtonEnableIfLogon(bool isLogon, ClientState status)
        {
            switch (status)
            {
                case ClientState.Online:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetOnineItems);
                    break;
                case ClientState.WaitConnect:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetNoConnectItems);
                    break;
                case ClientState.WaitLogin:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetNoLoginItems);
                    break;
            }
            this.PresetText.SelectedIndex = 0;
            CheckMix.Enabled = isLogon;
            CheckTeam.Enabled = isLogon;
            CheckHasPass.Enabled = isLogon;
            StartMatch.Enabled = isLogon;
            CreateRoom.Enabled = isLogon;
            RoomBox.Enabled = isLogon;
            AccountSetting.Enabled = isLogon;
            Stock.Enabled = isLogon;
            Store.Enabled = isLogon;
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="isDouble"></param>
        /// <param name="roomid"></param>
        private async Task<bool> JoinRoom(bool isDouble, string roomid)
        {
            if (!isDouble)
            {
                if (!RoomText.Text.Equals("") && !RoomText.ForeColor.Equals(Color.DarkGray))
                {
                    return await JoinRoom_Handler(roomid);
                }
                else
                {
                    RoomText.Enabled = false;
                    ShowMessage(ShowMessageType.Tip, "请输入房间号。");
                    RoomText.Enabled = true;
                    RoomText.Focus();
                    return false;
                }
            }
            else
            {
                return await JoinRoom_Handler(roomid);
            }
        }

        /// <summary>
        /// 重复处理加入房间的方法
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        private async Task<bool> JoinRoom_Handler(string roomid)
        {
            if (MainController != null)
            {
                await MainController.UpdateRoomAsync();
                if (CheckRoomIDExist(roomid))
                {
                    if (Usercfg.InRoom.Roomid == "-1")
                    {
                        if (ShowMessage(ShowMessageType.YesNo, "已找到房间 -> [ " + roomid + " ]\n是否加入？", "已找到房间") == MessageResult.Yes)
                        {
                            Room r = GetRoom(roomid);
                            if (MainController != null && await MainController.IntoRoomAsync(r))
                            {
                                SetRoomid(r);
                                InRoom();
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        ShowMessage(ShowMessageType.Tip, "你需要先退出房间才可以加入新的房间。");
                        return false;
                    }
                }
            }
            ShowMessage(ShowMessageType.Warning, "未找到此房间！");
            return false;
        }

        /// <summary>
        /// 这里实现匹配相关的方法
        /// </summary>
        /// <param name="i">主要参数：触发方法的哪一个分支</param>
        /// <param name="objs">可传多个参数</param>
        private void StartMatch_Method(int i, object[]? objs = null)
        {
            switch (i)
            {
                case (int)StartMatchState.Matching:
                    // 开始匹配
                    Config.FunGame_isMatching = true;
                    int loop = 0;
                    string roomid = Convert.ToString(new Random().Next(1, 10000));
                    // 匹配中 匹配成功返回房间号
                    Task.Factory.StartNew(() =>
                    {
                        // 创建新线程，防止主界面阻塞
                        Thread.Sleep(3000);
                        while (loop < 10000 && Config.FunGame_isMatching)
                        {
                            loop++;
                            if (loop == Convert.ToInt32(roomid))
                            {
                                // 创建委托，操作主界面
                                StartMatch_Action = (int i, object[]? objs) =>
                                {
                                    StartMatch_Method(i, objs);
                                };
                                if (InvokeRequired)
                                {
                                    Invoke(StartMatch_Action, (int)StartMatchState.Success, new object[] { roomid });
                                }
                                else
                                {
                                    StartMatch_Action((int)StartMatchState.Success, new object[] { roomid });
                                }
                                break;
                            }
                        }
                    });
                    break;
                case (int)StartMatchState.Success:
                    Config.FunGame_isMatching = false;
                    // 匹配成功返回房间号
                    roomid = "-1";
                    if (objs != null) roomid = (string)objs[0];
                    if (!roomid.Equals(-1))
                    {
                        WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 匹配成功");
                        WritelnGameInfo(">> 房间号： " + roomid);
                        SetRoomid(GetRoom(roomid));
                    }
                    else
                    {
                        WritelnGameInfo("ERROR：匹配失败！");
                        break;
                    }
                    // 设置按钮可见性
                    InRoom();
                    // 创建委托，操作主界面
                    StartMatch_Action = (i, objs) =>
                    {
                        StartMatch_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(StartMatch_Action, (int)StartMatchState.Enable, new object[] { true });
                    }
                    else
                    {
                        StartMatch_Action((int)StartMatchState.Enable, new object[] { true });
                    }
                    MatchFunGame = null;
                    break;
                case (int)StartMatchState.Enable:
                    // 设置匹配过程中的各种按钮是否可用
                    bool isPause = false;
                    if (objs != null) isPause = (bool)objs[0];
                    CheckMix.Enabled = isPause;
                    CheckTeam.Enabled = isPause;
                    CheckHasPass.Enabled = isPause;
                    CreateRoom.Enabled = isPause;
                    RoomBox.Enabled = isPause;
                    Login.Enabled = isPause;
                    break;
                case (int)StartMatchState.Cancel:
                    WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 终止匹配");
                    WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已终止匹配。");
                    Config.FunGame_isMatching = false;
                    StartMatch_Action = (i, objs) =>
                    {
                        StartMatch_Method(i, objs);
                    };
                    if (InvokeRequired)
                    {
                        Invoke(StartMatch_Action, (int)StartMatchState.Enable, new object[] { true });
                    }
                    else
                    {
                        StartMatch_Action((int)StartMatchState.Enable, new object[] { true });
                    }
                    MatchFunGame = null;
                    StopMatch.Visible = false;
                    StartMatch.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// 登录账号，显示登出按钮
        /// </summary>
        private void LoginAccount(params object[]? objs)
        {
            if (objs != null && objs.Length > 0)
            {
                Usercfg.LoginUser = (User)objs[0];
                if (Usercfg.LoginUser.Id == 0)
                {
                    throw new NoUserLogonException();
                }
                Usercfg.LoginUserName = Usercfg.LoginUser.Username;
            }
            NowAccount.Text = "[ID] " + Usercfg.LoginUserName;
            Login.Visible = false;
            Logout.Visible = true;
            UpdateUI(MainInvokeType.SetGreenAndPing);
            string welcome = $"欢迎回来， {Usercfg.LoginUserName}！";
            ShowMessage(ShowMessageType.General, welcome, "登录成功", 5);
            WritelnSystemInfo(welcome);
        }

        /// <summary>
        /// 登出账号，显示登录按钮
        /// </summary>
        private void LogoutAccount()
        {
            Usercfg.InRoom = General.HallInstance;
            Usercfg.LoginUser = General.UnknownUserInstance;
            Usercfg.LoginUserName = "";
            InMain();
            NowAccount.Text = "请登录账号";
            Logout.Visible = false;
            Login.Visible = true;
        }

        /// <summary>
        /// 终止匹配实现方法
        /// </summary>
        private void StopMatch_Click()
        {
            StartMatch_Action = (i, objs) =>
            {
                StartMatch_Method(i, objs);
            };
            if (InvokeRequired)
            {
                Invoke(StartMatch_Action, (int)StartMatchState.Cancel, new object[] { true });
            }
            else
            {
                StartMatch_Action((int)StartMatchState.Cancel, new object[] { true });
            }
        }

        /// <summary>
        /// 发送消息实现
        /// </summary>
        /// <param name="isLeave">是否离开焦点</param>
        private void SendTalkText_Click(bool isLeave)
        {
            // 向消息队列发送消息
            string text = TalkText.Text;
            if (text.Trim() != "" && !TalkText.ForeColor.Equals(Color.DarkGray))
            {
                string msg;
                if (Usercfg.LoginUserName == "")
                {
                    msg = ":> " + text;
                }
                else
                {
                    msg = DateTimeUtility.GetNowShortTime() + " [ " + Usercfg.LoginUserName + " ] 说： " + text;
                }
                WritelnGameInfo(msg);
                TalkText.Text = "";
                if (isLeave) TalkText_Leave(); // 回车不离开焦点
                if (MainController != null && Usercfg.LoginUser.Id != 0 && !SwitchTalkMessage(text))
                {
                    TaskUtility.StartAndAwaitTask(async () =>
                    {
                        if (!await MainController.ChatAsync(" [ " + Usercfg.LoginUserName + " ] 说： " + text))
                        {
                            WritelnGameInfo("联网消息发送失败。");
                        }
                    });
                }
            }
            else
            {
                TalkText.Enabled = false;
                ShowMessage(ShowMessageType.Tip, "消息不能为空，请重新输入。");
                TalkText.Enabled = true;
                TalkText.Focus();
            }
        }

        /// <summary>
        /// 创建房间的处理方法
        /// </summary>
        /// <param name="RoomType"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        private async Task CreateRoom_Handler(string RoomType, string Password = "")
        {
            if (Usercfg.InRoom.Roomid != "-1")
            {
                ShowMessage(ShowMessageType.Warning, "已在房间中，无法创建房间。");
                return;
            }
            if (MainController != null)
            {
                string roomid = await MainController.CreateRoomAsync(RoomType, Password);
                if (roomid != "" && roomid != "-1")
                {
                    await MainController.UpdateRoomAsync();
                    Room r = GetRoom(roomid);
                    await InvokeController_IntoRoom(r);
                    SetRoomid(r);
                    InRoom();
                    WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 创建" + RoomType + "房间");
                    WritelnGameInfo(">> 创建" + RoomType + "房间成功！房间号： " + roomid);
                    ShowMessage(ShowMessageType.General, "创建" + RoomType + "房间成功！\n房间号是 -> [ " + roomid + " ]", "创建成功");
                    return;
                }
            }
            ShowMessage(ShowMessageType.General, "创建" + RoomType + "房间失败！", "创建失败");
        }

        /// <summary>
        /// 发送消息实现，往消息队列发送消息
        /// </summary>
        /// <param name="msg"></param>
        private void SendTalkText_Click(string msg)
        {
            WritelnGameInfo((!Usercfg.LoginUserName.Equals("") ? DateTimeUtility.GetNowShortTime() + " [ " + Usercfg.LoginUserName + " ] 说： " : ":> ") + msg);
        }

        /// <summary>
        /// 焦点离开聊天框时设置灰色文本
        /// </summary>
        private void TalkText_Leave()
        {
            if (TalkText.Text.Equals(""))
            {
                TalkText.ForeColor = Color.DarkGray;
                TalkText.Text = "向消息队列发送消息...";
            }
        }

        /// <summary>
        /// 登录成功后的处理
        /// </summary>
        /// <returns></returns>
        private async Task SucceedLoginEvent_Handler()
        {
            if (MainController != null)
            {
                // 获取在线的房间列表
                await MainController.UpdateRoomAsync();
                // 接入-1号房间聊天室
                await InvokeController_IntoRoom(Rooms["-1"] ?? General.HallInstance);
            }
        }

        /// <summary>
        /// 设置服务器连接状态指示灯
        /// </summary>
        /// <param name="light"></param>
        /// <param name="ping"></param>
        private void SetServerStatusLight(int light, bool waitlogin = false, int ping = 0)
        {
            switch (light)
            {
                case (int)LightType.Green:
                    Connection.Text = "服务器连接成功";
                    this.Light.Image = Properties.Resources.green;
                    break;
                case (int)LightType.Yellow:
                    Connection.Text = waitlogin ? "等待登录账号" : "等待连接服务器";
                    this.Light.Image = Properties.Resources.yellow;
                    break;
                case (int)LightType.Red:
                default:
                    Connection.Text = "服务器连接失败";
                    this.Light.Image = Properties.Resources.red;
                    break;
            }
            if (ping > 0)
            {
                Connection.Text = "心跳延迟  " + ping + "ms";
                if (ping < 100)
                    this.Light.Image = Properties.Resources.green;
                else if (ping >= 100 && ping < 200)
                    this.Light.Image = Properties.Resources.yellow;
                else if (ping >= 200)
                    this.Light.Image = Properties.Resources.red;
            }
        }

        /// <summary>
        /// 显示FunGame信息
        /// </summary>
        private void ShowFunGameInfo()
        {
            WritelnGameInfo(FunGameInfo.GetInfo((FunGameInfo.FunGame)Constant.FunGameType));
        }

        /// <summary>
        /// 关闭所有登录后才能访问的窗口
        /// </summary>
        private static void CloseConnectedWindows()
        {
            RunTime.Login?.Close();
            RunTime.Register?.Close();
            RunTime.Store?.Close();
            RunTime.Inventory?.Close();
            RunTime.RoomSetting?.Close();
            RunTime.UserCenter?.Close();
            foreach (Form form in OpenForm.Forms)
            {
                form.Close();
            }
        }

        /// <summary>
        /// 退出游戏时处理
        /// </summary>
        private async Task ExitFunGame()
        {
            if (ShowMessage(ShowMessageType.OKCancel, "你确定关闭游戏？", "退出") == MessageResult.OK)
            {
                if (MainController != null) await LogOut();
                RunTime.Controller?.Close();
                Environment.Exit(0);
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e)
        {
            TaskUtility.StartAndAwaitTask(ExitFunGame);
        }

        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMatch_Click(object sender, EventArgs e)
        {
            // 开始匹配
            WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 开始匹配");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 开始匹配");
            WriteGameInfo(">> 匹配参数：");
            WritelnGameInfo(Config.FunGame_GameMode);
            // 显示停止匹配按钮
            StartMatch.Visible = false;
            StopMatch.Visible = true;
            // 暂停其他按钮
            StartMatch_Method((int)StartMatchState.Enable, new object[] { false });
            // 创建委托，开始匹配
            StartMatch_Action = (i, objs) =>
            {
                StartMatch_Method(i, objs);
            };
            // 创建新线程匹配
            MatchFunGame = Task.Factory.StartNew(() =>
            {

                if (InvokeRequired)
                {
                    Invoke(StartMatch_Action, (int)StartMatchState.Matching, null);
                }
                else
                {
                    StartMatch_Action((int)StartMatchState.Matching, null);
                }
            });
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateRoom_Click(object sender, EventArgs e)
        {
            string password = "";
            if (CheckMix.Checked && CheckTeam.Checked)
            {
                ShowMessage(ShowMessageType.Warning, "创建房间不允许同时勾选混战和团队！");
                return;
            }
            if (CheckHasPass.Checked)
            {
                password = ShowInputMessage("请输入该房间的密码：", "创建密码房间").Trim();
                if (password == "" || password.Length > 10)
                {
                    ShowMessage(ShowMessageType.Warning, "密码无效！密码不能为空或大于10个字符。");
                    return;
                }
            }
            if (Config.FunGame_GameMode.Equals(""))
            {
                ShowMessage(ShowMessageType.Warning, "请勾选你要创建的房间类型！");
                return;
            }
            TaskUtility.StartAndAwaitTask(() => CreateRoom_Handler(Config.FunGame_GameMode, password));
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitRoom_Click(object sender, EventArgs e)
        {
            string roomid = Usercfg.InRoom.Roomid;
            bool isMaster = Usercfg.InRoom.RoomMaster?.Id == Usercfg.LoginUser.Id;
            bool result = false;
            TaskUtility.StartAndAwaitTask(async () =>
            {
                if (await InvokeController_QuitRoom(Usercfg.InRoom, isMaster))
                {
                    WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 离开房间");
                    WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已离开房间 -> [ " + roomid + " ]");
                    InMain();
                    _ = MainController?.UpdateRoomAsync();
                    result = true;
                }
            }).OnCompleted(() =>
            {
                if (!result) ShowMessage(ShowMessageType.Error, "无法退出房间！", "退出房间");
            });
        }

        /// <summary>
        /// 房间设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomSetting_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 复制房间号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyRoomID_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(Usercfg.InRoom.Roomid);
            ShowMessage(ShowMessageType.Tip, "已复制房间号到剪贴板");
        }

        /// <summary>
        /// 查找房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryRoom_Click(object sender, EventArgs e)
        {
            TaskUtility.StartAndAwaitTask(async() => await JoinRoom(false, RoomText.Text));
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, EventArgs e)
        {
            TaskUtility.StartAndAwaitTask(async () =>
            {
                if (ShowMessage(ShowMessageType.OKCancel, "你确定要退出登录吗？", "退出登录") == MessageResult.OK)
                {
                    if (MainController == null || !await LogOut())
                        ShowMessage(ShowMessageType.Warning, "请求无效：退出登录失败！");
                }
            });
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, EventArgs e)
        {
            if (MainController != null && Config.FunGame_isConnected)
                OpenForm.SingleForm(FormType.Login, OpenFormType.Dialog);
            else
                ShowMessage(ShowMessageType.Warning, "请先连接服务器！");
        }

        /// <summary>
        /// 终止匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMatch_Click(object sender, EventArgs e)
        {
            StopMatch_Click();
        }

        /// <summary>
        /// 双击房间列表中的项可以加入房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (RoomList.SelectedItem != null)
            {
                TaskUtility.StartAndAwaitTask(async() => await JoinRoom(true, RoomList.SelectedItem.ToString() ?? ""));
            }
        }

        /// <summary>
        /// 点击发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendTalkText_Click(object sender, EventArgs e)
        {
            SendTalkText_Click(true);
        }

        /// <summary>
        /// 勾选任意模式选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckGameMode_CheckedChanged(object sender, EventArgs e)
        {
            bool IsMix = CheckMix.Checked;
            bool IsTeam = CheckTeam.Checked;
            bool IsHasPass = CheckHasPass.Checked;
            if (IsMix && IsTeam && !IsHasPass) Config.FunGame_GameMode = GameMode.GameMode_All;
            else if (IsMix && IsTeam && IsHasPass) Config.FunGame_GameMode = GameMode.GameMode_AllHasPass;
            else if (IsMix && !IsTeam && !IsHasPass) Config.FunGame_GameMode = GameMode.GameMode_Mix;
            else if (IsMix && !IsTeam && IsHasPass) Config.FunGame_GameMode = GameMode.GameMode_MixHasPass;
            else if (!IsMix && IsTeam && !IsHasPass) Config.FunGame_GameMode = GameMode.GameMode_Team;
            else if (!IsMix && IsTeam && IsHasPass) Config.FunGame_GameMode = GameMode.GameMode_TeamHasPass;
            else Config.FunGame_GameMode = GameMode.GameMode_All;
        }

        /// <summary>
        /// 房间号输入框点击/焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomText_ClickAndFocused(object sender, EventArgs e)
        {
            if (RoomText.Text.Equals("键入房间代号..."))
            {
                RoomText.ForeColor = Color.DarkGray;
                RoomText.Text = "";
            }
        }

        /// <summary>
        /// 焦点离开房间号输入框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomText_Leave(object sender, EventArgs e)
        {
            if (RoomText.Text.Equals(""))
            {
                RoomText.ForeColor = Color.DarkGray;
                RoomText.Text = "键入房间代号...";
            }
        }

        /// <summary>
        /// 房间号输入框内容改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomText_KeyUp(object sender, KeyEventArgs e)
        {
            RoomText.ForeColor = Color.Black;
            if (e.KeyCode.Equals(Keys.Enter))
            {
                // 按下回车加入房间
                TaskUtility.StartAndAwaitTask(async() => await JoinRoom(false, RoomText.Text));
            }
        }

        /// <summary>
        /// 聊天框点击/焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalkText_ClickAndFocused(object sender, EventArgs e)
        {
            if (TalkText.Text.Equals("向消息队列发送消息..."))
            {
                TalkText.ForeColor = Color.DarkGray;
                TalkText.Text = "";
            }
        }

        /// <summary>
        /// TalkText离开焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalkText_Leave(object sender, EventArgs e)
        {
            TalkText_Leave();
        }

        /// <summary>
        /// 聊天框内容改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalkText_KeyUp(object sender, KeyEventArgs e)
        {
            TalkText.ForeColor = Color.Black;
            if (e.KeyCode.Equals(Keys.Enter))
            {
                // 按下回车发送
                SendTalkText_Click(false);
            }
        }

        /// <summary>
        /// 版权链接点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Copyright 2023 mili.cyou
            Process.Start(new ProcessStartInfo("https://mili.cyou/fungame") { UseShellExecute = true });
        }

        /// <summary>
        /// 点击快捷消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetText_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 发送快捷消息并执行功能
            if (PresetText.SelectedIndex != 0)
            {
                string s = PresetText.SelectedItem.ToString() ?? "";
                SendTalkText_Click(s);
                SwitchTalkMessage(s);
                PresetText.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 关闭主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Disposed(object? sender, EventArgs e)
        {
            MainController?.Dispose();
        }

        /// <summary>
        /// 连接服务器失败后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public EventResult FailedConnectEvent(object sender, GeneralEventArgs e)
        {
            // 自动重连
            if (Config.FunGame_isConnected && Config.FunGame_isAutoRetry && CurrentRetryTimes <= MaxRetryTimes)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    if (Config.FunGame_isConnected && Config.FunGame_isAutoRetry) InvokeController_Connect(); // 再次判断是否开启自动重连
                });
                GetMessage("连接服务器失败，5秒后自动尝试重连。");
            }
            else GetMessage("无法连接至服务器，请检查你的网络连接。");
            return EventResult.Success;
        }

        /// <summary>
        /// 连接服务器成功后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public EventResult SucceedConnectEvent(object sender, GeneralEventArgs e)
        {
            // 创建MainController
            MainController = new MainController(this);
            if (MainController != null && Config.FunGame_isAutoLogin && Config.FunGame_AutoLoginUser != "" && Config.FunGame_AutoLoginPassword != "" && Config.FunGame_AutoLoginKey != "")
            {
                // 自动登录
                RunTime.Controller?.AutoLogin(Config.FunGame_AutoLoginUser, Config.FunGame_AutoLoginPassword, Config.FunGame_AutoLoginKey);
            }
            return EventResult.Success;
        }

        /// <summary>
        /// 登录成功后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private EventResult SucceedLoginEvent(object sender, GeneralEventArgs e)
        {
            TaskUtility.StartAndAwaitTask(SucceedLoginEvent_Handler);
            return EventResult.Success;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 判断是否存在这个房间
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        private bool CheckRoomIDExist(string roomid) => Rooms.IsExist(roomid);

        /// <summary>
        /// 获取房间对象
        /// </summary>
        /// <param name="roomid">房间号</param>
        /// <returns></returns>
        private Room GetRoom(string roomid)
        {
            Room? r = (Room?)Rooms[roomid];
            return r ?? General.HallInstance;
        }

        /// <summary>
        /// 判断快捷消息
        /// </summary>
        /// <param name="s"></param>
        private bool SwitchTalkMessage(string s)
        {
            switch (s)
            {
                case Constant.FunGame_SignIn:
                    break;
                case Constant.FunGame_ShowCredits:
                    break;
                case Constant.FunGame_ShowStock:
                    break;
                case Constant.FunGame_ShowStore:
                    break;
                case Constant.FunGame_ClearGameInfo:
                    GameInfo.Clear();
                    break;
                case Constant.FunGame_CreateMix:
                    TaskUtility.StartAndAwaitTask(() => CreateRoom_Handler(GameMode.GameMode_Mix));
                    break;
                case Constant.FunGame_CreateTeam:
                    TaskUtility.StartAndAwaitTask(() => CreateRoom_Handler(GameMode.GameMode_Team));
                    break;
                case Constant.FunGame_StartGame:
                    break;
                case Constant.FunGame_AutoRetryOn:
                    WritelnGameInfo(">> 自动重连开启");
                    Config.FunGame_isAutoRetry = true;
                    break;
                case Constant.FunGame_AutoRetryOff:
                    WritelnGameInfo(">> 自动重连关闭");
                    Config.FunGame_isAutoRetry = false;
                    break;
                case Constant.FunGame_Retry:
                    if (!Config.FunGame_isRetrying)
                    {
                        CurrentRetryTimes = -1;
                        Config.FunGame_isAutoRetry = true;
                        InvokeController_Connect();
                    }
                    else
                        WritelnGameInfo(">> 你不能在连接服务器的同时重试连接！");
                    break;
                case Constant.FunGame_Connect:
                    if (!Config.FunGame_isConnected)
                    {
                        CurrentRetryTimes = -1;
                        Config.FunGame_isAutoRetry = true;
                        InvokeController_Connect();
                    }
                    break;
                case Constant.FunGame_Disconnect:
                    if (Config.FunGame_isConnected && MainController != null)
                    {
                        // 先退出登录再断开连接
                        bool SuccessLogOut = false;
                        TaskUtility.StartAndAwaitTask(async() =>
                        {
                            if (await LogOut()) SuccessLogOut = true;
                        }).OnCompleted(() =>
                        {
                            if (SuccessLogOut) InvokeController_Disconnect();
                        });
                    }
                    break;
                case Constant.FunGame_DisconnectWhenNotLogin:
                    if (Config.FunGame_isConnected && MainController != null)
                    {
                        InvokeController_Disconnect();
                    }
                    break;
                case Constant.FunGame_ConnectTo:
                    string msg = ShowInputMessage("请输入服务器IP地址和端口号，如: 127.0.0.1:22222。", "连接指定服务器");
                    if (msg.Equals("")) return true;
                    string[] addr = msg.Split(':');
                    string ip;
                    int port;
                    if (addr.Length < 2)
                    {
                        ip = addr[0];
                        port = 22222;
                    }
                    else if (addr.Length < 3)
                    {
                        ip = addr[0];
                        port = Convert.ToInt32(addr[1]);
                    }
                    else
                    {
                        ShowMessage(ShowMessageType.Error, "格式错误！\n这不是一个服务器地址。");
                        return true;
                    }
                    ErrorIPAddressType ErrorType = NetworkUtility.IsServerAddress(ip, port);
                    if (ErrorType == Core.Library.Constant.ErrorIPAddressType.None)
                    {
                        RunTime.Session.Server_IP = ip;
                        RunTime.Session.Server_Port = port;
                        CurrentRetryTimes = -1;
                        Config.FunGame_isAutoRetry = true;
                        InvokeController_Connect();
                    }
                    else if (ErrorType == Core.Library.Constant.ErrorIPAddressType.IsNotIP) ShowMessage(ShowMessageType.Error, "这不是一个IP地址！");
                    else if (ErrorType == Core.Library.Constant.ErrorIPAddressType.IsNotPort) ShowMessage(ShowMessageType.Error, "这不是一个端口号！\n正确范围：1~65535");
                    else ShowMessage(ShowMessageType.Error, "格式错误！\n这不是一个服务器地址。");
                    break;
                default:
                    break;
            }
            return false;
        }

        #endregion

        #region 调用控制器

        /// <summary>
        /// 连接服务器，并处理事件
        /// </summary>
        /// <returns></returns>
        private void InvokeController_Connect()
        {
            try
            {
                ConnectEventArgs EventArgs = new(RunTime.Session.Server_IP, RunTime.Session.Server_Port);
                ConnectResult result = ConnectResult.CanNotConnect;

                TaskUtility.StartAndAwaitTask(() =>
                {
                    if (OnBeforeConnectEvent(EventArgs) == EventResult.Fail) return;
                    result = RunTime.Controller?.Connect() ?? result;
                    EventArgs.ConnectResult = result;
                }).OnCompleted(() =>
                {
                    if (result == ConnectResult.Success) OnSucceedConnectEvent(EventArgs);
                    else OnFailedConnectEvent(EventArgs);
                    OnAfterConnectEvent(EventArgs);
                });
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
            }
        }

        /// <summary>
        /// 断开服务器的连接，并处理事件
        /// </summary>
        /// <returns></returns>
        public void InvokeController_Disconnect()
        {
            try
            {
                bool result = false;

                TaskUtility.StartAndAwaitTask(() =>
                {
                    if (OnBeforeDisconnectEvent(new GeneralEventArgs()) == EventResult.Fail) return;
                    result = RunTime.Controller?.Disconnect() ?? false;
                }).OnCompleted(() =>
                {
                    if (result) OnSucceedDisconnectEvent(new GeneralEventArgs());
                    else OnFailedDisconnectEvent(new GeneralEventArgs());
                    OnAfterDisconnectEvent(new GeneralEventArgs());
                });
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
            }
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<bool> InvokeController_IntoRoom(Room room)
        {
            bool result = false;

            try
            {
                RoomEventArgs EventArgs = new(room);
                if (OnBeforeIntoRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = MainController is not null && await MainController.IntoRoomAsync(room);

                if (result) OnSucceedIntoRoomEvent(EventArgs);
                else OnFailedIntoRoomEvent(EventArgs);
                OnAfterIntoRoomEvent(EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public async Task<bool> InvokeController_QuitRoom(Room room, bool isMaster)
        {
            bool result = false;

            try
            {
                RoomEventArgs EventArgs = new(room);
                if (OnBeforeIntoRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = MainController is not null && await MainController.QuitRoomAsync(room.Roomid, isMaster);

                if (result) OnSucceedIntoRoomEvent(EventArgs);
                else OnFailedIntoRoomEvent(EventArgs);
                OnAfterIntoRoomEvent(EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogOut()
        {
            bool result = false;

            try
            {
                GeneralEventArgs EventArgs = new();
                if (OnBeforeLogoutEvent(EventArgs) == EventResult.Fail) return result;

                if (Usercfg.LoginUser.Id == 0) return result;

                if (Usercfg.InRoom.Roomid != "-1")
                {
                    string roomid = Usercfg.InRoom.Roomid;
                    bool isMaster = Usercfg.InRoom.RoomMaster?.Id == Usercfg.LoginUser?.Id;
                    MainController?.QuitRoomAsync(roomid, isMaster);
                }

                result = MainController is not null && await MainController.LogOutAsync();

                if (result) OnSucceedLogoutEvent(EventArgs);
                else OnFailedLogoutEvent(EventArgs);
                OnAfterLogoutEvent(EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }

        #endregion
    }
}
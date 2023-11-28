using System.Diagnostics;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
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
        private MainController? MainController = null;
        private readonly Core.Model.RoomList Rooms = RunTime.RoomList;
        private readonly Core.Model.Session Usercfg = RunTime.Session;
        private int _MatchSeconds = 0; // 记录匹配房间的秒数
        private bool _InGame = false;

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
            TaskUtility.NewTask(() =>
            {
                while (true)
                {
                    if (IsHandleCreated)
                    {
                        break;
                    }
                }
                // 加载模组
                RunTime.Controller.LoadGameModes();
                // 加载插件
                RunTime.Controller.LoadPlugins();
                // 自动连接服务器
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
            SucceedIntoRoom += SucceedIntoRoomEvent;
            FailedIntoRoom += FailedIntoRoomEvent;
            SucceedCreateRoom += SucceedCreateRoomEvent;
            FailedCreateRoom += FailedCreateRoomEvent;
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
                            SetServerStatusLight(LightType.Green);
                            if (Usercfg.InRoom.Roomid != "-1") SetButtonEnableIfLogon(true, ClientState.InRoom);
                            else SetButtonEnableIfLogon(true, ClientState.Online);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetGreenAndPing:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight(LightType.Green, ping: NetworkUtility.GetServerPing(RunTime.Session.Server_IP));
                            if (Usercfg.InRoom.Roomid != "-1") SetButtonEnableIfLogon(true, ClientState.InRoom);
                            else SetButtonEnableIfLogon(true, ClientState.Online);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight(LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.WaitConnectAndSetYellow:
                            Config.FunGame_isRetrying = false;
                            SetServerStatusLight(LightType.Yellow);
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
                            SetServerStatusLight(LightType.Yellow, true);
                            SetButtonEnableIfLogon(false, ClientState.WaitLogin);
                            Config.FunGame_isConnected = true;
                            CurrentRetryTimes = 0;
                            break;

                        case MainInvokeType.SetRed:
                            SetServerStatusLight(LightType.Red);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            Config.FunGame_isConnected = false;
                            break;

                        case MainInvokeType.Disconnected:
                            Rooms.Clear();
                            RoomList.Items.Clear();
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isConnected = false;
                            SetServerStatusLight(LightType.Red);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            LogoutAccount();
                            MainController?.MainController_Disposed();
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
                            SetServerStatusLight(LightType.Yellow);
                            SetButtonEnableIfLogon(false, ClientState.WaitConnect);
                            LogoutAccount();
                            MainController?.MainController_Disposed();
                            break;

                        case MainInvokeType.LogIn:
                            break;

                        case MainInvokeType.LogOut:
                            Config.FunGame_isRetrying = false;
                            Config.FunGame_isAutoLogin = false;
                            SetServerStatusLight(LightType.Yellow, true);
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
                                List<Room> list = (List<Room>)objs[0];
                                Rooms.AddRooms(list);
                                foreach (Room r in list)
                                {
                                    if (r.Roomid != "-1")
                                    {
                                        string item = r.Roomid;
                                        if (r.Name.Trim() != "")
                                        {
                                            item += " [ " + r.Name + " ]";
                                        }
                                        RoomList.Items.Add(item);
                                    }
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

                        case MainInvokeType.MatchRoom:
                            if (objs != null && objs.Length > 0)
                            {
                                StartMatch_Method((StartMatchState)objs[0], objs[1..]);
                            }
                            break;

                        case MainInvokeType.StartGame:
                            if (objs != null && objs.Length > 0)
                            {
                                Room room = (Room)objs[0];
                                List<User> users = (List<User>)objs[1];
                                StartGame(room, users);
                            }
                            break;

                        case MainInvokeType.EndGame:
                            if (objs != null && objs.Length > 0)
                            {
                                Room room = (Room)objs[0];
                                List<User> users = (List<User>)objs[1];
                                EndGame(room, users);
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    WritelnGameInfo(e.GetErrorInfo());
                    UpdateUI(MainInvokeType.Disconnected);
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
            StopMatch.Visible = false;
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
            StartMatch.Visible = false;
            StopMatch.Visible = false;
            QuitRoom.Visible = true;
            CreateRoom.Visible = false;
            RoomSetting.Visible = true;
            NowRoomID.Visible = true;
            CopyRoomID.Visible = true;
            // 禁用和激活按钮，并切换预设快捷消息
            SetButtonEnableIfLogon(true, ClientState.InRoom);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame(Room room, List<User> users)
        {
            _InGame = true;
            TaskUtility.NewTask(async () =>
            {
                int PlayerCount = users.Count;
                for (int i = 10; i > 0; i--)
                {
                    WritelnGameInfo("房间 [ " + room.Roomid + " (" + PlayerCount + "人" + RoomSet.GetTypeString(room.RoomType) + ") ] 的游戏将在" + i + "秒后开始...");
                    await Task.Delay(1000);
                }
                WritelnGameInfo("房间 [ " + room.Roomid + " (" + PlayerCount + "人" + RoomSet.GetTypeString(room.RoomType) + ") ] 的游戏正式开始！");
                if (RunTime.GameModeLoader?.Modes.ContainsKey(room.GameMode) ?? false)
                {
                    RunTime.GameModeLoader[room.GameMode].StartUI();
                    Visible = false; // 隐藏主界面
                }
            });
            SetButtonEnableIfLogon(false, ClientState.InRoom);
        }

        /// <summary>
        /// 游戏结束，结算
        /// </summary>
        private void EndGame(Room room, List<User> users)
        {
            Visible = true;
            // test
            WritelnGameInfo("===== TEST =====");
            SetButtonEnableIfLogon(true, ClientState.InRoom);
            _InGame = false;
            WritelnGameInfo("游戏结束！" + " [ " + users[new Random().Next(users.Count)] + " ] " + "是赢家！");
        }

        /// <summary>
        /// 未登录和离线时，停用按钮
        /// 登录的时候要激活按钮
        /// </summary>
        /// <param name="isLogon">是否登录</param>
        /// <param name="status">客户端状态</param>
        private void SetButtonEnableIfLogon(bool isLogon, ClientState status)
        {
            if (_InGame)
            {
                AccountSetting.Enabled = isLogon;
                Stock.Enabled = isLogon;
                Store.Enabled = isLogon;
                RoomBox.Enabled = isLogon;
                RefreshRoomList.Enabled = isLogon;
                CheckMix.Enabled = isLogon;
                CheckTeam.Enabled = isLogon;
                CheckHasPass.Enabled = isLogon;
                QuitRoom.Enabled = isLogon;
                RoomSetting.Enabled = isLogon;
                PresetText.Enabled = isLogon;
                TalkText.Enabled = isLogon;
                SendTalkText.Enabled = isLogon;
                Logout.Enabled = isLogon;
                if (!isLogon) return;
            }
            switch (status)
            {
                case ClientState.WaitConnect:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetNoConnectItems);
                    break;
                case ClientState.WaitLogin:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetNoLoginItems);
                    break;
                case ClientState.Online:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetOnlineItems);
                    break;
                case ClientState.InRoom:
                    PresetText.Items.Clear();
                    PresetText.Items.AddRange(Constant.PresetInRoomItems);
                    break;
            }
            this.PresetText.SelectedIndex = 0;
            StartMatch.Enabled = isLogon;
            AccountSetting.Enabled = isLogon;
            Stock.Enabled = isLogon;
            Store.Enabled = isLogon;
            if (!Config.FunGame_isMatching)
            {
                // 匹配中时不修改部分按钮状态
                RoomBox.Enabled = isLogon;
                CreateRoom.Enabled = isLogon;
                RefreshRoomList.Enabled = isLogon;
                CheckMix.Enabled = isLogon;
                CheckTeam.Enabled = isLogon;
                CheckHasPass.Enabled = isLogon;
            }
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
        /// 加入房间的具体处理方法
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
                        if (await MainController.GetRoomPlayerCountAsync(roomid) < 8)
                        {
                            if (ShowMessage(ShowMessageType.YesNo, "已找到房间 -> [ " + roomid + " ]\n是否加入？", "已找到房间") == MessageResult.Yes)
                            {
                                Room r = GetRoom(roomid);
                                return await InvokeController_IntoRoom(r);
                            }
                            return false;
                        }
                        else
                        {
                            ShowMessage(ShowMessageType.Warning, "房间已满，拒绝加入！");
                            return false;
                        }
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
        private void StartMatch_Method(StartMatchState status, params object[] objs)
        {
            switch (status)
            {
                case StartMatchState.Matching:
                    // 开始匹配
                    if (MainController != null)
                    {
                        TaskUtility.NewTask(async () =>
                        {
                            Config.FunGame_isMatching = true;
                            if (await MainController.MatchRoomAsync(Config.FunGame_RoomType))
                            {
                                // 开始匹配
                                while (Config.FunGame_isMatching)
                                {
                                    if (_MatchSeconds < 60)
                                    {
                                        await Task.Delay(1000);
                                        _MatchSeconds++;
                                        SetMatchSecondsText();
                                        continue;
                                    }
                                    // 达到60秒时
                                    if (Config.FunGame_isMatching && await MainController.MatchRoomAsync(Config.FunGame_RoomType, true))
                                    {
                                        // 取消匹配
                                        UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Success, General.HallInstance);
                                        UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Cancel);
                                        break;
                                    }
                                }
                            }
                            else Config.FunGame_isMatching = false;
                        });
                    }
                    else
                    {
                        WritelnGameInfo("ERROR：匹配失败！");
                    }
                    break;
                case StartMatchState.Success:
                    // 匹配成功返回房间号
                    TaskUtility.NewTask(async () =>
                    {
                        Room room = General.HallInstance;
                        if (objs != null) room = (Room)objs[0];
                        if (room.Roomid != "-1")
                        {
                            GetMessage("匹配成功 -> 房间号： " + room.Roomid);
                            if (MainController != null) await MainController.UpdateRoomAsync();
                            if (Rooms.IsExist(room.Roomid))
                            {
                                Room target = Rooms[room.Roomid];
                                await InvokeController_IntoRoom(target);
                            }
                            else
                            {
                                GetMessage("加入房间失败！原因：房间号不存在或已被解散。");
                            }
                        }
                        else
                        {
                            GetMessage("匹配失败！暂时无法找到符合条件的房间。");
                        }
                        // 更新按钮图标和文字
                        UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Enable, true);
                        StopMatch.Visible = false;
                        StartMatch.Visible = true;
                    });
                    break;
                case StartMatchState.Enable:
                    // 设置匹配过程中的各种按钮是否可用
                    bool isEnabel = false;
                    if (objs != null) isEnabel = (bool)objs[0];
                    CheckMix.Enabled = isEnabel;
                    CheckTeam.Enabled = isEnabel;
                    CheckHasPass.Enabled = isEnabel;
                    CreateRoom.Enabled = isEnabel;
                    RoomBox.Enabled = isEnabel;
                    RefreshRoomList.Enabled = isEnabel;
                    Logout.Enabled = isEnabel;
                    break;
                case StartMatchState.Cancel:
                    Config.FunGame_isMatching = false;
                    WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 终止匹配");
                    WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 已终止匹配。");
                    UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Enable, true);
                    StopMatch.Visible = false;
                    StartMatch.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// 更新当前匹配时间
        /// </summary>
        private void SetMatchSecondsText()
        {
            if (_MatchSeconds <= 0) StopMatch.Text = "停止匹配";
            if (_MatchSeconds < 60)
            {
                StopMatch.Text = _MatchSeconds + " 秒";
            }
            else StopMatch.Text = "1 分 " + (_MatchSeconds - 60) + " 秒";
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
            TaskUtility.NewTask(async () =>
            {
                if (MainController != null && await MainController.MatchRoomAsync(Config.FunGame_RoomType, true))
                {
                    UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Cancel);
                }
            });
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
                    TaskUtility.NewTask(async () =>
                    {
                        if (!await InvokeController_SendTalk(" [ " + Usercfg.LoginUserName + " ] 说： " + text))
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
        private async Task CreateRoom_Handler(string RoomType, string GameMode, string GameMap, string Password = "")
        {
            if (Usercfg.InRoom.Roomid != "-1")
            {
                ShowMessage(ShowMessageType.Warning, "已在房间中，无法创建房间。");
                return;
            }
            GameMode? mode = RunTime.GameModeLoader?.Modes.Values.FirstOrDefault() ?? default;
            if (mode is null)
            {
                ShowMessage(ShowMessageType.Error, ">> 缺少" + Config.FunGame_RoomType + "所需的模组，无法创建房间。");
                return;
            }
            Room room = await InvokeController_CreateRoom(RoomType, GameMode, GameMap, Password);
            if (MainController is not null && room.Roomid != "-1")
            {
                await MainController.UpdateRoomAsync();
                await InvokeController_IntoRoom(room);
                return;
            }
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
        private void SetServerStatusLight(LightType light, bool waitlogin = false, int ping = 0)
        {
            switch (light)
            {
                case LightType.Green:
                    Connection.Text = "服务器连接成功";
                    this.Light.Image = Properties.Resources.green;
                    break;
                case LightType.Yellow:
                    Connection.Text = waitlogin ? "等待登录账号" : "等待连接服务器";
                    this.Light.Image = Properties.Resources.yellow;
                    break;
                case LightType.Red:
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
        private void ExitFunGame()
        {
            bool exit = false;
            TaskUtility.NewTask(() =>
            {
                if (ShowMessage(ShowMessageType.OKCancel, "你确定关闭游戏？", "退出") == MessageResult.OK)
                {
                    InvokeController_Disconnect();
                    exit = true;
                }
            }).OnCompleted(() =>
            {
                if (exit)
                {
                    Environment.Exit(0);
                }
            });
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
            TaskUtility.NewTask(ExitFunGame);
        }

        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMatch_Click(object sender, EventArgs e)
        {
            // 开始匹配
            _MatchSeconds = 0;
            SetMatchSecondsText();
            WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 开始匹配");
            WritelnGameInfo("[ " + Usercfg.LoginUserName + " ] 开始匹配");
            WriteGameInfo(">> 匹配参数：");
            WritelnGameInfo(Config.FunGame_RoomType);
            // 显示停止匹配按钮
            StartMatch.Visible = false;
            StopMatch.Visible = true;
            // 暂停其他按钮
            UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Enable, false);
            // 开始匹配
            UpdateUI(MainInvokeType.MatchRoom, StartMatchState.Matching);
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
            if (Config.FunGame_RoomType.Equals(""))
            {
                ShowMessage(ShowMessageType.Warning, "请勾选你要创建的房间类型！");
                return;
            }
            GameMode? mode = RunTime.GameModeLoader?.Modes.Values.FirstOrDefault() ?? default;
            if (mode is null)
            {
                ShowMessage(ShowMessageType.Error, ">> 缺少" + Config.FunGame_RoomType + "所需的模组，无法创建房间。");
                return;
            }
            TaskUtility.NewTask(() => CreateRoom_Handler(Config.FunGame_RoomType, mode.Name, mode.DefaultMap, password));
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
            TaskUtility.NewTask(async () =>
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
            TaskUtility.NewTask(async () => await JoinRoom(false, RoomText.Text));
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, EventArgs e)
        {
            TaskUtility.NewTask(async () =>
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
                TaskUtility.NewTask(async () => await JoinRoom(true, RoomList.SelectedItem.ToString() ?? ""));
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
            if (IsMix && IsTeam && !IsHasPass) Config.FunGame_RoomType = RoomSet.All;
            else if (IsMix && IsTeam && IsHasPass) Config.FunGame_RoomType = RoomSet.All;
            else if (IsMix && !IsTeam && !IsHasPass) Config.FunGame_RoomType = RoomSet.Mix;
            else if (IsMix && !IsTeam && IsHasPass) Config.FunGame_RoomType = RoomSet.Mix;
            else if (!IsMix && IsTeam && !IsHasPass) Config.FunGame_RoomType = RoomSet.Team;
            else if (!IsMix && IsTeam && IsHasPass) Config.FunGame_RoomType = RoomSet.Team;
            else Config.FunGame_RoomType = RoomSet.All;
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
                TaskUtility.NewTask(async () => await JoinRoom(false, RoomText.Text));
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
            // Copyright 2023 milimoe
            Process.Start(new ProcessStartInfo("https://github.com/milimoe") { UseShellExecute = true });
        }

        /// <summary>
        /// 点击快捷消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetText_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 发送快捷消息并执行功能
            if (PresetText.SelectedIndex != 0 && PresetText.SelectedItem != null)
            {
                string s = PresetText.SelectedItem.ToString() ?? "";
                SendTalkText_Click(s);
                SwitchTalkMessage(s);
                PresetText.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 刷新房间列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshRoomList_Click(object sender, EventArgs e)
        {
            TaskUtility.NewTask(async () =>
            {
                if (MainController != null)
                {
                    await MainController.UpdateRoomAsync();
                }
            });
        }

        /// <summary>
        /// 鼠标离开停止匹配按钮时，恢复按钮文本为匹配时长
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void StopMatch_MouseLeave(object? sender, EventArgs e) => SetMatchSecondsText();

        /// <summary>
        /// 鼠标轻触停止匹配按钮时，将文本从匹配时长转为停止匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void StopMatch_MouseHover(object? sender, EventArgs e)
        {
            StopMatch.Text = "停止匹配";
        }

        /// <summary>
        /// 关闭主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Disposed(object? sender, EventArgs e)
        {
            MainController?.MainController_Disposed();
        }

        /// <summary>
        /// 连接服务器失败后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public void FailedConnectEvent(object sender, GeneralEventArgs e)
        {
            // 自动重连
            if (!Config.FunGame_isConnected)
            {
                if (Config.FunGame_isAutoRetry && CurrentRetryTimes <= MaxRetryTimes)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(5000);
                        if (!Config.FunGame_isConnected && Config.FunGame_isAutoRetry) InvokeController_Connect(); // 再次判断是否开启自动重连
                    });
                    GetMessage("连接服务器失败，5秒后自动尝试重连。");
                }
                else GetMessage("无法连接至服务器，请稍后再试。");
            }
        }

        /// <summary>
        /// 连接服务器成功后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public void SucceedConnectEvent(object sender, GeneralEventArgs e)
        {
            // 创建MainController
            MainController = new MainController(this);
            if (MainController != null && Config.FunGame_isAutoLogin && Config.FunGame_AutoLoginUser != "" && Config.FunGame_AutoLoginPassword != "" && Config.FunGame_AutoLoginKey != "")
            {
                // 自动登录
                RunTime.Controller?.AutoLogin(Config.FunGame_AutoLoginUser, Config.FunGame_AutoLoginPassword, Config.FunGame_AutoLoginKey);
            }
        }

        /// <summary>
        /// 登录成功后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void SucceedLoginEvent(object sender, GeneralEventArgs e)
        {
            TaskUtility.NewTask(SucceedLoginEvent_Handler);
        }

        /// <summary>
        /// 进入房间失败后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FailedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            ShowMessage(ShowMessageType.Warning, "加入房间失败！");
        }

        /// <summary>
        /// 成功进入房间后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SucceedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            SetRoomid(e.Room);
            InRoom();
        }

        /// <summary>
        /// 创建房间失败后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FailedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            ShowMessage(ShowMessageType.General, "创建" + e.RoomTypeString + "房间失败！", "创建失败");
        }

        /// <summary>
        /// 成功创建房间后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SucceedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            WritelnGameInfo(DateTimeUtility.GetNowShortTime() + " 创建" + e.RoomTypeString + "房间");
            WritelnGameInfo(">> 创建" + e.RoomTypeString + "房间成功！房间号： " + e.RoomID);
            ShowMessage(ShowMessageType.General, "创建" + e.RoomTypeString + "房间成功！\n房间号是 -> [ " + e.RoomID + " ]", "创建成功");
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
                    if (Usercfg.InRoom.Roomid == "-1")
                    {
                        GameMode? mode = RunTime.GameModeLoader?.Modes.Values.FirstOrDefault() ?? default;
                        if (mode != null) TaskUtility.NewTask(() => CreateRoom_Handler(RoomSet.Mix, mode.Name, mode.DefaultMap));
                        else WritelnGameInfo(">> 缺少" + RoomSet.GetTypeString(RoomType.Mix) + "所需的模组，无法创建房间。");
                    }
                    else WritelnGameInfo(">> 先退出当前房间才可以创建房间。");
                    break;
                case Constant.FunGame_CreateTeam:
                    if (Usercfg.InRoom.Roomid == "-1")
                    {
                        GameMode? mode = RunTime.GameModeLoader?.Modes.Values.FirstOrDefault() ?? default;
                        if (mode != null) TaskUtility.NewTask(() => CreateRoom_Handler(RoomSet.Team, mode.Name, mode.DefaultMap));
                        else WritelnGameInfo(">> 缺少" + RoomSet.GetTypeString(RoomType.Team) + "所需的模组，无法创建房间。");
                    }
                    else WritelnGameInfo(">> 先退出当前房间才可以创建房间。");
                    break;
                case Constant.FunGame_Ready:
                case ".r":
                case ".ready":
                    if (MainController != null)
                    {
                        if (Usercfg.InRoom.Roomid != "-1")
                        {
                            TaskUtility.NewTask(async () =>
                            {
                                if (await MainController.SetReadyAsync(Usercfg.InRoom.Roomid))
                                {
                                    await InvokeController_SendTalk(" [ " + Usercfg.LoginUser.Username + " ] 已准备。");
                                }
                            });
                        }
                        else WritelnGameInfo(">> 不在房间中无法使用此命令。");
                    }
                    break;
                case Constant.FunGame_CancelReady:
                case ".cr":
                case ".ready -c":
                case ".cancelready":
                    if (MainController != null)
                    {
                        if (Usercfg.InRoom.Roomid != "-1")
                        {
                            TaskUtility.NewTask(async () =>
                            {
                                if (await MainController.CancelReadyAsync(Usercfg.InRoom.Roomid))
                                {
                                    await InvokeController_SendTalk(" [ " + Usercfg.LoginUser.Username + " ] 已取消准备。");
                                }
                            });
                        }
                        else WritelnGameInfo(">> 不在房间中无法使用此命令。");
                    }
                    break;
                case Constant.FunGame_StartGame:
                    if (MainController != null)
                    {
                        if (Usercfg.InRoom.Roomid != "-1")
                        {
                            TaskUtility.NewTask(async () => await InvokeController_StartGame(Usercfg.InRoom.Roomid, Usercfg.InRoom.RoomMaster?.Id == Usercfg.LoginUser.Id));
                        }
                        else WritelnGameInfo(">> 不在房间中无法使用此命令。");
                    }
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
                    if (Config.FunGame_isConnected && !Config.FunGame_isMatching && MainController != null)
                    {
                        // 先退出登录再断开连接
                        bool SuccessLogOut = false;
                        TaskUtility.NewTask(async () =>
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
                    if (!Config.FunGame_isConnected)
                    {
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
                        if (ErrorType == ErrorIPAddressType.None)
                        {
                            RunTime.Session.Server_IP = ip;
                            RunTime.Session.Server_Port = port;
                            CurrentRetryTimes = -1;
                            Config.FunGame_isAutoRetry = true;
                            InvokeController_Connect();
                        }
                        else if (ErrorType == ErrorIPAddressType.IsNotIP) ShowMessage(ShowMessageType.Error, "这不是一个IP地址！");
                        else if (ErrorType == ErrorIPAddressType.IsNotPort) ShowMessage(ShowMessageType.Error, "这不是一个端口号！\n正确范围：1~65535");
                        else ShowMessage(ShowMessageType.Error, "格式错误！\n这不是一个服务器地址。");
                    }
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
            ConnectEventArgs EventArgs = new(RunTime.Session.Server_IP, RunTime.Session.Server_Port);

            ConnectResult result = ConnectResult.CanNotConnect;

            TaskUtility.NewTask(() =>
            {
                OnBeforeConnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeConnectEvent(this, EventArgs);
                if (EventArgs.Cancel) return;
                result = RunTime.Controller?.Connect(RunTime.Session.Server_IP, RunTime.Session.Server_Port) ?? result;
                EventArgs.ConnectResult = result;
            }).OnCompleted(() =>
            {
                if (result == ConnectResult.Success)
                {
                    OnSucceedConnectEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnSucceedConnectEvent(this, EventArgs);
                }
                else
                {
                    OnFailedConnectEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnFailedConnectEvent(this, EventArgs);
                }
                OnAfterConnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterConnectEvent(this, EventArgs);
            }).OnError(e =>
            {
                EventArgs.ConnectResult = ConnectResult.ConnectFailed;
                GetMessage(e.InnerException?.ToString() ?? e.ToString(), TimeType.None);
                UpdateUI(MainInvokeType.SetRed);
                Config.FunGame_isRetrying = false;
                OnFailedConnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedConnectEvent(this, EventArgs);
                OnAfterConnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterConnectEvent(this, EventArgs);
            });
        }

        /// <summary>
        /// 断开服务器的连接，并处理事件
        /// </summary>
        /// <returns></returns>
        public void InvokeController_Disconnect()
        {
            GeneralEventArgs EventArgs = new();
            bool result = false;

            TaskUtility.NewTask(async () =>
            {
                OnBeforeDisconnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeDisconnectEvent(this, EventArgs);
                if (EventArgs.Cancel) return;

                if (Usercfg.LoginUser.Id != 0)
                {
                    await LogOut();
                }

                result = RunTime.Controller?.Disconnect() ?? false;
            }).OnCompleted(() =>
            {
                if (result)
                {
                    OnSucceedDisconnectEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnSucceedDisconnectEvent(this, EventArgs);
                }
                else
                {
                    OnFailedDisconnectEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnFailedDisconnectEvent(this, EventArgs);
                }
                OnAfterDisconnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterDisconnectEvent(this, EventArgs);
            }).OnError(e =>
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
                OnFailedDisconnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedDisconnectEvent(this, EventArgs);
                OnAfterDisconnectEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterDisconnectEvent(this, EventArgs);
            });
        }

        /// <summary>
        /// 发送聊天信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<bool> InvokeController_SendTalk(string msg)
        {
            SendTalkEventArgs EventArgs = new(msg);
            bool result = false;

            try
            {
                OnBeforeSendTalkEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeSendTalkEvent(this, EventArgs);
                if (EventArgs.Cancel) return result;

                result = MainController is not null && await MainController.ChatAsync(msg);

                if (result)
                {
                    OnSucceedSendTalkEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnSucceedSendTalkEvent(this, EventArgs);
                }
                else
                {
                    OnFailedSendTalkEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnFailedSendTalkEvent(this, EventArgs);
                }
                OnAfterSendTalkEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterSendTalkEvent(this, EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
                OnFailedSendTalkEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedSendTalkEvent(this, EventArgs);
                OnAfterSendTalkEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterSendTalkEvent(this, EventArgs);
            }

            return result;
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<bool> InvokeController_IntoRoom(Room room)
        {
            RoomEventArgs EventArgs = new(room);
            bool result = false;

            try
            {
                OnBeforeIntoRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeIntoRoomEvent(this, EventArgs);
                if (EventArgs.Cancel) return result;

                result = MainController is not null && await MainController.IntoRoomAsync(room);

                if (room.Roomid != "-1")
                {
                    if (result)
                    {
                        OnSucceedIntoRoomEvent(this, EventArgs);
                        RunTime.PluginLoader?.OnSucceedIntoRoomEvent(this, EventArgs);
                    }
                    else
                    {
                        OnFailedIntoRoomEvent(this, EventArgs);
                        RunTime.PluginLoader?.OnFailedIntoRoomEvent(this, EventArgs);
                    }
                    OnAfterIntoRoomEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnAfterIntoRoomEvent(this, EventArgs);
                }
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
                OnFailedIntoRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedIntoRoomEvent(this, EventArgs);
                OnAfterIntoRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterIntoRoomEvent(this, EventArgs);
            }

            return result;
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<Room> InvokeController_CreateRoom(string RoomType, string GameMode, string GameMap, string Password = "")
        {
            RoomEventArgs EventArgs = new(RoomType, Password);
            Room room = General.HallInstance;

            try
            {
                OnBeforeCreateRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeCreateRoomEvent(this, EventArgs);
                if (EventArgs.Cancel) return room;

                room = MainController is null ? room : await MainController.CreateRoomAsync(RoomType, GameMode, GameMap, Password);

                if (room.Roomid != "-1")
                {
                    EventArgs = new(room);
                    OnSucceedCreateRoomEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnSucceedCreateRoomEvent(this, EventArgs);
                }
                else
                {
                    OnFailedCreateRoomEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnFailedCreateRoomEvent(this, EventArgs);
                }
                OnAfterCreateRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterCreateRoomEvent(this, EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
                OnFailedCreateRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedCreateRoomEvent(this, EventArgs);
                OnAfterCreateRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterCreateRoomEvent(this, EventArgs);
            }

            return room;
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public async Task<bool> InvokeController_QuitRoom(Room room, bool isMaster)
        {
            RoomEventArgs EventArgs = new(room);
            bool result = false;

            try
            {
                OnBeforeIntoRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeIntoRoomEvent(this, EventArgs);
                if (EventArgs.Cancel) return result;

                result = MainController is not null && await MainController.QuitRoomAsync(room.Roomid, isMaster);

                if (result)
                {
                    OnSucceedQuitRoomEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnSucceedQuitRoomEvent(this, EventArgs);
                    // 禁用和激活按钮，并切换预设快捷消息
                    SetButtonEnableIfLogon(true, ClientState.Online);
                }
                else
                {
                    OnFailedQuitRoomEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnFailedQuitRoomEvent(this, EventArgs);
                }
                OnAfterQuitRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterQuitRoomEvent(this, EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
                OnFailedQuitRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedQuitRoomEvent(this, EventArgs);
                OnAfterQuitRoomEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterQuitRoomEvent(this, EventArgs);
                // 禁用和激活按钮，并切换预设快捷消息
                SetButtonEnableIfLogon(true, ClientState.Online);
            }

            return result;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogOut()
        {
            GeneralEventArgs EventArgs = new();
            bool result = false;

            try
            {
                OnBeforeLogoutEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeLogoutEvent(this, EventArgs);
                if (EventArgs.Cancel) return result;

                if (Usercfg.LoginUser.Id == 0) return result;

                if (Usercfg.InRoom.Roomid != "-1")
                {
                    bool isMaster = Usercfg.InRoom.RoomMaster?.Id == Usercfg.LoginUser?.Id;
                    await InvokeController_QuitRoom(Usercfg.InRoom, isMaster);
                }

                result = MainController is not null && await MainController.LogOutAsync();

                if (result)
                {
                    OnSucceedLogoutEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnSucceedLogoutEvent(this, EventArgs);
                }
                else
                {
                    OnFailedLogoutEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnFailedLogoutEvent(this, EventArgs);
                }
                OnAfterLogoutEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterLogoutEvent(this, EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
                OnFailedLogoutEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedLogoutEvent(this, EventArgs);
                OnAfterLogoutEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterLogoutEvent(this, EventArgs);
            }

            return result;
        }

        /// <summary>
        /// 开始游戏/提醒玩家准备/提醒房主开始游戏
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="isMaster"></param>
        /// <returns></returns>
        public async Task<bool> InvokeController_StartGame(string roomid, bool isMaster)
        {
            GeneralEventArgs EventArgs = new();
            bool result = false;

            try
            {
                OnBeforeStartGameEvent(this, EventArgs);
                RunTime.PluginLoader?.OnBeforeStartGameEvent(this, EventArgs);
                if (EventArgs.Cancel) return result;

                result = MainController is not null && await MainController.StartGameAsync(roomid, isMaster);

                if (result)
                {
                    OnSucceedStartGameEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnSucceedStartGameEvent(this, EventArgs);
                }
                else
                {
                    OnFailedStartGameEvent(this, EventArgs);
                    RunTime.PluginLoader?.OnFailedStartGameEvent(this, EventArgs);
                }
                OnAfterStartGameEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterStartGameEvent(this, EventArgs);
            }
            catch (Exception e)
            {
                GetMessage(e.GetErrorInfo(), TimeType.None);
                OnFailedStartGameEvent(this, EventArgs);
                RunTime.PluginLoader?.OnFailedStartGameEvent(this, EventArgs);
                OnAfterStartGameEvent(this, EventArgs);
                RunTime.PluginLoader?.OnAfterStartGameEvent(this, EventArgs);
            }

            return result;
        }

        #endregion
    }
}
using System.Collections.Generic;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController : SocketHandlerController
    {
        private readonly Main Main;
        private readonly Session Usercfg = RunTime.Session;
        private readonly DataRequest UpdateRoomRequest;
        private readonly DataRequest IntoRoomRequest;
        private readonly DataRequest QuitRoomRequest;

        public MainController(Main main) : base(RunTime.Socket)
        {
            Main = main;
            UpdateRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_UpdateRoom);
            IntoRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_IntoRoom);
            QuitRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_QuitRoom);
            Disposed += MainController_Disposed;
        }

        #region 公开方法

        public async Task<bool> LogOut()
        {
            try
            {
                // 需要当时登录给的Key发回去，确定是账号本人在操作才允许登出
                if (Usercfg.LoginKey != Guid.Empty)
                {
                    SetWorking();
                    if (RunTime.Socket?.Send(SocketMessageType.RunTime_Logout, Usercfg.LoginKey) == SocketResult.Success)
                    {
                        string msg = "";
                        Guid key = Guid.Empty;
                        (msg, key) = await Task.Factory.StartNew(SocketHandler_LogOut);
                        if (key == Usercfg.LoginKey)
                        {
                            Usercfg.LoginKey = Guid.Empty;
                            Main.UpdateUI(MainInvokeType.LogOut, msg ?? "");
                            return true;
                        }
                    }
                }
                throw new CanNotLogOutException();
            }
            catch (Exception e)
            {
                ShowMessage.ErrorMessage("无法登出您的账号，请联系服务器管理员。", "登出失败", 5);
                Main.GetMessage(e.GetErrorInfo());
            }
            return false;
        }

        public async Task<bool> IntoRoom(Room room)
        {
            try
            {
                string rid = room.Roomid;
                IntoRoomRequest.AddRequestData("room", rid);
                await IntoRoomRequest.SendRequestAsync();
                if (IntoRoomRequest.Result == RequestResult.Success)
                {
                    string roomid = IntoRoomRequest.GetResult<string>("roomid") ?? "";
                    if (rid == roomid)
                    {
                        // 先确认是否是加入的房间，防止服务端返回错误的房间
                        if (roomid.Trim() != "" && roomid == "-1")
                        {
                            Main.GetMessage($"已连接至公共聊天室。");
                        }
                        else
                        {
                            Usercfg.InRoom = room;
                        }
                        return true;
                    }
                }
                throw new CanNotIntoRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return false;
            }
        }

        public async Task<bool> UpdateRoom()
        {
            bool result = false;

            try
            {
                List<Room> list = new();
                await UpdateRoomRequest.SendRequestAsync();
                if (UpdateRoomRequest.Result == RequestResult.Success)
                {
                    list = UpdateRoomRequest.GetResult<List<Room>>("roomid") ?? new();
                    Main.UpdateUI(MainInvokeType.UpdateRoom, list);
                }
                else throw new CanNotIntoRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }

            return result;
        }

        public async Task<int> GetRoomPlayerCount(string roomid)
        {
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.Room_GetRoomPlayerCount, roomid) == SocketResult.Success)
                {
                    return await Task.Factory.StartNew(SocketHandler_GetRoomPlayerCount);
                }
                return 0;
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return 0;
            }
        }

        public async Task<bool> QuitRoom(string roomid, bool isMaster)
        {
            bool result = false;

            try
            {
                QuitRoomRequest.AddRequestData("roomid", roomid);
                QuitRoomRequest.AddRequestData("isMaster", isMaster);
                await QuitRoomRequest.SendRequestAsync();
                if (QuitRoomRequest.Result == RequestResult.Success)
                {
                    result = QuitRoomRequest.GetResult<bool>("result");
                    if (result)
                    {
                        Usercfg.InRoom = General.HallInstance;
                        return result;
                    }
                }
                throw new CanNotIntoRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return result;
            }
        }

        public async Task<string> CreateRoom(string RoomType, string Password = "")
        {
            string roomid = "-1";

            try
            {
                DataRequest request = RunTime.NewDataRequest(DataRequestType.Main_CreateRoom);
                request.AddRequestData("roomtype", RoomType);
                request.AddRequestData("user", Usercfg.LoginUser);
                request.AddRequestData("password", Password);
                await UpdateRoomRequest.SendRequestAsync();
                if (request.Result == RequestResult.Success)
                {
                    roomid = request.GetResult<string>("roomid") ?? "-1";
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }

            return roomid;
        }

        public bool Chat(string msg)
        {
            try
            {
                if (RunTime.Socket?.Send(SocketMessageType.Main_Chat, msg) == SocketResult.Success)
                {
                    return true;
                }
                throw new CanNotSendTalkException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return false;
            }
        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                // 定义接收的通信类型
                SocketMessageType[] SocketMessageTypes = new SocketMessageType[] { SocketMessageType.Main_GetNotice, SocketMessageType.RunTime_Logout, SocketMessageType.Main_IntoRoom, SocketMessageType.Main_QuitRoom,
                    SocketMessageType.Main_Chat, SocketMessageType.Main_UpdateRoom, SocketMessageType.Main_CreateRoom };
                if (SocketObject.SocketType == SocketMessageType.RunTime_HeartBeat)
                {
                    // 心跳包单独处理
                    if ((RunTime.Socket?.Connected ?? false) && Usercfg.LoginUser.Id != 0)
                        Main.UpdateUI(MainInvokeType.SetGreenAndPing);
                }
                else if (SocketObject.SocketType == SocketMessageType.RunTime_ForceLogout)
                {
                    // 服务器强制下线登录
                    Guid key = Guid.Empty;
                    string? msg = "";
                    if (SocketObject.Length > 0) key = SocketObject.GetParam<Guid>(0);
                    if (SocketObject.Length > 1) msg = SocketObject.GetParam<string>(1);
                    msg ??= "";
                    if (key == Usercfg.LoginKey)
                    {
                        Usercfg.LoginKey = Guid.Empty;
                        Main.UpdateUI(MainInvokeType.LogOut, msg ?? "");
                    }
                }
                else if (SocketObject.SocketType == SocketMessageType.Main_Chat)
                {
                    // 收到房间聊天信息
                    string? user = "", msg = "";
                    if (SocketObject.Length > 0) user = SocketObject.GetParam<string>(0);
                    if (SocketObject.Length > 1) msg = SocketObject.GetParam<string>(1);
                    if (user != Usercfg.LoginUserName)
                    {
                        Main.GetMessage(msg, TimeType.None);
                    }
                }
                else if (SocketObject.SocketType == SocketMessageType.Room_UpdateRoomMaster)
                {
                    // 收到房间更换房主的信息
                    User user = General.UnknownUserInstance;
                    Room room = General.HallInstance;
                    if (SocketObject.Length > 0) user = SocketObject.GetParam<User>(0) ?? General.UnknownUserInstance;
                    if (SocketObject.Length > 1) room = SocketObject.GetParam<Room>(1) ?? General.HallInstance;
                    if (room.Roomid != "-1" && room.Roomid == Usercfg.InRoom.Roomid) Main.UpdateUI(MainInvokeType.UpdateRoomMaster, room);
                }
                else if (SocketMessageTypes.Contains(SocketObject.SocketType))
                {
                    Work = SocketObject;
                    Working = false;
                }
            }
            catch (Exception e)
            {
                RunTime.Controller?.Error(e);
            }
        }

        #endregion

        #region 私有方法

        private (string, Guid) SocketHandler_LogOut()
        {
            string? msg = "";
            Guid key = Guid.Empty;
            try
            {
                WaitForWorkDone();
                // 返回一个Key，如果这个Key是空的，登出失败
                if (Work.Length > 0) key = Work.GetParam<Guid>(0);
                if (Work.Length > 1) msg = Work.GetParam<string>(1);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            msg ??= "";
            return (msg, key);
        }

        private int SocketHandler_GetRoomPlayerCount()
        {
            int count = 0;
            try
            {
                WaitForWorkDone();
                if (Work.Length > 0) count = Work.GetParam<int>(0);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            return count;
        }

        private void MainController_Disposed()
        {
            UpdateRoomRequest.Dispose();
            IntoRoomRequest.Dispose();
            QuitRoomRequest.Dispose();
        }

        #endregion
    }

    public class MainController2 : SocketHandlerController
    {
        private MainModel MainModel { get; }
        private Main Main { get; }

        private readonly Core.Model.Session Usercfg = RunTime.Session;

        public MainController2(Main Main)
        {
            this.Main = Main;
            MainModel = new MainModel(Main);
        }

        public override void Dispose()
        {
            MainModel.Dispose();
        }

        public async Task<bool> LogOut()
        {
            bool result = false;

            try
            {
                GeneralEventArgs EventArgs = new();
                if (Main.OnBeforeLogoutEvent(EventArgs) == EventResult.Fail) return result;

                if (Usercfg.LoginUser.Id == 0) return result;

                if (Usercfg.InRoom.Roomid != "-1")
                {
                    string roomid = Usercfg.InRoom.Roomid;
                    bool isMaster = Usercfg.InRoom.RoomMaster?.Id == Usercfg.LoginUser?.Id;
                    await MainModel.QuitRoom(roomid, isMaster);
                }

                result = await MainModel.LogOut();

                if (result) Main.OnSucceedLogoutEvent(EventArgs);
                else Main.OnFailedLogoutEvent(EventArgs);
                Main.OnAfterLogoutEvent(EventArgs);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }

        public async Task<bool> UpdateRoom()
        {
            return await MainModel.UpdateRoom();
        }

        public async Task<bool> IntoRoom(Room room)
        {
            bool result = false;

            try
            {
                RoomEventArgs EventArgs = new(room);
                if (Main.OnBeforeIntoRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = await MainModel.IntoRoom(room);

                if (result) Main.OnSucceedIntoRoomEvent(EventArgs);
                else Main.OnFailedIntoRoomEvent(EventArgs);
                Main.OnAfterIntoRoomEvent(EventArgs);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }
        
        public async Task<bool> QuitRoom(Room room, bool isMaster)
        {
            bool result = false;
            string roomid = room.Roomid;

            try
            {
                RoomEventArgs EventArgs = new(room);
                if (Main.OnBeforeQuitRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = await MainModel.QuitRoom(roomid, isMaster);

                if (result) Main.OnSucceedQuitRoomEvent(EventArgs);
                else Main.OnFailedQuitRoomEvent(EventArgs);
                Main.OnAfterQuitRoomEvent(EventArgs);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }
        
        public async Task<string> CreateRoom(string RoomType, string Password = "")
        {
            string result = "";

            try
            {
                RoomEventArgs EventArgs = new(RoomType, Password);
                if (Main.OnBeforeCreateRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = await MainModel.CreateRoom(RoomType, Password);

                if (result.Trim() != "") Main.OnSucceedCreateRoomEvent(EventArgs);
                else Main.OnFailedCreateRoomEvent(EventArgs);
                Main.OnAfterCreateRoomEvent(EventArgs);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }

        public async Task<int> GetRoomPlayerCount(string roomid)
        {
            return await MainModel.GetRoomPlayerCount(roomid);
        }

        public bool Chat(string msg)
        {
            bool result = false;

            try
            {
                SendTalkEventArgs EventArgs = new(msg);
                if (Main.OnBeforeSendTalkEvent(EventArgs) == EventResult.Fail) return result;

                if (msg.Trim() != "")
                {
                    result = MainModel.Chat(msg);
                }

                if (result) Main.OnSucceedSendTalkEvent(EventArgs);
                else Main.OnFailedSendTalkEvent(EventArgs);
                Main.OnAfterSendTalkEvent(EventArgs);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
            
            return result;
        }
    }
}

using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController : SocketHandlerController
    {
        private readonly Main Main;
        private readonly Session Usercfg = RunTime.Session;
        private readonly DataRequest ChatRequest;
        private readonly DataRequest CreateRoomRequest;
        private readonly DataRequest GetRoomPlayerCountRequest;
        private readonly DataRequest UpdateRoomRequest;
        private readonly DataRequest IntoRoomRequest;
        private readonly DataRequest QuitRoomRequest;

        public MainController(Main main) : base(RunTime.Socket)
        {
            Main = main;
            ChatRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_Chat);
            CreateRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_CreateRoom);
            GetRoomPlayerCountRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Room_GetRoomPlayerCount);
            UpdateRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_UpdateRoom);
            IntoRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_IntoRoom);
            QuitRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_QuitRoom);
            Disposed += MainController_Disposed;
        }

        #region 公开方法

        public async Task<bool> LogOutAsync()
        {
            try
            {
                // 需要当时登录给的Key发回去，确定是账号本人在操作才允许登出
                if (Usercfg.LoginKey != Guid.Empty)
                {
                    DataRequest request = RunTime.NewDataRequest(DataRequestType.RunTime_Logout);
                    request.AddRequestData("loginkey", Usercfg.LoginKey);
                    await request.SendRequestAsync();
                    if (request.Result == RequestResult.Success)
                    {
                        string msg = "";
                        Guid key = Guid.Empty;
                        msg = request.GetResult<string>("msg") ?? "";
                        key = request.GetResult<Guid>("key");
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

        public async Task<bool> IntoRoomAsync(Room room)
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

        public async Task<bool> UpdateRoomAsync()
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

        public async Task<int> GetRoomPlayerCountAsync(string roomid)
        {
            try
            {
                GetRoomPlayerCountRequest.AddRequestData("roomid", roomid);
                await GetRoomPlayerCountRequest.SendRequestAsync();
                return GetRoomPlayerCountRequest.GetResult<int>("count");
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return 0;
            }
        }

        public async Task<bool> QuitRoomAsync(string roomid, bool isMaster)
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

        public async Task<string> CreateRoomAsync(string RoomType, string Password = "")
        {
            string roomid = "-1";

            try
            {
                CreateRoomRequest.AddRequestData("roomtype", RoomType);
                CreateRoomRequest.AddRequestData("user", Usercfg.LoginUser);
                CreateRoomRequest.AddRequestData("password", Password);
                await CreateRoomRequest.SendRequestAsync();
                if (CreateRoomRequest.Result == RequestResult.Success)
                {
                    roomid = CreateRoomRequest.GetResult<string>("roomid") ?? "-1";
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }

            return roomid;
        }

        public async Task<bool> ChatAsync(string msg)
        {
            try
            {
                ChatRequest.AddRequestData("msg", msg);
                await ChatRequest.SendRequestAsync();
                if (ChatRequest.Result == RequestResult.Success)
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
                if (SocketObject.SocketType == SocketMessageType.HeartBeat)
                {
                    // 心跳包单独处理
                    if ((RunTime.Socket?.Connected ?? false) && Usercfg.LoginUser.Id != 0)
                        Main.UpdateUI(MainInvokeType.SetGreenAndPing);
                }
                else if (SocketObject.SocketType == SocketMessageType.ForceLogout)
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
                else if (SocketObject.SocketType == SocketMessageType.Chat)
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
                else if (SocketObject.SocketType == SocketMessageType.UpdateRoomMaster)
                {
                    // 收到房间更换房主的信息
                    User user = General.UnknownUserInstance;
                    Room room = General.HallInstance;
                    if (SocketObject.Length > 0) user = SocketObject.GetParam<User>(0) ?? General.UnknownUserInstance;
                    if (SocketObject.Length > 1) room = SocketObject.GetParam<Room>(1) ?? General.HallInstance;
                    if (room.Roomid != "-1" && room.Roomid == Usercfg.InRoom.Roomid) Main.UpdateUI(MainInvokeType.UpdateRoomMaster, room);
                }
            }
            catch (Exception e)
            {
                RunTime.Controller?.Error(e);
            }
        }

        #endregion

        #region 私有方法

        private void MainController_Disposed()
        {
            ChatRequest.Dispose();
            CreateRoomRequest.Dispose();
            GetRoomPlayerCountRequest.Dispose();
            UpdateRoomRequest.Dispose();
            IntoRoomRequest.Dispose();
            QuitRoomRequest.Dispose();
        }

        #endregion
    }
}

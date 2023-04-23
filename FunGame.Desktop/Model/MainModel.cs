using System.Collections;
using System.Data;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Model
{
    public class MainModel : BaseModel
    {
        private readonly Main Main;
        private readonly Core.Model.Session Usercfg = RunTime.Session;

        public MainModel(Main main) : base(RunTime.Socket)
        {
            Main = main;
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
                    if (RunTime.Socket?.Send(SocketMessageType.Logout, Usercfg.LoginKey) == SocketResult.Success)
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
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.IntoRoom, room.Roomid) == SocketResult.Success)
                {
                    string roomid = await Task.Factory.StartNew(SocketHandler_IntoRoom);
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
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.UpdateRoom) == SocketResult.Success)
                {
                    List<Room> list = await Task.Factory.StartNew(SocketHandler_UpdateRoom);
                    Main.UpdateUI(MainInvokeType.UpdateRoom, list);
                    return true;
                }
                throw new GetRoomListException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return false;
            }
        }
        
        public async Task<int> GetRoomPlayerCount(string roomid)
        {
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.GetRoomPlayerCount, roomid) == SocketResult.Success)
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
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.QuitRoom, roomid, isMaster) == SocketResult.Success)
                {
                    result = await Task.Factory.StartNew(SocketHandler_QuitRoom);
                    if (result)
                    {
                        Usercfg.InRoom = General.HallInstance;
                        return result;
                    }
                }
                throw new QuitRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return result;
            }
        }
        
        public async Task<string> CreateRoom(string RoomType, string Password = "")
        {
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.CreateRoom, RoomType, Usercfg.LoginUser.Id, Password) == SocketResult.Success)
                {
                    string roomid = await Task.Factory.StartNew(SocketHandler_CreateRoom);
                    if (roomid.Trim() != "")
                    {
                        return roomid;
                    }
                }
                throw new CreateRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return "";
            }
        }

        public bool Chat(string msg)
        {
            try
            {
                if (RunTime.Socket?.Send(SocketMessageType.Chat, msg) == SocketResult.Success)
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
                SocketMessageType[] SocketMessageTypes = new SocketMessageType[] { SocketMessageType.GetNotice, SocketMessageType.Logout, SocketMessageType.IntoRoom, SocketMessageType.QuitRoom,
                    SocketMessageType.Chat, SocketMessageType.UpdateRoom, SocketMessageType.CreateRoom };
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
                    DataSet? user = null, room = null;
                    if (SocketObject.Length > 0) user = SocketObject.GetParam<DataSet>(0);
                    if (SocketObject.Length > 1) room = SocketObject.GetParam<DataSet>(1);
                    Room r = Core.Api.Utility.Factory.GetRoom(room, user);
                    if (r.Roomid != "-1" && r.Roomid == Usercfg.InRoom.Roomid) Main.UpdateUI(MainInvokeType.UpdateRoomMaster, r);
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

        #region SocketHandler

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
        
        private string SocketHandler_IntoRoom()
        {
            string? roomid = "";
            try
            {
                WaitForWorkDone();
                if (Work.Length > 0) roomid = Work.GetParam<string>(0);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            roomid ??= "";
            return roomid;
        }
        
        private string SocketHandler_CreateRoom()
        {
            string? roomid = "";
            try
            {
                WaitForWorkDone();
                if (Work.Length > 0) roomid = Work.GetParam<string>(0);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            roomid ??= "";
            return roomid;
        }
        
        private bool SocketHandler_QuitRoom()
        {
            bool result = false;
            try
            {
                WaitForWorkDone();
                if (Work.Length > 0) result = Work.GetParam<bool>(0);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            return result;
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
        
        private List<Room> SocketHandler_UpdateRoom()
        {
            List<Room> list = new();
            try
            {
                WaitForWorkDone();
                DataSet? DsRoom = new(), DsUser = new();
                if (Work.Length > 0) DsRoom = Work.GetParam<DataSet>(0);
                if (Work.Length > 1) DsUser = Work.GetParam<DataSet>(1);
                if (DsRoom != null && DsUser != null)
                {
                    list = Core.Api.Utility.Factory.GetList<Room>(DsRoom, DsUser);
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            return list;
        }
        
        #endregion
    }
}

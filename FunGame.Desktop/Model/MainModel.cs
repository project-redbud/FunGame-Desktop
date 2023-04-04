﻿using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;
using System.Collections.Generic;

namespace Milimoe.FunGame.Desktop.Model
{
    public class MainModel : BaseModel
    {
        private readonly Main Main;

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
                if (Config.Guid_LoginKey != Guid.Empty)
                {
                    SetWorking();
                    if (RunTime.Socket?.Send(SocketMessageType.Logout, Config.Guid_LoginKey) == SocketResult.Success)
                    {
                        string msg = "";
                        Guid key = Guid.Empty;
                        (msg, key) = await Task.Factory.StartNew(SocketHandler_LogOut);
                        if (key == Config.Guid_LoginKey)
                        {
                            Config.Guid_LoginKey = Guid.Empty;
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

        public async Task<bool> IntoRoom(string roomid)
        {
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.IntoRoom, roomid) == SocketResult.Success)
                {
                    roomid = await Task.Factory.StartNew(SocketHandler_IntoRoom);
                    if (roomid.Trim() != "" && roomid == "-1")
                    {
                        Main.GetMessage($"已连接至公共聊天室。");
                    }
                    else
                    {
                        Config.FunGame_Roomid = roomid;
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
                    List<string> list = await Task.Factory.StartNew(SocketHandler_UpdateRoom);
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
        
        public async Task<bool> QuitRoom(string roomid)
        {
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.QuitRoom, roomid) == SocketResult.Success)
                {
                    bool result = await Task.Factory.StartNew(SocketHandler_QuitRoom);
                    if (result)
                    {
                        Config.FunGame_Roomid = "-1";
                    }
                }
                throw new QuitRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return false;
            }
        }
        
        public bool CreateRoom()
        {
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.CreateRoom) == SocketResult.Success)
                {
                    
                }
                throw new CreateRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
                return false;
            }
        }

        public async Task<bool> Chat(string msg)
        {
            try
            {
                SetWorking();
                if (RunTime.Socket?.Send(SocketMessageType.Chat, msg) == SocketResult.Success)
                {
                    string user = "";
                    (user, msg) = await Task.Factory.StartNew(SocketHandler_Chat);
                    if (user != Usercfg.LoginUserName)
                    {
                        Main.GetMessage(msg, TimeType.None);
                        return true;
                    }
                    else return false;
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
                    SocketMessageType.Chat, SocketMessageType.UpdateRoom };
                if (SocketObject.SocketType == SocketMessageType.HeartBeat)
                {
                    // 心跳包单独处理
                    if ((RunTime.Socket?.Connected ?? false) && Usercfg.LoginUser != null)
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
                    if (key == Config.Guid_LoginKey)
                    {
                        Config.Guid_LoginKey = Guid.Empty;
                        Main.UpdateUI(MainInvokeType.LogOut, msg ?? "");
                    }
                }
                else if (SocketMessageTypes.Contains(SocketObject.SocketType))
                {
                    Work = SocketObject;
                    Working = false;
                }
            }
            catch (Exception e)
            {
                RunTime.Connector?.Error(e);
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
        
        private List<string> SocketHandler_UpdateRoom()
        {
            List<string>? list = null;
            try
            {
                WaitForWorkDone();
                if (Work.Length > 0) list = Work.GetParam<List<string>>(0);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            list ??= new List<string>();
            return list;
        }
        
        private (string, string) SocketHandler_Chat()
        {
            string? user = "", msg = "";
            try
            {
                WaitForWorkDone();
                if (Work.Length > 0) user = Work.GetParam<string>(0);
                if (Work.Length > 1) msg = Work.GetParam<string>(1);
            }                               
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo());
            }
            user ??= "";
            msg ??= "";
            return (user, msg);
        }

        #endregion
    }
}

using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Model;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController(Main Main)
    {
        private readonly Session Usercfg = RunTime.Session;
        private readonly DataRequest ChatRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_Chat);
        private readonly DataRequest CreateRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_CreateRoom);
        private readonly DataRequest MatchRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_MatchRoom);
        private readonly DataRequest GetRoomPlayerCountRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Room_GetRoomPlayerCount);
        private readonly DataRequest UpdateRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_UpdateRoom);
        private readonly DataRequest IntoRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_IntoRoom);
        private readonly DataRequest QuitRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_QuitRoom);
        private readonly DataRequest SetReadyRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_Ready);
        private readonly DataRequest CancelReadyRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_CancelReady);
        private readonly DataRequest StartGameRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_StartGame);

        #region 公开方法

        public void MainController_Disposed()
        {
            ChatRequest.Dispose();
            CreateRoomRequest.Dispose();
            GetRoomPlayerCountRequest.Dispose();
            UpdateRoomRequest.Dispose();
            IntoRoomRequest.Dispose();
            QuitRoomRequest.Dispose();
            SetReadyRequest.Dispose();
            CancelReadyRequest.Dispose();
            StartGameRequest.Dispose();
        }

        public async Task<bool> LogOutAsync()
        {
            try
            {
                // 需要当时登录给的Key发回去，确定是账号本人在操作才允许登出
                if (Usercfg.LoginKey != Guid.Empty)
                {
                    DataRequest request = RunTime.NewDataRequest(DataRequestType.RunTime_Logout);
                    request.AddRequestData("key", Usercfg.LoginKey);
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
                Main.ShowMessage(ShowMessageType.Error, "无法登出您的账号，请联系服务器管理员。", "登出失败", 5);
                Main.GetMessage(e.GetErrorInfo());
            }
            return false;
        }

        public async Task<bool> IntoRoomAsync(Room room)
        {
            try
            {
                IntoRoomRequest.AddRequestData("roomid", room.Roomid);
                await IntoRoomRequest.SendRequestAsync();
                if (IntoRoomRequest.Result == RequestResult.Success)
                {
                    return IntoRoomRequest.GetResult<bool>("result");
                }
                throw new CanNotIntoRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                return false;
            }
        }

        public async Task<bool> UpdateRoomAsync()
        {
            bool result = false;

            try
            {
                List<Room> list = [];
                await UpdateRoomRequest.SendRequestAsync();
                if (UpdateRoomRequest.Result == RequestResult.Success)
                {
                    list = UpdateRoomRequest.GetResult<List<Room>>("rooms") ?? [];
                    Main.UpdateUI(MainInvokeType.UpdateRoom, list);
                }
                else throw new CanNotIntoRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
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
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                return 0;
            }
        }

        public async Task<bool> SetReadyAsync(string roomid)
        {
            try
            {
                bool result = true;

                SetReadyRequest.AddRequestData("roomid", roomid);
                await SetReadyRequest.SendRequestAsync();
                if (SetReadyRequest.Result == RequestResult.Success)
                {
                    result = SetReadyRequest.GetResult<bool>("result");
                    if (result)
                    {
                        Config.FunGame_isInRoom = true;
                        Main.GetMessage("[ " + Usercfg.LoginUser.Username + " ] 准备完毕。");
                    }
                    List<User> ReadyPlayerList = SetReadyRequest.GetResult<List<User>>("ready") ?? [];
                    if (ReadyPlayerList.Count > 0) Main.GetMessage("已准备的玩家：" + string.Join(", ", ReadyPlayerList.Select(u => u.Username)));
                    List<User> NotReadyPlayerList = SetReadyRequest.GetResult<List<User>>("notready") ?? [];
                    if (NotReadyPlayerList.Count > 0) Main.GetMessage("仍未准备的玩家：" + string.Join(", ", NotReadyPlayerList.Select(u => u.Username)));
                }
                return result;
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                return false;
            }
        }

        public async Task<bool> CancelReadyAsync(string roomid)
        {
            try
            {
                bool result = true;

                CancelReadyRequest.AddRequestData("roomid", roomid);
                await CancelReadyRequest.SendRequestAsync();
                if (CancelReadyRequest.Result == RequestResult.Success)
                {
                    result = CancelReadyRequest.GetResult<bool>("result");
                    if (result)
                    {
                        Config.FunGame_isInRoom = false;
                        Main.GetMessage("[ " + Usercfg.LoginUser.Username + " ] 已取消准备。");
                    }
                    List<User> ReadyPlayerList = CancelReadyRequest.GetResult<List<User>>("ready") ?? [];
                    if (ReadyPlayerList.Count > 0) Main.GetMessage("已准备的玩家：" + string.Join(", ", ReadyPlayerList.Select(u => u.Username)));
                    List<User> NotReadyPlayerList = CancelReadyRequest.GetResult<List<User>>("notready") ?? [];
                    if (NotReadyPlayerList.Count > 0) Main.GetMessage("仍未准备的玩家：" + string.Join(", ", NotReadyPlayerList.Select(u => u.Username)));
                }
                return result;
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                return false;
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
                throw new QuitRoomException();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                return result;
            }
        }

        public async Task<Room> CreateRoomAsync(RoomType roomType, string gameModuleServer, string gameMap, bool isRank, int maxUsers, string password = "")
        {
            Room room = General.HallInstance;

            try
            {
                CreateRoomRequest.AddRequestData("roomType", roomType);
                CreateRoomRequest.AddRequestData("gameModuleServer", gameModuleServer);
                CreateRoomRequest.AddRequestData("gameMap", gameMap);
                CreateRoomRequest.AddRequestData("master", Usercfg.LoginUser);
                CreateRoomRequest.AddRequestData("password", password);
                CreateRoomRequest.AddRequestData("isRank", isRank);
                CreateRoomRequest.AddRequestData("maxUsers", maxUsers);
                await CreateRoomRequest.SendRequestAsync();
                if (CreateRoomRequest.Result == RequestResult.Success)
                {
                    room = CreateRoomRequest.GetResult<Room>("room") ?? room;
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return room;
        }

        public async Task<bool> MatchRoomAsync(RoomType roomType, bool isCancel = false)
        {
            bool result = false;

            try
            {
                MatchRoomRequest.AddRequestData("roomType", roomType);
                MatchRoomRequest.AddRequestData("matcher", Usercfg.LoginUser);
                MatchRoomRequest.AddRequestData("isCancel", isCancel);
                await MatchRoomRequest.SendRequestAsync();
                if (MatchRoomRequest.Result == RequestResult.Success)
                {
                    result = MatchRoomRequest.GetResult<bool>("result");
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
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
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
                return false;
            }
        }

        public async Task<bool> StartGameAsync(string roomid, bool isMaster)
        {
            try
            {
                StartGameRequest.AddRequestData("roomid", roomid);
                StartGameRequest.AddRequestData("isMaster", isMaster);
                await StartGameRequest.SendRequestAsync();
                if (StartGameRequest.Result == RequestResult.Success)
                {
                    return StartGameRequest.GetResult<bool>("result");
                }
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return false;
        }

        #endregion
    }
}

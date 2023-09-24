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
    public class MainController
    {
        private readonly Main Main;
        private readonly Session Usercfg = RunTime.Session;
        private readonly DataRequest ChatRequest;
        private readonly DataRequest CreateRoomRequest;
        private readonly DataRequest GetRoomPlayerCountRequest;
        private readonly DataRequest UpdateRoomRequest;
        private readonly DataRequest IntoRoomRequest;
        private readonly DataRequest QuitRoomRequest;

        public MainController(Main main)
        {
            Main = main;
            ChatRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_Chat);
            CreateRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_CreateRoom);
            GetRoomPlayerCountRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Room_GetRoomPlayerCount);
            UpdateRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_UpdateRoom);
            IntoRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_IntoRoom);
            QuitRoomRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Main_QuitRoom);
        }

        #region 公开方法

        public void MainController_Disposed()
        {
            ChatRequest.Dispose();
            CreateRoomRequest.Dispose();
            GetRoomPlayerCountRequest.Dispose();
            UpdateRoomRequest.Dispose();
            IntoRoomRequest.Dispose();
            QuitRoomRequest.Dispose();
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
                List<Room> list = new();
                await UpdateRoomRequest.SendRequestAsync();
                if (UpdateRoomRequest.Result == RequestResult.Success)
                {
                    list = UpdateRoomRequest.GetResult<List<Room>>("rooms") ?? new();
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

        public async Task<Room> CreateRoomAsync(string RoomType, string Password = "")
        {
            Room room = General.HallInstance;

            try
            {
                CreateRoomRequest.AddRequestData("roomtype", RoomType);
                CreateRoomRequest.AddRequestData("master", Usercfg.LoginUser);
                CreateRoomRequest.AddRequestData("password", Password);
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

        #endregion
    }
}

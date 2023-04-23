using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController : BaseController
    {
        private MainModel MainModel { get; }
        private Main Main { get; }

        private readonly Core.Model.Session Usercfg = RunTime.Session;

        public MainController(Main Main)
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

using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Core.Library.Exception;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class MainController : BaseController
    {
        private MainModel MainModel { get; }
        private Main Main { get; }

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

        public async Task<bool> IntoRoom(string roomid)
        {
            bool result = false;

            try
            {
                RoomEventArgs EventArgs = new(roomid);
                if (Main.OnBeforeIntoRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = await MainModel.IntoRoom(roomid);

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
        
        public async Task<bool> QuitRoom(string roomid)
        {
            bool result = false;

            try
            {
                RoomEventArgs EventArgs = new(roomid);
                if (Main.OnBeforeQuitRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = await MainModel.QuitRoom(roomid);

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
        
        public bool CreateRoom()
        {
            bool result = false;

            try
            {
                RoomEventArgs EventArgs = new();
                if (Main.OnBeforeCreateRoomEvent(EventArgs) == EventResult.Fail) return result;

                result = MainModel.CreateRoom();

                if (result) Main.OnSucceedCreateRoomEvent(EventArgs);
                else Main.OnFailedCreateRoomEvent(EventArgs);
                Main.OnAfterCreateRoomEvent(EventArgs);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
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

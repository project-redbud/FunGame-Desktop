using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseMain : GeneralForm, IConnectEventHandler, IDisconnectEventHandler, ILoginEventHandler, ILogoutEventHandler, IIntoRoomEventHandler, ISendTalkEventHandler,
        ICreateRoomEventHandler, IQuitRoomEventHandler, IStartMatchEventHandler, IStartGameEventHandler, IOpenInventoryEventHandler, IOpenStoreEventHandler
    {
        public event IConnectEventHandler.BeforeEventHandler? BeforeConnect;
        public event IConnectEventHandler.AfterEventHandler? AfterConnect;
        public event IConnectEventHandler.SucceedEventHandler? SucceedConnect;
        public event IConnectEventHandler.FailedEventHandler? FailedConnect;

        public void OnAfterConnectEvent(ConnectEventArgs e)
        {
            if (AfterConnect != null)
            {
                AfterConnect(this, e);
            }
        }

        public void OnBeforeConnectEvent(ConnectEventArgs e)
        {
            if (BeforeConnect != null)
            {
                BeforeConnect(this, e);
            }
        }

        public void OnSucceedConnectEvent(ConnectEventArgs e)
        {
            if (SucceedConnect != null)
            {
                SucceedConnect(this, e);
            }
        }

        public void OnFailedConnectEvent(ConnectEventArgs e)
        {
            if (FailedConnect != null)
            {
                FailedConnect(this, e);
            }
        }

        public event IEventHandler.BeforeEventHandler? BeforeDisconnect;
        public event IEventHandler.AfterEventHandler? AfterDisconnect;
        public event IEventHandler.SucceedEventHandler? SucceedDisconnect;
        public event IEventHandler.FailedEventHandler? FailedDisconnect;

        public void OnAfterDisconnectEvent(GeneralEventArgs e)
        {
            if (AfterDisconnect != null)
            {
                AfterDisconnect(this, e);
            }
        }

        public void OnBeforeDisconnectEvent(GeneralEventArgs e)
        {
            if (BeforeDisconnect != null)
            {
                BeforeDisconnect(this, e);
            }
        }

        public void OnFailedDisconnectEvent(GeneralEventArgs e)
        {
            if (FailedDisconnect != null)
            {
                FailedDisconnect(this, e);
            }
        }

        public void OnSucceedDisconnectEvent(GeneralEventArgs e)
        {
            if (SucceedDisconnect != null)
            {
                SucceedDisconnect(this, e);
            }
        }

        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;
        public event ILoginEventHandler.SucceedEventHandler? SucceedLogin;
        public event ILoginEventHandler.FailedEventHandler? FailedLogin;

        public void OnBeforeLoginEvent(LoginEventArgs e)
        {
            if (BeforeLogin != null)
            {
                BeforeLogin(this, e);
            }
        }

        public void OnAfterLoginEvent(LoginEventArgs e)
        {
            if (AfterLogin != null)
            {
                AfterLogin(this, e);
            }
        }

        public void OnSucceedLoginEvent(LoginEventArgs e)
        {
            if (SucceedLogin != null)
            {
                SucceedLogin(this, e);
            }
        }

        public void OnFailedLoginEvent(LoginEventArgs e)
        {
            if (FailedLogin != null)
            {
                FailedLogin(this, e);
            }
        }

        public event IEventHandler.BeforeEventHandler? BeforeLogout;
        public event IEventHandler.AfterEventHandler? AfterLogout;
        public event IEventHandler.SucceedEventHandler? SucceedLogout;
        public event IEventHandler.FailedEventHandler? FailedLogout;

        public void OnAfterLogoutEvent(GeneralEventArgs e)
        {
            if (AfterLogout != null)
            {
                AfterLogout(this, e);
            }
        }

        public void OnBeforeLogoutEvent(GeneralEventArgs e)
        {
            if (BeforeLogout != null)
            {
                BeforeLogout(this, e);
            }
        }

        public void OnFailedLogoutEvent(GeneralEventArgs e)
        {
            if (FailedLogout != null)
            {
                FailedLogout(this, e);
            }
        }

        public void OnSucceedLogoutEvent(GeneralEventArgs e)
        {
            if (SucceedLogout != null)
            {
                SucceedLogout(this, e);
            }
        }

        public event IIntoRoomEventHandler.BeforeEventHandler? BeforeIntoRoom;
        public event IIntoRoomEventHandler.AfterEventHandler? AfterIntoRoom;
        public event IIntoRoomEventHandler.SucceedEventHandler? SucceedIntoRoom;
        public event IIntoRoomEventHandler.FailedEventHandler? FailedIntoRoom;

        public void OnBeforeIntoRoomEvent(RoomEventArgs e)
        {
            if (BeforeIntoRoom != null)
            {
                BeforeIntoRoom(this, e);
            }
        }

        public void OnAfterIntoRoomEvent(RoomEventArgs e)
        {
            if (AfterIntoRoom != null)
            {
                AfterIntoRoom(this, e);
            }
        }

        public void OnSucceedIntoRoomEvent(RoomEventArgs e)
        {
            if (SucceedIntoRoom != null)
            {
                SucceedIntoRoom(this, e);
            }
        }

        public void OnFailedIntoRoomEvent(RoomEventArgs e)
        {
            if (FailedIntoRoom != null)
            {
                FailedIntoRoom(this, e);
            }
        }

        public event ISendTalkEventHandler.BeforeEventHandler? BeforeSendTalk;
        public event ISendTalkEventHandler.AfterEventHandler? AfterSendTalk;
        public event ISendTalkEventHandler.SucceedEventHandler? SucceedSendTalk;
        public event ISendTalkEventHandler.FailedEventHandler? FailedSendTalk;

        public void OnBeforeSendTalkEvent(SendTalkEventArgs e)
        {
            if (BeforeSendTalk != null)
            {
                BeforeSendTalk(this, e);
            }
        }

        public void OnAfterSendTalkEvent(SendTalkEventArgs e)
        {
            if (AfterSendTalk != null)
            {
                AfterSendTalk(this, e);
            }
        }

        public void OnSucceedSendTalkEvent(SendTalkEventArgs e)
        {
            if (SucceedSendTalk != null)
            {
                SucceedSendTalk(this, e);
            }
        }

        public void OnFailedSendTalkEvent(SendTalkEventArgs e)
        {
            if (FailedSendTalk != null)
            {
                FailedSendTalk(this, e);
            }
        }

        public event ICreateRoomEventHandler.BeforeEventHandler? BeforeCreateRoom;
        public event ICreateRoomEventHandler.AfterEventHandler? AfterCreateRoom;
        public event ICreateRoomEventHandler.SucceedEventHandler? SucceedCreateRoom;
        public event ICreateRoomEventHandler.FailedEventHandler? FailedCreateRoom;

        public void OnBeforeCreateRoomEvent(RoomEventArgs e)
        {
            if (BeforeCreateRoom != null)
            {
                BeforeCreateRoom(this, e);
            }
        }

        public void OnAfterCreateRoomEvent(RoomEventArgs e)
        {
            if (AfterCreateRoom != null)
            {
                AfterCreateRoom(this, e);
            }
        }

        public void OnSucceedCreateRoomEvent(RoomEventArgs e)
        {
            if (SucceedCreateRoom != null)
            {
                SucceedCreateRoom(this, e);
            }
        }

        public void OnFailedCreateRoomEvent(RoomEventArgs e)
        {
            if (FailedCreateRoom != null)
            {
                FailedCreateRoom(this, e);
            }
        }

        public event IQuitRoomEventHandler.BeforeEventHandler? BeforeQuitRoom;
        public event IQuitRoomEventHandler.AfterEventHandler? AfterQuitRoom;
        public event IQuitRoomEventHandler.SucceedEventHandler? SucceedQuitRoom;
        public event IQuitRoomEventHandler.FailedEventHandler? FailedQuitRoom;

        public void OnBeforeQuitRoomEvent(RoomEventArgs e)
        {
            if (BeforeQuitRoom != null)
            {
                BeforeQuitRoom(this, e);
            }
        }

        public void OnAfterQuitRoomEvent(RoomEventArgs e)
        {
            if (AfterQuitRoom != null)
            {
                AfterQuitRoom(this, e);
            }
        }

        public void OnSucceedQuitRoomEvent(RoomEventArgs e)
        {
            if (SucceedQuitRoom != null)
            {
                SucceedQuitRoom(this, e);
            }
        }

        public void OnFailedQuitRoomEvent(RoomEventArgs e)
        {
            if (FailedQuitRoom != null)
            {
                FailedQuitRoom(this, e);
            }
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartMatch;
        public event IEventHandler.AfterEventHandler? AfterStartMatch;
        public event IEventHandler.SucceedEventHandler? SucceedStartMatch;
        public event IEventHandler.FailedEventHandler? FailedStartMatch;

        public void OnBeforeStartMatchEvent(GeneralEventArgs e)
        {
            if (BeforeStartMatch != null)
            {
                BeforeStartMatch(this, e);
            }
        }

        public void OnAfterStartMatchEvent(GeneralEventArgs e)
        {
            if (AfterStartMatch != null)
            {
                AfterStartMatch(this, e);
            }
        }

        public void OnSucceedStartMatchEvent(GeneralEventArgs e)
        {
            if (SucceedStartMatch != null)
            {
                SucceedStartMatch(this, e);
            }
        }

        public void OnFailedStartMatchEvent(GeneralEventArgs e)
        {
            if (FailedStartMatch != null)
            {
                FailedStartMatch(this, e);
            }
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartGame;
        public event IEventHandler.AfterEventHandler? AfterStartGame;
        public event IEventHandler.SucceedEventHandler? SucceedStartGame;
        public event IEventHandler.FailedEventHandler? FailedStartGame;

        public void OnBeforeStartGameEvent(GeneralEventArgs e)
        {
            if (BeforeStartGame != null)
            {
                BeforeStartGame(this, e);
            }
        }

        public void OnAfterStartGameEvent(GeneralEventArgs e)
        {
            if (AfterStartGame != null)
            {
                AfterStartGame(this, e);
            }
        }

        public void OnSucceedStartGameEvent(GeneralEventArgs e)
        {
            if (SucceedStartGame != null)
            {
                SucceedStartGame(this, e);
            }
        }

        public void OnFailedStartGameEvent(GeneralEventArgs e)
        {
            if (FailedStartGame != null)
            {
                FailedStartGame(this, e);
            }
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenInventory;
        public event IEventHandler.AfterEventHandler? AfterOpenInventory;
        public event IEventHandler.SucceedEventHandler? SucceedOpenInventory;
        public event IEventHandler.FailedEventHandler? FailedOpenInventory;

        public void OnBeforeOpenInventoryEvent(GeneralEventArgs e)
        {
            if (BeforeOpenInventory != null)
            {
                BeforeOpenInventory(this, e);
            }
        }

        public void OnAfterOpenInventoryEvent(GeneralEventArgs e)
        {
            if (AfterOpenInventory != null)
            {
                AfterOpenInventory(this, e);
            }
        }

        public void OnSucceedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (SucceedOpenInventory != null)
            {
                SucceedOpenInventory(this, e);
            }
        }

        public void OnFailedOpenInventoryEvent(GeneralEventArgs e)
        {
            if (FailedOpenInventory != null)
            {
                FailedOpenInventory(this, e);
            }
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenStore;
        public event IEventHandler.AfterEventHandler? AfterOpenStore;
        public event IEventHandler.SucceedEventHandler? SucceedOpenStore;
        public event IEventHandler.FailedEventHandler? FailedOpenStore;

        public void OnBeforeOpenStoreEvent(GeneralEventArgs e)
        {
            if (BeforeOpenStore != null)
            {
                BeforeOpenStore(this, e);
            }
        }

        public void OnAfterOpenStoreEvent(GeneralEventArgs e)
        {
            if (AfterOpenStore != null)
            {
                AfterOpenStore(this, e);
            }
        }

        public void OnSucceedOpenStoreEvent(GeneralEventArgs e)
        {
            if (SucceedOpenStore != null)
            {
                SucceedOpenStore(this, e);
            }
        }

        public void OnFailedOpenStoreEvent(GeneralEventArgs e)
        {
            if (FailedOpenStore != null)
            {
                FailedOpenStore(this, e);
            }
        }
    }
}

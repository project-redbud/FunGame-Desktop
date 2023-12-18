using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseMain : GeneralForm, IConnectEventHandler, IDisconnectEventHandler, ILoginEventHandler, ILogoutEventHandler, IIntoRoomEventHandler, ISendTalkEventHandler,
        ICreateRoomEventHandler, IQuitRoomEventHandler, IStartMatchEventHandler, IStartGameEventHandler, IEndGameEventHandler, IOpenInventoryEventHandler, IOpenStoreEventHandler
    {
        public event IConnectEventHandler.BeforeEventHandler? BeforeConnect;
        public event IConnectEventHandler.AfterEventHandler? AfterConnect;
        public event IConnectEventHandler.SucceedEventHandler? SucceedConnect;
        public event IConnectEventHandler.FailedEventHandler? FailedConnect;

        public void OnAfterConnectEvent(object sender, ConnectEventArgs e)
        {
            AfterConnect?.Invoke(sender, e);
        }

        public void OnBeforeConnectEvent(object sender, ConnectEventArgs e)
        {
            BeforeConnect?.Invoke(sender, e);
        }

        public void OnSucceedConnectEvent(object sender, ConnectEventArgs e)
        {
            SucceedConnect?.Invoke(sender, e);
        }

        public void OnFailedConnectEvent(object sender, ConnectEventArgs e)
        {
            FailedConnect?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeDisconnect;
        public event IEventHandler.AfterEventHandler? AfterDisconnect;
        public event IEventHandler.SucceedEventHandler? SucceedDisconnect;
        public event IEventHandler.FailedEventHandler? FailedDisconnect;

        public void OnAfterDisconnectEvent(object sender, GeneralEventArgs e)
        {
            AfterDisconnect?.Invoke(sender, e);
        }

        public void OnBeforeDisconnectEvent(object sender, GeneralEventArgs e)
        {
            BeforeDisconnect?.Invoke(sender, e);
        }

        public void OnFailedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            FailedDisconnect?.Invoke(sender, e);
        }

        public void OnSucceedDisconnectEvent(object sender, GeneralEventArgs e)
        {
            SucceedDisconnect?.Invoke(sender, e);
        }

        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;
        public event ILoginEventHandler.SucceedEventHandler? SucceedLogin;
        public event ILoginEventHandler.FailedEventHandler? FailedLogin;

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e)
        {
            BeforeLogin?.Invoke(sender, e);
        }

        public void OnAfterLoginEvent(object sender, LoginEventArgs e)
        {
            AfterLogin?.Invoke(sender, e);
        }

        public void OnSucceedLoginEvent(object sender, LoginEventArgs e)
        {
            SucceedLogin?.Invoke(sender, e);
        }

        public void OnFailedLoginEvent(object sender, LoginEventArgs e)
        {
            FailedLogin?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeLogout;
        public event IEventHandler.AfterEventHandler? AfterLogout;
        public event IEventHandler.SucceedEventHandler? SucceedLogout;
        public event IEventHandler.FailedEventHandler? FailedLogout;

        public void OnAfterLogoutEvent(object sender, GeneralEventArgs e)
        {
            AfterLogout?.Invoke(sender, e);
        }

        public void OnBeforeLogoutEvent(object sender, GeneralEventArgs e)
        {
            BeforeLogout?.Invoke(sender, e);
        }

        public void OnFailedLogoutEvent(object sender, GeneralEventArgs e)
        {
            FailedLogout?.Invoke(sender, e);
        }

        public void OnSucceedLogoutEvent(object sender, GeneralEventArgs e)
        {
            SucceedLogout?.Invoke(sender, e);
        }

        public event IIntoRoomEventHandler.BeforeEventHandler? BeforeIntoRoom;
        public event IIntoRoomEventHandler.AfterEventHandler? AfterIntoRoom;
        public event IIntoRoomEventHandler.SucceedEventHandler? SucceedIntoRoom;
        public event IIntoRoomEventHandler.FailedEventHandler? FailedIntoRoom;

        public void OnBeforeIntoRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeIntoRoom?.Invoke(sender, e);
        }

        public void OnAfterIntoRoomEvent(object sender, RoomEventArgs e)
        {
            AfterIntoRoom?.Invoke(sender, e);
        }

        public void OnSucceedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            SucceedIntoRoom?.Invoke(sender, e);
        }

        public void OnFailedIntoRoomEvent(object sender, RoomEventArgs e)
        {
            FailedIntoRoom?.Invoke(sender, e);
        }

        public event ISendTalkEventHandler.BeforeEventHandler? BeforeSendTalk;
        public event ISendTalkEventHandler.AfterEventHandler? AfterSendTalk;
        public event ISendTalkEventHandler.SucceedEventHandler? SucceedSendTalk;
        public event ISendTalkEventHandler.FailedEventHandler? FailedSendTalk;

        public void OnBeforeSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            BeforeSendTalk?.Invoke(sender, e);
        }

        public void OnAfterSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            AfterSendTalk?.Invoke(sender, e);
        }

        public void OnSucceedSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            SucceedSendTalk?.Invoke(sender, e);
        }

        public void OnFailedSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            FailedSendTalk?.Invoke(sender, e);
        }

        public event ICreateRoomEventHandler.BeforeEventHandler? BeforeCreateRoom;
        public event ICreateRoomEventHandler.AfterEventHandler? AfterCreateRoom;
        public event ICreateRoomEventHandler.SucceedEventHandler? SucceedCreateRoom;
        public event ICreateRoomEventHandler.FailedEventHandler? FailedCreateRoom;

        public void OnBeforeCreateRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeCreateRoom?.Invoke(sender, e);
        }

        public void OnAfterCreateRoomEvent(object sender, RoomEventArgs e)
        {
            AfterCreateRoom?.Invoke(sender, e);
        }

        public void OnSucceedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            SucceedCreateRoom?.Invoke(sender, e);
        }

        public void OnFailedCreateRoomEvent(object sender, RoomEventArgs e)
        {
            FailedCreateRoom?.Invoke(sender, e);
        }

        public event IQuitRoomEventHandler.BeforeEventHandler? BeforeQuitRoom;
        public event IQuitRoomEventHandler.AfterEventHandler? AfterQuitRoom;
        public event IQuitRoomEventHandler.SucceedEventHandler? SucceedQuitRoom;
        public event IQuitRoomEventHandler.FailedEventHandler? FailedQuitRoom;

        public void OnBeforeQuitRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeQuitRoom?.Invoke(sender, e);
        }

        public void OnAfterQuitRoomEvent(object sender, RoomEventArgs e)
        {
            AfterQuitRoom?.Invoke(sender, e);
        }

        public void OnSucceedQuitRoomEvent(object sender, RoomEventArgs e)
        {
            SucceedQuitRoom?.Invoke(sender, e);
        }

        public void OnFailedQuitRoomEvent(object sender, RoomEventArgs e)
        {
            FailedQuitRoom?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartMatch;
        public event IEventHandler.AfterEventHandler? AfterStartMatch;
        public event IEventHandler.SucceedEventHandler? SucceedStartMatch;
        public event IEventHandler.FailedEventHandler? FailedStartMatch;

        public void OnBeforeStartMatchEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartMatch?.Invoke(sender, e);
        }

        public void OnAfterStartMatchEvent(object sender, GeneralEventArgs e)
        {
            AfterStartMatch?.Invoke(sender, e);
        }

        public void OnSucceedStartMatchEvent(object sender, GeneralEventArgs e)
        {
            SucceedStartMatch?.Invoke(sender, e);
        }

        public void OnFailedStartMatchEvent(object sender, GeneralEventArgs e)
        {
            FailedStartMatch?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartGame;
        public event IEventHandler.AfterEventHandler? AfterStartGame;
        public event IEventHandler.SucceedEventHandler? SucceedStartGame;
        public event IEventHandler.FailedEventHandler? FailedStartGame;

        public void OnBeforeStartGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartGame?.Invoke(sender, e);
        }

        public void OnAfterStartGameEvent(object sender, GeneralEventArgs e)
        {
            AfterStartGame?.Invoke(sender, e);
        }

        public void OnSucceedStartGameEvent(object sender, GeneralEventArgs e)
        {
            SucceedStartGame?.Invoke(sender, e);
        }

        public void OnFailedStartGameEvent(object sender, GeneralEventArgs e)
        {
            FailedStartGame?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenInventory;
        public event IEventHandler.AfterEventHandler? AfterOpenInventory;
        public event IEventHandler.SucceedEventHandler? SucceedOpenInventory;
        public event IEventHandler.FailedEventHandler? FailedOpenInventory;

        public void OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenInventory?.Invoke(sender, e);
        }

        public void OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenInventory?.Invoke(sender, e);
        }

        public void OnSucceedOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            SucceedOpenInventory?.Invoke(sender, e);
        }

        public void OnFailedOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            FailedOpenInventory?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenStore;
        public event IEventHandler.AfterEventHandler? AfterOpenStore;
        public event IEventHandler.SucceedEventHandler? SucceedOpenStore;
        public event IEventHandler.FailedEventHandler? FailedOpenStore;

        public void OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenStore?.Invoke(sender, e);
        }

        public void OnAfterOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenStore?.Invoke(sender, e);
        }

        public void OnSucceedOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            SucceedOpenStore?.Invoke(sender, e);
        }

        public void OnFailedOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            FailedOpenStore?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeEndGame;
        public event IEventHandler.AfterEventHandler? AfterEndGame;
        public event IEventHandler.SucceedEventHandler? SucceedEndGame;
        public event IEventHandler.FailedEventHandler? FailedEndGame;

        public void OnBeforeEndGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeEndGame?.Invoke(sender, e);
        }

        public void OnAfterEndGameEvent(object sender, GeneralEventArgs e)
        {
            AfterEndGame?.Invoke(sender, e);
        }

        public void OnSucceedEndGameEvent(object sender, GeneralEventArgs e)
        {
            SucceedEndGame?.Invoke(sender, e);
        }

        public void OnFailedEndGameEvent(object sender, GeneralEventArgs e)
        {
            FailedEndGame?.Invoke(sender, e);
        }
    }
}

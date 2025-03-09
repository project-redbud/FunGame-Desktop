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

        public void OnAfterConnectEvent(object sender, ConnectEventArgs e)
        {
            AfterConnect?.Invoke(sender, e);
        }

        public void OnBeforeConnectEvent(object sender, ConnectEventArgs e)
        {
            BeforeConnect?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeDisconnect;
        public event IEventHandler.AfterEventHandler? AfterDisconnect;

        public void OnAfterDisconnectEvent(object sender, GeneralEventArgs e)
        {
            AfterDisconnect?.Invoke(sender, e);
        }

        public void OnBeforeDisconnectEvent(object sender, GeneralEventArgs e)
        {
            BeforeDisconnect?.Invoke(sender, e);
        }

        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e)
        {
            BeforeLogin?.Invoke(sender, e);
        }

        public void OnAfterLoginEvent(object sender, LoginEventArgs e)
        {
            AfterLogin?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeLogout;
        public event IEventHandler.AfterEventHandler? AfterLogout;

        public void OnAfterLogoutEvent(object sender, GeneralEventArgs e)
        {
            AfterLogout?.Invoke(sender, e);
        }

        public void OnBeforeLogoutEvent(object sender, GeneralEventArgs e)
        {
            BeforeLogout?.Invoke(sender, e);
        }

        public event IIntoRoomEventHandler.BeforeEventHandler? BeforeIntoRoom;
        public event IIntoRoomEventHandler.AfterEventHandler? AfterIntoRoom;

        public void OnBeforeIntoRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeIntoRoom?.Invoke(sender, e);
        }

        public void OnAfterIntoRoomEvent(object sender, RoomEventArgs e)
        {
            AfterIntoRoom?.Invoke(sender, e);
        }

        public event ISendTalkEventHandler.BeforeEventHandler? BeforeSendTalk;
        public event ISendTalkEventHandler.AfterEventHandler? AfterSendTalk;

        public void OnBeforeSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            BeforeSendTalk?.Invoke(sender, e);
        }

        public void OnAfterSendTalkEvent(object sender, SendTalkEventArgs e)
        {
            AfterSendTalk?.Invoke(sender, e);
        }

        public event ICreateRoomEventHandler.BeforeEventHandler? BeforeCreateRoom;
        public event ICreateRoomEventHandler.AfterEventHandler? AfterCreateRoom;

        public void OnBeforeCreateRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeCreateRoom?.Invoke(sender, e);
        }

        public void OnAfterCreateRoomEvent(object sender, RoomEventArgs e)
        {
            AfterCreateRoom?.Invoke(sender, e);
        }

        public event IQuitRoomEventHandler.BeforeEventHandler? BeforeQuitRoom;
        public event IQuitRoomEventHandler.AfterEventHandler? AfterQuitRoom;

        public void OnBeforeQuitRoomEvent(object sender, RoomEventArgs e)
        {
            BeforeQuitRoom?.Invoke(sender, e);
        }

        public void OnAfterQuitRoomEvent(object sender, RoomEventArgs e)
        {
            AfterQuitRoom?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartMatch;
        public event IEventHandler.AfterEventHandler? AfterStartMatch;

        public void OnBeforeStartMatchEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartMatch?.Invoke(sender, e);
        }

        public void OnAfterStartMatchEvent(object sender, GeneralEventArgs e)
        {
            AfterStartMatch?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeStartGame;
        public event IEventHandler.AfterEventHandler? AfterStartGame;

        public void OnBeforeStartGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeStartGame?.Invoke(sender, e);
        }

        public void OnAfterStartGameEvent(object sender, GeneralEventArgs e)
        {
            AfterStartGame?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenInventory;
        public event IEventHandler.AfterEventHandler? AfterOpenInventory;

        public void OnBeforeOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenInventory?.Invoke(sender, e);
        }

        public void OnAfterOpenInventoryEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenInventory?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeOpenStore;
        public event IEventHandler.AfterEventHandler? AfterOpenStore;

        public void OnBeforeOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            BeforeOpenStore?.Invoke(sender, e);
        }

        public void OnAfterOpenStoreEvent(object sender, GeneralEventArgs e)
        {
            AfterOpenStore?.Invoke(sender, e);
        }

        public event IEventHandler.BeforeEventHandler? BeforeEndGame;
        public event IEventHandler.AfterEventHandler? AfterEndGame;

        public void OnBeforeEndGameEvent(object sender, GeneralEventArgs e)
        {
            BeforeEndGame?.Invoke(sender, e);
        }

        public void OnAfterEndGameEvent(object sender, GeneralEventArgs e)
        {
            AfterEndGame?.Invoke(sender, e);
        }
    }
}

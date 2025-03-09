using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseLogin : GeneralForm, ILoginEventHandler
    {
        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;

        public void OnAfterLoginEvent(object sender, LoginEventArgs e)
        {
            AfterLogin?.Invoke(sender, e);
        }

        public void OnBeforeLoginEvent(object sender, LoginEventArgs e)
        {
            BeforeLogin?.Invoke(sender, e);
        }
    }
}

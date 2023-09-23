using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseLogin : GeneralForm, ILoginEventHandler
    {
        public event ILoginEventHandler.BeforeEventHandler? BeforeLogin;
        public event ILoginEventHandler.AfterEventHandler? AfterLogin;
        public event ILoginEventHandler.SucceedEventHandler? SucceedLogin;
        public event ILoginEventHandler.FailedEventHandler? FailedLogin;

        public void OnAfterLoginEvent(LoginEventArgs e)
        {
            if (AfterLogin != null)
            {
                AfterLogin.Invoke(this, e);
            }
        }

        public void OnBeforeLoginEvent(LoginEventArgs e)
        {
            if (BeforeLogin != null)
            {
                BeforeLogin.Invoke(this, e);
            }
        }

        public void OnFailedLoginEvent(LoginEventArgs e)
        {
            if (FailedLogin != null)
            {
                FailedLogin.Invoke(this, e);
            }
        }

        public void OnSucceedLoginEvent(LoginEventArgs e)
        {
            if (SucceedLogin != null)
            {
                SucceedLogin.Invoke(this, e);
            }
        }
    }
}

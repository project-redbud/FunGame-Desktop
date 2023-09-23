using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Library.Base
{
    public class BaseReg : GeneralForm, IRegEventHandler
    {
        public event IRegEventHandler.BeforeEventHandler? BeforeReg;
        public event IRegEventHandler.AfterEventHandler? AfterReg;
        public event IRegEventHandler.SucceedEventHandler? SucceedReg;
        public event IRegEventHandler.FailedEventHandler? FailedReg;

        public void OnAfterRegEvent(RegisterEventArgs e)
        {
            if (AfterReg != null)
            {
                AfterReg(this, e);
            }
        }

        public void OnBeforeRegEvent(RegisterEventArgs e)
        {
            if (BeforeReg != null)
            {
                BeforeReg(this, e);
            }
        }

        public void OnFailedRegEvent(RegisterEventArgs e)
        {
            if (FailedReg != null)
            {
                FailedReg(this, e);
            }
        }

        public void OnSucceedRegEvent(RegisterEventArgs e)
        {
            if (SucceedReg != null)
            {
                SucceedReg(this, e);
            }
        }
    }
}

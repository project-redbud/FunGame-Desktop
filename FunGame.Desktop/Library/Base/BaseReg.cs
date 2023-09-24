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

        public void OnAfterRegEvent(object sender, RegisterEventArgs e)
        {
            AfterReg?.Invoke(sender, e);
        }

        public void OnBeforeRegEvent(object sender, RegisterEventArgs e)
        {
            BeforeReg?.Invoke(sender, e);
        }

        public void OnFailedRegEvent(object sender, RegisterEventArgs e)
        {
            FailedReg?.Invoke(sender, e);
        }

        public void OnSucceedRegEvent(object sender, RegisterEventArgs e)
        {
            SucceedReg?.Invoke(sender, e);
        }
    }
}

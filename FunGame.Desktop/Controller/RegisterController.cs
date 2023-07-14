using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Controller;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RegisterController : SocketHandlerController
    {
        private readonly Register Register;
        private readonly RegisterModel RegModel;

        public RegisterController(Register reg)
        {
            Register = reg;
            RegModel = new RegisterModel();
        }

        public override void Dispose()
        {
            RegModel.Dispose();
        }

        public async Task<bool> Reg(params object[]? objs)
        {
            bool result = false;

            try
            {
                RegisterEventArgs RegEventArgs = new (objs);
                if (Register.OnBeforeRegEvent(RegEventArgs) == EventResult.Fail) return false;

                result = await RegModel.Reg(objs);

                if (result) Register.OnSucceedRegEvent(RegEventArgs);
                else Register.OnFailedRegEvent(RegEventArgs);
                Register.OnAfterRegEvent(RegEventArgs);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            
            return result;
        }
    }
}

using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RegisterController
    {
        private readonly Register Register;

        public RegisterController(Register reg)
        {
            Register = reg;
        }

        public async Task<bool> RegAsync(string username = "", string password = "", string email = "")
        {
            bool result = false;

            try
            {
                password = password.Encrypt(username);
                RegisterEventArgs RegEventArgs = new(username, password, email);
                if (Register.OnBeforeRegEvent(RegEventArgs) == EventResult.Fail) return false;

                DataRequest request = RunTime.NewLongRunningDataRequest(DataRequestType.Reg_GetRegVerifyCode);
                request.AddRequestData("username", username);
                request.AddRequestData("password", password);
                request.AddRequestData("email", email);
                request.AddRequestData("verifycode", "");
                await request.SendRequestAsync();
                if (request.Result == RequestResult.Success)
                {
                    RegInvokeType InvokeType = request.GetResult<RegInvokeType>("type");
                    while (true)
                    {
                        string verifycode = ShowMessage.InputMessageCancel("请输入注册邮件中的6位数字验证码", "注册验证码", out MessageResult cancel);
                        if (cancel != MessageResult.Cancel)
                        {
                            request.AddRequestData("verifycode", verifycode);
                            await request.SendRequestAsync();
                            if (request.Result == RequestResult.Success)
                            {
                                bool success = request.GetResult<bool>("success");
                                string msg = request.GetResult<string>("msg") ?? "";
                                if (msg != "") ShowMessage.Message(msg, "注册结果");
                                if (success) return success;
                            }
                        }
                        else break;
                    }
                }
                request.Dispose();

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

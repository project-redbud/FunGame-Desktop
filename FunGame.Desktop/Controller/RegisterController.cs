using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

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
                RegisterEventArgs RegEventArgs = new(username, password, email);
                Register.OnBeforeRegEvent(Register, RegEventArgs);
                RunTime.PluginLoader?.OnBeforeRegEvent(Register, RegEventArgs);
                if (RegEventArgs.Cancel) return false;

                DataRequest request = RunTime.NewLongRunningDataRequest(DataRequestType.Reg_Reg);
                request.AddRequestData("username", username);
                request.AddRequestData("password", password);
                request.AddRequestData("email", email);
                request.AddRequestData("verifycode", "");
                await request.SendRequestAsync();
                if (request.Result == RequestResult.Success)
                {
                    RegInvokeType InvokeType = request.GetResult<RegInvokeType>("type");
                    switch (InvokeType)
                    {
                        case RegInvokeType.InputVerifyCode:
                            {
                                while (!result)
                                {
                                    string verifycode = ShowMessage.InputMessageCancel("请输入注册邮件中的6位数字验证码", "注册验证码", out MessageResult cancel);
                                    if (cancel != MessageResult.Cancel)
                                    {
                                        request.AddRequestData("verifycode", verifycode);
                                        await request.SendRequestAsync();
                                        if (request.Result == RequestResult.Success)
                                        {
                                            result = request.GetResult<bool>("success");
                                            string msg = request.GetResult<string>("msg") ?? "";
                                            if (msg != "") ShowMessage.Message(msg, "注册结果");
                                        }
                                    }
                                    else break;
                                }
                                break;
                            }
                        case RegInvokeType.DuplicateUserName:
                            {
                                result = request.GetResult<bool>("success");
                                string msg = request.GetResult<string>("msg") ?? "";
                                ShowMessage.Message(msg, "注册结果");
                                break;
                            }
                        case RegInvokeType.DuplicateEmail:
                            {
                                result = request.GetResult<bool>("success");
                                string msg = request.GetResult<string>("msg") ?? "";
                                ShowMessage.Message(msg, "注册结果");
                                break;
                            }
                    }
                }
                request.Dispose();

                RegEventArgs.Success = result;
                Register.OnAfterRegEvent(Register, RegEventArgs);
                RunTime.PluginLoader?.OnAfterRegEvent(Register, RegEventArgs);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
        }
    }
}

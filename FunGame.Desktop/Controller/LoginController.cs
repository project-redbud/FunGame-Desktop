using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.SQLScript.Common;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController
    {
        private readonly GeneralForm UIForm;
        private readonly DataRequest LoginRequest;
        private readonly DataRequest ForgetPasswordRequest;
        private readonly DataRequest UpdatePasswordRequest;

        public LoginController(GeneralForm form)
        {
            UIForm = form;
            LoginRequest = RunTime.NewLongRunningDataRequest(DataRequestType.RunTime_Login);
            ForgetPasswordRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Login_GetFindPasswordVerifyCode);
            UpdatePasswordRequest = RunTime.NewLongRunningDataRequest(DataRequestType.Login_UpdatePassword);
        }

        public void Dispose()
        {
            LoginRequest.Dispose();
            ForgetPasswordRequest.Dispose();
            UpdatePasswordRequest.Dispose();
        }

        public async Task<bool> LoginAccountAsync(string username, string password, string autokey = "")
        {
            bool result = false;
            string msg = "";
            LoginEventArgs args = new(username, password, autokey);

            try
            {
                if (OnBeforeLoginEvent(args))
                {
                    LoginRequest.AddRequestData("username", username);
                    LoginRequest.AddRequestData("password", password);
                    LoginRequest.AddRequestData("autokey", autokey);
                    await LoginRequest.SendRequestAsync();
                    if (LoginRequest.Result == RequestResult.Success)
                    {
                        Guid key = LoginRequest.GetResult<Guid>("checkloginkey");
                        msg = LoginRequest.GetResult<string>("msg") ?? "";
                        if (msg != "")
                        {
                            UIForm.ShowMessage(ShowMessageType.Error, msg);
                        }
                        else if (key != Guid.Empty)
                        {
                            LoginRequest.AddRequestData("checkloginkey", key);
                            await LoginRequest.SendRequestAsync();
                            if (LoginRequest.Result == RequestResult.Success)
                            {
                                msg = LoginRequest.GetResult<string>("msg") ?? "";
                                if (msg != "")
                                {
                                    UIForm.ShowMessage(ShowMessageType.Error, msg);
                                }
                                else
                                {
                                    User user = LoginRequest.GetResult<User>("user") ?? Factory.GetUser();
                                    if (user.Id != 0)
                                    {
                                        // 创建User对象并返回到Main
                                        RunTime.Main?.UpdateUI(MainInvokeType.SetUser, user);
                                        result = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            if (!result && msg == "")
            {
                UIForm.ShowMessage(ShowMessageType.Error, "登录失败！");
            }

            OnAfterLoginEvent(result, args);

            return result;
        }

        public async Task<string> ForgetPassword_CheckVerifyCodeAsync(string username, string email, string verifycode)
        {
            string msg = "无法找回您的密码，请稍后再试。";

            try
            {
                ForgetPasswordRequest.AddRequestData(ForgetVerifyCodes.Column_Username, username);
                ForgetPasswordRequest.AddRequestData(ForgetVerifyCodes.Column_Email, email);
                if (verifycode.Trim() == "")
                {
                    // 未发送verifycode，说明需要系统生成一个验证码
                    ForgetPasswordRequest.AddRequestData(ForgetVerifyCodes.Column_ForgetVerifyCode, "");
                    await ForgetPasswordRequest.SendRequestAsync();
                    if (ForgetPasswordRequest.Result == RequestResult.Success)
                    {
                        msg = ForgetPasswordRequest.GetResult<string>("msg") ?? msg;
                    }
                    else RunTime.WritelnSystemInfo(ForgetPasswordRequest.Error);
                }
                else
                {
                    // 发送verifycode，需要验证
                    ForgetPasswordRequest.AddRequestData(ForgetVerifyCodes.Column_ForgetVerifyCode, verifycode);
                    await ForgetPasswordRequest.SendRequestAsync();
                    if (ForgetPasswordRequest.Result == RequestResult.Success)
                    {
                        msg = ForgetPasswordRequest.GetResult<string>("msg") ?? msg;
                    }
                    else RunTime.WritelnSystemInfo(ForgetPasswordRequest.Error);
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return msg;
        }

        public async Task<string> ForgetPassword_UpdatePasswordAsync(string username, string password)
        {
            string msg = "无法更新您的密码，请稍后再试。";

            try
            {
                UpdatePasswordRequest.AddRequestData(UserQuery.Column_Username, username);
                UpdatePasswordRequest.AddRequestData(UserQuery.Column_Password, password.Encrypt(username));
                await UpdatePasswordRequest.SendRequestAsync();
                if (UpdatePasswordRequest.Result == RequestResult.Success)
                {
                    msg = UpdatePasswordRequest.GetResult<string>("msg") ?? msg;
                }
                else RunTime.WritelnSystemInfo(UpdatePasswordRequest.Error);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return msg;
        }

        private bool OnBeforeLoginEvent(LoginEventArgs LoginEventArgs)
        {
            if (UIForm.GetType() == typeof(Login))
            {
                return ((Login)UIForm).OnBeforeLoginEvent(LoginEventArgs) == EventResult.Success;
            }
            return true;
        }

        private void OnAfterLoginEvent(bool result, LoginEventArgs LoginEventArgs)
        {
            try
            {
                if (UIForm.GetType() == typeof(Login))
                {
                    Login login = (Login)UIForm;
                    if (result) login.OnSucceedLoginEvent(LoginEventArgs);
                    else login.OnFailedLoginEvent(LoginEventArgs);
                    login.OnAfterLoginEvent(LoginEventArgs);
                }
                else if (UIForm.GetType() == typeof(Main))
                {
                    if (result) ((Main)UIForm).OnSucceedLoginEvent(LoginEventArgs);
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }
    }
}

using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.SQLScript.Common;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController
    {
        private readonly GeneralForm UIForm;

        public LoginController(GeneralForm form)
        {
            UIForm = form;
        }

        public async Task<bool> LoginAccountAsync(string username, string password, string autokey = "", bool encrypt = true)
        {
            bool result = false;
            string msg = "";

            try
            {
                if (encrypt) password = password.Encrypt(username);
                LoginEventArgs args = new(username, password, autokey);

                if (OnBeforeLoginEvent(args))
                {
                    DataRequest request = RunTime.NewLongRunningDataRequest(DataRequestType.Login_Login);
                    request.AddRequestData("username", username);
                    request.AddRequestData("password", password);
                    request.AddRequestData("autokey", autokey);
                    request.AddRequestData("key", Guid.Empty);
                    await request.SendRequestAsync();
                    if (request.Result == RequestResult.Success)
                    {
                        Guid key = request.GetResult<Guid>("key");
                        msg = request.GetResult<string>("msg") ?? "";
                        if (msg != "")
                        {
                            UIForm.ShowMessage(ShowMessageType.Error, msg);
                        }
                        else if (key != Guid.Empty)
                        {
                            request.AddRequestData("key", key);
                            await request.SendRequestAsync();
                            if (request.Result == RequestResult.Success)
                            {
                                msg = request.GetResult<string>("msg") ?? "";
                                if (msg != "")
                                {
                                    UIForm.ShowMessage(ShowMessageType.Error, msg);
                                }
                                else
                                {
                                    User user = request.GetResult<User>("user") ?? Factory.GetUser();
                                    if (user.Id != 0)
                                    {
                                        // 创建User对象并返回到Main
                                        args.Username = user.Username;
                                        RunTime.Session.LoginKey = key;
                                        RunTime.Main?.UpdateUI(MainInvokeType.SetUser, user);
                                        result = true;
                                    }
                                }
                            }
                        }
                    }
                    request.Dispose();
                }

                if (!result && msg == "")
                {
                    UIForm.ShowMessage(ShowMessageType.Error, "登录失败！");
                }

                OnAfterLoginEvent(result, args);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
        }

        public static async Task<string> ForgetPassword_CheckVerifyCodeAsync(string username, string email, string verifycode)
        {
            string msg = "无法找回您的密码，请稍后再试。";

            try
            {
                DataRequest request = RunTime.NewLongRunningDataRequest(DataRequestType.Login_GetFindPasswordVerifyCode);
                request.AddRequestData(ForgetVerifyCodes.Column_Username, username);
                request.AddRequestData(ForgetVerifyCodes.Column_Email, email);
                if (verifycode.Trim() == "")
                {
                    // 未发送verifycode，说明需要系统生成一个验证码
                    request.AddRequestData(ForgetVerifyCodes.Column_ForgetVerifyCode, "");
                    await request.SendRequestAsync();
                    if (request.Result == RequestResult.Success)
                    {
                        msg = request.GetResult<string>("msg") ?? msg;
                    }
                    else RunTime.WritelnSystemInfo(request.Error);
                }
                else
                {
                    // 发送verifycode，需要验证
                    request.AddRequestData(ForgetVerifyCodes.Column_ForgetVerifyCode, verifycode);
                    await request.SendRequestAsync();
                    if (request.Result == RequestResult.Success)
                    {
                        msg = request.GetResult<string>("msg") ?? msg;
                    }
                    else RunTime.WritelnSystemInfo(request.Error);
                }
                request.Dispose();
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return msg;
        }

        public static async Task<string> ForgetPassword_UpdatePasswordAsync(string username, string password)
        {
            string msg = "无法更新您的密码，请稍后再试。";

            try
            {
                DataRequest request = RunTime.NewLongRunningDataRequest(DataRequestType.Login_UpdatePassword);
                request.AddRequestData(UserQuery.Column_Username, username);
                request.AddRequestData(UserQuery.Column_Password, password.Encrypt(username));
                await request.SendRequestAsync();
                if (request.Result == RequestResult.Success)
                {
                    msg = request.GetResult<string>("msg") ?? msg;
                }
                else RunTime.WritelnSystemInfo(request.Error);
                request.Dispose();
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
                ((Login)UIForm).OnBeforeLoginEvent(UIForm, LoginEventArgs);
                return !LoginEventArgs.Cancel;
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
                    if (result) login.OnSucceedLoginEvent(UIForm, LoginEventArgs);
                    else login.OnFailedLoginEvent(UIForm, LoginEventArgs);
                    login.OnAfterLoginEvent(UIForm, LoginEventArgs);
                }
                else if (UIForm.GetType() == typeof(Main))
                {
                    if (result) ((Main)UIForm).OnSucceedLoginEvent(UIForm, LoginEventArgs);
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }
    }
}

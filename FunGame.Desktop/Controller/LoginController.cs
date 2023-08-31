using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Controller;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.SQLScript.Common;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
using Milimoe.FunGame.Desktop.Library.Component;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class LoginController
    {
        public static async Task<bool> LoginAccountAsync(string username, string password, string autokey = "")
        {
            bool result = false;

            try
            {
                DataRequest request = RunTime.NewDataRequest(DataRequestType.RunTime_Login);
                request.AddRequestData("username", username);
                request.AddRequestData("password", password);
                request.AddRequestData("autokey", autokey);
                await request.SendRequestAsync();
                if (request.Result == RequestResult.Success)
                {
                    Guid key = request.GetResult<Guid>("checkloginkey");
                    string msg = request.GetResult<string>("msg") ?? "";
                    if (msg != "")
                    {
                        ShowMessage.ErrorMessage(msg);
                    }
                    else if (key != Guid.Empty)
                    {
                        request.AddRequestData("checkloginkey", key);
                        await request.SendRequestAsync();
                        if (request.Result == RequestResult.Success)
                        {
                            msg = request.GetResult<string>("msg") ?? "";
                            if (msg != "")
                            {
                                ShowMessage.ErrorMessage(msg);
                            }
                            else
                            {
                                User user = request.GetResult<User>("user") ?? Factory.GetUser();
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
                DataRequest request = RunTime.NewDataRequest(DataRequestType.Login_GetFindPasswordVerifyCode);
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
                DataRequest request = RunTime.NewDataRequest(DataRequestType.Login_UpdatePassword);
                request.AddRequestData(UserQuery.Column_Username, username);
                request.AddRequestData(UserQuery.Column_Password, password.Encrypt(username));
                await request.SendRequestAsync();
                if (request.Result == RequestResult.Success)
                {
                    msg = request.GetResult<string>("msg") ?? msg;
                }
                else RunTime.WritelnSystemInfo(request.Error);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return msg;
        }

        public static bool LoginAccount(string username, string password, string autokey = "")
        {
            bool result = false;

            try
            {
                DataRequest request = RunTime.NewDataRequest(DataRequestType.RunTime_Login);
                request.AddRequestData("username", username);
                request.AddRequestData("password", password);
                request.AddRequestData("autokey", autokey);
                request.SendRequestAsync();
                if (request.Result == RequestResult.Success)
                {
                    Guid key = request.GetResult<Guid>("checkloginkey");
                    string msg = request.GetResult<string>("msg") ?? "";
                    if (msg != "")
                    {
                        ShowMessage.ErrorMessage(msg);
                    }
                    else if (key != Guid.Empty)
                    {
                        request.AddRequestData("checkloginkey", key);
                        request.SendRequestAsync();
                        if (request.Result == RequestResult.Success)
                        {
                            msg = request.GetResult<string>("msg") ?? "";
                            if (msg != "")
                            {
                                ShowMessage.ErrorMessage(msg);
                            }
                            else
                            {
                                User user = request.GetResult<User>("user") ?? Factory.GetUser();
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
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return result;
        }

        public static string ForgetPassword_CheckVerifyCode(string username, string email, string verifycode)
        {
            string msg = "无法找回您的密码，请稍后再试。";

            try
            {
                DataRequest request = RunTime.NewDataRequest(DataRequestType.Login_GetFindPasswordVerifyCode);
                request.AddRequestData(ForgetVerifyCodes.Column_Username, username);
                request.AddRequestData(ForgetVerifyCodes.Column_Email, email);
                if (verifycode.Trim() == "")
                {
                    // 未发送verifycode，说明需要系统生成一个验证码
                    request.AddRequestData(ForgetVerifyCodes.Column_ForgetVerifyCode, "");
                    request.SendRequest();
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
                    request.SendRequest();
                    if (request.Result == RequestResult.Success)
                    {
                        msg = request.GetResult<string>("msg") ?? msg;
                    }
                    else RunTime.WritelnSystemInfo(request.Error);
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return msg;
        }

        public static string ForgetPassword_UpdatePassword(string username, string password)
        {
            string msg = "无法更新您的密码，请稍后再试。";

            try
            {
                DataRequest request = RunTime.NewDataRequest(DataRequestType.Login_UpdatePassword);
                request.AddRequestData(UserQuery.Column_Username, username);
                request.AddRequestData(UserQuery.Column_Password, password.Encrypt(username));
                request.SendRequest();
                if (request.Result == RequestResult.Success)
                {
                    msg = request.GetResult<string>("msg") ?? msg;
                }
                else RunTime.WritelnSystemInfo(request.Error);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return msg;
        }
    }
}

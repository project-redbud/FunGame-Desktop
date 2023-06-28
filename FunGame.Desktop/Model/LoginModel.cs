using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Architecture;
using Milimoe.FunGame.Core.Library.Common.Network;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.SQLScript.Common;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.Model
{
    /// <summary>
    /// 请不要越过Controller直接调用Model中的方法。
    /// </summary>
    public class LoginModel : BaseModel
    {
        private static new SocketObject Work;
        private static new bool Working = false;

        public LoginModel() : base(RunTime.Socket)
        {

        }

        public override void SocketHandler(SocketObject SocketObject)
        {
            try
            {
                if (SocketObject.SocketType == SocketMessageType.RunTime_Login || SocketObject.SocketType == SocketMessageType.RunTime_CheckLogin)
                {
                    Work = SocketObject;
                    Working = false;
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }

        public static async Task<bool> LoginAccountAsync(params object[]? objs)
        {
            try
            {
                Socket? Socket = RunTime.Socket;
                if (Socket != null && objs != null)
                {
                    string username = "";
                    string password = "";
                    string autokey = "";
                    if (objs.Length > 0) username = (string)objs[0];
                    if (objs.Length > 1) password = (string)objs[1];
                    if (objs.Length > 2) autokey = (string)objs[2];
                    password = password.Encrypt(username);
                    SetWorking();
                    if (Socket.Send(SocketMessageType.RunTime_Login, username, password, autokey) == SocketResult.Success)
                    {
                        string ErrorMsg = "";
                        Guid CheckLoginKey = Guid.Empty;
                        (CheckLoginKey, ErrorMsg) = await Task.Factory.StartNew(GetCheckLoginKeyAsync);
                        if (ErrorMsg != null && ErrorMsg.Trim() != "")
                        {
                            ShowMessage.ErrorMessage(ErrorMsg, "登录失败");
                            return false;
                        }
                        SetWorking();
                        if (Socket.Send(SocketMessageType.RunTime_CheckLogin, CheckLoginKey) == SocketResult.Success)
                        {
                            User user = await Task.Factory.StartNew(GetLoginUserAsync);
                            // 创建User对象并返回到Main
                            RunTime.Main?.UpdateUI(MainInvokeType.SetUser, user);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }

            return false;
        }

        public static (Guid, string) GetCheckLoginKeyAsync()
        {
            Guid key = Guid.Empty;
            string? msg = "";
            try
            {
                WaitForWorkDone();
                // 返回一个确认登录的Key
                if (Work.Length > 0) key = Work.GetParam<Guid>(0);
                if (Work.Length > 1) msg = Work.GetParam<string>(1);
                if (key != Guid.Empty)
                {
                    RunTime.Session.LoginKey = key;
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            msg ??= "";
            return (key, msg);
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

        private static User GetLoginUserAsync()
        {
            User user = General.UnknownUserInstance;
            try
            {
                WaitForWorkDone();
                // 返回构造User对象的DataSet
                if (Work.Length > 0) user = Work.GetParam<User>(0) ?? General.UnknownUserInstance;
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
            return user;
        }

        private static new void SetWorking()
        {
            Working = true;
            Work = default;
        }

        private static new void WaitForWorkDone()
        {
            while (true)
            {
                if (!Working) break;
            }
        }
    }
}

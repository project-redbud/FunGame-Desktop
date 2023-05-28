using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Core.Library.SQLScript.Common;
using Milimoe.FunGame.Core.Library.SQLScript.Entity;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Component;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class ForgetPassword
    {
        public ForgetPassword()
        {
            InitializeComponent();
        }

        protected override void BindEvent()
        {
            base.BindEvent();
        }

        private void FindPassword_Click(object sender, EventArgs e)
        {
            if (RunTime.Socket != null)
            {
                string username = UsernameText.Text.Trim();
                string email = EmailText.Text.Trim();
                if (username == "" || email == "")
                {
                    ShowMessage.ErrorMessage("账号或邮箱不能为空！");
                    UsernameText.Focus();
                    return;
                }

                string msg = "";
                bool success = false;

                try
                {
                    DataRequest request = RunTime.NewDataRequest(DataRequestType.GetFindPasswordVerifyCode);
                    request.AddRequestData(UserQuery.Column_Username, username);
                    request.AddRequestData(UserQuery.Column_Email, email);
                    request.AddRequestData(ForgetVerifyCodes.Column_ForgetVerifyCode, "");
                    request.SendRequest();
                    if (request.Result == RequestResult.Success)
                    {
                        msg = request.GetResult<string>("msg") ?? "";
                        if (msg.Trim() != "")
                        {
                            // 如果返回一个信息，则停止找回密码
                            ShowMessage.ErrorMessage(msg);
                        }
                        else
                        {
                            while (!success)
                            {
                                request[ForgetVerifyCodes.Column_ForgetVerifyCode] = "";
                                string verifycode = ShowMessage.InputMessageCancel("请输入找回密码邮件中的6位数字验证码", "注册验证码", out MessageResult result);
                                if (result != MessageResult.Cancel)
                                {
                                    if (verifycode.Trim() != "")
                                    {
                                        request[ForgetVerifyCodes.Column_ForgetVerifyCode] = verifycode;
                                        request.SendRequest();
                                        if (request.Result == RequestResult.Success)
                                        {
                                            msg = request.GetResult<string>("msg") ?? "";
                                            if (msg.Trim() != "")
                                            {
                                                ShowMessage.ErrorMessage(msg);
                                            }
                                            else
                                            {
                                                success = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            RunTime.WritelnSystemInfo(request.Error);
                                        }
                                    }
                                    else
                                    {
                                        ShowMessage.WarningMessage("不能输入空值！");
                                    }
                                }
                                else break;
                            }
                            if (success)
                            {
                                bool checkpass = true;
                                while (checkpass)
                                {
                                    string newpass = ShowMessage.InputMessageCancel("请输入新密码", "新密码", out MessageResult result);
                                    if (newpass.Trim() != "")
                                    {
                                        if (newpass.Length < 6 || newpass.Length > 15) // 字节范围 3~12
                                        {
                                            ShowMessage.ErrorMessage("密码长度不符合要求：6~15个字符数");
                                        }
                                        else checkpass = false;
                                    }
                                }
                                // TODO. 等更新UpdatePassword
                            }
                        }
                    }
                    else
                    {
                        RunTime.WritelnSystemInfo(request.Error);
                    }
                }
                catch (Exception ex)
                {
                    RunTime.WritelnSystemInfo(ex.GetErrorInfo());
                }
            }
        }
    }
}

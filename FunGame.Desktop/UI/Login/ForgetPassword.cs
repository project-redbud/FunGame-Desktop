using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
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

                string msg;
                bool success = false;

                try
                {
                    // 发送找回密码请求
                    msg = LoginController.ForgetPassword_CheckVerifyCode(username, email);

                    if (msg.Trim() != "")
                    {
                        // 如果返回一个信息，则停止找回密码
                        ShowMessage.ErrorMessage(msg);
                    }
                    else
                    {
                        while (!success)
                        {
                            string verifycode = ShowMessage.InputMessageCancel("请输入找回密码邮件中的6位数字验证码", "注册验证码", out MessageResult result);
                            if (result != MessageResult.Cancel)
                            {
                                if (verifycode.Trim() != "")
                                {
                                    msg = LoginController.ForgetPassword_CheckVerifyCode(username, email, verifycode);
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
                                    ShowMessage.WarningMessage("不能输入空值！");
                                }
                            }
                            else break;
                        }
                        if (success)
                        {
                            while (true)
                            {
                                string newpass = ShowMessage.InputMessageCancel("请输入新密码", "设置新密码", out MessageResult result);
                                if (result != MessageResult.Cancel)
                                {
                                    if (newpass.Trim() != "")
                                    {
                                        if (newpass.Length < 6 || newpass.Length > 15) // 字节范围 3~12
                                        {
                                            ShowMessage.ErrorMessage("密码长度不符合要求：6~15个字符数");
                                        }
                                        else
                                        {
                                            msg = LoginController.ForgetPassword_UpdatePassword(username, newpass);
                                            if (msg.Trim() != "")
                                            {
                                                ShowMessage.ErrorMessage(msg);
                                            }
                                            else
                                            {
                                                ShowMessage.Message("密码更新成功！请您牢记新的密码。", "找回密码");
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (ShowMessage.OKCancelMessage("确定放弃设置新密码吗？", "找回密码") == MessageResult.OK)
                                    {
                                        success = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (success)
                        {
                            Dispose();
                        }
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

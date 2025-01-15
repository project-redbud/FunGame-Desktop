using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class ForgetPassword
    {
        public ForgetPassword()
        {
            InitializeComponent();
        }

        private async void FindPassword_Click(object sender, EventArgs e)
        {
            if (RunTime.Socket == null) return;

            string username = UsernameText.Text.Trim();
            string email = EmailText.Text.Trim();

            if (!ValidateInput(username, email)) return;

            try
            {
                if (!await RequestVerificationCodeAsync(username, email)) return;

                string newPassword = await GetNewPasswordAsync(username, email);

                if (!string.IsNullOrEmpty(newPassword) && await UpdatePasswordAsync(username, newPassword))
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                RunTime.WritelnSystemInfo(ex.GetErrorInfo());
            }
        }

        private bool ValidateInput(string username, string email)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                ShowMessage(ShowMessageType.Error, "账号或邮箱不能为空！");
                InvokeUpdateUI(() => UsernameText.Focus());
                return false;
            }
            return true;
        }

        private async Task<bool> RequestVerificationCodeAsync(string username, string email)
        {
            string msg = await LoginController.ForgetPassword_CheckVerifyCodeAsync(username, email, "");
            if (!string.IsNullOrEmpty(msg))
            {
                ShowMessage(ShowMessageType.Error, msg);
                return false;
            }
            return true;
        }

        private async Task<string> GetNewPasswordAsync(string username, string email)
        {
            bool success = false;
            do
            {
                string verifycode = ShowInputMessageCancel("请输入找回密码邮件中的6位数字验证码", "注册验证码", out MessageResult result);
                if (result == MessageResult.Cancel)
                {
                    break;
                }

                if (string.IsNullOrEmpty(verifycode))
                {
                    ShowMessage(ShowMessageType.Warning, "不能输入空值！");
                    continue;
                }

                string msg = await LoginController.ForgetPassword_CheckVerifyCodeAsync(username, email, verifycode);
                if (!string.IsNullOrEmpty(msg))
                {
                    ShowMessage(ShowMessageType.Error, msg);
                }
                else
                {
                    success = true;
                }
            } while (!success);

            if (!success) return "";

            string newPassword;
            success = false;
            do
            {
                newPassword = ShowInputMessageCancel("请输入新密码", "设置新密码", out MessageResult result);
                if (result == MessageResult.Cancel)
                {
                    if (ShowMessage(ShowMessageType.OKCancel, "确定放弃设置新密码吗？", "找回密码") == MessageResult.OK)
                    {
                        break;
                    }
                    continue;
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    continue;
                }

                if (newPassword.Length < 6 || newPassword.Length > 15)
                {
                    ShowMessage(ShowMessageType.Error, "密码长度不符合要求：6~15个字符数");
                    continue;
                }
                success = true;
            } while (!success);

            if (!success) return "";

            return newPassword;
        }

        private async Task<bool> UpdatePasswordAsync(string username, string newPassword)
        {
            string msg = await LoginController.ForgetPassword_UpdatePasswordAsync(username, newPassword);
            if (!string.IsNullOrEmpty(msg))
            {
                ShowMessage(ShowMessageType.Error, msg);
            }
            else
            {
                ShowMessage(ShowMessageType.General, "密码更新成功！请您牢记新的密码。", "找回密码");
                return true;
            }
            return false;
        }
    }
}

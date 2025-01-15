using System.ComponentModel;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library.Base;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Register : BaseReg
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CheckReg { get; set; } = false;

        private readonly RegisterController RegController;
        private readonly LoginController LoginController;

        public Register()
        {
            InitializeComponent();
            RegController = new(this);
            LoginController = new(this);
        }

        protected override void BindEvent()
        {
            base.BindEvent();
            SucceedReg += SucceedRegEvent;
        }

        private async Task<bool> Reg_HandlerAsync(string username, string password, string email)
        {
            try
            {
                return await RegController.RegAsync(username, password, email);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private async void SucceedRegEvent(object sender, GeneralEventArgs e)
        {
            string username = ((RegisterEventArgs)e).Username;
            string password = ((RegisterEventArgs)e).Password;
            await LoginController.LoginAccountAsync(username, password, encrypt: false);
            RunTime.Login?.Dispose();
        }

        private async void RegButton_Click(object sender, EventArgs e)
        {
            RegButton.Enabled = false;

            string username = UsernameText.Text.Trim();
            string password = PasswordText.Text.Trim();
            string checkpassword = CheckPasswordText.Text.Trim();
            string email = EmailText.Text.Trim();

            if (!ValidateInput(username, password, checkpassword, email))
            {
                RegButton.Enabled = true;
                return;
            }

            bool result = await Reg_HandlerAsync(username, password, email);

            if (!result)
            {
                RegButton.Enabled = true;
            }
            else
            {
                Dispose();
            }
        }

        private bool ValidateInput(string username, string password, string checkpassword, string email)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(checkpassword))
            {
                ShowMessage(ShowMessageType.Error, "请将账号和密码填写完整！");
                UsernameText.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                ShowMessage(ShowMessageType.Error, "邮箱不能为空！");
                EmailText.Focus();
                return false;
            }

            if (!NetworkUtility.IsUserName(username))
            {
                ShowMessage(ShowMessageType.Error, "账号名不符合要求：不能包含特殊字符");
                UsernameText.Focus();
                return false;
            }

            int usernameLength = NetworkUtility.GetUserNameLength(username);
            if (usernameLength < 3 || usernameLength > 12)
            {
                ShowMessage(ShowMessageType.Error, "账号名长度不符合要求：3~12个字符数（一个中文2个字符）");
                UsernameText.Focus();
                return false;
            }

            if (password != checkpassword)
            {
                ShowMessage(ShowMessageType.Error, "两个密码不相同，请重新输入！");
                CheckPasswordText.Focus();
                return false;
            }

            if (password.Length < 6 || password.Length > 15)
            {
                ShowMessage(ShowMessageType.Error, "密码长度不符合要求：6~15个字符数");
                PasswordText.Focus();
                return false;
            }

            if (!NetworkUtility.IsEmail(email))
            {
                ShowMessage(ShowMessageType.Error, "这不是一个邮箱地址！");
                EmailText.Focus();
                return false;
            }
            return true;
        }

        private void GoToLogin_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}

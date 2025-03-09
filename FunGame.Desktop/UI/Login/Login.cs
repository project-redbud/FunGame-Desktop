using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library.Base;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.Utility;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class Login : BaseLogin
    {
        private readonly LoginController LoginController;

        public Login()
        {
            InitializeComponent();
            LoginController = new(this);
        }

        protected override void BindEvent()
        {
            base.BindEvent();
            BeforeLogin += BeforeLoginEvent;
            AfterLogin += AfterLoginEvent;
        }

        private async Task<bool> Login_HandlerAsync(string username, string password)
        {
            try
            {
                return await LoginController.LoginAccountAsync(username, password);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            OpenForm.SingleForm(FormType.Register, OpenFormType.Dialog);
        }

        private void FastLogin_Click(object sender, EventArgs e)
        {
            ShowMessage(ShowMessageType.Tip, "与No.16对话即可获得快速登录秘钥，快去试试吧！");
        }

        private async void GoToLogin_Click(object sender, EventArgs e)
        {
            GoToLogin.Enabled = false;
            string username = UsernameText.Text.Trim();
            string password = PasswordText.Text.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage(ShowMessageType.Error, "账号或密码不能为空！");
                UsernameText.Focus();
                GoToLogin.Enabled = true;
                return;
            }
            bool result = await Login_HandlerAsync(username, password);
            if (result)
            {
                Dispose();
            }
            else
            {
                GoToLogin.Enabled = true;
            }
        }


        private void ForgetPassword_Click(object sender, EventArgs e)
        {
            OpenForm.SingleForm(FormType.ForgetPassword, OpenFormType.Dialog);
            UsernameText.Focus();
        }

        private void AfterLoginEvent(object sender, LoginEventArgs e)
        {
            if (!e.Success)
            {
                UpdateFailedLoginUI();
            }
            RunTime.Main?.OnAfterLoginEvent(sender, e);
            RunTime.PluginLoader?.OnAfterLoginEvent(sender, e);
        }

        private void UpdateFailedLoginUI()
        {
            InvokeUpdateUI(() =>
            {
                GoToLogin.Enabled = true;
            });
        }

        private void BeforeLoginEvent(object sender, LoginEventArgs e)
        {
            RunTime.Main?.OnBeforeLoginEvent(sender, e);
            RunTime.PluginLoader?.OnBeforeLoginEvent(sender, e);
            if (e.Cancel) return;
        }
    }
}

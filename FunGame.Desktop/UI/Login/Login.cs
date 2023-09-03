using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Library.Base;
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
            Disposed += Login_Disposed;
            BeforeLogin += BeforeLoginEvent;
            AfterLogin += AfterLoginEvent;
            FailedLogin += FailedLoginEvent;
            SucceedLogin += SucceedLoginEvent;
        }

        private async Task<bool> Login_Handler()
        {
            try
            {
                string username = UsernameText.Text.Trim();
                string password = PasswordText.Text.Trim();
                if (username == "" || password == "")
                {
                    ShowMessage(ShowMessageType.Error, "账号或密码不能为空！");
                    UsernameText.Focus();
                    return false;
                }
                return await LoginController.LoginAccountAsync(username, password);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
        }

        /// <summary>
        /// 打开注册界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegButton_Click(object sender, EventArgs e)
        {
            OpenForm.SingleForm(FormType.Register, OpenFormType.Dialog);
        }

        private void FastLogin_Click(object sender, EventArgs e)
        {
            ShowMessage(ShowMessageType.Tip, "与No.16对话即可获得快速登录秘钥，快去试试吧！");
        }

        private void GoToLogin_Click(object sender, EventArgs e)
        {
            GoToLogin.Enabled = false;
            bool result = false;
            TaskUtility.StartAndAwaitTask(async () =>
            {
                result = await Login_Handler();
            }).OnCompleted(() =>
            {
                if (result) Dispose();
                else GoToLogin.Enabled = true;
            });
        }
        private void Login_Disposed(object? sender, EventArgs e)
        {
            LoginController.Dispose();
        }


        private void ForgetPassword_Click(object sender, EventArgs e)
        {
            OpenForm.SingleForm(FormType.ForgetPassword, OpenFormType.Dialog);
            UsernameText.Focus();
        }

        public EventResult FailedLoginEvent(object sender, LoginEventArgs e)
        {
            GoToLogin.Enabled = true;
            RunTime.Main?.OnFailedLoginEvent(e);
            return EventResult.Success;
        }

        private EventResult SucceedLoginEvent(object sender, LoginEventArgs e)
        {
            RunTime.Main?.OnSucceedLoginEvent(e);
            return EventResult.Success;
        }

        private EventResult BeforeLoginEvent(object sender, LoginEventArgs e)
        {
            if (RunTime.Main?.OnBeforeLoginEvent(e) == EventResult.Fail) return EventResult.Fail;
            return EventResult.Success;
        }

        private EventResult AfterLoginEvent(object sender, LoginEventArgs e)
        {
            RunTime.Main?.OnAfterLoginEvent(e);
            return EventResult.Success;
        }
    }
}

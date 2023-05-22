using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;
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

                bool finding = false;

                DataRequest request = RunTime.NewDataRequest(DataRequestType.GetFindPasswordVerifyCode);
                request.AddRequestData(UserQuery.Column_Username, username);
                request.AddRequestData(UserQuery.Column_Email, email);
                request.SendRequest();
                if (request.Success)
                {
                    finding = request.GetResult<bool>("finding");
                    ShowMessage.TipMessage(finding.ToString());
                }
                else
                {
                    RunTime.WritelnSystemInfo(request.Error);
                }
            }
        }
    }
}

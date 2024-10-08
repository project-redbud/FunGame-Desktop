﻿using Milimoe.FunGame.Core.Api.Utility;
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

        private async Task<bool> Reg_Handler()
        {
            try
            {
                string username = "";
                string password = "";
                string checkpassword = "";
                string email = "";
                InvokeUpdateUI(() =>
                {
                    username = UsernameText.Text.Trim();
                    password = PasswordText.Text.Trim();
                    checkpassword = CheckPasswordText.Text.Trim();
                    email = EmailText.Text.Trim();
                });
                if (username != "")
                {
                    if (NetworkUtility.IsUserName(username))
                    {
                        int length = NetworkUtility.GetUserNameLength(username);
                        if (length >= 3 && length <= 12) // 字节范围 3~12
                        {
                            if (password != checkpassword)
                            {
                                ShowMessage(ShowMessageType.Error, "两个密码不相同，请重新输入！");
                                InvokeUpdateUI(() => CheckPasswordText.Focus());
                                return false;
                            }
                        }
                        else
                        {
                            ShowMessage(ShowMessageType.Error, "账号名长度不符合要求：3~12个字符数（一个中文2个字符）");
                            InvokeUpdateUI(() => UsernameText.Focus());
                            return false;
                        }
                    }
                    else
                    {
                        ShowMessage(ShowMessageType.Error, "账号名不符合要求：不能包含特殊字符");
                        InvokeUpdateUI(() => UsernameText.Focus());
                        return false;
                    }
                }
                if (password != "")
                {
                    int length = password.Length;
                    if (length < 6 || length > 15) // 字节范围 6~15
                    {
                        ShowMessage(ShowMessageType.Error, "密码长度不符合要求：6~15个字符数");
                        InvokeUpdateUI(() => PasswordText.Focus());
                        return false;
                    }
                }
                if (username == "" || password == "" || checkpassword == "")
                {
                    ShowMessage(ShowMessageType.Error, "请将账号和密码填写完整！");
                    InvokeUpdateUI(() => UsernameText.Focus());
                    return false;
                }
                if (email == "")
                {
                    ShowMessage(ShowMessageType.Error, "邮箱不能为空！");
                    InvokeUpdateUI(() => EmailText.Focus());
                    return false;
                }
                if (!NetworkUtility.IsEmail(email))
                {
                    ShowMessage(ShowMessageType.Error, "这不是一个邮箱地址！");
                    InvokeUpdateUI(() => EmailText.Focus());
                    return false;
                }
                return await RegController.RegAsync(username, password, email);
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
                return false;
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void SucceedRegEvent(object sender, GeneralEventArgs e)
        {
            string username = ((RegisterEventArgs)e).Username;
            string password = ((RegisterEventArgs)e).Password;
            TaskUtility.NewTask(async () => await LoginController.LoginAccountAsync(username, password, encrypt: false));
            if (RunTime.Login != null)
            {
                RunTime.Login.InvokeUpdateUI(RunTime.Login.Close);
            }
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            RegButton.Enabled = false;
            TaskUtility.NewTask(async () =>
            {
                if (!await Reg_Handler()) InvokeUpdateUI(() => RegButton.Enabled = true);
                else InvokeUpdateUI(Close);
            });
        }

        private void GoToLogin_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}

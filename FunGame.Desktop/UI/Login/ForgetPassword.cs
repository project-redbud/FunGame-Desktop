﻿using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Controller;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.UI
{
    public partial class ForgetPassword
    {
        private readonly LoginController LoginController;

        public ForgetPassword()
        {
            InitializeComponent();
            LoginController = new(this);
        }

        private void FindPassword_Click(object sender, EventArgs e)
        {
            TaskUtility.NewTask(async () =>
            {
                if (RunTime.Socket != null)
                {
                    string username = "";
                    string email = "";
                    InvokeUpdateUI(() =>
                    {
                        username = UsernameText.Text.Trim();
                        email = EmailText.Text.Trim();
                    });
                    if (username == "" || email == "")
                    {
                        ShowMessage(ShowMessageType.Error, "账号或邮箱不能为空！");
                        InvokeUpdateUI(() => UsernameText.Focus());
                        return;
                    }

                    string msg;
                    bool success = false;

                    try
                    {
                        // 发送找回密码请求
                        msg = await LoginController.ForgetPassword_CheckVerifyCodeAsync(username, email, "");

                        if (msg.Trim() != "")
                        {
                            // 如果返回一个信息，则停止找回密码
                            ShowMessage(ShowMessageType.Error, msg);
                        }
                        else
                        {
                            while (!success)
                            {
                                string verifycode = ShowInputMessageCancel("请输入找回密码邮件中的6位数字验证码", "注册验证码", out MessageResult result);
                                if (result != MessageResult.Cancel)
                                {
                                    if (verifycode.Trim() != "")
                                    {
                                        msg = await LoginController.ForgetPassword_CheckVerifyCodeAsync(username, email, verifycode);
                                        if (msg.Trim() != "")
                                        {
                                            ShowMessage(ShowMessageType.Error, msg);
                                        }
                                        else
                                        {
                                            success = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        ShowMessage(ShowMessageType.Warning, "不能输入空值！");
                                    }
                                }
                                else break;
                            }
                            if (success)
                            {
                                while (true)
                                {
                                    string newpass = ShowInputMessageCancel("请输入新密码", "设置新密码", out MessageResult result);
                                    if (result != MessageResult.Cancel)
                                    {
                                        if (newpass.Trim() != "")
                                        {
                                            if (newpass.Length < 6 || newpass.Length > 15) // 字节范围 3~12
                                            {
                                                ShowMessage(ShowMessageType.Error, "密码长度不符合要求：6~15个字符数");
                                            }
                                            else
                                            {
                                                msg = await LoginController.ForgetPassword_UpdatePasswordAsync(username, newpass);
                                                if (msg.Trim() != "")
                                                {
                                                    ShowMessage(ShowMessageType.Error, msg);
                                                }
                                                else
                                                {
                                                    ShowMessage(ShowMessageType.General, "密码更新成功！请您牢记新的密码。", "找回密码");
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ShowMessage(ShowMessageType.OKCancel, "确定放弃设置新密码吗？", "找回密码") == MessageResult.OK)
                                        {
                                            success = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (success)
                            {
                                InvokeUpdateUI(Dispose);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        RunTime.WritelnSystemInfo(ex.GetErrorInfo());
                    }
                }
            });
        }
    }
}

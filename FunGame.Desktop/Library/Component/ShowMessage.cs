using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    /// <summary>
    /// ShowMessage提供三个按钮，三种按钮组合方式(单一，是否，是否+取消)，提供自动关闭弹窗的功能。
    /// </summary>
    public partial class ShowMessage : GeneralForm
    {
        private MessageResult MessageResult = MessageResult.Cancel;
        private readonly MessageResult AutoResult = MessageResult.OK;
        private string InputResult = "";

        private const string TITLE_TIP = "提示";
        private const string TITLE_WARNING = "警告";
        private const string TITLE_ERROR = "错误";
        private const string BUTTON_OK = "确定";
        private const string BUTTON_CANCEL = "取消";
        private const string BUTTON_YES = "是";
        private const string BUTTON_NO = "否";
        private const string BUTTON_RETRY = "重试";

        /// <summary>
        /// 弹出单一按钮（确定）的弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public static MessageResult Message(string msg, string title, int autoclose = 0)
        {
            MessageResult result = new ShowMessage(title, msg, autoclose, MessageButtonType.OK, BUTTON_OK).MessageResult;
            return result;
        }

        /// <summary>
        /// 弹出单一按钮（确定）的弹窗，标题默认为“提示”
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public static MessageResult TipMessage(string msg, string title = "", int autoclose = 0)
        {
            MessageResult result = new ShowMessage(title == "" ? TITLE_TIP : title, msg, autoclose, MessageButtonType.OK, BUTTON_OK).MessageResult;
            return result;
        }

        /// <summary>
        /// 弹出单一按钮（确定）的弹窗，标题默认为“警告”
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public static MessageResult WarningMessage(string msg, string title = "", int autoclose = 0)
        {
            MessageResult result = new ShowMessage(title == "" ? TITLE_WARNING : title, msg, autoclose, MessageButtonType.OK, BUTTON_OK).MessageResult;
            return result;
        }

        /// <summary>
        /// 弹出单一按钮（确定）的弹窗，标题默认为“错误”
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public static MessageResult ErrorMessage(string msg, string title = "", int autoclose = 0)
        {
            MessageResult result = new ShowMessage(title == "" ? TITLE_ERROR : title, msg, autoclose, MessageButtonType.OK, BUTTON_OK).MessageResult;
            return result;
        }

        /// <summary>
        /// 弹出选择（是，否）的弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public static MessageResult YesNoMessage(string msg, string title, int autoclose = 0)
        {
            MessageResult result = new ShowMessage(title, msg, autoclose, MessageButtonType.YesNo, BUTTON_CANCEL, BUTTON_YES, BUTTON_NO).MessageResult;
            return result;
        }

        /// <summary>
        /// 弹出选择（确定，取消）的弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public static MessageResult OKCancelMessage(string msg, string title, int autoclose = 0)
        {
            MessageResult result = new ShowMessage(title, msg, autoclose, MessageButtonType.OKCancel, BUTTON_CANCEL, BUTTON_OK, BUTTON_CANCEL).MessageResult;
            return result;
        }

        /// <summary>
        /// 弹出选择（重试，取消）的弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        public static MessageResult RetryCancelMessage(string msg, string title, int autoclose = 0)
        {
            MessageResult result = new ShowMessage(title, msg, autoclose, MessageButtonType.RetryCancel, BUTTON_CANCEL, BUTTON_RETRY, BUTTON_CANCEL).MessageResult;
            return result;
        }

        /// <summary>
        /// 弹出具有输入框的弹窗
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <returns>输入框内的文本</returns>
        public static string InputMessage(string msg, string title)
        {
            string result = new ShowMessage(title, msg, 0, MessageButtonType.Input).InputResult;
            return result;
        }

        /// <summary>
        /// 弹出具有输入框的弹窗，多增加了（点击的按钮）返回值，可用于判断是否取消操作
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="cancel">点击的按钮</param>
        /// <returns>输入框内的文本</returns>
        public static string InputMessageCancel(string msg, string title, out MessageResult cancel)
        {
            ShowMessage window = new(title, msg, 0, MessageButtonType.Input);
            string result = window.InputResult;
            cancel = window.MessageResult;
            return result;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">信息</param>
        /// <param name="autoclose">自动关闭倒计时(秒)</param>
        /// <param name="type">按钮类型(组合类型)</param>
        /// <param name="mid">中间按钮的文本</param>
        /// <param name="left">左边按钮的文本</param>
        /// <param name="right">右边按钮的文本</param>
        private ShowMessage(string title = "", string msg = "", int autoclose = 0, MessageButtonType type = MessageButtonType.OK, string mid = "", string left = "", string right = "")
        {
            InitializeComponent();
            Opacity = 0.85; // 透明度
            // 标题
            Title.Text = title;
            Text = title;
            // 信息
            MsgText.Text = msg;
            // 按钮类型(组合类型)
            switch (type)
            {
                case MessageButtonType.OK:
                    MidButton.Text = BUTTON_OK;
                    InputText.Visible = false;
                    InputButton.Visible = false;
                    LeftButton.Visible = false;
                    RightButton.Visible = false;
                    MidButton.Visible = true;
                    break;
                case MessageButtonType.OKCancel:
                    LeftButton.Text = BUTTON_OK;
                    RightButton.Text = BUTTON_CANCEL;
                    InputText.Visible = false;
                    InputButton.Visible = false;
                    LeftButton.Visible = true;
                    RightButton.Visible = true;
                    MidButton.Visible = false;
                    AutoResult = MessageResult.Cancel;
                    break;
                case MessageButtonType.YesNo:
                    LeftButton.Text = BUTTON_YES;
                    RightButton.Text = BUTTON_NO;
                    InputText.Visible = false;
                    InputButton.Visible = false;
                    LeftButton.Visible = true;
                    RightButton.Visible = true;
                    MidButton.Visible = false;
                    AutoResult = MessageResult.No;
                    break;
                case MessageButtonType.RetryCancel:
                    LeftButton.Text = BUTTON_RETRY;
                    RightButton.Text = BUTTON_CANCEL;
                    InputText.Visible = false;
                    InputButton.Visible = false;
                    LeftButton.Visible = true;
                    RightButton.Visible = true;
                    MidButton.Visible = false;
                    AutoResult = MessageResult.Cancel;
                    break;
                case MessageButtonType.Input:
                    InputButton.Text = BUTTON_OK;
                    LeftButton.Visible = false;
                    RightButton.Visible = false;
                    MidButton.Visible = false;
                    InputText.Visible = true;
                    InputButton.Visible = true;
                    AutoResult = MessageResult.Cancel;
                    break;
            }
            // 按钮文本
            MidButton.Text = mid;
            LeftButton.Text = left;
            RightButton.Text = right;
            // 自动关闭倒计时
            TaskUtility.NewTask(async () => await AutoClose(autoclose));
            ShowDialog();
        }

        /// <summary>
        /// 自动关闭窗口的实现
        /// </summary>
        /// <param name="autoclose"></param>
        /// <returns></returns>
        private async Task AutoClose(int autoclose)
        {
            if (autoclose > 0)
            {
                string msg = MsgText.Text;
                await Task.Run(() =>
                {
                    while (IsHandleCreated)
                    {
                        break;
                    }
                });
                ChangeSecond(msg, autoclose);
                while (!Disposing)
                {
                    if (autoclose > 0) await Task.Delay(1000);
                    else break;
                    autoclose--;
                    ChangeSecond(msg, autoclose);
                }
                MessageResult = AutoResult;
                InvokeUpdateUI(Dispose);
            }
        }

        /// <summary>
        /// 设置窗体按钮返回值
        /// </summary>
        /// <param name="text"></param>
        private void SetButtonResult(string text)
        {
            MessageResult = text switch
            {
                BUTTON_OK => MessageResult.OK,
                BUTTON_CANCEL => MessageResult.Cancel,
                BUTTON_YES => MessageResult.Yes,
                BUTTON_NO => MessageResult.No,
                BUTTON_RETRY => MessageResult.Retry,
                _ => MessageResult.Cancel
            };
            Dispose();
        }

        /// <summary>
        /// 修改倒计时显示
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="s"></param>
        private void ChangeSecond(string msg, int s)
        {
            InvokeUpdateUI(() => MsgText.Text = msg + "\n[ " + s + " 秒后自动关闭 ]");
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            SetButtonResult(LeftButton.Text);
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            SetButtonResult(RightButton.Text);
        }

        private void MidButton_Click(object sender, EventArgs e)
        {
            SetButtonResult(MidButton.Text);
        }

        private void InputButton_Click()
        {
            MessageResult = MessageResult.OK;
            if (InputText.Text != null && !InputText.Text.Equals(""))
            {
                InputResult = InputText.Text;
                Dispose();
            }
            else
            {
                InputText.Enabled = false;
                WarningMessage("不能输入空值！");
                InputText.Enabled = true;
            }
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            InputButton_Click();
        }

        private void InputText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                InputButton_Click();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            MessageResult = MessageResult.Cancel;
            Dispose();
        }
    }
}

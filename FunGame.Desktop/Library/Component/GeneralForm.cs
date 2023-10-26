using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Desktop.Utility;

namespace Milimoe.FunGame.Desktop.Library.Component
{
    public partial class GeneralForm : Form
    {
        protected int loc_x, loc_y; // 窗口当前坐标

        public GeneralForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 提供公共方法给Controller发送消息弹窗（这样可以防止跨线程时，弹窗不在最上层）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="autoclose"></param>
        public MessageResult ShowMessage(ShowMessageType type, string msg, string title = "", int autoclose = 0)
        {
            MessageResult result = MessageResult.OK;

            void action()
            {
                if (msg == "") return;
                switch (type)
                {
                    case ShowMessageType.General:
                        result = Component.ShowMessage.Message(msg, title, autoclose);
                        break;
                    case ShowMessageType.Tip:
                        result = Component.ShowMessage.TipMessage(msg, "", autoclose);
                        break;
                    case ShowMessageType.Warning:
                        result = Component.ShowMessage.WarningMessage(msg, "", autoclose);
                        break;
                    case ShowMessageType.Error:
                        result = Component.ShowMessage.ErrorMessage(msg, "", autoclose);
                        break;
                    case ShowMessageType.YesNo:
                        result = Component.ShowMessage.YesNoMessage(msg, title, autoclose);
                        break;
                    case ShowMessageType.OKCancel:
                        result = Component.ShowMessage.OKCancelMessage(msg, title, autoclose);
                        break;
                    case ShowMessageType.RetryCancel:
                        result = Component.ShowMessage.RetryCancelMessage(msg, title, autoclose);
                        break;
                    default:
                        break;
                }
            };
            InvokeUpdateUI(action);

            return result;
        }

        /// <summary>
        /// 提供公共方法给Controller发送消息弹窗（这样可以防止跨线程时，弹窗不在最上层）
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        public string ShowInputMessage(string msg, string title)
        {
            string input = "";

            void action()
            {
                if (msg == "") return;
                input = Component.ShowMessage.InputMessage(msg, title);
            };
            InvokeUpdateUI(action);

            return input;
        }

        /// <summary>
        /// 提供公共方法给Controller发送消息弹窗（这样可以防止跨线程时，弹窗不在最上层）<para/>
        /// 支持返回点击的按钮，用于判断是否取消输入
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="result"></param>
        public string ShowInputMessageCancel(string msg, string title, out MessageResult result)
        {
            MessageResult resultThisMethod = MessageResult.Cancel;
            string input = "";

            void action()
            {
                if (msg == "") return;
                input = Component.ShowMessage.InputMessageCancel(msg, title, out MessageResult resultShowMessage);
                resultThisMethod = resultShowMessage;
            };
            InvokeUpdateUI(action);

            result = resultThisMethod;

            return input;
        }

        /// <summary>
        /// 绑定事件，子类需要重写
        /// </summary>
        protected virtual void BindEvent()
        {

        }

        /// <summary>
        /// 鼠标按下，开始移动主窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Title_MouseDown(object sender, MouseEventArgs e)
        {
            //判断是否为鼠标左键
            if (e.Button == MouseButtons.Left)
            {
                //获取鼠标左键按下时的位置
                loc_x = e.Location.X;
                loc_y = e.Location.Y;
            }
        }

        /// <summary>
        /// 鼠标移动，正在移动主窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //计算鼠标移动距离
                Left += e.Location.X - loc_x;
                Top += e.Location.Y - loc_y;
            }
        }

        /// <summary>
        /// 自定义窗体销毁方法
        /// </summary>
        protected virtual void FormClosedEvent(object? sender, FormClosedEventArgs e)
        {
            if (GetType() != typeof(ShowMessage))
            {
                Singleton.Remove(this);
                if (OpenForm.Forms.Contains(this))
                {
                    OpenForm.Forms.Remove(this);
                }
                if (GetType() == typeof(Main))
                {
                    RunTime.Main = null;
                }
                else if (GetType() == typeof(Login))
                {
                    RunTime.Login = null;
                }
                else if (GetType() == typeof(Register))
                {
                    RunTime.Register = null;
                }
                else if (GetType() == typeof(InventoryUI))
                {
                    RunTime.Inventory = null;
                }
                else if (GetType() == typeof(StoreUI))
                {
                    RunTime.Store = null;
                }
                else if (GetType() == typeof(RoomSetting))
                {
                    RunTime.RoomSetting = null;
                }
                else if (GetType() == typeof(UserCenter))
                {
                    RunTime.UserCenter = null;
                }
                Dispose();
            }
        }

        /// <summary>
        /// 窗体加载事件，触发BindEvent()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void LoadEvent(object? sender, EventArgs e)
        {
            BindEvent();
        }

        /// <summary>
        /// 委托更新UI
        /// </summary>
        /// <param name="action"></param>
        protected virtual void InvokeUpdateUI(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }

    }
}

using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.Model;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Controller
{
    public class RunTimeController
    {
        public bool Connected => RunTimeModel.Connected;

        private RunTimeModel RunTimeModel { get; }
        private Main Main { get; }

        public RunTimeController(Main Main)
        {
            this.Main = Main;
            RunTimeModel = new RunTimeModel(Main);
        }

        public async Task<bool> GetServerConnection()
        {
            bool result = false;

            try
            {
                RunTimeModel.GetServerConnection();
                result = await Connect() == ConnectResult.Success;
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }

        public async Task<ConnectResult> Connect()
        {
            ConnectResult result = ConnectResult.ConnectFailed;

            try
            {
                ConnectEventArgs EventArgs = new(Constant.Server_IP, Constant.Server_Port);
                if (Main.OnBeforeConnectEvent(EventArgs) == EventResult.Fail) return ConnectResult.ConnectFailed;

                result = await RunTimeModel.Connect();

                if (result == ConnectResult.Success) Main.OnSucceedConnectEvent(EventArgs);
                else Main.OnFailedConnectEvent(EventArgs);
                Main.OnAfterConnectEvent(EventArgs);
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
            
            return result;
        }

        public bool Disconnect()
        {
            bool result = false;

            try
            {
                if (Main.OnBeforeDisconnectEvent(new GeneralEventArgs()) == EventResult.Fail) return result;

                result = RunTimeModel.Disconnect();

                if (result) Main.OnSucceedDisconnectEvent(new GeneralEventArgs());
                else Main.OnFailedDisconnectEvent(new GeneralEventArgs());
                Main.OnAfterDisconnectEvent(new GeneralEventArgs());
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }

            return result;
        }

        public bool Close(params object[] args)
        {
            bool result;

            if (Connected) Disconnect();

            if (args != null && args.Length > 0)
            {
                RunTimeModel.Error((Exception)args[0]);
                result = true;
            }
            else result = RunTimeModel.Close();

            return result;
        }

        public bool Error(Exception e)
        {
            return Close(e);
        }

        public async Task AutoLogin(params object[] objs)
        {
            try
            {
                LoginController LoginController = new();
                await LoginController.LoginAccount(objs);
                LoginController.Dispose();
            }
            catch (Exception e)
            {
                Main.GetMessage(e.GetErrorInfo(), TimeType.None);
            }
        }
    }
}

using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.Library
{
    /// <summary>
    /// 运行时单例
    /// 插件接口可以从这里拿Socket和窗体
    /// </summary>
    public class RunTime
    {
        public static Core.Model.RoomList RoomList { get; } = new();
        public static Core.Model.Session Session { get; } = new();
        public static Core.Library.Common.Network.Socket? Socket { get; set; } = null;
        public static Controller.RunTimeController? Controller { get; set; } = null;
        public static UI.Main? Main { get; set; } = null;
        public static UI.Login? Login { get; set; } = null;
        public static UI.Register? Register { get; set; } = null;
        public static UI.StoreUI? Store { get; set; } = null;
        public static UI.InventoryUI? Inventory { get; set; } = null;
        public static UI.RoomSetting? RoomSetting { get; set; } = null;
        public static UI.UserCenter? UserCenter { get; set; } = null;

        public static void WritelnSystemInfo(string msg)
        {
            Controller?.WritelnSystemInfo(msg);
        }

        public static DataRequest NewDataRequest(DataRequestType RequestType)
        {
            if (Socket != null)
            {
                DataRequest request = new(Socket, RequestType);
                return request;
            }
            throw new ConnectFailedException();
        }
    }
}

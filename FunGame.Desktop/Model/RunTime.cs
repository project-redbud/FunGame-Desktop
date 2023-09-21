using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Library.Common.Plugin;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Desktop.Model
{
    /// <summary>
    /// 运行时单例
    /// </summary>
    public class RunTime
    {
        public static Core.Model.RoomList RoomList { get; } = new();
        public static Core.Model.Session Session { get; } = new();
        public static Core.Model.FunGameConfig Config { get; } = new();
        public static Dictionary<string, BasePlugin> Plugins { get; } = new();
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
            DataRequest? request = Controller?.NewDataRequest(RequestType);
            return request is null ? throw new ConnectFailedException() : request;
        }

        public static DataRequest NewLongRunningDataRequest(DataRequestType RequestType)
        {
            DataRequest? request = Controller?.NewLongRunningDataRequest(RequestType);
            return request is null ? throw new ConnectFailedException() : request;
        }
    }
}

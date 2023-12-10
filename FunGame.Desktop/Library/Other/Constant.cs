using System.Collections.Generic;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Model;

namespace Milimoe.FunGame.Desktop.Library
{
    public class Constant
    {
        /**
         * Game Configs
         */
        public static FunGameInfo.FunGame FunGameType => FunGameInfo.FunGame.FunGame_Desktop;

        /**
         * FunGame Configs
         */
        public const string FunGame_PresetMessage = "- 快捷消息 -";
        public const string FunGame_SignIn = "签到";
        public const string FunGame_ShowCredits = "积分";
        public const string FunGame_ShowStock = "查看库存";
        public const string FunGame_ShowStore = "游戏商店";
        public const string FunGame_ClearGameInfo = "清空消息队列";
        public const string FunGame_CreateMix = "创建游戏 混战";
        public const string FunGame_CreateTeam = "创建游戏 团队";
        public const string FunGame_StartGame = "开始游戏";
        public const string FunGame_Connect = "连接服务器";
        public const string FunGame_ConnectTo = "连接指定服务器";
        public const string FunGame_Disconnect = "登出并断开连接";
        public const string FunGame_DisconnectWhenNotLogin = "断开连接";
        public const string FunGame_Retry = "重新连接";
        public const string FunGame_AutoRetryOn = "开启自动重连";
        public const string FunGame_AutoRetryOff = "关闭自动重连";
        public const string FunGame_Ready = "准备就绪";
        public const string FunGame_CancelReady = "取消准备";
        public static readonly object[] PresetOnlineItems =
        {
            FunGame_PresetMessage,
            FunGame_SignIn,
            FunGame_ShowCredits,
            FunGame_ShowStock,
            FunGame_ShowStore,
            FunGame_ClearGameInfo,
            FunGame_CreateMix,
            FunGame_CreateTeam,
            FunGame_Disconnect,
            FunGame_AutoRetryOn,
            FunGame_AutoRetryOff
        };
        public static readonly object[] PresetInRoomItems =
        {
            FunGame_PresetMessage,
            FunGame_SignIn,
            FunGame_ShowCredits,
            FunGame_ShowStock,
            FunGame_ShowStore,
            FunGame_ClearGameInfo,
            FunGame_Ready,
            FunGame_CancelReady,
            FunGame_StartGame,
            FunGame_Disconnect
        };
        public static readonly object[] PresetNoLoginItems =
        {
            FunGame_PresetMessage,
            FunGame_DisconnectWhenNotLogin,
            FunGame_AutoRetryOn,
            FunGame_AutoRetryOff
        };
        public static readonly object[] PresetNoConnectItems =
        {
            FunGame_PresetMessage,
            FunGame_Connect,
            FunGame_ConnectTo,
            FunGame_Retry,
            FunGame_AutoRetryOn,
            FunGame_AutoRetryOff
        };
        public static readonly object[] AllComboItem = ["全部"];
        public static object[] SupportedRoomType()
        {
            List<string> objs = [];
            objs.Add(RoomSet.GetTypeString(RoomType.All));
            objs.Add(RoomSet.GetTypeString(RoomType.Mix));
            objs.Add(RoomSet.GetTypeString(RoomType.Team));
            objs.Add(RoomSet.GetTypeString(RoomType.Solo));
            objs.Add(RoomSet.GetTypeString(RoomType.FastAuto));
            objs.Add(RoomSet.GetTypeString(RoomType.Custom));
            return objs.ToArray();
        }
        public static object[] SupportedGameMode(RoomType type)
        {
            if (RunTime.GameModeLoader != null)
            {
                IEnumerable<object> list;
                if (type == RoomType.All)
                {
                    list = RunTime.GameModeLoader.Modes.Values.Select(mod => mod.Name).Distinct();
                }
                else
                {
                    list = RunTime.GameModeLoader.Modes.Values.Where(mod => mod.RoomType == type).Select(mod => mod.Name).Distinct();
                }
                if (list.Any()) return AllComboItem.Union(list).ToArray();
            }
            return ["- 缺少模组 -"];
        }
        public static object[] SupportedGameMap()
        {
            List<string> list = [];
            if (RunTime.GameModeLoader != null)
            {
                foreach (GameMode mod in RunTime.GameModeLoader.Modes.Values)
                {
                    list.AddRange(mod.Maps.Distinct());
                }
            }
            if (list.Count != 0) return AllComboItem.Union(list).ToArray();
            return ["- 缺少地图 -"];
        }
        public static object[] SupportedGameMap(GameMode mod)
        {
            IEnumerable<object> list = mod.Maps.Where(map => mod.Maps.Contains(map)).Distinct();
            if (list.Any()) return AllComboItem.Union(list).ToArray();
            return ["- 缺少地图 -"];
        }
    }
}

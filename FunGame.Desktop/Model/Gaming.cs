using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Desktop.Controller;

namespace Milimoe.FunGame.Desktop.Model
{
    public class Gaming
    {
        private readonly GamingController Controller;
        private readonly GameMode GameMode;
        private readonly Room Room;
        private readonly List<User> Users;
        private readonly List<Character> Characters;
        private readonly GamingEventArgs EventArgs;

        private Gaming(GameMode GameMode, Room Room, List<User> Users)
        {
            this.GameMode = GameMode;
            this.Room = Room;
            this.Users = Users;
            Characters = [];
            EventArgs = new(Room, Users, Characters);
            Controller = new(this);
        }

        public static Gaming StartGame(GameMode GameMode, Room Room, List<User> Users)
        {
            Gaming instance = new(GameMode, Room, Users);
            RunTime.Gaming = instance;
            return instance;
        }

        private void ConnectToGame()
        {
            GameMode.OnBeforeGamingConnectEvent(this, EventArgs);
            if (EventArgs.Cancel)
            {
                return;
            }
            if (!EventArgs.Cancel)
            {
                GameMode.OnSucceedGamingConnectEvent(this, EventArgs);
            }
            else
            {
                GameMode.OnFailedGamingConnectEvent(this, EventArgs);
            }
            GameMode.OnAfterGamingConnectEvent(this, EventArgs);
        }
    }
}

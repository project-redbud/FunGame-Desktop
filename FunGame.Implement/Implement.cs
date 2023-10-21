using Milimoe.FunGame.Core.Interface;

namespace Milimoe.FunGame.Core.Implement
{
    public class IClientImpl : IClient
    {
        public string FunGameIcon => throw new NotImplementedException();

        public string FunGameBackGround => throw new NotImplementedException();

        public string FunGameMainMusic => throw new NotImplementedException();

        public string FunGameMusic1 => throw new NotImplementedException();

        public string FunGameMusic2 => throw new NotImplementedException();

        public string FunGameMusic3 => throw new NotImplementedException();

        public string RemoteServerIP()
        {
            // 此处修改连接远程服务器IP
            string serverIP = "127.0.0.1";
            string serverPort = "22222";
            return serverIP + ":" + serverPort;
        }
    }
}
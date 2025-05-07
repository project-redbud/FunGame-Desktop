namespace Milimoe.FunGame.Desktop.UI
{
    internal static class Start
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(params string[] args)
        {
            foreach (string option in args)
            {
                switch (option)
                {
                    case "-debug":
                        Core.Library.Constant.FunGameInfo.FunGame_DebugMode = true;
                        break;
                }
            }
            // 初始化 WinForms 配置
            ApplicationConfiguration.Initialize();

            // 初始化 WPF Application
            System.Windows.Application wpfApp = new()
            {
                ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown
            };
            wpfApp.Dispatcher.Invoke(() => { });

            Application.Run(new Main());

            wpfApp.Shutdown();
        }
    }
}
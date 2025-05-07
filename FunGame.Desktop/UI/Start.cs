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
            // ��ʼ�� WinForms ����
            ApplicationConfiguration.Initialize();

            // ��ʼ�� WPF Application
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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FateReader
{
    public class HTML
    {
        private bool IsBrowserOpen = false;
        /// <summary>
        /// Writes the text to an HTML output file that opens in your default browser.
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {           
            string programFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
            string fullPath = @$"{programFolder}\index.html";
            text = Regex.Replace(text, "%\\[(.*)%:(.*)%\\]", "<ruby>$1<rt>$2</rt></ruby>"); // Furigana conversion
            text = text.Replace("\r\n", "<br/>");
            using (StreamWriter sw = new StreamWriter(fullPath))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head><meta charset='UTF-8'/>");
                sb.Append("</head>");
                sb.Append($"<body style='" +
                    $"background:{Constants.BrowserBackground};" +
                    $"color:{Constants.BrowserFontColor};" +
                    $"font-size:{Constants.BrowserFontSize};" +
                    $"margin:{Constants.BrowserMargins};" +
                    $"'>");
                sb.Append($"<p>{text}</p>");
                sb.Append("</body>");
                sb.Append("</html>");
                sw.Write(sb);
                sw.Flush();
                sw.Close();
                sw.Dispose();
                sb.Clear();
            }
            if (!IsBrowserOpen)
            {
                Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                IsBrowserOpen = true;
            }
            else RefreshBrowser();
        }
        #region Private methods
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        /// <summary>
        /// Emulates refreshing your default web browser by sending the F5 key press to the process.
        /// </summary>
        private static void RefreshBrowser()
        {
            const UInt32 WM_KEYDOWN = 0x0100;   // Key press
            const int VK_F5 = 0x74; // F5 Key

            Process[] webBrowserProcesses = Process.GetProcessesByName(Constants.BrowserProcessName);
            if (webBrowserProcesses[0].MainWindowHandle != IntPtr.Zero)
            {
                PostMessage(webBrowserProcesses[0].MainWindowHandle, WM_KEYDOWN, VK_F5, 0);
            }
        }
        #endregion
    }
}

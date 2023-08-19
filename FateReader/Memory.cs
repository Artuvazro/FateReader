using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FateReader
{
    public class Memory
    {
        /// <summary>
        /// Reads the RAM of the process and exports the text of the video game to an HTML file that opens in your default web browser.
        /// </summary>
        public static void Read()
        {
            [DllImport("kernel32.dll")]
            static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
            [DllImport("kernel32.dll")]
            static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

            Process[] processes = Process.GetProcessesByName(Constants.GameProcessName);
            processes = GetProcess();

            IntPtr processHandle = OpenProcess(Constants.ProcessAccess, false, processes[0].Id);

            int bytesRead = 0;
            byte[] buffer = new byte[Constants.ReadBuffer];
            string previousText = "";
            HTML html = new HTML();
            while (true)
            {
                ReadProcessMemory((int)processHandle, Constants.TextMemoryAdress, buffer, buffer.Length, ref bytesRead);
                string text = Encoding.GetEncoding(932).GetString(buffer);
                string cleanText = CleanText(text);
                if (cleanText != previousText)
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.Write(cleanText);
                    html.Write(cleanText);
                    previousText = cleanText;
                }
                Thread.Sleep(1000);
            }
        }

        #region Private methods
        /// <summary>
        /// Returns a list of processes matching the process name provided.
        /// </summary>
        /// <returns></returns>
        private static Process[] GetProcess()
        {
            Process[] processes = new Process[] { };
            while (processes.Count() == 0)
            {
                Console.WriteLine($"Waiting for {Constants.GameProcessName} process...");
                Thread.Sleep(5000);
                Console.SetCursorPosition(0, 0);
                processes = Process.GetProcessesByName(Constants.GameProcessName);
            }
            Console.Clear();
            Console.WriteLine($"{Constants.GameProcessName} process found.");
            return processes;
        }

        /// <summary>
        /// Removes script information from the text such as timings, SFX instructions, etc. leaving only plain text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string CleanText(string text)
        {
            string cleanText;

            cleanText = text.Replace("\0", string.Empty);
            cleanText = Regex.Replace(cleanText, "%(s|n|ch).*?i[0-9]", string.Empty);
            cleanText = Regex.Replace(cleanText, "(。|」)　", "$1\r\n");
            cleanText = Regex.Replace(cleanText, "%l[0-9],0", "ー");
            cleanText = Regex.Replace(cleanText, "%n", "▼");
            cleanText = Regex.Replace(cleanText, "(。|」)「", "$1\r\n「");

            return cleanText;
        }
        #endregion
    }
}

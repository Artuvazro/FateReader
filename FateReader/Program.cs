using FateReader;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.GetEncoding(932);
Console.SetWindowSize(50, 10);
Memory.Read();
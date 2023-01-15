using System.Text.Json;

namespace TTT.Tesseract.Sample
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            //Application.Run(new Form());

            const string file = @".\Screenshot_20230112-002430.png";
            var page = Helper.Parse(file);
            var match = Helper.FindText("sms with an activation", page);
            var json = JsonSerializer.Serialize(new { match, page }, RectangleJsonConverter.SerializerOptions);
            Console.WriteLine(json);
        }
    }
}
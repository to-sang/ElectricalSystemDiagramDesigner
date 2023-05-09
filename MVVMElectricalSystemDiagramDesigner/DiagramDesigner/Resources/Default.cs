using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace DiagramDesigner
{
    public static class Default
    {

        #region Guid

        public static Guid String2Guid(string value)
        {
            byte[] bytes = new byte[16];
            Encoding.ASCII.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static string Guid2String(Guid value) => Encoding.ASCII.GetString(value.ToByteArray()).Trim('\0');

        public static Guid Int2Guid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static int Guid2Int(Guid value) => BitConverter.ToInt32(value.ToByteArray(), 0);

        public static int IdInt(Guid id) => ((int.TryParse(Guid2String(id), out int idInt)) ? idInt : -1);

        #endregion

        public static Stack<Action> undoActions = new();
        public static Stack<Action> redoActions = new();

        public static Action HasChangedAction { get; set; }

        public static BitmapImage ConvertBitmapSourceToBitmapImage(BitmapSource source)
        {
            JpegBitmapEncoder encoder = new();
            MemoryStream memoryStream = new();
            BitmapImage bImg = new();

            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bImg.BeginInit();
            bImg.StreamSource = memoryStream;
            bImg.EndInit();

            memoryStream.Close();

            return bImg;
        }

    }
}

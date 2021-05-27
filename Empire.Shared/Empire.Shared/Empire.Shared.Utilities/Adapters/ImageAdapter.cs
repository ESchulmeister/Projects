using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Empire.Shared.Utilities
{
    public class ImageAdapter
    {
        #region Properties
        public ImageFormat Format
        {
            get; set;
        }
        #endregion

        #region Constructors
        public ImageAdapter(ImageFormat oImageFormat)
        {
            this.Format = oImageFormat;
        }
        #endregion

        #region Methods
        public byte[] Convert(byte[] aInputBytes)
        {
            byte[] aReturnBytes = null;

            using (var oInputMemoryStream = new MemoryStream(aInputBytes))
            {
                using (var oOutputMemoryStream = new MemoryStream())
                {
                    using (var oImage = Image.FromStream(oInputMemoryStream))
                    {
                        oImage.Save(oOutputMemoryStream, this.Format);
                        aReturnBytes = oOutputMemoryStream.ToArray();
                    }
                }
            }

            return aReturnBytes;
        }
        #endregion
    }
}

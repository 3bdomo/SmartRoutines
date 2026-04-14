using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace SmartRoutines.UI.Core.Helper
{
    /// <summary>
    /// Centralized utility to load and cache PNG icons from the Resources/Icon folder.
    /// Handles proportional scaling and ensures high-quality rendering.
    /// </summary>
    public static class IconLoader
    {
        private static readonly string IconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Icon");
        private static readonly System.Collections.Generic.Dictionary<string, Image> _cache = new();

        /// <summary>
        /// Loads an icon by filename (e.g., "dashboard.png") and scales it to the specified size.
        /// </summary>
        public static Image? GetIcon(string fileName, int size = 24)
        {
            string cacheKey = $"{fileName}_{size}";
            if (_cache.TryGetValue(cacheKey, out var cachedImage))
                return cachedImage;

            string fullPath = Path.Combine(IconPath, fileName);
            if (!File.Exists(fullPath))
            {
                // Fallback attempt: check case-sensitively or without extension
                if (!fileName.EndsWith(".png"))
                    fullPath = Path.Combine(IconPath, fileName + ".png");
            }

            if (!File.Exists(fullPath))
                return null;

            try
            {
                using var original = Image.FromFile(fullPath);
                var scaled = ScaleImage(original, size, size);
                _cache[cacheKey] = scaled;
                return scaled;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading icon {fileName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Scales an image to a specific size with high quality.
        /// </summary>
        private static Image ScaleImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}

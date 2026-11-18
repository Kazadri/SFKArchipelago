using System.IO;
using System.Reflection;
using UnityEngine;

namespace SfkArchipelago.Assets
{
    public class ModAssets
    {
        private static Sprite _archipelagoIcon;

        public static Sprite ArchipelagoIcon
        {
            get
            {
                if (_archipelagoIcon != null) return _archipelagoIcon;

                var asm = Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("SfkArchipelago.Assets.archipelago.png");
                using var ms = new MemoryStream();
                stream.CopyTo(ms);

                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, ms.ToArray());

                _archipelagoIcon = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                return _archipelagoIcon;
            }
        }
    }
}
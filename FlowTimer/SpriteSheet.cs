using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FlowTimer {

    public class SpriteSheet {

        private Bitmap[] SubBitmaps;

        public SpriteSheet(Bitmap bitmap, int spriteWidth, int spriteHeight) {
            int numSpritesPerColumn = bitmap.Width / spriteWidth;
            int numSpritesPerRow = bitmap.Height / spriteHeight;

            SubBitmaps = new Bitmap[numSpritesPerColumn * numSpritesPerRow];

            for(int x = 0; x < numSpritesPerColumn; x++) {
                for(int y = 0; y < numSpritesPerRow; y++) {
                    int index = x + y * numSpritesPerColumn;
                    SubBitmaps[index] = new Bitmap(spriteWidth, spriteHeight);
                    using(Graphics graphics = Graphics.FromImage(SubBitmaps[index])) {
                        graphics.DrawImage(bitmap, new Rectangle(0, 0, spriteWidth, spriteHeight), x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight, GraphicsUnit.Pixel);
                    }
                }
            }
        }

        public Bitmap this[int index] {
            get { return SubBitmaps[index]; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.MobileFortress.Old
{
    public abstract class FortressComponent
    {
        string modelFile;
        string textureFile;

        int width, length, height;
        int layer, x, y;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Editor.Common
{
    public enum DrawingMode
    {
        [Description("原始图像")]
        Raw = 0,
        [Description("去除底色")]
        MaskColor = 1,
        [Description("Alpha混合")]
        AlphaBlend = 2
    }
}

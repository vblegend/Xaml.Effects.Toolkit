﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Editor.Models
{
    public class VideoInfo
    {
        public VideoInfo()
        {
            this.Domain = "";
            this.Url = "";
            this.Title = "";
            this.VideoUrl = "";

        }
        public Int32 Index { get; set; }
        public String VideoUrl { get; set; }
        public String Title { get; set; }

        public String Domain { get; set; }

        public String Url { get; set; }


    }
}


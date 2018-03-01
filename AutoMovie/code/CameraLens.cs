using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class CameraLens
    {
        //角度FOV，Angular FOV
        public static double AFOV(double h,double f)
        {
            return 2 * Math.Atan(h / (2 * f)) * 180 / Math.PI;
        }

        public static double HFOV(double wd,double fov)
        {
            return 2 * wd * Math.Tan(fov / 2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HAKGERSoft.Synapse.Samples {
    public static class Digitalizer {
        public const double Black=0.8;
        public const double White=-0.8;
             
        public static IEnumerable<double> GetInput(Bitmap bitmap,int offsetX,int offsetY,int w,int h, Func<Color,bool> validator) {
            for(int y=0;y<w;y++) {
                for(int x=0;x<h;x++) {
                    Color c=bitmap.GetPixel(x+offsetX,y+offsetY);
                    yield return (validator(c))?Black:White;
                }
            }
        }

        public static IEnumerable<double> GetOutput(int count,int nail) {
            for(int i=0;i<count;i++)
                yield return i==nail?Black:White;
        }

        public static Bitmap Resize(Bitmap b,int nWidth,int nHeight) {
            Bitmap result = new Bitmap(nWidth,nHeight);
            using(Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(b,0,0,nWidth,nHeight);
            return result;
        }

        public static Bitmap AutoCrop(Bitmap img) {
            int x1=LeftBorder(img);
            int x2=RightBorder(img);
            int y1=TopBorder(img);
            int y2=BottomBorder(img);

            int w=x2-x1>0?x2-x1:img.Width;
            int h=y2-y1>0?y2-y1:img.Height;

            if(w<45) {
                x1= x1-20<0?0:x1-20;
                x2= x2+20>img.Width-1?img.Width-1:x2+20;
                w=x2-x1>0?x2-x1:img.Width;
            }
            Rectangle rect=new Rectangle(x1,y1,w,h);
            return (Bitmap)Crop(img,rect);
        }

        static Image Crop(Image img,Rectangle cropArea) {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea,bmpImage.PixelFormat);
            bmpImage.Dispose();
            return (Image)(bmpCrop);
        }

        static int TopBorder(Bitmap img) {
            for(int y=0;y<img.Height;y++) {
                for(int x=0;x<img.Width;x++) {
                    Color c=img.GetPixel(x,y);
                    if(c.Name=="ff000000")
                        return y;
                }
            }
            return 0;
        }

        static int LeftBorder(Bitmap img) {
            for(int x=0;x<img.Width;x++) {
                for(int y=0;y<img.Height;y++) {
                    Color c=img.GetPixel(x,y);
                    if(c.Name=="ff000000")
                        return x;
                }
            }
            return 0;
        }

        static int BottomBorder(Bitmap img) {
            for(int y=img.Height-1;y>0;y--) {
                for(int x=img.Width-1;x>0;x--) {
                    Color c=img.GetPixel(x,y);
                    if(c.Name=="ff000000")
                        return y;
                }
            }
            return img.Height-1;
        }

        static int RightBorder(Bitmap img) {
            for(int x=img.Width-1;x>0;x--) {
                for(int y=0;y<img.Height;y++) {
                    Color c=img.GetPixel(x,y);
                    if(c.Name=="ff000000")
                        return x;
                }
            }
            return img.Width-1;
        }


    }
}

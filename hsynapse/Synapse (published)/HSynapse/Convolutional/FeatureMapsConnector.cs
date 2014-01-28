using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public abstract class FeatureMapsConnector: LayerConnector {

        protected Link[][][] GetKernelCollection(Link[][] priorFeatureMaps, int kernelSize, int overlap) {
            List<Link[][]> kc=new List<Link[][]>(priorFeatureMaps.Length);
            int step=kernelSize-overlap;
            for(int f=0;f<priorFeatureMaps.Length;f++) {
                int size=(int)Math.Sqrt(priorFeatureMaps[f].Length);
                List<Link[]> fmks=new List<Link[]>();
                for(int y=0;y+kernelSize<=size;y+=step) {
                    for(int x=0;x+kernelSize<=size;x+=step) {
                        Link[] kernel=GetKernel(priorFeatureMaps[f],kernelSize,x,y).ToArray();
                        fmks.Add(kernel);
                    }
                }
                kc.Add(fmks.ToArray());
            }
            return kc.ToArray();
        }

        protected T[][] GetFeatureMaps<T>(T[] layer,int count) where T:Link {
            T[][] fm=new T[count][];
            int idx=0;
            int fml=layer.Length / count;
            for(var i=0;i<fm.Length;i++) {
                fm[i]=layer.Skip(idx).Take(fml).ToArray();
                idx+=fml;
            }
            return fm;
        }

        IEnumerable<Link> GetKernel(Link[] layer,int kernelSize,int offsetX,int offsetY) {
            int size=(int)Math.Sqrt(layer.Length);
            for(int y=offsetY;y<offsetY+kernelSize;y++) {
                for(int x=offsetX;x<offsetX+kernelSize;x++) {
                    yield return layer[y*size + x];
                }
            }
        }



    }
}

using System;

namespace HAKGERSoft.Synapse.Tools {

    public static class BinaryTool {

        public static int[] IntToBinary(int n,int bitsCount) {
            Utility.Verify(bitsCount,b => n-1<=Math.Pow(2,bitsCount),"bitsCount must be greater");
            int[] binary=new int[bitsCount];
            int i = 0;
            do {
                binary[i++]=n%2;
            }
            while((n/=2)>0);
            return binary;
        }

        public static int BinaryToInt(int[] binary){
            int ans=0;
            for(int i=0;i<=binary.Length-1;++i) {
                ans=ans+(int)Math.Pow(2,i) * binary[i];
            }
            return ans;
        }

        public static int[] BinaryToGray(int[] binary){
            int[] gray = new int[binary.Length];
            gray[binary.Length-1] = binary[binary.Length-1]; // copy high-order bit
            for(int i=binary.Length-2; i>=0; --i){ // remaining bits
                if(binary[i]==0 && binary[i+1]==0)
                    gray[i]=0;
                else if(binary[i]==1 && binary[i+1]==1)
                    gray[i]=0;
                else if(binary[i]==0 && binary[i+1]==1)
                    gray[i]=1;
                else if(binary[i]==1 && binary[i+1]==0)
                    gray[i]=1;
            }
            return gray;
        }

        public static int[] GrayToBinary(int[] gray) {
            int[] binary = new int[gray.Length];
            binary[gray.Length-1] = gray[gray.Length-1]; // copy high-order bit
            for(int i=gray.Length-2; i>=0; --i) { // remaining bits
                if(gray[i]==0 && binary[i+1]==0)
                    binary[i]=0;
                else if(gray[i]==1 && binary[i+1]==1)
                    binary[i]=0;
                else if(gray[i]==0 && binary[i+1]==1)
                    binary[i]=1;
                else if(gray[i]==1 && binary[i+1]==0)
                    binary[i]=1;
            }
            return binary;
        }

        public static int[] IntToGray(int n,int bitsCount) {
            int[] binary = IntToBinary(n,bitsCount);
            int[] gray = BinaryToGray(binary);
            return gray;
        }

        public static int GrayToInt(int[] gray) {
            int[] binary = GrayToBinary(gray);
            int n=BinaryToInt(binary);
            return n;
        }

        public static double[] IntArray2Double(int[] array){
            double[] toret=new double[array.Length];
            Array.Copy(array,toret,array.Length);
            return toret;
        }

        public static double[] GetMarker(int marker,int count,double posValue,double negValue) {
            double[] toret=new double[count];
            for(int i=0; i<count; i++) {
                double v=negValue;
                if(i==marker)
                    v=posValue;
                toret[i]=v;
            }
            return toret;
        }

    }
}

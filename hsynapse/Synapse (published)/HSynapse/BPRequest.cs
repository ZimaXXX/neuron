using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class BPRequest {
        static readonly Random Seed=new Random();
        readonly TrainingData[] TrainingSet;

        public readonly int MaxEpochs;
        public readonly int MinEpochs;
        public readonly double MaxMSE;

        public bool ShuffleTrainingSet;

        public BPRequest(TrainingData[] trainingSet,int maxEpochs)
            :this(trainingSet,maxEpochs,0){
        }

        public BPRequest(TrainingData[] trainingSet,int maxEpochs,int minEpochs)
            : this(trainingSet,maxEpochs,minEpochs,0) {
        }

        public BPRequest(TrainingData[] trainingSet,int maxEpochs,int minEpochs,double maxMSE){
            Utility.Verify(() => trainingSet!=null,"BPRequest trainingSet");
            Utility.Verify(() => maxEpochs>0,"BPRequest maxEpochs");
            Utility.Verify(() => minEpochs>=0,"BPRequest minEpochs");
            Utility.Verify(() => minEpochs<maxEpochs,"BPRequest minEpochs<maxEpochs");

            TrainingSet=trainingSet;
            MaxEpochs=maxEpochs;
            MinEpochs=minEpochs;
            MaxMSE=maxMSE;
        }

        public IEnumerable<TrainingData> GetTrainingData() {
            var toret=TrainingSet;
            if(ShuffleTrainingSet)
                toret=Shuffle(toret).ToArray();
            foreach(TrainingData td in TrainingSet)
                yield return td;
        }

        ICollection<T> Shuffle<T>(ICollection<T> c) {
            T[] a = new T[c.Count];
            c.CopyTo(a,0);
            byte[] b = new byte[a.Length];
            Seed.NextBytes(b);
            Array.Sort(b,a);
            return new List<T>(a);
        }

        

    }
}
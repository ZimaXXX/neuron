using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace HAKGERSoft.Synapse {

    public class Utility {

        public static IEnumerable<T> Generate<T>(Func<T> generator, int count) {
            for(int i=0; i<count; i++)
                yield return generator();
        }

        public static IEnumerable<T> Generate<T>(Func<int,T> generator,int count) {
            for(int i=0;i<count;i++)
                yield return generator(i);
        }

        public static IEnumerable<T> Generate<T>(int count) where T:new() {
            for(int i=0;i<count;i++)
                yield return new T();
        }

        public static void ExecCtor<T>(T[] array) where T:new() {
            for(int i=0; i<array.Length; i++)
                array[i] = new T();
        }

        public static void ExecCtor<T>(T[][] array) where T: new() {
            for(int i=0; i<array.Length; i++) {
                ExecCtor<T>(array[i]);
            }
        }

        public static void ExecCtor<T,D>(T[] array) where D:T,new() {
            for(int i=0;i<array.Length;i++)
                array[i] = new D();
        }

        public static void Verify<T>(T x, Func<T, bool> pred, string msg) {
            if(!pred(x))
                throw new ArgumentException(msg);
        }

        public static void Verify<T>(T x, T y ,Func<T,T,bool> pred, string msg) {
            if(!pred(x, y))
                throw new ArgumentException(msg);
        }

        public static void Verify(Func<bool> pred) {
            if(!pred())
                throw new ArgumentException();
        }

        public static void Verify(Func<bool> pred,string msg) {
            if(!pred())
                throw new ArgumentException(msg);
        }

        public static void Each<T>(T[] t,Action<T> action) {
            for(int i=0; i<t.Length; i++)
                action(t[i]);
        }

        public static void EachPair<T,V>(T[] t, V[] v, Action<T,V> action) {
            for(int i=0; i<t.Length; i++)
                action(t[i], v[i]);
        }

        public static int FirstNull<T>(T[] array) where T:class{
            return Array.IndexOf(array,null);
        }
        
    }
}

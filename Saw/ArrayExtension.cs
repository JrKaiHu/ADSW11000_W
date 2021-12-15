using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProVLib;

namespace Saw
{
    public static class ArrayExtension
    {
        public static T[] MoveToLastByIdx<T>(this T[] arr, int nIdx)
        {
            T dr = arr[nIdx].Copy();
            RemoveAt(ref arr, nIdx);

            List<T> tmpLst = arr.ToList();
            tmpLst.Add(dr);

            return tmpLst.ToArray();
        }

        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                // moving elements downwards, to fill the gap at [index]
                arr[a] = arr[a + 1];
            }

            // finally, let's decrement Array's size by one
            Array.Resize(ref arr, arr.Length - 1);
        }
    }
}

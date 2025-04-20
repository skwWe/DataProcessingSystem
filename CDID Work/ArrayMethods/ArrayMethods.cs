using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArrayMethods
{
    public class ArrayClass
    {
        public int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public int[] ArraySort()
        {
            Array.Sort(array);
            //Console.WriteLine("");
            foreach (var value in array)
                Console.WriteLine(value + " ");
            return array;
        }

        public void ArrayFilter()
        {
            int j = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] % 3 == 0)
                {
                    array[j] = array[i];
                    j++;
                }
            }


            if (j < array.Length)
            {
                Array.Resize(ref array, j);
            }
        }

        public int ArraySum()
        {
            int sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            return sum;
        }

    }
}

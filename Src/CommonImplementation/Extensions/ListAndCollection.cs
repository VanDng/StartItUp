using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonImplementation.Extensions
{
    static public class ListAndCollection
    {
        static public ObservableCollection<T> ToObservableCollection<T>(this T[] array)
        {
            ObservableCollection<T> collection = new ObservableCollection<T>();

            if (array != null)
            {
                foreach (var item in array)
                {
                    collection.Add(item);
                }
            }

            return collection;
        }

        static public void AddRange<T>(this ObservableCollection<T> observableCollection, T[] newItems)
        {
            foreach(var item in newItems)
            {
                observableCollection.Add(item);
            }
        }
    }
}

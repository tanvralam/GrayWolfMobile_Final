using MvvmHelpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GrayWolf.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items) =>
            new ObservableCollection<T>(items ?? new List<T>());

        public static ObservableRangeCollection<T> ToObservableRangeCollection<T>(this IEnumerable<T> items) =>
            new ObservableRangeCollection<T>(items ?? new List<T>());

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items.ToList()) // ToList to avoid modifying while iterating
            {
                collection.Remove(item);
            }
        }
    }
}

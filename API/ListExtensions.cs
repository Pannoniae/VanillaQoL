using System.Collections.Generic;

namespace VanillaQoL.API;

public static class ListExtensions {
    /// <summary>
    /// Add an item after a specified element to a list.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="element">The element to add after.</param>
    /// <param name="item">The item to add.</param>
    /// <typeparam name="T"></typeparam>
    public static void AddAfter<T>(this List<T> list, T element, T item) {
        var idx = list.IndexOf(element);
        list.Insert(idx + 1, item);
    }
    
    public static void AddRangeN1(this List<int> list, IEnumerable<int> collection) {
        foreach (var item in collection) {
            if (item != 0 && item != -1) {
                list.Add(item);
            }
        }
    }
}
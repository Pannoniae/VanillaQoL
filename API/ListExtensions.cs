using System.Collections.Generic;

namespace ZenithQoL.API;

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
}
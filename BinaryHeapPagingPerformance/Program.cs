using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BinaryHeapPagingPerformance {
  class Program {
    static void Main(string[] args) {
      var start = 80;
      var length = 20;
      var dataToSortAndPage = new Guid[10000000];

      for(int i = dataToSortAndPage.Length; i-- > 0; ) dataToSortAndPage[i] = Guid.NewGuid();

      var stopwatch = new Stopwatch();
      var sortAndPage = new ISortAndPage[] { new SortAll(), new PriorityQueueUsingSortedList() };
      var sortedAndPaged = new Guid[sortAndPage.Length][];
      for(var i = sortAndPage.Length; i-- > 0; ) {
        stopwatch.Reset();
        stopwatch.Start();
        sortedAndPaged[i] = sortAndPage[i].SortAndPage(start, length, dataToSortAndPage);
        stopwatch.Stop();
        Console.WriteLine("{0}: {1}", sortAndPage[i].Name, stopwatch.Elapsed);
      }

      for(var i = sortedAndPaged.Length - 1; i-- > 0; ) {
        sortedAndPaged[i].ShouldEqual(sortedAndPaged[i + 1]);
      }
    }
  }

  static class GuidArrayExtensionMethods {
    public static void ShouldEqual(this Guid[] me, Guid[] other) {
      if(me.Length != other.Length) throw new ApplicationException("Arrays are not the same length. " + me.Length + " vs " + other.Length + ".");
      for(var i = me.Length; i-- > 0; ) if(me[i] != other[i]) throw new ApplicationException("Values at " + i + " are not the same. " + me[i] + " vs " + other[i] + ".");
    }
  }

  interface ISortAndPage {
    string Name { get; }
    Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage);
  }

  class SortAll : ISortAndPage {
    public string Name { get { return "Sort All"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      Array.Sort(dataToSortAndPage);
      var sortedAndPaged = new Guid[length];
      Array.Copy(dataToSortAndPage, start, sortedAndPaged, 0, length);
      return sortedAndPaged;
    }
  }

  class PriorityQueueUsingSortedList : ISortAndPage {
    public string Name { get { return "Priority Queue Using Sorted List"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      var maxItems = start + length;
      var upperBound = maxItems - 1;
      var queue = new SortedList<Guid, object>(maxItems + 1);

      for(var i = maxItems; i-- > 0; ) queue[dataToSortAndPage[i]] = null;

      var maxItem = queue.Keys[upperBound];
      for(var i = dataToSortAndPage.Length; i-- > maxItems; ) {
        var currentItem = dataToSortAndPage[i];
        if(currentItem.CompareTo(maxItem) < 0) {
          queue[currentItem] = null;
          queue.RemoveAt(maxItems);
          maxItem = queue.Keys[upperBound];
        }
      }

      return queue.Keys.Skip(start).Take(length).ToArray();
    }
  }
}

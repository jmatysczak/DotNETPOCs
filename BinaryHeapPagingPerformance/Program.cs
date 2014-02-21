using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BinaryHeapPagingPerformance {
  class Program {
    static void Main(string[] args) {
      var start = 80;
      var length = 20;
      var dataToSortAndPage = new Guid[20000000];

      for(var i = dataToSortAndPage.Length; i-- > 0; ) dataToSortAndPage[i] = Guid.NewGuid();

      var stopwatch = new Stopwatch();
      var sortAndPage = new ISortAndPage[] { new SortAll(), /*new SortAllWithLinq(),*/ new PriorityQueueUsingSortedList(), new PriorityQueueUsingBinaryHeap(), new PriorityQueueUsingSortedList(), new PriorityQueueUsingBinaryHeap(), new PriorityQueueUsingSortedList(), new PriorityQueueUsingBinaryHeap() };
      var sortedAndPaged = new Guid[sortAndPage.Length][];
      for(var i = sortAndPage.Length; i-- > 0; ) {
        stopwatch.Reset();
        stopwatch.Start();
        sortedAndPaged[i] = sortAndPage[i].SortAndPage(start, length, dataToSortAndPage);
        stopwatch.Stop();
        Console.WriteLine("{0}: {1}", sortAndPage[i].Name, stopwatch.Elapsed);
      }

      stopwatch.Reset();
      stopwatch.Start();
      Guid empty = Guid.Empty;
      for(var i = dataToSortAndPage.Length; i-- > 0; ) dataToSortAndPage[i].CompareTo(empty);
      stopwatch.Stop();
      Console.WriteLine("Loop and compare: {0}", stopwatch.Elapsed);

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
      return dataToSortAndPage.Skip(start).Take(length).ToArray();
    }
  }

  class SortAllWithLinq : ISortAndPage {
    public string Name { get { return "Sort All With Linq"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      return dataToSortAndPage.OrderBy(d => d).Skip(start).Take(length).ToArray();
    }
  }

  class PriorityQueueUsingSortedList : ISortAndPage {
    public string Name { get { return "Priority Queue Using Sorted List"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      var maxItems = start + length;
      var upperBound = maxItems - 1;
      var queue = new SortedList<Guid, object>(maxItems + 1);

      for(var i = dataToSortAndPage.Length - maxItems; i < dataToSortAndPage.Length; i++) queue[dataToSortAndPage[i]] = null;

      var keys = queue.Keys;
      var maxItem = keys[upperBound];
      for(var i = dataToSortAndPage.Length - maxItems; i-- > 0; ) {
        if(dataToSortAndPage[i].CompareTo(maxItem) >= 0) continue;
        var currentItem = dataToSortAndPage[i];
        queue[currentItem] = null;
        queue.RemoveAt(maxItems);
        maxItem = keys[upperBound];
      }

      return keys.Skip(start).Take(length).ToArray();
    }
  }

  class PriorityQueueUsingBinaryHeap : ISortAndPage {
    public string Name { get { return "Priority Queue Using Binary Heap"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      var maxItems = start + length;
      if(maxItems % 2 == 0) maxItems++; // Ensure there is always a "right" child.
      var upperBound = maxItems - 1;
      var heap = new Guid[maxItems];

      /*
            for(int i = dataToSortAndPage.Length - maxItems, j = 0; i < dataToSortAndPage.Length; i++, j++) {
              var currentIndex = j;
              var currentItem = dataToSortAndPage[i];
              while(currentIndex > 0) {
                var parentIndex = (currentIndex - 1) / 2;
                var parentItem = heap[parentIndex];
                if(currentItem.CompareTo(parentItem) <= 0) break;
                heap[currentIndex] = parentItem;
                currentIndex = parentIndex;
              }
              heap[currentIndex] = currentItem;
            }
      */

      // Funny shit.
      Array.Copy(dataToSortAndPage, dataToSortAndPage.Length - maxItems, heap, 0, maxItems);
      Array.Sort(heap);
      Array.Reverse(heap);

      var maxItem = heap[0];
      for(var i = dataToSortAndPage.Length - maxItems; i-- > 0; ) {
        if(dataToSortAndPage[i].CompareTo(maxItem) >= 0) continue;

        var currentItem = dataToSortAndPage[i];
        var parentIndex = 0;
        var maxChildIndex = 1; // parentIndex * 2 + 1;
        do {
          var rightChildIndex = maxChildIndex + 1;
          var maxChildItem = heap[maxChildIndex];
          var rightChildItem = heap[rightChildIndex];
          if(rightChildItem.CompareTo(maxChildItem) > 0) {
            maxChildItem = rightChildItem;
            maxChildIndex = rightChildIndex;
          }
          if(currentItem.CompareTo(maxChildItem) >= 0) break;
          heap[parentIndex] = maxChildItem;
          parentIndex = maxChildIndex;
          maxChildIndex = parentIndex * 2 + 1;
        } while(maxChildIndex <= upperBound);
        heap[parentIndex] = currentItem;
        maxItem = heap[0];
      }

      return heap.OrderBy(g => g).Skip(start).Take(length).ToArray();
    }
  }
}

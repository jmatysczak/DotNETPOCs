using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SortingAndPagingPerformance {
  class Program {
    static Stopwatch STOPWATCH = new Stopwatch();

    static void Main(string[] args) {
      var start = 80;
      var length = 20;
      var iterations = 5;
      var dataToSortAndPage = new Guid[10000000];

      time("Created: {0}", 0, () => {
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i] = Guid.NewGuid();
        return dataToSortAndPage;
      });

      time("Baseline (loop and compare): {0}", 0, () => {
        Guid empty = Guid.Empty;
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i].CompareTo(empty);
        return dataToSortAndPage;
      });

      var sortAndPages = new ISortAndPage[] { new PriorityQueueUsingSortedList(), new PriorityQueueUsingBinaryHeap() };
      var actuals = sortAndPages.Select(sortAndPage => {
        return time(sortAndPage.Name + "(Random): {0}", iterations, () => sortAndPage.SortAndPage(start, length, dataToSortAndPage));
      }).ToArray();

      var fullSort = new FullSort();
      var expected = time(fullSort.Name + ": {0}", 0, () => fullSort.SortAndPage(start, length, dataToSortAndPage));

      for(var i = 0; i < actuals.Length; i++) {
        actuals[i].ShouldEqual(expected);
      }
    }

    delegate Guid[] Action();
    static Guid[] time(string descr, int iterations, Action action) {
      Guid[] result;
      // Warm up
      STOPWATCH.Reset();
      STOPWATCH.Start();
      result = action();
      STOPWATCH.Stop();
      var timeSpan = STOPWATCH.Elapsed;

      if(iterations > 0) {
        timeSpan = TimeSpan.Zero;
        for(var i = 0; i < iterations; i++) {
          STOPWATCH.Reset();
          STOPWATCH.Start();
          result = action();
          STOPWATCH.Stop();
          timeSpan += STOPWATCH.Elapsed;
        }
        timeSpan = TimeSpan.FromTicks(timeSpan.Ticks / iterations);
      }

      Console.WriteLine(descr, timeSpan);

      return result;
    }
  }

  static class GuidArrayExtensionMethods {
    public static void ShouldEqual(this Guid[] me, Guid[] other) {
      if(me.Length != other.Length) throw new ApplicationException("Arrays are not the same length. " + me.Length + " vs " + other.Length + ".");
      for(var i = 0; i < me.Length; i++) if(me[i] != other[i]) throw new ApplicationException("Values at " + i + " are not the same. " + me[i] + " vs " + other[i] + ".");
    }
  }

  interface ISortAndPage {
    string Name { get; }
    Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage);
  }

  class FullSort : ISortAndPage {
    public string Name { get { return "Full Sort"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      Array.Sort(dataToSortAndPage);
      return dataToSortAndPage.Skip(start).Take(length).ToArray();
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

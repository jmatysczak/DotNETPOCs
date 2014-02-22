using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SortingAndPagingPerformance {
  class Program {
    static Stopwatch STOPWATCH = new Stopwatch();

    static void Main(string[] args) {
      var length = 20;
      var iterations = 5;
      var fullSortAndPage = new FullSort();
      var dataToSortAndPage = new Guid[10000000];
      var sortAndPageTypes = new ISortAndPage[] { new PriorityQueueUsingSortedList(), new PriorityQueueUsingBinaryHeap() };

      time("Created", 0, () => {
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i] = Guid.NewGuid();
        return dataToSortAndPage;
      });

      time("Baseline (loop and compare)", 0, () => {
        Guid empty = Guid.Empty;
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i].CompareTo(empty);
        return dataToSortAndPage;
      });


      foreach(var start in new int[] { 80, 180 }) {
        Console.WriteLine();

        var actuals = sortAndPageTypes.Select(sortAndPageType => {
          return time(sortAndPageType.Name + " (Random) s" + start, iterations, () => sortAndPageType.SortAndPage(start, length, dataToSortAndPage));
        }).ToArray();

        var expected = time(fullSortAndPage.Name, 0, () => fullSortAndPage.SortAndPage(start, length, dataToSortAndPage));

        actuals = sortAndPageTypes.Select(sortAndPageType => {
          return time(sortAndPageType.Name + " (Ascending) s" + start, iterations, () => sortAndPageType.SortAndPage(start, length, dataToSortAndPage));
        }).Concat(actuals).ToArray();

        Array.Reverse(dataToSortAndPage);

        actuals = sortAndPageTypes.Select(sortAndPageType => {
          return time(sortAndPageType.Name + " (Descending) s" + start, iterations, () => sortAndPageType.SortAndPage(start, length, dataToSortAndPage));
        }).Concat(actuals).ToArray();

        for(var i = 0; i < actuals.Length; i++) actuals[i].ShouldEqual(expected);

        // Randomize for the next iteration.
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i] = Guid.NewGuid();
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

      Console.WriteLine(descr + ": {0}", timeSpan);

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

      for(var i = 0; i < maxItems; i++) queue[dataToSortAndPage[i]] = null;

      var keys = queue.Keys;
      var maxItem = keys[upperBound];
      for(var i = maxItems; i < dataToSortAndPage.Length; i++) {
        if(dataToSortAndPage[i].CompareTo(maxItem) >= 0) continue;
        queue[dataToSortAndPage[i]] = null;
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

      for(int i = 0; i < maxItems; i++) {
        var currentIndex = i;
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

      /* An alternative to the above.
      Array.Copy(dataToSortAndPage, 0, heap, 0, maxItems);
      Array.Sort(heap);
      Array.Reverse(heap);
      */

      var maxItem = heap[0];
      for(var i = maxItems; i < dataToSortAndPage.Length; i++) {
        if(dataToSortAndPage[i].CompareTo(maxItem) >= 0) continue;

        var currentItem = dataToSortAndPage[i];
        var parentIndex = 0;
        var leftChildIndex = 1;
        do {
          var rightChildIndex = leftChildIndex + 1;
          var maxChildIndex = heap[leftChildIndex].CompareTo(heap[rightChildIndex]) < 0 ? rightChildIndex : leftChildIndex;
          if(heap[maxChildIndex].CompareTo(currentItem) <= 0) break;
          heap[parentIndex] = heap[maxChildIndex];
          parentIndex = maxChildIndex;
          leftChildIndex = parentIndex * 2 + 1;
        } while(leftChildIndex <= upperBound);
        heap[parentIndex] = currentItem;
        maxItem = heap[0];
      }

      return heap.OrderBy(g => g).Skip(start).Take(length).ToArray();
    }
  }
}

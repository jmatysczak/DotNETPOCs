using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SortingAndPagingPerformance {
  class Program {
    static Stopwatch STOPWATCH = new Stopwatch();

    static void Main(string[] args) {
      var pageSize = 20;
      var iterations = 5;
      var fullSortAndPage = new FullSort();
      var dataToSortAndPage = new Guid[10000000];
      var sortAndPageImplementations = new ISortAndPage[] { new PriorityQueueUsingASortedList(), new PriorityQueueUsingBinaryHeap() };

      Time("Created Data", 0, () => {
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i] = Guid.NewGuid();
        return null;
      });

      Time("Baseline (loop and compare)", 0, () => {
        Guid empty = Guid.Empty;
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i].CompareTo(empty);
        return null;
      });


      foreach(var pageStart in new int[] { 80, 180 }) {
        Console.WriteLine();

        var actuals = sortAndPageImplementations.Select(sortAndPageImplementation => {
          return Time(sortAndPageImplementation.Name + " (Random) s" + pageStart, iterations, () => sortAndPageImplementation.SortAndPage(pageStart, pageSize, dataToSortAndPage));
        }).ToArray();

        var expected = Time(fullSortAndPage.Name, 0, () => fullSortAndPage.SortAndPage(pageStart, pageSize, dataToSortAndPage));

        actuals = sortAndPageImplementations.Select(sortAndPageImplementation => {
          return Time(sortAndPageImplementation.Name + " (Ascending) s" + pageStart, iterations, () => sortAndPageImplementation.SortAndPage(pageStart, pageSize, dataToSortAndPage));
        }).Concat(actuals).ToArray();

        Time("Reverse", 0, () => {
          Array.Reverse(dataToSortAndPage);
          return null;
        });

        actuals = sortAndPageImplementations.Select(sortAndPageImplementation => {
          return Time(sortAndPageImplementation.Name + " (Descending) s" + pageStart, iterations, () => sortAndPageImplementation.SortAndPage(pageStart, pageSize, dataToSortAndPage));
        }).Concat(actuals).ToArray();

        for(var i = 0; i < actuals.Length; i++) actuals[i].ShouldEqual(expected);

        // Randomize for the next iteration.
        for(var i = 0; i < dataToSortAndPage.Length; i++) dataToSortAndPage[i] = Guid.NewGuid();
      }
    }

    delegate Guid[] OperationToTime();
    static Guid[] Time(string descr, int iterations, OperationToTime action) {
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
      // A little over twice as slow...and doesn't sort in place - so it wrecks the subsequent tests.
      //return dataToSortAndPage.OrderBy(d => d).Skip(pageStart).Take(pageSize).ToArray();
      Array.Sort(dataToSortAndPage);
      return dataToSortAndPage.Skip(start).Take(length).ToArray();
    }
  }

  // A priority queue keeps track of the "top" N items.
  // In the below two cases "top" means the least/lowest items based on IComparable.CompareTo.
  // "N" is the offset we want to start at plus the number of items we want.
  // In otherwords, we keep track of all of the items that make up the "page" of data we want and all previous pages.
  // NOTE: The worst case example is when the items are sorted in descending order because ever item is put into the queue.

  // A priority queue that uses the standard .NET SortedList to keep track of its items.
  // The SortedList keeps the code simple.
  // But it has a higher performance cost as the number of items being put into the queue increases.
  // This is because the SortedList is ensuring that the items in it are always sorted...which, of course, we don't care about.
  class PriorityQueueUsingASortedList : ISortAndPage {
    public string Name { get { return "Priority Queue Using A Sorted List"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      var totalLength = start + length;
      var upperBound = totalLength - 1;
      var queue = new SortedList<Guid, object>(totalLength + 1);

      for(var i = 0; i < totalLength; i++) queue[dataToSortAndPage[i]] = null;

      var keys = queue.Keys;
      var maxItem = keys[upperBound];
      for(var i = totalLength; i < dataToSortAndPage.Length; i++) {
        if(dataToSortAndPage[i].CompareTo(maxItem) > 0) continue;
        queue[dataToSortAndPage[i]] = null;
        queue.RemoveAt(totalLength);
        maxItem = keys[upperBound];
      }

      return keys.Skip(start).Take(length).ToArray();
    }
  }

  // A priority queue that uses a binary heap to keep track of its items.
  // Ideally, or rather, in a "real" implementation, we'd create a class that represents the binary heap.
  // This would probably make the code simpler than the implementation above.
  // A binary heap has a lower performance cost compared to the SortedList as the number of items being put into the queue increases.
  // This is because the binary heap is only keeping a "loose" ordering of items.
  class PriorityQueueUsingBinaryHeap : ISortAndPage {
    public string Name { get { return "Priority Queue Using Binary Heap"; } }

    public Guid[] SortAndPage(int start, int length, Guid[] dataToSortAndPage) {
      var totalLength = start + length;
      if(totalLength % 2 == 0) totalLength++; // Ensure there is always a "right" child.
      var upperBound = totalLength - 1;
      var heap = new Guid[totalLength];

      for(int i = 0; i < totalLength; i++) {
        var currentIndex = i;
        var currentItem = dataToSortAndPage[i];
        while(currentIndex > 0) {
          var parentIndex = (currentIndex - 1) / 2;
          if(heap[parentIndex].CompareTo(currentItem) >= 0) break;
          heap[currentIndex] = heap[parentIndex];
          currentIndex = parentIndex;
        }
        heap[currentIndex] = currentItem;
      }

      /* An alternative to the above.
      Array.Copy(dataToSortAndPage, 0, heap, 0, totalLength);
      Array.Sort(heap);
      Array.Reverse(heap);
      */

      var maxItem = heap[0];
      for(var i = totalLength; i < dataToSortAndPage.Length; i++) {
        if(dataToSortAndPage[i].CompareTo(maxItem) > 0) continue;

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

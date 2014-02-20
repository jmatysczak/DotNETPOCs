using System;
using System.Diagnostics;

namespace BinaryHeapPagingPerformance {
  class Program {
    static void Main(string[] args) {
      var start = 80;
      var length = 20;
      var dataToSortAndPage = new Guid[10000000];

      for(int i = dataToSortAndPage.Length; i-- > 0; ) dataToSortAndPage[i] = Guid.NewGuid();

      var stopwatch = new Stopwatch();
      var sortAndPage = new ISortAndPage[] { new SortAll() };
      var sortedAndPaged = new Guid[sortAndPage.Length][];
      for(var i = sortAndPage.Length; i-- > 0; ) {
        stopwatch.Reset();
        stopwatch.Start();
        sortedAndPaged[i] = sortAndPage[i].SortAndPage(start, length, dataToSortAndPage);
        stopwatch.Stop();
        Console.WriteLine("{0}: {1}", sortAndPage[i].Name, stopwatch.Elapsed);
      }
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
}

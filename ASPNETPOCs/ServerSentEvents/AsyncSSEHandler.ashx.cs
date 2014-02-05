using System;
using System.Threading;
using System.Web;
using Timer = System.Timers.Timer;

namespace ASPNETPOCs.ServerSentEvents {
  public class AsyncSSEHandler : IHttpAsyncHandler {
    private Timer intervalTimer;

    public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
      var end = DateTime.Now.AddSeconds(10);
      var result = new SSEAsyncResult(context, cb, extraData);

      context.Response.ContentType = "text/event-stream";
      context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

      this.intervalTimer = new Timer(1000);
      this.intervalTimer.Elapsed += delegate {
        if(DateTime.Now < end) {
          context.Response.Output.Write("data: Timestamp: {0:s}, Thread ID: {1}\n\n", DateTime.Now, Thread.CurrentThread.ManagedThreadId);
          context.Response.Flush();
        } else {
          this.intervalTimer.Stop();
          result.SetComplete();
        }
      };
      this.intervalTimer.Start();

      return result;
    }

    public void EndProcessRequest(IAsyncResult result) {
    }

    public bool IsReusable { get { return false; } }

    public void ProcessRequest(HttpContext context) { throw new NotImplementedException(); }
  }

  public class SSEAsyncResult : IAsyncResult {
    private readonly object state;
    private readonly AsyncCallback cb;
    private readonly HttpContext context;

    public SSEAsyncResult(HttpContext context, AsyncCallback cb, object state) {
      this.cb = cb;
      this.state = state;
      this.context = context;
    }

    public void SetComplete() {
      this.isCompleted = true;
      this.cb(this);
    }

    public object AsyncState { get { return this.state; } }

    public WaitHandle AsyncWaitHandle { get { throw new NotImplementedException(); } }

    public bool CompletedSynchronously { get { return false; } }

    private bool isCompleted = false;
    public bool IsCompleted { get { return this.isCompleted; } }
  }
}

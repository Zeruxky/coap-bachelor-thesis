private Task<int> m_lastTask;
 
public override Task<int> ReadAsync(
  byte [] buffer, int offset, int count,
  CancellationToken cancellationToken)
{
  if (cancellationToken.IsCancellationRequested) {
    var tcs = new TaskCompletionSource<int>();
    tcs.SetCanceled();
    return tcs.Task;
  }
 
  try {
      int numRead = this.Read(buffer, offset, count);
      return m_lastTask != null && numRead == m_lastTask.Result ?
        m_lastTask : (m_lastTask = Task.FromResult(numRead));
  }
  catch(Exception e) {
    var tcs = new TaskCompletionSource<int>();
    tcs.SetException(e);
    return tcs.Task;
  }
}
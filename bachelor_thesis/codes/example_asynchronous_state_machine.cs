[DebuggerStepThrough]     
public static Task SimpleBodyAsync() {
  <SimpleBodyAsync>d__0 d__ = new <SimpleBodyAsync>d__0();
  d__.<>t__builder = AsyncTaskMethodBuilder.Create();
  d__.MoveNext();
  return d__.<>t__builder.Task;
}
 
[CompilerGenerated]
[StructLayout(LayoutKind.Sequential)]
private struct <SimpleBodyAsync>d__0 : <>t__IStateMachine {
  private int <>1__state;
  public AsyncTaskMethodBuilder <>t__builder;
  public Action <>t__MoveNextDelegate;
 
  public void MoveNext() {
    try {
      if (this.<>1__state == -1) return;
      Console.WriteLine("Hello, Async World!");
    }
    catch (Exception e) {
      this.<>1__state = -1;
      this.<>t__builder.SetException(e);
      return;
    }
 
    this.<>1__state = -1;
    this.<>t__builder.SetResult();
  }
 
  ...
}
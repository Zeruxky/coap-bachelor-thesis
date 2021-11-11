public class StreamWriter
{
    public async Task WriteAsync(
        Stream stream,
        ReadonlyMemory<byte> value,
        CancellationToken ct)
    {
        await stream.WriteAsync(value, ct).ConfigureAwait(false);
    }
}
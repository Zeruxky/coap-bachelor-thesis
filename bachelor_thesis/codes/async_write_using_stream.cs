public async Task WriteAsync(Stream stream, string value, CancellationToken ct)
{
    var bytes = Encoding.UTF8.GetBytes(value);
    await stream.WriteAsync(bytes, ct).ConfigureAwait(false);
}
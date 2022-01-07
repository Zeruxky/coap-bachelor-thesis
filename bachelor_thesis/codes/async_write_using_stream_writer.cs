public async Task WriteAsync(Stream stream, string value, CancellationToken ct)
{
    await using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
    {
        await writer.WriteAsync(value.AsMemory(), ct).ConfigureAwait(false);
    }
}
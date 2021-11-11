public class StreamReader
{
    private readonly IBufferWriter writer;

    public StreamReader(IBufferWriter writer)
    {
        this.writer = writer;
    }

    public async Task<byte[]> ReadAsync(Stream stream, CancellationToken ct)
    {
        while (true)
        {
            // Ausleihen eines Speicherbereichs als Puffer.
            var buffer = this.writer.GetMemory(2048);

            // Daten vom Stream lesen.
            var bytesRead = await stream.ReadAsync(buffer, ct)
            .ConfigureAwait(false);

            // Keinen Daten mehr im Stream.
            if (bytesRead == -1)
            {
                // Lesevorgang beenden.
                break;
            }

            // Position des IBufferWriters verschieben.
            this.writer.Advance(bytesRead);
        }

        // Rückgabe alles Daten, die vom Stream gelesen wurden.
        return this.writer.WrittenMemory.ToArray();
    }
}
public async Task<byte[]> ReadAsync(Stream stream, CancellationToken ct)
{
    // Ausleihen eines Speichers.
    var owner = MemoryPool<byte>.Shared.Rent(2048);
    var buffer = owner.Memory;

    // Überprüfen, ob der gesamte Stream in den Puffer geladen werden kann.
    if (stream.Length <= 2048)
    {
        // Lesen des Speichers in synchroner Variante.
        var bytesRead = stream.Read(buffer.Span);
        return buffer.ToArray();
    }

    // Sonst asynchrones Lesen des gesamten Streams.
    // Das passiert folgendermaßen:
    // 1) Lesen des Streams mittels ReadAsync.
    // 2) Überprüfen ob Ende des Streams erreicht wurde.
    // 2.1) Wenn ja --> den Inhalt des Puffers zurückgeben.
    // 2.2) Wenn nein --> Puffer verdoppeln und mit Punkt 1 beginnen.
}
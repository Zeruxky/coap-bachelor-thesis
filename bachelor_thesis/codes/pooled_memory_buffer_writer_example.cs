// Erzeugung des PooledMemoryBufferWriters.
using (var writer = new PooledMemoryBufferWriter())
{
    // Anfordern eines Memory<byte> in der Größe von 2048 Bytes.
    var buffer = writer.GetMemory(2048);

    // Erzeugung eines Zufallgenerators.
    var random = new Random();

    // Befüllung des Puffers mit zufälligen Werten.
    random.NextBytes(buffer.Span);

    // Benachrichtung des Writers, dass der gesamte Puffer beschrieben wurde.
    writer.Advance(buffer.Length);
}
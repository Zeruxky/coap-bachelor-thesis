public class BenchmarkExample
{
    // Initialisierung eines Zufallgenerators.
    private static readonly Random Random = new Random();
    private readonly byte[] data;
    private readonly SHA256 sha256 = SHA256.Create();
    private readonly MD5 md5 = MD5.Create();

    // Initialisierung der Daten für den Benchmark
    public BenchmarkExample()
    {
        this.data = new byte[10000];
        this.Random.NextBytes(this.data);
    }

    // Deklarierung eines Benchmarks für SHA256 Berechnung.
    [Benchmark]
    public byte[] SHA256 => this.sha256.ComputeHash(this.data);

    // Deklarierung eines Benchmarks für MD5 Berechnung.
    [Benchmark]
    public byte[] MD5 => this.md5.ComputeHash(this.data);
}

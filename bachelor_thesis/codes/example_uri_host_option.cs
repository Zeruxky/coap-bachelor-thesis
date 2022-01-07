public class UriHost : StringOption
{
    public const ushort NUMBER = OptionNumberRegistry.StringOptions.UriHost;
    private const ushort MAX_LENGTH = 255;
    private const ushort MIN_LENGTH = 1;

    public UriHost(string value)
        : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
    {
    }

    public UriHost(ReadOnlySpan<byte> value)
        : base(NUMBER, value, MIN_LENGTH, MAX_LENGTH, false)
    {
    }

    public class Factory : IOptionFactory
    {
        public int Number => NUMBER;

        public CoapOption Create(OptionData src)
        {
            if (src.Number != this.Number)
            {
                var msg = "Option number is not valid for Uri-Host factory.";
                throw new ArgumentException(msg);
            }

            return new UriHost(src.Value);
        }
    }
}
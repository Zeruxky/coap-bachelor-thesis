public class OptionsReader : IReader<ReadOnlyOptionCollection>
{
    private static readonly IOptionFactory UnrecognizedOptionFactory = new UnrecognizedOptionFactory();

    private const byte MASK_DELTA = 0xF0;
    private const byte MASK_LENGTH = 0x0F;
    private const byte PAYLOAD_MARKER = 0xFF;

    private readonly SortedDictionary<int, IOptionFactory> factories;

    public OptionsReader(IEnumerable<IOptionFactory> factories)
    {
        this.factories = new SortedDictionary<int, IOptionFactory>(factories.ToDictionary(f => f.Number));
    }

    public int Read(ReadOnlySpan<byte> value, out ReadOnlyOptionCollection result)
    {
        var options = new OptionCollection();
        var offset = 0;
        var previousNumber = 0;
        while (OptionsReader.HasNext(value[offset..]))
        {
            var optionData = this.ReadOptionData(previousNumber, value[offset..], out var bytesConsumed);
            previousNumber = optionData.Number;
            offset += bytesConsumed;

            if (!this.factories.TryGetValue(optionData.Number, out var factory))
            {
                factory = UnrecognizedOptionFactory;
            }

            var option = factory.Create(optionData);
            options.Add(option);
        }

        result = new ReadOnlyOptionCollection(options);
        return offset;
    }

    public async IAsyncEnumerable<CoapOption> ReadAsync(Stream stream, [EnumeratorCancellation] CancellationToken ct)
    {
        var previousNumber = 0;
        while (true)
        {
            if (!this.TryReadOptionHeader(stream, out var optionHeader))
            {
                break;
            }

            if (optionHeader.IsPayloadMarker)
            {
                break;
            }

            var optionData = await this.ReadOptionAsync(stream, optionHeader, previousNumber, ct).ConfigureAwait(false);
            previousNumber = optionData.Number;
            if (!this.factories.TryGetValue(optionData.Number, out var factory))
            {
                factory = UnrecognizedOptionFactory;
            }

            var option = factory.Create(optionData);
            yield return option;
        }
    }

    private async Task<ushort> ReadOptionDeltaAsync(Stream stream, OptionHeader optionHeader, CancellationToken ct)
    {
        var delta = optionHeader.Delta;
        var length = optionHeader.Length;
        if (delta == 15 && length != 15)
        {
            throw new MessageFormatErrorException("Delta value of 15 is reserved for payload marker.");
        }

        if (delta == 14)
        {
            using (var owner = MemoryPool<byte>.Shared.Rent(2))
            {
                var buffer = owner.Memory[..2];
                _ = await stream.ReadAsync(buffer, ct).ConfigureAwait(false);
                var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer.Span) + 269);
                return extended;
            }
        }

        if (delta == 13)
        {
            var value = stream.ReadByte() + 13;
            return (ushort)value;
        }

        return delta;
    }

    private async Task<ushort> ReadOptionLengthAsync(Stream stream, OptionHeader optionHeader, CancellationToken ct)
    {
        var length = optionHeader.Length;
        if (length == 15)
        {
            throw new MessageFormatErrorException("Length value of 15 is reserved for future use.");
        }

        if (length == 14)
        {
            using (var owner = MemoryPool<byte>.Shared.Rent(2))
            {
                var buffer = owner.Memory[..2];
                _ = await stream.ReadAsync(buffer, ct).ConfigureAwait(false);
                var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer.Span) + 269);
                return extended;
            }
        }

        if (length == 13)
        {
            var value = stream.ReadByte() + 13;
            return (ushort)value;
        }

        return length;
    }

    private bool TryReadOptionHeader(Stream stream, out OptionHeader header)
    {
        var value = stream.ReadByte();
        if (value == -1)
        {
            header = default;
            return false;
        }

        header = new OptionHeader(value);
        return true;
    }

    private async Task<OptionData> ReadOptionAsync(Stream stream, OptionHeader optionHeader, int previousNumber, CancellationToken ct)
    {
        var delta = await this.ReadOptionDeltaAsync(stream, optionHeader, ct).ConfigureAwait(false);
        var length = await this.ReadOptionLengthAsync(stream, optionHeader, ct).ConfigureAwait(false);
        using (var owner = MemoryPool<byte>.Shared.Rent(length))
        {
            var buffer = owner.Memory[..length];
            _ = await stream.ReadAsync(buffer, ct).ConfigureAwait(false);
            return new OptionData((ushort)previousNumber, delta, length, buffer.Span);
        }
    }

    private OptionData ReadOptionData(int previousNumber, ReadOnlySpan<byte> src, out int bytesConsumed)
    {
        var delta = OptionsReader.ReadDelta(src);
        var length = OptionsReader.ReadLength(src, delta.Size);

        var valueOffset = 1 + delta.Size + length.Size;

        bytesConsumed = valueOffset + length.Value;

        var value = src.Slice(valueOffset, length.Value);
        return new OptionData((ushort)previousNumber, delta.Value, length.Value, value);
    }

    private static OptionsLength ReadLength(ReadOnlySpan<byte> value, int readDeltaBytes)
    {
        var length = (UInt4)(value[0] & MASK_LENGTH);
        var startOfExtendedLength = 1 + readDeltaBytes;
        if (length == 15)
        {
            throw new MessageFormatErrorException("Length value of 15 is reserved for future use.");
        }

        if (length == 14)
        {
            var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Slice(startOfExtendedLength, 2)) + 269);
            return new OptionsLength(extended, 2);
        }

        if (length == 13)
        {
            var extended = (ushort)(value[startOfExtendedLength] + 13);
            return new OptionsLength(extended, 1);
        }

        return new OptionsLength(length, 0);
    }

    private static OptionsDelta ReadDelta(ReadOnlySpan<byte> value)
    {
        var delta = (UInt4)((value[0] & MASK_DELTA) >> 4);
        var length = (UInt4)(value[0] & MASK_LENGTH);
        if (delta == 15 && length != 15)
        {
            throw new MessageFormatErrorException("Delta value of 15 is reserved for payload marker.");
        }

        if (delta == 14)
        {
            var extended = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(value.Slice(1, 2)) + 269);
            return new OptionsDelta(extended, 2);
        }

        if (delta == 13)
        {
            var extended = (ushort)(value[1] + 13);
            return new OptionsDelta(extended, 1);
        }

        return new OptionsDelta(delta, 0);
    }

    private static bool HasNext(ReadOnlySpan<byte> value)
    {
        if (value.Length == 0)
        {
            return false;
        }

        return value[0] != PAYLOAD_MARKER;
    }
}
public int Read(ReadOnlySpan<byte> value, out ReadOnlyOptionCollection result)
{
    var options = new OptionCollection();
    var offset = 0;
    var previousNumber = 0;
    while (OptionsReader.HasNext(value[offset..]))
    {
        var optionData = this.ReadOptionData(
            previousNumber,
            value[offset..],
            out var bytesConsumed);
            
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
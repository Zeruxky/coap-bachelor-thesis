public async IAsyncEnumerable<CoapOption> ReadAsync(
    Stream stream,
    [EnumeratorCancellation] CancellationToken ct)
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

        var optionData = await this.ReadOptionAsync(
            stream,
            optionHeader,
            previousNumber,
            ct)
            .ConfigureAwait(false);
            
        previousNumber = optionData.Number;
        if (!this.factories.TryGetValue(optionData.Number, out var factory))
        {
            factory = UnrecognizedOptionFactory;
        }

        var option = factory.Create(optionData);
        yield return option;
    }
}
public async Task WriteAsync(
    Stream stream,
    ReadOnlyOptionCollection options,
    CancellationToken ct)
{
    CoapOption previousOption = null;
    foreach (var option in options)
    {
        var serializer = this.serializers
        .SingleOrDefault(s => s.CanSerialize(option));
        if (serializer == null)
        {
            continue;
        }

        await serializer
        .SerializeAsync(stream, previousOption, option, ct)
        .ConfigureAwait(false);
        previousOption = option;
    }
}
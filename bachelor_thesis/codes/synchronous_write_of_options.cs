public void Write(PooledMemoryBufferWriter writer, ReadOnlyOptionCollection value)
{
    CoapOption previousOption = null;
    foreach (var option in value)
    {
        var serializer = this.serializers.SingleOrDefault(s => s.CanSerialize(option));
        if (serializer == null)
        {
            continue;
        }

        serializer.Serialize(writer, previousOption, option);
        previousOption = option;
    }
}
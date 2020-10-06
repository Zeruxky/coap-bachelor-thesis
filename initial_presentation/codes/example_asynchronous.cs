public async Task<int> CalculateAsync() {
    await Task.Delay(1000).ConfigureAwait(false);
    return Task.FromResult(1);
}

public static Task Main(string[] args) {
    var result = await CalculateAsync().ConfigureAwait(false);
}

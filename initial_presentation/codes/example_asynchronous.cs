public async Task<string> DownloadAsync(Uri uri, CancellationToken ct) {
    var client = new DownloadClient();
    var result = await client.DownloadAsync(uri, ct);

    return result;
}
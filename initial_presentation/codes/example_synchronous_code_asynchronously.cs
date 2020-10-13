public async Task<string> DownloadAsync(Uri uri) {
    var client = new DownloadClient();
    var result = await Task.Run(() => client.Download());

    return result;
}
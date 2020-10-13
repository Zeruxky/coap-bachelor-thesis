public string Download(Uri uri) {
    var client = new DownloadClient();
    var result = client.Download(uri);

    return result;
}
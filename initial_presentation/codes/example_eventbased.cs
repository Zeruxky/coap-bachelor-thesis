public DownloadResult Download(Uri uri) {
    var client = new DownloadClient();
    var result = new DownloadResult();

    client.DownloadComplete += (content) => result.SetComplete(content);
    client.StartDownload(uri);

    return result;
}


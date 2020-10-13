public class DownloadResult {
    string result = null;

    bool IsComplete() {
        return result != null;
    }

    public void SetComplete(string result) {
        this.result = result;
    }

    public string GetResult() {
        return this.result;
    }
}

public DownloadResult Download(Uri uri) {
    var client = new DownloadClient();
    var result = new DownloadResult();

    client.DownloadComplete += (content) => result.SetComplete(content);
    client.StartDownload(uri);

    return result;
}


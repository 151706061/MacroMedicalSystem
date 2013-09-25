function DownloadProgress(sender, eventArgs) {
    var rectBar = sender.findName("rectBar");
    var rectBorder = sender.findName("rectBorder");
    var txtLoading = sender.findName("txtLoading");
    if (eventArgs.progress) {
        rectBar.Width = eventArgs.progress * rectBorder.Width;
        txtLoading.Text = "Loading: " + Math.round(eventArgs.progress * 100) + "%";
    } else {
        rectBar.Width = eventArgs.get_progress() * rectBorder.Width;
    }
}
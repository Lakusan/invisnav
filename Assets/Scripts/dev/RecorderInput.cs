using Niantic.ARDK.AR.Scanning;
using Niantic.Lightship.AR.Scanning;
using UnityEngine;

public class RecorderInput : MonoBehaviour
{
    [SerializeField] public ARScanningManager _arScanningManager;

    public async void StopRecordingAndExport()
    {

        MyConsole.instance.Log("Stop recording");
        // save the recording with SaveScan()
        // use ScanStore() to get a reference to it, then ScanArchiveBuilder() to export it
        // output the path to the playback recording as a debug message
        string scanId = _arScanningManager.GetCurrentScanId();
        MyConsole.instance.Log("scan id: " + scanId);
        await _arScanningManager.SaveScan();
        var savedScan = _arScanningManager.GetScanStore().GetSavedScans().Find(scan => scan.ScanId == scanId);
        UploadUserInfo uinfo = new UploadUserInfo();
        uinfo.ScanLabels.Add("bla");
        uinfo.Note = "blah";
        ScanArchiveBuilder builder = new ScanArchiveBuilder(savedScan,uinfo );
        while (builder.HasMoreChunks())
        {
            var task = builder.CreateTaskToGetNextChunk();
            task.Start();
            await task;
            MyConsole.instance.Log(task.Result);   // <- this is the path to the playback recording.
        }
        _arScanningManager.enabled = false;
    }

    public void StartRecording()
    {
        _arScanningManager.enabled = true;
        MyConsole.instance.Log("start recording: " + _arScanningManager.isActiveAndEnabled.ToString());
        MyConsole.instance.Log("Id: " + _arScanningManager.ScanTargetId);
        UploadUserInfo uploadUserInfo = new UploadUserInfo();
        MyConsole.instance.Log("my user info: " + uploadUserInfo);
    }

}
using UdonSharp;
using Texel;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TXLPlayPlaylistButton : UdonSharpBehaviour
{
    [Header("The playlist this button should start")]
    public Playlist playlist;

    [Header("The VideoTXL Video Source UI")]
    public VideoSourceUI videoSourceUI;

    public void PlayAndShow()
    {
        if (playlist == null || videoSourceUI == null)
            return;

        // Start the first video in this playlist.
        if (!playlist._MoveTo(0))
            return;

        // VideoTXL now knows which source is active.
        // Show that active playlist in the playlist UI.
        videoSourceUI._SelectActive();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
#if DEBUG
    DebugWindow debugWindow;
    [SerializeField, FMODUnity.EventRef] string _audioTest;
    private void Awake()
    {
        debugWindow = new DebugWindow(this.ToString(), DebugContents);
    }

    //Input
    void OnCHEATS() => _debug = !_debug;
    void OnTimescaleUp() => Time.timeScale = Mathf.Clamp(Time.timeScale + .2f, 0, 1);
    void OnTimescaleDown() => Time.timeScale = Mathf.Clamp(Time.timeScale - .2f, 0, 1);

    bool _debug = false;
    private void OnGUI()
    {
        _debug = debugWindow.DrawWindow(_debug);

        AudioManager.Instance.debug = AudioManager.Instance.debugWindow.DrawWindow(AudioManager.Instance.debug);
    }

    void DebugContents(System.Func<bool, int> nextLine)
    {
        debugWindow.Label($"Timescale: {Time.timeScale}");

        if(debugWindow.Button("Play test sound"))
            AudioManager.Instance.PlayTrack(_audioTest, AudioManager.EAudioLayer.MUSIC);

        if (debugWindow.Button("AudioManager"))
            AudioManager.Instance.debug = true;

        if (debugWindow.Button("GameManager"))
            FindObjectOfType<GameplayManager>().debug = true;

        if (debugWindow.Button("MusicSheetManager"))
            FindObjectOfType<MusicSheetManager>().debug = true;
    }
#endif
}

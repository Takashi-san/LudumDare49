using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSheetManager : SingletonMonobehaviour<MusicSheetManager>
#if DEBUG
    ,IDebugable
#endif
{
    public List<MusicSheet> MusicSheetList => _musicSheetList;
    public Dictionary<string, MusicSheet> MusicSheetDict => _musicSheetDict;
    
    List<MusicSheet> _musicSheetList = new List<MusicSheet>();
    Dictionary<string, MusicSheet> _musicSheetDict = new Dictionary<string, MusicSheet>();
    
    protected override void Awake()
    {
        base.Awake();
#if DEBUG
        debugWindow = new DebugWindow(this.ToString(), DebugContents);
#endif
    }

    void Start() {
        MusicSheetFileManager.Instance.LoadedFiles += LoadMusicSheets;
        if (MusicSheetFileManager.Instance.SheetFileList.Count != 0) {
            LoadMusicSheets();
        }
        foreach (MusicSheet musicSheet in _musicSheetList)
        {
            foreach (KeyValuePair<SuitType, List<MusicNote>> pair in musicSheet.SuitSheets)
                pair.Value.Sort((a, b) =>
                {
                    if (b.hitTime > a.hitTime)
                        return -1;

                    return 1;
                });
            /*
            foreach (KeyValuePair<SuitType, List<MusicNote>> pair in musicSheet.SuitSheets)
            {
                Debug.Log($"Suit: {pair.Key}");
                foreach (MusicNote note in pair.Value)
                    Debug.Log($"    note: hitTime{note.hitTime} type: {note.noteType}");
            }*/
        }

    }

    public MusicNote GetNearestNote(string music, SuitType type, int timelinePosition, int tolerance)
    {
        List<MusicNote> notes = _musicSheetList.Find(s => s.FileName == music).SuitSheets[type];

        MusicNote n = default;
        foreach (MusicNote note in notes)
        {
            if (note.hitTime > timelinePosition - tolerance)
            {
                n = note;
                break;
            }
        }

        return n;
    }

    void LoadMusicSheets() {
        foreach (var file in MusicSheetFileManager.Instance.SheetFileList) {
            MusicSheet musicSheet = file.GetMusicSheet();
            if (musicSheet != null) {
                musicSheet.FileName = file.FileName;
                musicSheet.Assets = file.Assets;
                _musicSheetList.Add(musicSheet);
                _musicSheetDict.Add(musicSheet.Name, musicSheet);
            }
        }
    }

#if DEBUG
    public bool _debug = false;
    public bool debug { get => _debug; set => _debug = value; }

    public DebugWindow debugWindow { get; private set; }
    void OnGUI()
    {
        _debug = debugWindow.DrawWindow(_debug);
    }

    void DebugContents(System.Func<bool, int> nextLine)
    {
        debugWindow.Label("Sheets:");
        foreach (KeyValuePair<string, MusicSheet> pair in _musicSheetDict)
            debugWindow.Label($"{pair.Key}");
    }
#endif
}

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
    public System.Action MusicSheetsLoaded;
    
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

    }

    public MusicNote GetNearestNote(string music, SuitType type, int timelinePosition, int tolerance)
    {
        MusicNote n = default;

        if (!_musicSheetList.Find(s => s.FileName == music).SuitSheets.ContainsKey(type))
            return n;

        List<MusicNote> notes = _musicSheetList.Find(s => s.FileName == music).SuitSheets[type];

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
                musicSheet.MusicData = file.MusicData;
                _musicSheetList.Add(musicSheet);
                _musicSheetDict.Add(musicSheet.Name, musicSheet);
            }
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
        }
        MusicSheetsLoaded?.Invoke();
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

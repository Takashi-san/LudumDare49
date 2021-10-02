using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSheetManager : SingletonMonobehaviour<MusicSheetManager>
{
    public List<MusicSheet> MusicSheetList => _musicSheetList;
    public Dictionary<string, MusicSheet> MusicSheetDict => _musicSheetDict;
    
    List<MusicSheet> _musicSheetList = new List<MusicSheet>();
    Dictionary<string, MusicSheet> _musicSheetDict = new Dictionary<string, MusicSheet>();
    
    void Start() {
        MusicSheetFileManager.Instance.LoadedFiles += LoadMusicSheets;
        if (MusicSheetFileManager.Instance.SheetFileList.Count != 0) {
            LoadMusicSheets();
        }
    }

    void LoadMusicSheets() {
        foreach (var file in MusicSheetFileManager.Instance.SheetFileList) {
            MusicSheet musicSheet = file.GetMusicSheet();
            if (musicSheet != null) {
                _musicSheetList.Add(musicSheet);
                _musicSheetDict.Add(musicSheet.Name, musicSheet);
            }
        }
    }
}

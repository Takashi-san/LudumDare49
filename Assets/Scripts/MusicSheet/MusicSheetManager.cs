using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSheetManager : SingletonMonobehaviour<MusicSheetManager>
{
    public List<MusicSheet> MusicSheetList => _musicSheetList;
    
    List<MusicSheet> _musicSheetList = new List<MusicSheet>();
    
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
            }
        }
    }
}

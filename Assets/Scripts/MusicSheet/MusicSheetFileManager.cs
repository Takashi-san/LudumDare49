using System.Collections.Generic;
using System;
using UnityEngine;

public class MusicSheetFileManager : SingletonMonobehaviour<MusicSheetFileManager>
{
    public Action LoadedFiles;
    public List<MusicSheetFile> SheetFileList => _sheetFileList;
    
    // [SerializeField] List<MusicAssetsData> _musicFileList = new List<MusicAssetsData>();
    [SerializeField] AllMusicData _musicFileList;
    List<MusicSheetFile> _sheetFileList = new List<MusicSheetFile>();
    
    void Start() {
        LoadAllFiles();
    }

    void LoadAllFiles() {
        foreach (var file in _musicFileList._musics) {
            List<List<string>> fileData = CSVFileManager.GetCSVData(file._textAsset);
            MusicSheetFile musicSheetFile = new MusicSheetFile(fileData);
            if (musicSheetFile.IsValid) {
                musicSheetFile.FileName = file.Name;
                musicSheetFile.MusicData = file;
                _sheetFileList.Add(musicSheetFile);
            }
        }

        LoadedFiles?.Invoke();
    }
}

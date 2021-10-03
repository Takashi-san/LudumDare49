using System.Collections.Generic;
using System;
using UnityEngine;

public class MusicSheetFileManager : SingletonMonobehaviour<MusicSheetFileManager>
{
    public Action LoadedFiles;
    public List<MusicSheetFile> SheetFileList => _sheetFileList;
    
    [SerializeField] List<MusicAssetsData> _musicFileList = new List<MusicAssetsData>();
    List<MusicSheetFile> _sheetFileList = new List<MusicSheetFile>();
    
    void Start() {
        LoadAllFiles();
    }

    void LoadAllFiles() {
        foreach (var file in _musicFileList) {
            List<List<string>> fileData = CSVFileManager.GetCSVData(file.textAsset);
            MusicSheetFile musicSheetFile = new MusicSheetFile(fileData);
            if (musicSheetFile.IsValid) {
                musicSheetFile.FileName = file.textAsset.name;
                musicSheetFile.Assets = file;
                _sheetFileList.Add(musicSheetFile);
            }
        }

        LoadedFiles?.Invoke();
    }
}

[Serializable]
public struct MusicAssetsData {
    [SerializeField] public TextAsset textAsset;
    [SerializeField, FMODUnity.EventRef] public string music;
}

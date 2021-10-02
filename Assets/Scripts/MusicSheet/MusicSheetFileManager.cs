using System.Collections.Generic;
using System;
using UnityEngine;

public class MusicSheetFileManager : SingletonMonobehaviour<MusicSheetFileManager>
{
    public Action LoadedFiles;
    public List<MusicSheetFile> SheetFileList;
    
    [SerializeField] List<TextAsset> _musicFileList = new List<TextAsset>();
    
    void Start() {
        LoadAllFiles();
    }

    void LoadAllFiles() {
        foreach (var file in _musicFileList) {
            List<List<string>> fileData = CSVFileManager.GetCSVData(file);
            MusicSheetFile musicSheetFile = new MusicSheetFile(fileData);
            if (musicSheetFile.IsValid) {
                SheetFileList.Add(musicSheetFile);
            }
        }

        LoadedFiles?.Invoke();
    }
}

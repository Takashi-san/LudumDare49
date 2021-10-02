using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicSheetFile {
    public bool IsValid = false;

    public string Name {
        get {
            foreach (var line in _CSVData)
            {
                if (string.IsNullOrWhiteSpace(line[1]) && string.IsNullOrWhiteSpace(line[2])) {
                    return line[0];
                }
            }
            return "";
        }
    }

    List<List<string>> _CSVData;
    
    public MusicSheetFile(List<List<string>> p_CSVData) {
        if (IsCSVDataValid(p_CSVData)) {
            _CSVData = p_CSVData;
            IsValid = true;
        }
    }

    public MusicSheet GetMusicSheet() {
        if (_CSVData == null) {
            return null;
        }

        MusicSheet result = new MusicSheet();
        result.Name = Name;
        InsertNotesInto(result);

        return result;
    }

    bool IsCSVDataValid(List<List<string>> p_CSVData) {
        foreach (var line in p_CSVData)
        {
            if (line.Count != 3) {
                return false;
            }
        }
        return true;
    }

    void InsertNotesInto(MusicSheet p_musicSheet) {
        foreach (var line in _CSVData)
        {
            if (string.IsNullOrEmpty(line[1]) || string.IsNullOrEmpty(line[2])) {
                continue;
            }

            try {
                SuitType suitType = (SuitType) Enum.Parse(typeof(SuitType), line[0], true);
                MusicNoteType noteType = (MusicNoteType) Enum.Parse(typeof(MusicNoteType), line[1], true);
                // TODO converter o hit time em um valor do tipo que vamos utilizar!
                p_musicSheet.AddNote(suitType, noteType, line[2]);
            }
            catch {
                Debug.LogWarning($"[MusicSheetFile] Nota incorreta em: {Name}");
            }
        }
    }

    void PrintSCVData(List<List<string>> p_CSVData) {
        string content = "";
        foreach (var line in p_CSVData)
        {
            foreach (var field in line)
            {
                content += field + ", ";
            }
            content += "\n";
        }
        Debug.Log($"[CSVData] ------------------------------------------\n{content}");
    }
}

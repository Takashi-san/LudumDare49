using System.Collections.Generic;
using UnityEngine;

public class MusicSheetFile {
    public bool IsValid = false;
    
    public MusicSheetFile(List<List<string>> CSVData) {
        foreach (var line in CSVData)
        {
            string content = "";
            foreach (var field in line)
            {
                content += field + ", ";
            }
            Debug.Log($"{content}");
        }
    }
}

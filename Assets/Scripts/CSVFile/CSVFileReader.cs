using System.Collections.Generic;
using UnityEngine;

public class CSVFileManager
{
    const char lineSeparator = '\n';
    const char fieldSeparator = ',';
    
    public static List<List<string>> GetCSVData(TextAsset p_CSVFile) {
        List<List<string>> data = new List<List<string>>();
        
        string[] fileData = p_CSVFile.text.Split(lineSeparator);
        foreach (var lineData in fileData) {
            List<string> line = new List<string>();
            
            string[] fields = lineData.Split(fieldSeparator);
            foreach (var field in fields)
            {
                line.Add(field);
            }
            
            data.Add(line);
        }
        
        return data;
    }
}

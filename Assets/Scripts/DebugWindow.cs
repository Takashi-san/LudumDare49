using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebugable
{
    bool debug { get; set; }
    DebugWindow debugWindow { get; }
}
public class DebugWindow
{
    Action<Func<bool, int>> DebugContents;
    static int id = 0;
    public string windowName;
    int windowID;
    public int elementHeight = 20;
    public int windowWidth = 200;
    public int windowHeight = 300;
    public int minHeight = 40;
    public int minWidth = 200;
    public Rect windowRect = new Rect(0, 0, 0, 0);
    public bool minimized = false;
    public int neededHeight = 0;
    System.Func<bool, int> nextLine;
    public int lastElementY = 0;
    public Action OnUpdate;

    static GUIStyle guiStyle;

    Dictionary<string, List<bool>> boolbufferInfo = new Dictionary<string, List<bool>>();
    Dictionary<string, string> stringBufferInfo = new Dictionary<string, string>();

    List<SessionData> sessions = new List<SessionData>();
    bool drawAllSessions = true;
    public GUIStyle buttonGUIStyle;



    public DebugWindow(string windowName, Action<Func<bool, int>> DebugContents)
    {
        this.windowID = id++;
        this.windowName = windowName;
        this.DebugContents = DebugContents;
        if (guiStyle == null)
        {
            guiStyle = new GUIStyle();
            guiStyle.normal.textColor = Color.white;
            guiStyle.font = (Font)Resources.Load("secrcode");
        }
    }

    public bool DrawWindow(bool show, bool drawAllSessions = true)
    {
        if (!show) return false;

        this.drawAllSessions = drawAllSessions;
        windowRect.width = minimized ? minWidth : windowWidth;
        windowRect.height = minimized ? minHeight : Mathf.Max(neededHeight, windowHeight);

        GUI.depth = 0;
        if (GUI.Button(new Rect(windowRect.x, windowRect.y, elementHeight, elementHeight), "x"))
            return false;
        windowRect = GUI.Window(windowID, windowRect, DragWindow, windowName == null ? this.ToString() : windowName);

        return true;
    }

    public void Label(string label, bool nextLine = true) => GUI.Label(new Rect(0, this.nextLine(nextLine), windowWidth, elementHeight), label, guiStyle);
    public void Label(string label, int x, bool nextLine = true) => GUI.Label(new Rect(x, this.nextLine(nextLine), windowWidth, elementHeight), label, guiStyle);
    public void Label(string label, int x, int w, bool nextLine = true) => GUI.Label(new Rect(x, this.nextLine(nextLine), w, elementHeight), label, guiStyle);
    public void Label(string label, int x, int w, int h, bool nextLine = true) => GUI.Label(new Rect(x, this.nextLine(nextLine), w, h), label, guiStyle);
    public void Label(string label, int x, int y, int w, int h) => GUI.Label(new Rect(x, y, w, h), label, guiStyle);


    public bool Checkbox(string label, bool value, bool nextLine = true) => GUI.Toggle(new Rect(0, this.nextLine(nextLine), windowWidth, elementHeight), value, label);
    public bool Checkbox(string label, bool value, int x, bool nextLine = true) => GUI.Toggle(new Rect(x, this.nextLine(nextLine), windowWidth, elementHeight), value, label);
    public bool Checkbox(string label, bool value, int x, int w, bool nextLine = true) => GUI.Toggle(new Rect(x, this.nextLine(nextLine), w, elementHeight), value, label);
    public bool Checkbox(string label, bool value, int x, int w, int h, bool nextLine = true) => GUI.Toggle(new Rect(x, this.nextLine(nextLine), w, h), value, label);
    public bool Checkbox(string label, bool value, int x, int y, int w, int h) => GUI.Toggle(new Rect(x, y, w, h), value, label);


    public bool Button(string label, bool nextLine = true) => GUI.Button(new Rect(0, this.nextLine(nextLine), windowWidth, elementHeight), label, buttonGUIStyle);
    public bool Button(string label, int x, bool nextLine = true) => GUI.Button(new Rect(x, this.nextLine(nextLine), windowWidth, elementHeight), label, buttonGUIStyle);
    public bool Button(string label, int x, int w, bool nextLine = true) => GUI.Button(new Rect(x, this.nextLine(nextLine), w, elementHeight), label, buttonGUIStyle);
    public bool Button(string label, int x, int w, int h, bool nextLine = true) => GUI.Button(new Rect(x, this.nextLine(nextLine), w, h), label, buttonGUIStyle);
    public bool Button(string label, int x, int y, int w, int h) => GUI.Button(new Rect(x, y, w, h), label, buttonGUIStyle);

    public float HorizontalSlider(string label, float value, float minValue, float maxValue, bool nextLine = true)
    {
        Label(label, nextLine);
        Label($"{minValue}", windowWidth / 2 - 20, false);
        float v = GUI.HorizontalSlider(new Rect(windowWidth / 2, this.nextLine(false), windowWidth / 2 - 20, elementHeight), value, minValue, maxValue);
        Label($"{maxValue}", windowWidth - 20, false);

        return v;
    }

    public void ProgressBar(string label, string insideText, float value, Color color, bool nextLine = true, TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.alignment = textAnchor;
        GUI.backgroundColor = Color.gray;
        GUI.Box(new Rect(0, this.nextLine(nextLine), windowWidth, elementHeight), insideText); //_spawnTimer / timer, $"SpawnTimer: {_spawnTimer}/{ (_currentWaveIndex < _waves.Count ? _waves[_currentWaveIndex].IntervalPerEnemy : 0)}");
        style.normal.background = MakeTex(2, 2, color);
        GUI.Box(new Rect(0, this.nextLine(false), value * windowWidth, elementHeight), "", style); //_spawnTimer / timer, $"SpawnTimer: {_spawnTimer}/{ (_currentWaveIndex < _waves.Count ? _waves[_currentWaveIndex].IntervalPerEnemy : 0)}");
        GUI.color = Color.white;
    }

    public class BarMarker
    {
        public Color color;
        public float value;
    }
    public void ProgressBar(string label, string insideText, float value, Color color, List<BarMarker> markers, bool nextLine = true, TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        GUIStyle style = new GUIStyle();
        style.alignment = textAnchor;
        GUI.backgroundColor = Color.gray;
        GUI.Box(new Rect(0, this.nextLine(nextLine), windowWidth, elementHeight), insideText); //_spawnTimer / timer, $"SpawnTimer: {_spawnTimer}/{ (_currentWaveIndex < _waves.Count ? _waves[_currentWaveIndex].IntervalPerEnemy : 0)}");
        style.normal.background = MakeTex(1, 1, color);
        GUI.Box(new Rect(0, this.nextLine(false), value * windowWidth, elementHeight), "", style); //_spawnTimer / timer, $"SpawnTimer: {_spawnTimer}/{ (_currentWaveIndex < _waves.Count ? _waves[_currentWaveIndex].IntervalPerEnemy : 0)}");
        GUI.color = Color.white;
        foreach (BarMarker marker in markers)
        {
            style.normal.background = MakeTex(1, 1, marker.color);
            GUI.Box(new Rect(windowWidth * marker.value, this.nextLine(false), 2, elementHeight), "", style); //_spawnTimer / timer, $"SpawnTimer: {_spawnTimer}/{ (_currentWaveIndex < _waves.Count ? _waves[_currentWaveIndex].IntervalPerEnemy : 0)}");

        }
    }

    public void ColoredSquare(Color color, bool nextLine = true)
    {
        GUIStyle style = new GUIStyle();
        style.normal.background = MakeTex(1, 1, color);
        GUI.Box(new Rect(0, this.nextLine(nextLine), elementHeight, elementHeight), "", style);
        GUI.color = Color.white;
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public int currentHeight;
    void DragWindow(int windowID)
    {
        buttonGUIStyle = GUI.skin.button;
        buttonGUIStyle.alignment = TextAnchor.UpperLeft;


        GUI.depth = -1;
        currentHeight = 0;
        nextLine = (bool next) => (next ? currentHeight += elementHeight : currentHeight);
        if (GUI.Button(new Rect(elementHeight, 0, elementHeight, elementHeight), minimized ? "+" : "-"))
            minimized = !minimized;
        if (GUI.Button(new Rect(2 * elementHeight, 0, elementHeight, elementHeight), "<"))
            windowWidth = Mathf.Clamp(windowWidth - 100, 200, 1500);
        if (GUI.Button(new Rect(3 * elementHeight, 0, elementHeight, elementHeight), ">"))
            windowWidth += 100;

        if (!minimized)
        {
            DebugContents(nextLine);
            if (drawAllSessions) DrawAllSessions();
        }
        nextLine(true);
        neededHeight = currentHeight;
        GUI.DragWindow(new Rect(0, 0, windowWidth, 40));
    }

    public void AddSession(string name, Action<Func<bool, int>> session, bool minimized = false)
    {
        sessions.Add(new SessionData(name, minimized, session));
    }

    public int totalSessionHeight = 0;
    public void DrawAllSessions()
    {
        foreach (SessionData session in sessions)
        {
            Label(session.name, elementHeight);
            if (Button(session.minimized ? "+" : "-", 0, elementHeight, false))
                session.minimized = !session.minimized;
            if (session.minimized) continue;

            session.content(nextLine);
            if (totalSessionHeight != 0)
            {
                currentHeight += totalSessionHeight;
                totalSessionHeight = 0;
            }
        }
    }

    public string DebugListToString<T>(List<T> elementList)
    {
        string listedStringable = "";
        for (int i = 0; i < elementList.Count; i++)
            listedStringable += elementList[i].ToString().Substring(0, 1).ToUpper();
        return listedStringable;
    }

    public void CreateBoolBufferInfo(string newBufferInfo)
    {
        boolbufferInfo.Add(newBufferInfo, new List<bool>());
    }

    public void CreateStringBufferInfo(string newBufferInfo)
    {
        if (!stringBufferInfo.ContainsKey(newBufferInfo))
            stringBufferInfo.Add(newBufferInfo, "");
    }

    public void AddToBoolBufferInfo(string key, bool info, int maxCount)
    {
        boolbufferInfo[key].Add(info);
        if (maxCount == -1) return;
        while (boolbufferInfo[key].Count > maxCount)
            boolbufferInfo[key].RemoveAt(0);
    }

    public void AddToStringBufferInfo(string key, string info, int maxCount)
    {
        stringBufferInfo[key] = info + stringBufferInfo[key];
        if (maxCount == -1) return;
        while (stringBufferInfo[key].Length > maxCount)
            stringBufferInfo[key] = stringBufferInfo[key].Substring(0, maxCount);
    }

    public void ClearBoolBufferBuffer()
    {
        foreach (KeyValuePair<string, List<bool>> pair in boolbufferInfo)
            pair.Value.Clear();
    }

    public void ClearStringBuffer()
    {
        List<string> keys = new List<string>(stringBufferInfo.Keys);
        for (int i = 0; i < keys.Count; i++)
            stringBufferInfo[keys[i]] = "";
    }

    public string GetStringFromStringBufferInfo(string key)
    {
        //return DebugListToString(stringBufferInfo[key]);
        return stringBufferInfo[key];
    }

    public string GetStringFromBoolBufferInfo(string key)
    {
        return DebugListToString(boolbufferInfo[key]);
    }

    public void ClearUnityConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    public class SessionData
    {
        public bool minimized;
        public string name;
        public Action<System.Func<bool, int>> content;
        public SessionData(string name, bool minimized, Action<System.Func<bool, int>> session)
        {
            this.minimized = minimized;
            this.name = name;
            this.content = session;
        }
    }

}

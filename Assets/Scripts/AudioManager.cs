using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
public class AudioManager
#if DEBUG
    : IDebugable
#endif
{
    public class AudioPack
    {
        public string fmodPath;
        public FMOD.Studio.EventInstance EventInstance;
    }
    public enum EAudioLayer { MUSIC, AMBIENCE }

    List<AudioPack> _tracks;
    Dictionary<object, List<AudioPack>> _soundEvents = new Dictionary<object, List<AudioPack>>();
    bool _canChangeTrack;

    static AudioManager _instance;
    public static AudioManager Instance => _instance == null ? _instance = new AudioManager() : _instance;  

    public AudioManager()
    {
        _tracks = new List<AudioPack>() { null, null };

#if DEBUG
        debugWindow = new DebugWindow(this.ToString(), DebugContents);
        debugWindow.AddSession("SoundEvents:", DebugSoundEvents);
#endif
    }

    public void PlayTrack(string fmodEventPath, EAudioLayer audioLayer, FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        if (string.IsNullOrEmpty(fmodEventPath)) return;
        if (fmodEventPath == "" || !_canChangeTrack)
            return;

        int index = (int)audioLayer;
        if (_tracks[index] == null)
            _tracks[index] = new AudioPack();
        else
        {
            _tracks[index].EventInstance.stop(mode);
            _tracks[index].EventInstance.release();
        }

        _tracks[index].fmodPath = fmodEventPath;

        try
        {
            _tracks[index].EventInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEventPath);
            _tracks[index].EventInstance.start();
        }
        catch
        {
            UnityEngine.Debug.Log($"Problem creating FMOD instance {fmodEventPath}");
        }
    }

    public void StopTrack(EAudioLayer audioLayer, FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        if (!_canChangeTrack)
            return;

        int index = (int)audioLayer;
        if (_tracks[index] != null)
        {
            UnityEngine.Debug.Log("Should stop");
            _tracks[index].EventInstance.stop(mode);
            _tracks[index].EventInstance.release();
        }
        _tracks[index] = null;
    }

    public FMOD.Studio.EventInstance PlayOneShotSound(string fmodEventPath, Vector3 position, Transform parent = null)
    {
        FMOD.Studio.EventInstance instance = default;
        if (string.IsNullOrEmpty(fmodEventPath)) return instance;

        instance = FMODUnity.RuntimeManager.CreateInstance(fmodEventPath);
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));
        if (parent != null)
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, parent);

        instance.start();
        instance.release();

        return instance;
    }

    public AudioPack PlayLoopSound(string fmodEventPath, object key, bool start = true)
    {
        if (string.IsNullOrEmpty(fmodEventPath)) return null;
        if (!_soundEvents.TryGetValue(key, out List<AudioPack> audioPacks))
        {
            audioPacks = new List<AudioPack>();
            _soundEvents.Add(key, audioPacks);
        }

        AudioPack audioPack = new AudioPack()
        {
            fmodPath = fmodEventPath,
            EventInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEventPath)
        };
        audioPacks.Add(audioPack);

        if (start)
            audioPack.EventInstance.start();

        return audioPack;
    }

    public void PlayLoopSound(string fmodEventPath, object key, Vector3 position, Transform parent = null)
    {
        if (string.IsNullOrEmpty(fmodEventPath)) return;
        AudioPack audioPack = PlayLoopSound(fmodEventPath, key, false);
        if (parent != null)
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(audioPack.EventInstance, parent);

        FMOD.ATTRIBUTES_3D att = FMODUnity.RuntimeUtils.To3DAttributes(position);
        audioPack.EventInstance.set3DAttributes(att);
        audioPack.EventInstance.start();
    }

    public void StopLoopSound(object obj, FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
    {
        if (_soundEvents.TryGetValue(obj, out List<AudioPack> packs))
        {
            foreach (AudioPack pack in packs)
            {
                pack.EventInstance.stop(mode);
                pack.EventInstance.release();
            }
            packs.Clear();
            _soundEvents.Remove(obj);
        }
    }

    public bool IsPlayingTrack(string path, EAudioLayer audioLayer) => _tracks[(int)audioLayer]?.fmodPath == path;

    ~AudioManager()
    {
        _tracks.ForEach(t =>
        {
            t.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            t.EventInstance.release();
        });

        _tracks.Clear();

        foreach (KeyValuePair<object, List<AudioPack>> audioPack in _soundEvents)
        {
            foreach (AudioPack ap in audioPack.Value)
            {
                var r1 = ap.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                var r2 = ap.EventInstance.release();
            }
        }
    }

#if DEBUG

    public bool _debug = false;
    public bool debug { get => _debug; set => _debug = value; }

    public DebugWindow debugWindow { get; }

    void DebugContents(System.Func<bool, int> nextLine)
    {
        debugWindow.Label($"Music: {(_tracks[(int)EAudioLayer.MUSIC] == null? "none" :_tracks[(int)EAudioLayer.MUSIC].fmodPath)}");
        if (_tracks[(int)EAudioLayer.MUSIC] != null)
        {
            _tracks[(int)EAudioLayer.MUSIC].EventInstance.getTimelinePosition(out int position);
            FMOD.Studio.EventDescription description = FMODUnity.RuntimeManager.GetEventDescription(_tracks[(int)EAudioLayer.MUSIC].fmodPath);
            description.getLength(out int length);

            debugWindow.ProgressBar("", $"{_tracks[(int)EAudioLayer.MUSIC].fmodPath} {(position / 1000.0).ToString("0.00")}/{(length / 1000.0).ToString("0.00")}", position / (float)length, new Color(0, 1, 1, .5f));
            _tracks[(int)EAudioLayer.MUSIC].EventInstance.getParameterByName("LOOPS", out loopsValue);
            loopsValue = debugWindow.HorizontalSlider($"Loops {(int)loopsValue}", loopsValue, 0, 5);
            _tracks[(int)EAudioLayer.MUSIC].EventInstance.setParameterByName("LOOPS", loopsValue);
        }

        debugWindow.Label($"Ambience: {(_tracks[(int)EAudioLayer.AMBIENCE] == null? "none" :_tracks[(int)EAudioLayer.AMBIENCE].fmodPath)}");
        if (_tracks[(int)EAudioLayer.AMBIENCE] != null)
        {
            _tracks[(int)EAudioLayer.AMBIENCE].EventInstance.getTimelinePosition(out int position);
            FMOD.Studio.EventDescription description = FMODUnity.RuntimeManager.GetEventDescription(_tracks[(int)EAudioLayer.AMBIENCE].fmodPath);
            description.getLength(out int length);

            debugWindow.ProgressBar("", $"{_tracks[(int)EAudioLayer.AMBIENCE].fmodPath} {(position / 1000.0).ToString("0.00")}/{(length / 1000.0).ToString("0.00")}", position / (float)length, new Color(0, 1, 1, .5f));
            _tracks[(int)EAudioLayer.AMBIENCE].EventInstance.getParameterByName("LOOPS", out loopsValue);
            loopsValue = debugWindow.HorizontalSlider($"Loops {(int)loopsValue}", loopsValue, 0, 5);
            _tracks[(int)EAudioLayer.AMBIENCE].EventInstance.setParameterByName("LOOPS", loopsValue);
        }
    }

    float loopsValue = 0;
    void DebugSoundEvents(System.Func<bool, int> nextLine)
    {
        int i = 0;
        foreach (KeyValuePair<object, List<AudioPack>> pair in _soundEvents)
        {
            debugWindow.Label($"{i++}");
            foreach (AudioPack ap in pair.Value)
            {
                ap.EventInstance.getTimelinePosition(out int position);
                FMOD.Studio.EventDescription description = FMODUnity.RuntimeManager.GetEventDescription(ap.fmodPath);
                description.getLength(out int length);

                debugWindow.ProgressBar("", $"{ap.fmodPath} {(position / 1000.0).ToString("0.00")}/{(length / 1000.0).ToString("0.00")}", position / (float)length, new Color(0, 1, 1, .5f));
                ap.EventInstance.getParameterByName("LOOPS", out loopsValue);
                loopsValue = debugWindow.HorizontalSlider($"Loops {(int)loopsValue}", loopsValue, 0, 5);
                ap.EventInstance.setParameterByName("LOOPS", loopsValue);
            }

        }
    }
#endif
}

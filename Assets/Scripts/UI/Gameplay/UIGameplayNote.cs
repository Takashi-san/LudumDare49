using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplayNote : MonoBehaviour {
    const float PIXEL_PER_MILLISECOND = 0.2f;

    [SerializeField] RectTransform _rectTransform = null;

    SuitType _suitType;
    MusicNote _musicNote;
    int _musicLength;

    public void Setup(SuitType p_suitType, MusicNote p_musicNote, int p_musicLength) {
        _suitType = p_suitType;
        _musicNote = p_musicNote;
        _musicLength = p_musicLength;
    }

    public void DestroyNote() {
        Destroy(gameObject);
    }

    public void MusicProgressUpdate(int p_musicProgress) {
        float musicDiff = _musicNote.hitTime - p_musicProgress;
        Debug.Log($"musicDiff {musicDiff} p_musicProgress {p_musicProgress}");
        _rectTransform.anchoredPosition = Vector2.right * musicDiff * PIXEL_PER_MILLISECOND;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplayNote : MonoBehaviour {
    const float PIXEL_PER_MILLISECOND = 0.2f;

    [SerializeField] RectTransform _rectTransform = null;
    [SerializeField] Animator _animator = null;

    SuitType _suitType;
    MusicNote _musicNote;
    int _musicLength;

    public void Setup(SuitType p_suitType, MusicNote p_musicNote, int p_musicLength) {
        _suitType = p_suitType;
        _musicNote = p_musicNote;
        _musicLength = p_musicLength;

        SetAnimation();
    }

    public void DestroyNote() {
        Destroy(gameObject);
    }

    public void MusicProgressUpdate(int p_musicProgress) {
        float musicDiff = _musicNote.hitTime - p_musicProgress;
        Debug.Log($"musicDiff {musicDiff} p_musicProgress {p_musicProgress}");
        _rectTransform.anchoredPosition = Vector2.right * musicDiff * PIXEL_PER_MILLISECOND;
    }

    void SetAnimation() {
        string animation = "";
        
        switch (_musicNote.noteType) {
            case MusicNoteType.Strike:
                switch (_suitType) {
                    case SuitType.Chord:
                        animation = "GameplayNotes_strike_yellow";
                        break;
                    
                    case SuitType.Metal:
                        animation = "GameplayNotes_strike_blue";
                        break;
                    
                    case SuitType.Wood:
                        animation = "GameplayNotes_strike_purple";
                        break;
                    
                    case SuitType.Percussion:
                        animation = "GameplayNotes_strike_green";
                        break;
                }
                break;
            
            case MusicNoteType.Avoid:
                switch (_suitType) {
                    case SuitType.Chord:
                        animation = "GameplayNotes_avoid_yellow";
                        break;
                    
                    case SuitType.Metal:
                        animation = "GameplayNotes_avoid_blue";
                        break;
                    
                    case SuitType.Wood:
                        animation = "GameplayNotes_avoid_purple";
                        break;
                    
                    case SuitType.Percussion:
                        animation = "GameplayNotes_avoid_green";
                        break;
                }
                break;
        }

        _animator?.Play(animation);
    }
}

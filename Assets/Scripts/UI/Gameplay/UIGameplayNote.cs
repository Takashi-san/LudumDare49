using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplayNote : MonoBehaviour {
    const float PIXEL_PER_MILLISECOND = 0.075f;

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

    bool destroyed = false;
    public void DestroyNote() {
        Destroy(gameObject);
        destroyed = true;
    }

    public void MusicProgressUpdate(int p_musicProgress) {
        if (destroyed)
            return;

        float musicDiff = _musicNote.hitTime - p_musicProgress;
        Vector2 position = _rectTransform.anchoredPosition;
        position.x = musicDiff * PIXEL_PER_MILLISECOND;
        _rectTransform.anchoredPosition = position;
    }

    void SetAnimation() {
        string animation = "";
        
        switch (_musicNote.noteType) {
            case MusicNoteType.Strike:
                switch (_suitType) {
                    case SuitType.Chord:
                        animation = "GameplayNotes_strike_blue";
                        break;
                    
                    case SuitType.Metal:
                        animation = "GameplayNotes_strike_yellow";
                        break;
                    
                    case SuitType.Wood:
                        animation = "GameplayNotes_strike_green";
                        break;
                    
                    case SuitType.Percussion:
                        animation = "GameplayNotes_strike_purple";
                        break;
                }
                break;
            
            case MusicNoteType.Avoid:
                switch (_suitType) {
                    case SuitType.Chord:
                        animation = "GameplayNotes_avoid_blue";
                        break;
                    
                    case SuitType.Metal:
                        animation = "GameplayNotes_avoid_yellow";
                        break;
                    
                    case SuitType.Wood:
                        animation = "GameplayNotes_avoid_green";
                        break;
                    
                    case SuitType.Percussion:
                        animation = "GameplayNotes_avoid_purple";
                        break;
                }
                break;
        }

        _animator?.Play(animation);
    }
}

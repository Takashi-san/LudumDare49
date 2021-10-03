using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicianSet : MonoBehaviour
{
    [SerializeField] List<Animator> _musicianAnimators = new List<Animator>();


    private void OnValidate()
    {
        _musicianAnimators.Clear();
        _musicianAnimators.AddRange(GetComponentsInChildren<Animator>());
    }

    public void PlaySet(bool play)
    {
        foreach (Animator animator in _musicianAnimators)
            animator.SetBool("PLAY", play);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayDestructableElement : MonoBehaviour {
    const float DESTRUCTION_TIME = 0.3f;
    const float FLASH_PERIOD = 0.07f;
    
    [SerializeField] Image _image = null;
    [SerializeField] Material _flashMaterial = null;
    [SerializeField] List<Sprite> _stageSprite = new List<Sprite>();

    int _currentStage = 0;
    Coroutine _destructionCoroutine = null;
    Material _originalMaterial;

    void Start() {
        _originalMaterial = _image.material;
        if (_stageSprite.Count > 0) {
            _image.sprite = _stageSprite[0];
            SetImageColor();
        }
    }

    public void UpdateDestructionStage(int p_stage) {
        if (p_stage < 0 || p_stage <= _currentStage) {
            return;
        }
        
        if (_destructionCoroutine != null) {
            StopCoroutine(_destructionCoroutine);
            DoInstantDestruction();
        }
        _currentStage = p_stage;

        if (_image.sprite == GetStageSprite()) {
            return;
        }
        _destructionCoroutine = StartCoroutine(DoDestruction());
    }

    void DoInstantDestruction() {
        _image.sprite = GetStageSprite();
        SetImageColor();
        _image.material = _originalMaterial;
    }

    void SetImageColor() {
        _image.color = _image.sprite == null ? Color.clear : Color.white;
    }
    
    Sprite GetStageSprite() {
        return _currentStage < _stageSprite.Count ? _stageSprite[_currentStage] : null;
    }

    IEnumerator DoDestruction() {
        float time = 0;
        while(time < DESTRUCTION_TIME / 2) {
            _image.material = _image.material == _originalMaterial ? _flashMaterial : _originalMaterial;
            yield return new WaitForSeconds(FLASH_PERIOD);
            time += FLASH_PERIOD;
        }

        _image.sprite = GetStageSprite();
        SetImageColor();
        
        time = 0;
        while(time < DESTRUCTION_TIME / 2) {
            _image.material = _image.material == _originalMaterial ? _flashMaterial : _originalMaterial;
            yield return new WaitForSeconds(FLASH_PERIOD);
            time += FLASH_PERIOD;
        }

        _image.material = _originalMaterial;
        _destructionCoroutine = null;
    }
}

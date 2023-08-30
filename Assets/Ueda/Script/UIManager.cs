using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IsGame;

public class UIManager : MonoBehaviour
{
    [SerializeField, Tooltip("�^�����Ԃ�\���X���C�_�[")] Slider _chargeSlider = null;
    
    [SerializeField,Tooltip("�c�莞�Ԃ�\������e�L�X�g")] TextMeshProUGUI _timerText = null;
    
    [SerializeField,Tooltip("�N���A���ɕ\������UI")] GameObject _clearUI = null;

    [SerializeField, Tooltip("�Q�[���I�[�o�[���ɕ\������UI")] GameObject _gameOverUI = null;

    [SerializeField,Tooltip("�N���A�^�C����\������e�L�X�g")] TextMeshProUGUI _clearTimeText = null;
    
    [SerializeField,Tooltip("�J�b�g�C���p�̃A�j���[�^�[")] Animator _cutIn = null;

    [SerializeField, Tooltip("�T�E���h�}�l�[�W���[")] SoundManager _soundManager = null;
    
    PlayerCalculation _player = null;

    //Animator _chargeAnim = null;
    // Start is called before the first frame update

    void Start()
    {
        _player = FindObjectOfType<PlayerCalculation>();
        _clearUI.SetActive(false);
        _gameOverUI.SetActive(false);
        GameManager.Instance.UIManagerSet(this);
    }
    private void Update()
    {
        GameManager.Instance.Timer();
    }

    public void ChargeSlider(float charge ) // �X���C�_�[�ƂЂ�����Ԃ��Ώۂ̃A�j���[�^�[�𐧌�
    {
        if (charge >= _chargeSlider.maxValue) charge = _chargeSlider.maxValue;
        _chargeSlider.value = charge;

        //�X���C�_�[�����^���ɂȂ�����v���C���[��bool��ς���
        if (charge == _chargeSlider.maxValue)
        {
            _player.PillowEnemy.ObjectRevers();
            _chargeSlider.value = 0;
            GameManager.Instance.CheckSleepingEnemy();
            _soundManager.GaugeStop();
            _player.InformationReset();
        }
    }
    public void TimerText(float time)
    {
        _timerText.text = time.ToString("F0");
    }
    public void Clear(float clearTime)
    {
        _clearTimeText.text = clearTime.ToString("F0")+" �b";
        _soundManager.GameClear();
        _clearUI.SetActive(true);
    }
    public void GameOver() 
    {
        _soundManager.GameOver();
        _gameOverUI.SetActive(true);
    }
    public void CutIn(bool before)
    {
        _soundManager.Cutin();
        _cutIn.SetBool("isChild",before);
    }
}

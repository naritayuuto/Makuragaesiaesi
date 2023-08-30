using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public VariableJoystick _joyStick;
    [SerializeField, Header("�q����Ԃ̓����X�s�[�h"), Tooltip("�������x�i�q���j")]
    float _childMoveSpeed = 10f;
    [SerializeField, Header("��l��Ԃ̓����X�s�[�h"), Tooltip("�������x�i��l�j")]
    float _adultMoveSpeed = 15f;
    [Tooltip("�ړ����x�v�Z����")]
    Vector2 _moveVelocity;
    [SerializeField, Tooltip("����Ԃ����߂̃{�^��")]
    GameObject _returnPillowButton = null;
    [SerializeField, Header("�v���C���[����l���q����"), Tooltip("��l�̎�True")]
    bool _adultState = false;
    [SerializeField, Header("�Q�Ă���G�̓����蔻����ɂ��āA�{�^���������ꂽ�Ƃ�True")]
    bool _returnPillowAction = false;
    [SerializeField, Tooltip("�v���C���[�̓����蔻��")]
    PlayerCalculation _playerCalculation = null;
    PlayerAnimator _playerAnimator = null;
    Rigidbody2D _rb;
    [Tooltip("�v���C���[�̏�Ԋm�F�A�O���Q�Ɨp")]
    public bool AdultState { get => _adultState; }
    [Tooltip("����Ԃ���ʒu�ɂ��ăX�y�[�X�L�[�������Ă���Ƃ�True, �O���Q�Ɨp")]

    public Rigidbody2D Rb { get => _rb;}
    public bool ReturnPillowAction { get => _returnPillowAction; set => _returnPillowAction = value; }

    private void Awake()
    {
        IsGame.GameManager.Instance.PlayerSet(this);
    }
    void Start()
    {
        _playerCalculation = GetComponent<PlayerCalculation>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        _rb = GetComponent<Rigidbody2D>();
        _returnPillowButton.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        float _joyX = _joyStick.Horizontal;
        float _joyY = _joyStick.Vertical;
        if(_playerCalculation.PillowEnemy)
        {
            _returnPillowButton.SetActive(true);
        }
        else
        {
            _returnPillowButton.SetActive(false);
        }
        if (!_playerAnimator.AutoAnim)
        {
            ModeCheck(_joyX, _joyY);
            Rb.velocity = _moveVelocity;
            _playerCalculation.VelocitySave(Rb.velocity);
        }
        //if (Input.GetButton("Jump"))//�X�y�[�X������
        //if (Input.GetMouseButton(0))
        if(_returnPillowAction || Input.GetButton("Jump"))
        {
            if (_playerCalculation.PillowEnemy)//���Ԃ������ɂ�����
            {
                _playerCalculation.TranslatePlayerPos();
            }
        }
        else
        {
            _playerCalculation.ReturnPillowInPos = false;
            _playerAnimator.AutoAnim = false;
        }
        //if (Input.GetButtonDown("Jump"))//�����œ������߂ɋ����v�Z���s��,�X�y�[�X�L�[��� || 
        if (_returnPillowAction || Input.GetButtonDown("Jump"))
        {
            if (_playerCalculation.PillowEnemy)//���Ԃ������ɂ�����
            {
                if (_playerCalculation.ReturnPillowPos == default)
                {
                    _playerCalculation.PlayerAndEnemyDis();
                }
            }
        }
    }
    private void ModeCheck(float h, float v)
    {
        if (!_playerAnimator.AutoAnim)
        {
            _moveVelocity = !_adultState ?
                       new Vector2(h, v).normalized * _childMoveSpeed : new Vector2(h, v).normalized * _adultMoveSpeed;
        }
    }
    public void ModeChange(bool change)//��l���A�q��������Ƃ��ɌĂяo���֐�
    {
        _adultState = change;
    }
    ///// <summary>���������ꍇ�Ă�,�A�j���[�V�����C�x���g��p�֐�</summary>
    //public void PlayerFind()
    //{
    //    IsGame.GameManager.Instance.GameOver();
    //}
}

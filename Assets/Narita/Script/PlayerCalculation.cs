using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCalculation : MonoBehaviour
{
    [Tooltip("�v���C���[��_returnPillowPos�̋���")]
    float _returnPillowDisToPlayer;
    [Tooltip("����Ԃ����̃J�E���g�p�^�C�}�[�AUIManager�֓n��")]
    float _returnCountTime = 0f;
    [SerializeField, Header("�덷�̋��e�͈�"), Tooltip("�덷�̋��e�͈�")]
    float _toleranceDis = 0.3f;
    [SerializeField, Header("���̉��ɍs�����Ƃ��Ă��鎞�i�X�y�[�X���͎��j�̃X�s�[�h")]
    float _autoMoveSpeed = 3f;
    [Tooltip("�Q�Ă���G��script���")]
    Returnpillow _pillowEnemy = null;
    [Tooltip("�Q�Ă���G���̂���")]
    GameObject _pillowEnemyObject = null;
    [SerializeField, Header("�v���C���[�������Ă��鎞True"), Tooltip("�v���C���[��velocity��0�ł͂Ȃ��ꍇTrue")]
    bool _playerMove = false;
    [SerializeField, Header("����Ԃ����Ƃ��Ă��鎞True"), Tooltip("����Ԃ���ʒu�ɂ��ăX�y�[�X�L�[�A�܂��̓{�^���������Ă���Ƃ�True")]
    bool _returnPillowInPos = false;
    [Tooltip("�E�����߂��Ƃ�True")]
    bool _closePos = false;
    [Tooltip("�����Ȃ��Ȃ������̍Ō�ɐi��ł�������")]
    Vector2 _lastMoveVelocity;
    [Tooltip("�v���C���[������Ԃ���ʒu���")]
    Vector2 _returnPillowPos = default;
    [Tooltip("�G�͈͓̔��ɓ������Ƃ��A�o���Ƃ��Ɏg�p")]
    SoundManager _sound = null;
    [Tooltip("�X���C�_�[�ɒl��n�����߂Ɏg�p")]
    UIManager _ui = null;
    PlayerAnimator _playerAnimator = null;
    PlayerController _playerController = null;
    [SerializeField]
    CapsuleCollider2D _collider = null;
    public bool PlayerMove { get => _playerMove; }
    public bool ReturnPillowInPos { get => _returnPillowInPos; set => _returnPillowInPos = value; }
    public bool ClosePos { get => _closePos; }
    public Vector2 LastMoveVelocity { get => _lastMoveVelocity; }
    public Returnpillow PillowEnemy { get => _pillowEnemy; set => _pillowEnemy = value; }
    public GameObject PillowEnemyObject { get => _pillowEnemyObject; }
    public Vector2 ReturnPillowPos { get => _returnPillowPos; }

    // Start is called before the first frame update
    void Start()
    {
        _sound = FindObjectOfType<SoundManager>();
        _ui = FindObjectOfType<UIManager>();
        _playerController = GetComponent<PlayerController>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        _collider.isTrigger = _returnPillowInPos == true ? true : false;
    }
    public void VelocitySave(Vector2 velo)
    {
        _playerMove = velo == Vector2.zero ? false : true;
        if (velo.x != 0)
        {
            _lastMoveVelocity.x = velo.x;
        }
        if (velo.y != 0)
        {
            _lastMoveVelocity.y = velo.y;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)//�Q�Ă���G�̏������
    {
        if (collision.TryGetComponent<Returnpillow>(out Returnpillow enemy))
        {
            _sound.SleepingVoice();
            _pillowEnemyObject = collision.gameObject;
            _pillowEnemy = enemy;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ReturnPillow"))
        {
            InformationReset();
        }
        _returnCountTime = 0;
        _ui.ChargeSlider(_returnCountTime);
        _sound.KillSleeping();
    }
    public void InformationReset()//�擾�����f�[�^�S�����A�X���C�_�[�̏�����
    {
        _playerAnimator.AutoAnim = false;
        _playerController.ReturnPillowAction = false;
        _returnPillowInPos = false;
        _pillowEnemyObject = null;
        _pillowEnemy = null;
        _returnPillowPos = default;
        _returnCountTime = 0;
        _ui.ChargeSlider(_returnCountTime);
    }
    public void PlayerAndEnemyDis()//�����v�Z
    {
        if (!_pillowEnemyObject)
            return;
        if (Vector2.Distance(transform.position, _pillowEnemy.ReturnPillouPosLeft.position)
        >= Vector2.Distance(transform.position, _pillowEnemy.ReturnPillouPosRight.position))
        {
            _returnPillowPos = _pillowEnemy.ReturnPillouPosRight.position;
            _closePos = false;
        }
        else
        {
            _returnPillowPos = _pillowEnemy.ReturnPillouPosLeft.position;
            _closePos = true;
        }
    }
    public void TranslatePlayerPos()
    {
        if (!_pillowEnemyObject)
            return;
        _playerAnimator.AutoAnim = true;
        _returnPillowDisToPlayer = Vector2.Distance(transform.position, _returnPillowPos);
        if (_returnPillowDisToPlayer > _toleranceDis)//�덷�͈�
        {
            if (Mathf.Abs(transform.position.x - _returnPillowPos.x) > _toleranceDis)
            {
                if (transform.position.x > _returnPillowPos.x)
                {
                    transform.Translate(Vector2.left * Time.deltaTime * _autoMoveSpeed);
                }
                else if (transform.position.x < _returnPillowPos.x)
                {
                    transform.Translate(Vector2.right * Time.deltaTime * _autoMoveSpeed);
                }
            }
            else
            {
                if (transform.position.y > _returnPillowPos.y)
                {
                    transform.Translate(Vector2.down * Time.deltaTime * _autoMoveSpeed);
                }
                else if (transform.position.y < _returnPillowPos.y)
                {
                    transform.Translate(Vector2.up * Time.deltaTime * _autoMoveSpeed);
                }
            }
        }
        else if (_returnPillowDisToPlayer < _toleranceDis)
        {
            _returnPillowInPos = true;
            _returnCountTime += Time.deltaTime;
            _ui.ChargeSlider(_returnCountTime);
        }
    }
}

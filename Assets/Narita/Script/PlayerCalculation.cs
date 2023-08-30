using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCalculation : MonoBehaviour
{
    [Tooltip("プレイヤーと_returnPillowPosの距離")]
    float _returnPillowDisToPlayer;
    [Tooltip("枕を返す時のカウント用タイマー、UIManagerへ渡す")]
    float _returnCountTime = 0f;
    [SerializeField, Header("誤差の許容範囲"), Tooltip("誤差の許容範囲")]
    float _toleranceDis = 0.3f;
    [SerializeField, Header("枕の横に行こうとしている時（スペース入力時）のスピード")]
    float _autoMoveSpeed = 3f;
    [Tooltip("寝ている敵のscript情報")]
    Returnpillow _pillowEnemy = null;
    [Tooltip("寝ている敵そのもの")]
    GameObject _pillowEnemyObject = null;
    [SerializeField, Header("プレイヤーが動いている時True"), Tooltip("プレイヤーのvelocityが0ではない場合True")]
    bool _playerMove = false;
    [SerializeField, Header("枕を返そうとしている時True"), Tooltip("枕を返せる位置にいてスペースキー、またはボタンを押しているときTrue")]
    bool _returnPillowInPos = false;
    [Tooltip("右側が近いときTrue")]
    bool _closePos = false;
    [Tooltip("動かなくなった時の最後に進んでいた方向")]
    Vector2 _lastMoveVelocity;
    [Tooltip("プレイヤーが枕を返せる位置情報")]
    Vector2 _returnPillowPos = default;
    [Tooltip("敵の範囲内に入ったとき、出たときに使用")]
    SoundManager _sound = null;
    [Tooltip("スライダーに値を渡すために使用")]
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
    private void OnTriggerEnter2D(Collider2D collision)//寝ている敵の情報を取る
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
    public void InformationReset()//取得したデータ全消し、スライダーの初期化
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
    public void PlayerAndEnemyDis()//距離計算
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
        if (_returnPillowDisToPlayer > _toleranceDis)//誤差範囲
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public VariableJoystick _joyStick;
    [SerializeField, Header("子供状態の動くスピード"), Tooltip("動く速度（子供）")]
    float _childMoveSpeed = 10f;
    [SerializeField, Header("大人状態の動くスピード"), Tooltip("動く速度（大人）")]
    float _adultMoveSpeed = 15f;
    [Tooltip("移動速度計算結果")]
    Vector2 _moveVelocity;
    [SerializeField, Tooltip("枕を返すためのボタン")]
    GameObject _returnPillowButton = null;
    [SerializeField, Header("プレイヤーが大人か子供か"), Tooltip("大人の時True")]
    bool _adultState = false;
    [SerializeField, Header("寝ている敵の当たり判定内にいて、ボタンが押されたときTrue")]
    bool _returnPillowAction = false;
    [SerializeField, Tooltip("プレイヤーの当たり判定")]
    PlayerCalculation _playerCalculation = null;
    PlayerAnimator _playerAnimator = null;
    Rigidbody2D _rb;
    [Tooltip("プレイヤーの状態確認、外部参照用")]
    public bool AdultState { get => _adultState; }
    [Tooltip("枕を返せる位置にいてスペースキーを押しているときTrue, 外部参照用")]

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
        //if (Input.GetButton("Jump"))//スペース長押し
        //if (Input.GetMouseButton(0))
        if(_returnPillowAction || Input.GetButton("Jump"))
        {
            if (_playerCalculation.PillowEnemy)//枕返し圏内にいたら
            {
                _playerCalculation.TranslatePlayerPos();
            }
        }
        else
        {
            _playerCalculation.ReturnPillowInPos = false;
            _playerAnimator.AutoAnim = false;
        }
        //if (Input.GetButtonDown("Jump"))//自動で動くために距離計算を行う,スペースキー一回 || 
        if (_returnPillowAction || Input.GetButtonDown("Jump"))
        {
            if (_playerCalculation.PillowEnemy)//枕返し圏内にいたら
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
    public void ModeChange(bool change)//大人化、子供化するときに呼び出す関数
    {
        _adultState = change;
    }
    ///// <summary>見つかった場合呼ぶ,アニメーションイベント専用関数</summary>
    //public void PlayerFind()
    //{
    //    IsGame.GameManager.Instance.GameOver();
    //}
}

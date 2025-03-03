
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>徘徊する敵の動きを制御するscript</summary>
public class NakaiEnemy : MonoBehaviour//辺りを見回すのはアニメーション内でコライダーの向きを変更すれば良い。
{
    [SerializeField,Tooltip("自身のレイヤーを取得するためのもの")]
    SpriteRenderer _nakaiSprite = null;
    [Tooltip("壁に当たった回数")]
    int _number = 0;
    [Tooltip("_numberの最大値")]
    private int _maxNumber = 4;
    [SerializeField, Tooltip("ナカイの動く速さ")]
    float _moveSpeed = 5f;
    [SerializeField, Header("目標との距離の余裕"), Tooltip("目標との距離の余裕")]
    float _pointDis = 0.5f;
    //[SerializeField, Tooltip("playerを見つけるための当たり判定")]
    //GameObject _atari = null;
    [Tooltip("ナカイの動きが変わる時のステージのレベル")]
    int _stageLevelBorder = 0;
    //[Tooltip("受け取ったpointの要素番号")]
    //int _pointArrayNumber = 0;
    [SerializeField, Tooltip("playerを見つけたときTrue、Trueの時にはナカイは動かない")]
    bool _playerFind = false;
    [SerializeField, Tooltip("受け取ったステージのレベルが_stageLevelBorder以上ならTrue")]
    bool _levelBorder = false;
    [Tooltip("アニメーションイベント用,徘徊アニメーションが一周したらtrue")]
    bool _lookAround = false;
    //[Tooltip("外部から受け取る、徘徊する位置情報")]
    //Transform[] _points = null;
    //[Tooltip("移動方向への速度計算結果")]
    //Vector2 _dir = default;
    [Tooltip("動かなくなった時の最後に進んでいた方向")]
    Vector2 _lastMoveVelocity = default;
    [SerializeField, Tooltip("playerを見つけたときに使用")]
    SoundManager _sound = null;
    Animator _anim = null;
    Rigidbody2D _rb = null;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sound = FindObjectOfType<SoundManager>();
        _anim.SetBool("levelBorder", _levelBorder);
    }
    void Update()
    {
        VelocitySave(_rb.velocity);
        if (!_playerFind)//プレイヤーを見つけていない
        {
            if (!_levelBorder || !_lookAround && _levelBorder)
            {
                switch (_number % _maxNumber)//0%4 = 0;1%4 = 1;...
                {
                    case 0:
                        {
                            _rb.velocity = Vector2.up * _moveSpeed;
                            break;
                        }
                    case 1:
                        {
                            _rb.velocity = Vector2.left * _moveSpeed;
                            break;
                        }
                    case 2:
                        {
                            _rb.velocity = Vector2.down * _moveSpeed;
                            break;
                        }
                    case 3:
                        {
                            _rb.velocity = Vector2.right * _moveSpeed;
                            break;
                        }
                }
            }
            else
            {
                _rb.velocity = Vector2.zero;
            }
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
        _anim.SetBool("lookAround", _lookAround);
    }
    /// <summary>渡す側は順番に気を付けること</summary>
    /// <param name="pointsArray"></param>
    //public void GetPoints(Transform[] pointsArray)
    //{
    //    _points = new Transform[pointsArray.Length];
    //    for (int i = 0; i < _points.Length; i++)
    //    {
    //        _points[i] = pointsArray[i];
    //    }
    //}
    public void GetPlayerLevel(int level)
    {
        _levelBorder = _stageLevelBorder <= level ? true : false;
        _anim.SetBool("levelBorder", _levelBorder);
    }

    public void LookAroundIsActive()//アニメーションイベント用
    {
        _lookAround = !_lookAround;
    }
    private void VelocitySave(Vector2 velo)
    {
        if (velo != Vector2.zero)
            _lastMoveVelocity = velo;

        if (!_anim)
            return;
        _anim.SetFloat("lastVeloX", _lastMoveVelocity.x);//のちに名前を決める
        _anim.SetFloat("lastVeloY", _lastMoveVelocity.y);
        _anim.SetFloat("vertical", Mathf.Abs(_lastMoveVelocity.x));
        _anim.SetFloat("horizontal", Mathf.Abs(_lastMoveVelocity.y));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerFind = true;
            _anim.SetBool("playerFind", _playerFind);
            _sound.Discoverd();
        }
        else
        {
            _number++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController player))
        {
            _playerFind = true;
            if (gameObject.transform.position.y < collision.transform.position.y)
            {//playerより下にいる状態で発見した場合は、自身のレイヤーをplayerと同じにすることで絵が隠れることを回避。
                SpriteRenderer _playerSprite = collision.GetComponent<SpriteRenderer>();
                _nakaiSprite.sortingOrder = _playerSprite.sortingOrder;
            }
            _anim.SetBool("playerFind", _playerFind);
            _sound.Discoverd();
        }
    }

    public void PlayerFind()//アニメーションイベント用
    {
        IsGame.GameManager.Instance.GameOver();
    }
}

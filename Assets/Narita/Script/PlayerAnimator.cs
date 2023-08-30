using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    PlayerController _playerController = null;
    PlayerCalculation _playerCalculation = null;
    Animator _playerAnim = null;
    [SerializeField, Header("–‚Ì‰¡‚ÉŽ©“®“I‚ÉˆÚ“®‚µ‚Ä‚¢‚é‚Æ‚«‚Étrue"), Tooltip("–‚Ì‰¡‚ÉŽ©“®“I‚ÉˆÚ“®‚µ‚Ä‚¢‚é‚Æ‚«‚Étrue")]
    bool _autoAnim = false;
    public bool AutoAnim { get => _autoAnim; set => _autoAnim = value; }

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerCalculation = GetComponent<PlayerCalculation>();
        _playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerAnim)
        {
            return;
        }
        else
        {
            _playerAnim.SetFloat("veloX", _playerController.Rb.velocity.x);
            _playerAnim.SetFloat("veloY", _playerController.Rb.velocity.y);
            _playerAnim.SetFloat("LastVeloX", _playerCalculation.LastMoveVelocity.x);
            _playerAnim.SetFloat("LastVeloY", _playerCalculation.LastMoveVelocity.y);
            _playerAnim.SetBool("playerMove", _playerCalculation.PlayerMove);
            _playerAnim.SetBool("adultState", _playerController.AdultState);
            _playerAnim.SetBool("closePos", _playerCalculation.ClosePos);
            _playerAnim.SetBool("returnPillowInPos", _playerCalculation.ReturnPillowInPos);
            _playerAnim.SetBool("autoMode", AutoAnim);
        }
    }
}

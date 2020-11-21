using UnityEngine;
//キーボードからのPlayerの入力処理
public class PlayerInput : IPlayerInput
{
    [SerializeField]private float _x;
    [SerializeField]private float _z;
    [SerializeField]private bool _jump;

    public void Inputting()
    {
        _x = Input.GetAxis("Horizontal");
        _z = Input.GetAxis("Vertical");
        _jump = Input.GetKeyDown(KeyCode.Space);
    }

    public float X
    {
        get { return this._x; }
        set { this._x = value; }
    }
    
    public float Z
    {
        get { return this._z; }
        set { this._z = value; }
    }

    public bool Jump
    {
        get { return this._jump; }
        set { this._jump = value; }
    }
}

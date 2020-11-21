using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerScript : MonoBehaviour
{

    public float speed = 3.0F;
    public float rotateSpeed = 3.0F;

    private CharacterController controller;

    void Start()
    {

        // コンポーネントの取得
        controller = GetComponent<CharacterController>();

    }

    void Update()
    {

        // 回転
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);

        // キャラクターのローカル空間での方向
        Vector3 forward = transform.transform.forward;

        float curSpeed = speed * Input.GetAxis("Vertical");

        // SimpleMove関数で移動させる
        controller.SimpleMove(forward * curSpeed);

    }

}
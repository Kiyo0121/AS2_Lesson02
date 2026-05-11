using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Player : MonoBehaviour
{
    [Header("** Shot Settings **")]
    public Transform shotPoint;
    public GameObject bulletPrefab;
    [SerializeField] private float shellSpeed = 1000.0f;
    [SerializeField] private int pelletsCount = 8;
    [SerializeField] private float spreadIntensity = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //PlayerInputから[Move]アクションを呼び出すメソッド
    public void OnMove(InputValue value)
    {
        //第一引数にPlayerInputから渡された値[InputValue]を取得する
        Debug.Log($"移動[{value.Get<Vector2>()}");

        Vector3 move = new Vector3(value.Get<Vector2>().x, value.Get<Vector2>().y, 0 );

        if (transform.position.x + value.Get<Vector2>().x < -8 || transform.position.x + value.Get<Vector2>().x > 8) return;

        if (transform.position.y + value.Get<Vector2>().y < -4 || transform.position.y + value.Get<Vector2>().y > 6) return;

        move.x = Mathf.Round(move.x);
        move.y = Mathf.Round(move.y);

        transform.Translate(move);
    }

    //PlayerInputから[Attack]アクションを呼び出すメソッド
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        for (int i = 0; i < pelletsCount; i++)
        {
            // 1. ランダムな拡散角度を計算
            // 正面方向を基準に、上下左右にランダムな角度をつける
            float randomSpreadX = Random.Range(-spreadIntensity, spreadIntensity);
            float randomSpreadY = Random.Range(-spreadIntensity, spreadIntensity);

            // 2. shotPointの回転にランダムな回転を合成
            // 3Dの場合は Y軸とX軸にランダムを加えるのが一般的です
            Quaternion spreadRotation = Quaternion.Euler(randomSpreadX, randomSpreadY, 0);
            Quaternion finalRotation = shotPoint.rotation * spreadRotation;

            // 3. 弾を生成
            GameObject shell = Instantiate(bulletPrefab, shotPoint.position, finalRotation);

            // 4. 物理演算で飛ばす
            Rigidbody shellRb = shell.GetComponent<Rigidbody>();
            if (shellRb != null)
            {
                // 個別の弾の向き(forward)に向かって飛ばす
                shellRb.AddForce(shell.transform.forward * shellSpeed);
            }

            // 弾が重なりすぎないよう、寿命を少しランダムにしても面白いです
            Destroy(shell, 2.0f + Random.Range(0f, 1.0f));
        }
    }
}
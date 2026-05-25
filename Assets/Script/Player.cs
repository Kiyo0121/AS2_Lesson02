using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("** Shot Settings **")]
    public Transform shotPoint;
    public GameObject bulletPrefab;
    [SerializeField] private float shellSpeed = 1000.0f;
    [SerializeField] private int pelletsCount = 8;
    [SerializeField] private float spreadIntensity = 10.0f;

    [Header("＊＊＊移動値の設定")]
    private Vector3 inputMoveVelocity;

    [Header("＊＊＊回転軸の設定")]
    public GameObject lookAxis;
    public GameObject gyroAxis;
    private Vector3 lookAngles;
    private float gyroAngles;
    public bool tiltInvart;

    void Start()
    {
        
    }

    void Update()
    {
        float zSpeed = 5 * Time.deltaTime;
        transform.Translate(0, 0, zSpeed);

        lookAngles.x += inputMoveVelocity.y * (tiltInvart ? -1 : 1);
        lookAngles.y += inputMoveVelocity.x;
        gyroAngles += inputMoveVelocity.x * -1;

        lookAngles = Vector3.Lerp(lookAngles, Vector3.zero, Time.deltaTime * 3);
        gyroAngles = Mathf.Lerp(gyroAngles, 0, Time.deltaTime * 3);


        lookAngles.x = Mathf.Clamp(lookAngles.x, -15, 15);
        lookAngles.y = Mathf.Clamp(lookAngles.y, -15, 15);
        gyroAngles = Mathf.Clamp(gyroAngles, -15, 15);

        lookAxis.transform.eulerAngles = lookAngles;
        gyroAxis.transform.eulerAngles = new Vector3(0, 0, gyroAngles);
    }


    //PlayerInputから[Move]アクションを呼び出すメソッド
    public void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        Debug.Log($"移動: {inputVector}");

        // 移動後の位置を予測して制限をかける
        Vector3 nextPosition = transform.position + new Vector3(inputVector.x, inputVector.y, 0);

        if (nextPosition.x < -8 || nextPosition.x > 8) return;
        if (nextPosition.y < -4 || nextPosition.y > 6) return;

        // グリッド移動（1マスずつ移動）にするための四捨五入
        Vector3 move = new Vector3(Mathf.Round(inputVector.x), Mathf.Round(inputVector.y), 0);

        transform.Translate(move, Space.World); // プレイヤーの回転に影響されない絶対方向への移動

        inputMoveVelocity = move;
    }

    //PlayerInputから[Attack]アクションを呼び出すメソッド
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        // shotPointが設定されていない場合は、自身の位置を発射点にする
        Vector3 spawnPosition = shotPoint != null ? shotPoint.position : transform.position;

        for (int i = 0; i < pelletsCount; i++)
        {
            // 1. ランダムな拡散角度を計算
            float randomSpreadX = Random.Range(-spreadIntensity, spreadIntensity);
            float randomSpreadY = Random.Range(-spreadIntensity, spreadIntensity);

            // 2. 【変更点】プレイヤーの回転（transform.rotation）にランダムな回転を合成
            Quaternion spreadRotation = Quaternion.Euler(randomSpreadX, randomSpreadY, 0);
            Quaternion finalRotation = transform.rotation * spreadRotation;

            // 3. 弾を生成
            GameObject shell = Instantiate(bulletPrefab, spawnPosition, finalRotation);

            // 4. 物理演算で飛ばす
            Rigidbody shellRb = shell.GetComponent<Rigidbody>();
            if (shellRb != null)
            {
                // 生成された弾の正面（forward）に向かって力を加える
                shellRb.AddForce(shell.transform.forward * shellSpeed);
            }
            // 2Dゲーム（Rigidbody2D）の場合は以下のように記述します
            // Rigidbody2D shellRb2D = shell.GetComponent<Rigidbody2D>();
            // if (shellRb2D != null) { shellRb2D.AddForce(shell.transform.up * shellSpeed); }

            Destroy(shell, 2.0f + Random.Range(0f, 1.0f));
        }
    }
}
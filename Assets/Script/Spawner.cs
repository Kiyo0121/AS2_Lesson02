using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnPrefab;

    private float spawnInterval = 2f;
    private float spawnTimer = 0f;

    // スクリプト側で範囲を固定するため、インスペクター用の変数は削除、
    // または初期値として範囲内に収まるIndexを設定します。
    public int minYIndex = -4; // 実際のY座標: -7.5
    public int maxYIndex = 3;  // 実際のY座標:  6.5

    void Start()
    {

    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnObject();
            spawnTimer = 0f;
        }
    }

    private void SpawnObject()
    {
        Player player = GameObject.FindAnyObjectByType<Player>();

        if (player == null) return;

        float playerZ = player.transform.position.z;

        Vector3 randomPos = Vector3.zero;

        // --- X軸の処理 (-6 〜 6 の間に収める) ---
        // 範囲: -3, -2, -1, 0, 1, 2 (最大値未満のため 2+1=3 を指定)
        // 実際の座標: -5.5, -3.5, -1.5, 0.5, 2.5, 4.5
        int randomXIndex = Random.Range(-3, 3);
        randomPos.x = (randomXIndex * 2f) + 0.5f;

        // --- Y軸の処理 (-8 〜 8 の間に収める) ---
        // インスペクターで設定された範囲でランダム（初期値の -4〜3 の場合、実際の座標は -7.5 〜 6.5）
        // ※もしインスペクターから変更する場合は、minは -4 以上、maxは 3 以下にしてください。
        int randomYIndex = Random.Range(minYIndex, maxYIndex + 1);
        randomPos.y = (randomYIndex * 2f) + 0.5f;

        randomPos.z = playerZ + 100;

        Instantiate(spawnPrefab, randomPos, transform.rotation);
    }
}
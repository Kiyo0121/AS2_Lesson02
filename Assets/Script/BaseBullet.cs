using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    private Rigidbody _rigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent(out _rigidbody);
    }

    // Update is called once per frame
    void Update()
    {
        Raycast();
    }

    public void Raycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        RaycastHit hit;
        bool collision = Physics.Raycast(ray, out hit, 2, 1 >> 1);
        if (collision)
        {
            Debug.Log($"線上にオブジェクトがあります　=>　{hit.transform?.name}");
            if (hit.transform.name.Contains("Enemy"))
            {
                Debug.Log($"敵オブジェクトに衝突しました");
                Destroy(hit.transform.gameObject);
            }
        }
    }
}

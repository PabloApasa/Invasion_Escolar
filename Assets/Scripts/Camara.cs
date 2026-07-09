using UnityEngine;

public class Camara : MonoBehaviour
{
    [SerializeField] private Transform transformPlayer;

    void Start()
    {

    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transformPlayer.position.x, transformPlayer.position.y, transform.position.z);
    }
}

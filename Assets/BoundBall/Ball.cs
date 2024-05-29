using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private PongPlayer _pongPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("PongPlayer"))
            _pongPlayer.Penalty();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(transform.localPosition.y + transform.localPosition.x) > 170f)
        {
            Debug.Log($"Out of bound!!");
            _pongPlayer.Reset();
        }
    }
}

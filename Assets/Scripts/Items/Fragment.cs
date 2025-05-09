using UnityEngine;

public class Fragment : MonoBehaviour
{
    public int scorePoints = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.inputManager.NearbyFragment = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.inputManager.NearbyFragment = null;
        }
    }

    public void Interact()
    {
        GameManager.Instance.fragmentAudio.Play();
        Debug.Log("OOII");
        GameManager.Instance.AddScore(scorePoints);
        Destroy(gameObject);
        GameManager.Instance.inputManager.NearbyFragment = null;
    }

}

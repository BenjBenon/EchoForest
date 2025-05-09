using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InterLadder : Interactable
{
    [Header("Placement Settings")]
    [SerializeField] private float distanceFromPlayer = 2f;
    [SerializeField] private float heightFromPlayer = 2f;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    [Header("Player Settings")]
    [SerializeField] private GameObject globalPlayer;
    [SerializeField] private Collider playerCollider;

    [Header("Climbing Settings")]
    [SerializeField] private float sizeGO = 1.5f;

    private void Awake()
    {
        Name = "ladder";
    }

    public override void UseFromInventory()
    {
        base.UseFromInventory();


        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        Transform playerTransform = playerObj.transform;

        Vector3 forward = playerTransform.forward;
        Vector3 newPosition = playerTransform.position + forward * distanceFromPlayer + playerTransform.up * heightFromPlayer;

        transform.position = newPosition;
        transform.rotation = Quaternion.LookRotation(-forward);
        transform.Rotate(rotationOffset);

        gameObject.SetActive(true);
        GetComponent<AudioSource>().Play();

    }

    public override void StartUse()
    {
        base.StartUse();
        StartCoroutine(ClimbLadder());
    }

    public override void EndUse()
    {
        base.EndUse();
        StartCoroutine(DownLadder());
    }

    IEnumerator ClimbLadder()
    {
        globalPlayer.GetComponent<Rigidbody>().isKinematic = true;
        playerCollider.enabled = false;
        globalPlayer.GetComponent<InputManager>().OnUseEnable(false);

        //Player climbing 
        float climbDuration = 2f;
        float elapsed = 0f;
        Vector3 startPos = transform.position - transform.up * sizeGO;
        Vector3 endPos = transform.position + transform.up * sizeGO;

        while (elapsed < climbDuration)
        {
            globalPlayer.transform.position = Vector3.Lerp(startPos, endPos, elapsed / climbDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        globalPlayer.transform.position = endPos;

        playerCollider.enabled = true;

    }

    IEnumerator DownLadder()
    {

        playerCollider.enabled = false;

        //Player climbing down
        float climbDuration = 2f;
        float elapsed = 0f;
        Vector3 startPos = transform.position + transform.up * sizeGO; 
        Vector3 endPos = transform.position - transform.up * sizeGO;

        while (elapsed < climbDuration)
        {
            globalPlayer.transform.position = Vector3.Lerp(startPos, endPos, elapsed / climbDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        globalPlayer.transform.position = endPos;


        globalPlayer.GetComponent<Rigidbody>().isKinematic = false;
        playerCollider.enabled = true;
        globalPlayer.GetComponent<InputManager>().OnUseEnable(true);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InterJetPack : Interactable
{
    [Header("Placement Settings")]
    [SerializeField] private float distanceFromPlayer = 0f;
    [SerializeField] private float heightFromPlayer = 1f;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    [Header("Player Settings")]
    [SerializeField] private GameObject globalPlayer;

    [Header("Flight Settings")]
    [SerializeField] private float flightHeight = 1f;

    private void Awake()
    {
        Name = "jetpack";
    }

    public override void UseFromInventory()
    {
        base.UseFromInventory();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        Transform playerTransform = playerObj.transform;

        Vector3 offset = playerTransform.forward * distanceFromPlayer + playerTransform.up * heightFromPlayer;

        // Attach jet pack to the player
        transform.SetParent(playerTransform);
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;
        transform.Rotate(rotationOffset);

        GameManager.Instance.inputManager.WoreJetPack = this; 

        gameObject.SetActive(true);

    }

    public override void StartUse()
    {
        base.StartUse();
        StartCoroutine(FlyWithJetpack());
    }

    public override void EndUse()
    {
        base.EndUse();

        Destroy(gameObject);
    }

    IEnumerator FlyWithJetpack()
    {
        if (globalPlayer == null) yield break;

        Rigidbody rb = globalPlayer.GetComponent<Rigidbody>();
        if (rb == null) yield break;

        GameManager.Instance.inputManager.isFlying = true;

        rb.useGravity = false;
        //rb.constraints |= RigidbodyConstraints.FreezePositionY;

        Vector3 targetPosition = new Vector3(globalPlayer.transform.position.x, flightHeight, globalPlayer.transform.position.z);

        float duration = 0.4f;
        float elapsed = 0f;
        Vector3 startPos = globalPlayer.transform.position;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector3.Lerp(startPos, targetPosition, elapsed / duration));
            //globalPlayer.transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        globalPlayer.transform.position = targetPosition;

        yield return new WaitForSeconds(5f);

        GameManager.Instance.inputManager.isFlying = false;
        rb.useGravity = true;
        //rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

        EndUse();
    }


}
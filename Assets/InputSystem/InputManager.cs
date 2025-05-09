using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputManager : MonoBehaviour
{
    [SerializeField] InputActionReference up;
    [SerializeField] InputActionReference left;
    [SerializeField] InputActionReference down;
    [SerializeField] InputActionReference right;
    [SerializeField] InputActionReference interact;
    [SerializeField] InputActionReference inventory;
    [SerializeField] InputActionReference use;
    [SerializeField] InputActionReference quit;

    [SerializeField] float moveSpeed = 5f;
    private Rigidbody rb;

    [SerializeField] Animator playerAnim;

    private AudioSource playerAudioSource;

    [SerializeField] Transform playerShape;
    public bool isMoving = false;

    [SerializeField] GameObject feet;

    private bool inputsEnabled = true;

    //inventory Canva display
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject inventoryCanvas;

    //Game detecton
    private Interactable nearbyInteractable = null;
    public Interactable NearbyInteractable { get { return nearbyInteractable; } set { nearbyInteractable = value; } }
    private Interactable woreJetPack = null;
    public Interactable WoreJetPack { get { return woreJetPack; } set { woreJetPack = value; } }
    private Fragment nearbyFragment = null;
    public Fragment NearbyFragment { get { return nearbyFragment; } set { nearbyFragment = value; } }


    //Use items
    private bool isUsing = false;
    public bool isFlying = false;


    // Start is called before the first frame update
    void Start()
    {
        EnableAllInputs(true);
        rb = GetComponent<Rigidbody>();

        //Audio source
        playerAudioSource = GetComponent<AudioSource>();
    }

    public void EnableAllInputs(bool enable)
    {
        inputsEnabled = enable;

        if (enable)
        {
            up.action.Enable();
            left.action.Enable();
            down.action.Enable();
            right.action.Enable();

            inventory.action.Enable();
            inventory.action.performed += OnInventory;

            interact.action.Enable();
            interact.action.performed += OnInteract;

            use.action.Enable();
            use.action.performed += OnUse;

            quit.action.Enable();
            quit.action.performed += OnQuit;
        }
        else
        {
            up.action.Disable();
            left.action.Disable();
            down.action.Disable();
            right.action.Disable();

            inventory.action.Disable();
            inventory.action.performed -= OnInventory;

            interact.action.Disable();
            interact.action.performed -= OnInteract;

            use.action.Disable();
            use.action.performed -= OnUse;

            quit.action.Disable();
            quit.action.performed -= OnQuit;

            rb.velocity = Vector3.zero + new Vector3(0, rb.velocity.y, 0);
        }
    }

    public void OnUseEnable(bool enable)
    {
        if (enable)
        {
            up.action.Enable();
            left.action.Enable();
            down.action.Enable();
            right.action.Enable();
        }
        else
        {
            up.action.Disable();
            left.action.Disable();
            down.action.Disable();
            right.action.Disable();
        }
    }

    void Update()
    {
        if (!inputsEnabled)
        {
            playerAudioSource.Stop();
            return;
        }
        if (!feet.GetComponent<FeetBehavior>().isGrounded)
        {
            playerAudioSource.Stop();
        }

        bool isMovingThisFrame = false;

        Vector3 moveDirection = Vector3.zero;

        if (up.action.ReadValue<float>() > 0)
        {
            moveDirection += Vector3.forward;
            isMoving = true;
            isMovingThisFrame = true;

            if (!playerAudioSource.isPlaying && feet.GetComponent<FeetBehavior>().isGrounded)
            {
                playerAudioSource.Play();
            }
        }
        if (down.action.ReadValue<float>() > 0)
        {
            moveDirection += Vector3.back;
            isMoving = true;
            isMovingThisFrame = true;

            if (!playerAudioSource.isPlaying && feet.GetComponent<FeetBehavior>().isGrounded)
            {
                playerAudioSource.Play();
            }
        }
        if (left.action.ReadValue<float>() > 0)
        {
            moveDirection += Vector3.left / (1.5f);
            isMoving = true;
            isMovingThisFrame = true;

            if (!playerAudioSource.isPlaying && feet.GetComponent<FeetBehavior>().isGrounded)
            {
                playerAudioSource.Play();
            }
        }
        if (right.action.ReadValue<float>() > 0)
        {
            moveDirection += Vector3.right / (1.5f);
            isMoving = true;
            isMovingThisFrame = true;

            if (!playerAudioSource.isPlaying && feet.GetComponent<FeetBehavior>().isGrounded)
            {
                playerAudioSource.Play();
            }
        }
        if (!isMovingThisFrame)
        {
            isMoving = false;
            playerAudioSource.Stop();
            playerAnim.SetFloat("MoveSpeed", 0f);
        }


        // Player movement
        if (moveDirection != Vector3.zero && rb != null && (feet.GetComponent<FeetBehavior>().isGrounded || isFlying))
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
            playerShape.rotation = Quaternion.Slerp(playerShape.rotation, targetRotation, Time.deltaTime * 10f);

            rb.velocity = moveSpeed * moveDirection.normalized + new Vector3(0, rb.velocity.y, 0);

            playerAnim.SetFloat("MoveSpeed", 1f);
        }
        else if (rb != null)
        {
            rb.velocity = Vector3.zero + new Vector3(0, rb.velocity.y, 0);
            playerAnim.SetFloat("MoveSpeed", 0f);
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!inputsEnabled) return;
        Debug.Log("interact");

        if (nearbyFragment != null)
        {
            Debug.Log("Prise frag");
            nearbyFragment.Interact();
            return;
        }
        else if (nearbyInteractable != null && !nearbyInteractable.isOnUse)
        {
            nearbyInteractable.Interact();
            return;
        }
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (!inputsEnabled) return;

        inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
    }

    private void OnUse(InputAction.CallbackContext context)
    {
        if (nearbyInteractable == null)
        {
            if (woreJetPack == null) return;

            woreJetPack.StartUse();

            return;
        }


        if (isUsing)
        {
            nearbyInteractable.EndUse();
        }
        else
        {
            nearbyInteractable.StartUse();
        }

        isUsing = !isUsing;
    }
    private void OnQuit(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Arrête le mode Play dans l'éditeur
#else
        Application.Quit();
#endif
    }
}
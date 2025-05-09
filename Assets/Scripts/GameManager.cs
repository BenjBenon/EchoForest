using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    //-------GAME LOGIC--------

    [Header("Managers")]
    public InputManager inputManager;
    public InventoryManager inventoryManager;

    [Header("Audio")]
    public AudioSource fragmentAudio;
    public AudioSource inventoryAudio;
    public AudioSource winAudio;

    [Header("Zone System")]
    //now
    [SerializeField] private GameObject zoneSphere;
    private int currentGrowthState = 0;
    //previous
    public HashSet<string> unlockedZones = new HashSet<string>();

    [Header("Fragment System")]
    [SerializeField] GameObject winCanvas;
    private int score = 0;
    public int Score { get { return score; } set { score = value; } }
    public event System.Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UnlockZone(string zoneID)
    {
        if (!unlockedZones.Contains(zoneID))
        {
            unlockedZones.Add(zoneID);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (OnScoreChanged != null)
        {
            OnScoreChanged.Invoke(score);
        }
        currentGrowthState++;

        Animator sphereAnim = zoneSphere.GetComponent<Animator>();
        if (sphereAnim != null)
        {
            sphereAnim.SetInteger("growthState",currentGrowthState);
        }

        if (score >= 18)
        {
            winCanvas.SetActive(true);
            inputManager.EnableAllInputs(false);
            winAudio.Play();
        }
    }
}

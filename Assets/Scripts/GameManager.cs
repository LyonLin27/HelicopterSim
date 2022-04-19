using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    [Header("Settings")]
    public int blobsPerSpawn = 5;
    public int maxBossBlobs = 2;
    public int maxUFOs = 10;
    [HideInInspector] public int UFOcount = 0;
    [HideInInspector] public int bossBlobCount = 0;
    public static GameManager instance;

    [Header("References")]
    public AudioSource bgm;
    public GameObject minionBlob;
    public GameObject bossBlob;
    public GameObject spawner;
    public GameObject UFO;
    public Transform BuildingParent;
    public Transform UFOSpawnPoints;
    public List<Building> BuildingList;
    public MeshRenderer holodeck;
    public HolodeckUIManager holodeckUI;
    public Transform levelPractice;
    public Transform levelCity;
    public List<Transform> hideOnTakeOff;
    private List<BlobFood> BlobFoodList = new List<BlobFood>();

    private float holodeckTimer = 0.0f;
    private bool holodeckFading = false;
    private float holodeckFadeDelay = 4.0f;
    private float holodeckFadeDuration = 2.0f;
    [HideInInspector]
    public bool playerTakeOff = false;

    public PlayerController player;
    public PlayerInput playerInput;
    [HideInInspector] public List<SpawnPoint> SpawnPointsList = new List<SpawnPoint>();

    [HideInInspector] public List<Transform> enemyList;

    public enum LevelType { practice, city}
    [HideInInspector]
    public LevelType currLevel = LevelType.practice;

    private int score;

    private void Awake() {
        instance = this;

        playerInput = new PlayerInput();

        if (PlayerPrefs.HasKey("SavedLevel"))
            SelectLevel((LevelType)PlayerPrefs.GetInt("SavedLevel"));

        BuildingList = new List<Building>();
        enemyList = new List<Transform>();

        foreach (Building building in BuildingParent.GetComponentsInChildren<Building>())
        {
            BuildingList.Add(building);
        }

        score = 0;
    }

	private void OnDestroy()
	{
        playerInput.Destroy();
	}

	public void Update()
    {
        if(!playerTakeOff){
            if(player.GetVerticalSpd() > 0.1f){
                InitiateHolodeckFade();
                if (currLevel == LevelType.city)
                {
                    InvokeRepeating("SpawnSomething", 5.0f, 20f);
                }
                holodeck.GetComponent<Collider>().enabled = false;
                playerTakeOff = true;
                foreach (var trans in hideOnTakeOff)
                {
                    trans.gameObject.SetActive(false);
                }
                bgm.Play();
            }
            return;
        }
        CleanEnemyList();
        if(holodeckFading)
        {
            holodeckTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, holodeckTimer / holodeckFadeDuration);
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", new Color(1, 1, 1, alpha));
            holodeck.SetPropertyBlock(block);
            if (holodeckTimer > holodeckFadeDuration)
            {
                holodeckFading = false;
                holodeckTimer = 0.0f;
                holodeck.gameObject.SetActive(false);
            }
        }
    }

    public void InitiateHolodeckFade()
    {
        holodeckFading = true;
    }

    private void Start() {
        SetupLevel();
    }

    private void SetupLevel()
    {
        levelCity.gameObject.SetActive(currLevel == LevelType.city);
        levelPractice.gameObject.SetActive(currLevel == LevelType.practice);
    }

    public void SpawnSomething() // creates a spawner teleport beam at a random spawnpoint
    {
        if(Random.value > 0.7f && UFOcount < maxUFOs) // spawn UFO
        {
            for(int i = 0; i < 5; i++){
                GameObject spawnedUfo = Instantiate(UFO, GetRandomUFOSpawnPoint(), Quaternion.identity, null);
                enemyList.Add(spawnedUfo.transform);
                UFOcount++;
            }
        }
        else // create telporter beam
        {
            SpawnPoint spawnPoint = GetRandSpawnPoint();
            GameObject beam = Instantiate(spawner, spawnPoint.transform.position, Quaternion.identity, null);
            beam.transform.position = new Vector3(beam.transform.position.x, 10000, beam.transform.position.z);
            Spawner spawnerCtrl = beam.transform.GetChild(0).GetComponent<Spawner>();
            spawnerCtrl.SetSpawnDestination(spawnPoint.transform.position);
            if (Random.value > 0.8f && bossBlobCount < maxBossBlobs) // spawn boss blob
            {
                spawnerCtrl.SetPrefab(bossBlob);
                spawnerCtrl.SetEnemyCount(1);
            }
            else // spawn minion blob
            {
                spawnerCtrl.SetPrefab(minionBlob);
                spawnerCtrl.SetEnemyCount(blobsPerSpawn);
            }
        }
        
    }

    public Building GetRandBuilding() {
        var activeBuildingList = BuildingList.FindAll(x => x.health > 0);
        if (activeBuildingList.Count == 0)
        {
            HandlePlayerDeath();
        }

        int randIndex = Random.Range(0, activeBuildingList.Count);
        while (activeBuildingList[randIndex].health <= 0) {
            randIndex = Random.Range(0, activeBuildingList.Count);
        }
        return activeBuildingList[randIndex];
    }

    public SpawnPoint GetRandSpawnPoint()
    {
        int randIndex = Random.Range(0, SpawnPointsList.Count);
        return SpawnPointsList[randIndex];
    }

    public Vector3 GetRandomUFOSpawnPoint()
    {
        int randIndex = Random.Range(0, UFOSpawnPoints.childCount);
        return UFOSpawnPoints.GetChild(randIndex).transform.position;
    }

    public Building GetClosestBuilding(Vector3 position)
    {
        Building closestBuilding = null;
        float closestDistance = 9999999f;
        foreach(Building building in BuildingList)
        {
            if(building.health <= 0)
            {
                continue;
            }
            float dist = Vector3.Distance(position, building.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestBuilding = building;
            }
        }
        return closestBuilding;
    }

    public BlobFood GetClosestBlobFood(Vector3 position)
    {
        BlobFood closestFood = null;
        float closestDistance = 9999999f;
        foreach (BlobFood food in BlobFoodList)
        {
            if(food.currentMinionBlob != null)
            {
                continue;
            }
            float dist = Vector3.Distance(position, food.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestFood = food;
            }
        }
        return closestFood;
    }

    public void AddBlobFood(BlobFood food)
    {
        if(BlobFoodList.Contains(food) == false) BlobFoodList.Add(food);

    }
    public void RemoveBlobFood(BlobFood food)
    {
        if (BlobFoodList.Contains(food) == true) BlobFoodList.Remove(food);
    }

    private void CleanEnemyList(){
        for(int i = 0; i < enemyList.Count; i++){
            if(!enemyList[i]){
                enemyList.RemoveAt(i);
                break;
            }
        }
    }

    public void HandlePlayerDeath()
    {
        if (PlayerPrefs.HasKey("HighestScore"))
        {
            if(PlayerPrefs.GetInt("HighestScore") <score)
                PlayerPrefs.SetInt("HighestScore", score);
        }
        else 
        {
            PlayerPrefs.SetInt("HighestScore", score);
        }

        StartCoroutine(RestartInTime(5f));
    }

    IEnumerator RestartInTime(float time)
    {
        float startTime = Time.time;
        while (Time.time - startTime < time)
        {
            yield return new WaitForEndOfFrame();
            bgm.pitch = (time - (Time.time - startTime)) / time;
        }
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    #region Score

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int value)
    {
        score += value;
        player.hud.SetDebugInfo(6, string.Format("{0} (+{1})", score, value));
    }

	#endregion Score
}

public partial class GameManager
{
    // menu callbacks
    public void SelectLevel(LevelType levelType)
    {
        currLevel = levelType;
        SetupLevel();

        PlayerPrefs.SetInt("SavedLevel", (int)levelType);
        PlayerPrefs.Save();

        string info = "Level: " + levelType.ToString();
        if (levelType == LevelType.city && PlayerPrefs.HasKey("HighestScore"))
            info += "\nHighest score: " + PlayerPrefs.GetInt("HighestScore");
        holodeckUI.SetLevelInfo(info);
    }

    public void SelectControlType(PlayerInput.AxisControlType type)
    {
        playerInput.RebindAxis(type);

        PlayerPrefs.SetInt("SavedAxisControl", (int)type);
        PlayerPrefs.Save();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Threading.Tasks;
using static GameData;

public class GameManeger : MonoBehaviour
{
    public static GameManeger Instance;
    public List<SmallFood> foods {get; private set;}
    public List<Ghost> ghosts {get; private set;}
    public List<Transform> livePrefabs {get; private set;}
    public Pacman pacman;
    public TextMeshProUGUI scoreTXT;
    public TextMeshProUGUI highScoreTXT;
    public TextMeshProUGUI LabelTXT;

    public Transform insideHome;
    public Transform outsidHome;

   
    public Transform liveUI_Pos;
    public Transform livePrefab;

    float powerModeDuration = 0;
    public bool powerMode = false;

    public int GhostMultiplier {get; private set;}
    public int liveCount {get; private set;}
    public int score {get; private set;}
    public int high_Socre {get; private set;}
    public bool pause = true;
    public bool gameRunning = false;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
        
        foods = new List<SmallFood>();
        ghosts = new List<Ghost>();
        livePrefabs = new List<Transform>();
        high_Socre = 0;
        
    }
    void Start()
    {
        SoundManeger.Instance.PlaySound(SoundManeger.PlayList.Pacman_StartSound);
        if(LoadGame())
        {
            ContinueGame();
            return;
        }
        LabelTXT.SetText("READY");
    }

    public void Exit()
    {
        Application.Quit();
    }

    void OnApplicationQuit() {
        SaveGame();
    }

    public void PauseGame()
    {
        pause = !pause;
        if(pause) LabelTXT.SetText("PAUSE");
        else LabelTXT.SetText("");
    }

    public bool LoadGame()
    {
        GameData gameData = SaveSystem.Instance.LoadGameData();
        List<GhostData> ghostDatas = SaveSystem.Instance.LoadGhostList(ghosts.Count);
        if(gameData.hadData)
        {
            this.liveCount = gameData.live;
            this.score = gameData.score;
            this.high_Socre = gameData.high_Socre;
            for (int i = 0; i < foods.Count; i++)
            {
                if(gameData.foods[i] == 0) foods[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < ghosts.Count; i++)
            {
                ghosts[i].transform.position = ghostDatas[i].ghostsPos;
                ghosts[i].movement.ChangeDir(ghostDatas[i].CurrentDirection);
                ghosts[i].behaviour = (Ghost.Behaviour)ghostDatas[i].ghostsBehaviour;
            }
            this.pacman.transform.position = gameData.pacman_Pos;
            return true;
        }
        return false;
    }

    public void SaveGame()
    {
        GameData gameData = new GameData(); 
        gameData.score = score;
        gameData.high_Socre = high_Socre;
        gameData.live = liveCount;
        gameData.pacman_Pos = this.pacman.transform.position;
        List<int> foodlists = new List<int>();
        foreach (var food in foods)
        {
            if(food.gameObject.activeSelf == false) foodlists.Add(0);
            else foodlists.Add(1);
        }
        gameData.foods = foodlists;
        SaveSystem.Instance.SaveGame(gameData);

        List<GhostData> ghostDatas = new List<GhostData>();
        for (int i = 0; i < ghosts.Count; i++)
        {
            GhostData data = new GhostData();
            data.ghostsPos = ghosts[i].transform.position;
            data.CurrentDirection = ghosts[i].movement.CurrentDirection;
            data.ghostsBehaviour = (int)ghosts[i].behaviour;
            ghostDatas.Add(data);
        }
        SaveSystem.Instance.SaveList(ghostDatas);
    }

    async public void GameStart()
    {
        LabelTXT.SetText("START");
        await Task.Delay(200);    
        NewGame();
        LabelTXT.SetText("");
        pause = false;
        gameRunning = true;
    }

    void Update() {

        if(pause) return;
        if(powerMode)
        {
            powerModeDuration -= Time.deltaTime; 
            if(powerModeDuration <= 0)
            {
                PacmanPowerDown();
                return;
            }
            if(powerModeDuration <= 3)
            {
                PacmanPowerLastSec();
            }
        }
    }

    void NewGame()
    {
        SoundManeger.Instance.PlaySound(SoundManeger.PlayList.Pacman_StartSound);
        highScoreTXT.SetText(high_Socre.ToString());
        score = 0;
        scoreTXT.SetText("0");
        liveCount = 3;
        gameRunning = false;  
        powerMode = false;
        powerModeDuration = 0;
        foreach (var food in foods)
        {
            food.gameObject.SetActive(true);
        }
        foreach (var ghost in ghosts)
        {
            ghost.Restart();
        }
        pacman.Restart();
        foreach (var ghost in ghosts)
        {
            ghost.gameObject.SetActive(true);
        }
        setupLive();
    }

    void ContinueGame()
    {
        powerMode = false;
        powerModeDuration = 0;
        LabelTXT.SetText("PAUSE");
        scoreTXT.SetText(score.ToString());
        highScoreTXT.SetText(high_Socre.ToString());
        gameRunning = true;
        pause = true;
        setupLive();
        foreach (var ghost in ghosts)
        {
            ghost.gameObject.SetActive(true);
        }
    }

    void NewRound()
    {
        SoundManeger.Instance.PlaySound(SoundManeger.PlayList.Pacman_StartSound);
        powerMode = false;
        powerModeDuration = 0;
        foreach (var food in foods)
        {
            food.gameObject.SetActive(true);
        }
        foreach (var ghost in ghosts)
        {
            ghost.Restart();
        }
        pacman.Restart();
        foreach (var ghost in ghosts)
        {
            ghost.gameObject.SetActive(true);
        }
        setupLive();
        LabelTXT.SetText("");
        gameRunning = true;
    }

    void Restart()
    {
        SoundManeger.Instance.PlaySound(SoundManeger.PlayList.Pacman_StartSound);
        powerMode = false;
        powerModeDuration = 0;
        foreach (var ghost in ghosts)
        {
            ghost.Restart();
        }
        pacman.Restart();
        foreach (var ghost in ghosts)
        {
            ghost.gameObject.SetActive(true);
        }
    }

    async void GameOver()
    {
        pause = true;
        foreach (var food in foods)
        {
            food.gameObject.SetActive(false);
        }
        foreach (var ghost in ghosts)
        {
            ghost.gameObject.SetActive(false);
        }
        pacman.gameObject.SetActive(false);
        if(score > high_Socre) high_Socre = score;
        LabelTXT.SetText("GAMEOVER");
        await Task.Delay(1000);
        LabelTXT.SetText("CONTINUE?");
        gameRunning = false;
    }
    void setupLive()
    {
        if(livePrefabs.Count > 0)
        {
            for (int i = 0; i < livePrefabs.Count; i++)
            {
                Transform prefab = livePrefabs[i];
                prefab.gameObject.SetActive(false);
                livePrefabs.Remove(prefab);  
                Destroy(prefab.gameObject);
            }
        }

        for (int i = 0; i < liveCount; i++)
        {
            Transform prefab = Instantiate(livePrefab,new Vector3(liveUI_Pos.position.x + 2 * i,liveUI_Pos.position.y,0),Quaternion.identity);
            livePrefabs.Add(prefab);
        }
    }

    void SetScore(int num)
    {
        score += num;
        scoreTXT.SetText(score.ToString());
    }

    void RemoveALive()
    {
        liveCount--;
        Transform prefab = livePrefabs[liveCount];
        prefab.gameObject.SetActive(false);
        livePrefabs.Remove(prefab);  

        Destroy(prefab.gameObject);
    }

    public void PacmanEaten()
    {
        SoundManeger.Instance.StopMusic();
        SoundManeger.Instance.PlaySound(SoundManeger.PlayList.Pacman_Death);
        RemoveALive();
        foreach (var ghost in ghosts)
        {
            ghost.movement.movemtEnabled = false;
        }

        if(liveCount > 0)
        {   
            Invoke(nameof(Restart),2f);
        }
        else
        {
            Invoke(nameof(GameOver),1f);
        }
    }
    public void PacmanPowerMode(float duration)
    {
        SoundManeger.Instance.PlayMusic(SoundManeger.PlayList.Pacman_Power);
        if(powerMode)
        {
            powerModeDuration += duration;
            foreach (var ghost in ghosts)
            {
                if(ghost.behaviour == Ghost.Behaviour.GhostHome)
                {
                    ghost.isSpooked = true;
                }
                else
                {
                    ghost.Spooked();
                }
            }
        }
        else
        {
            powerModeDuration = duration;   
            powerMode = true;
            foreach (var ghost in ghosts)
            {
                if(ghost.behaviour == Ghost.Behaviour.GhostHome)
                {
                    ghost.isSpooked = true;
                }
                else
                {
                    ghost.Spooked();
                }
            }
        }
    }

    public void PacmanPowerLastSec()
    {
        foreach (var ghost in ghosts)
        {          
            if(ghost.behaviour == Ghost.Behaviour.GhostSpooked) 
            {
                ghost.blinky = true;
            }
        }
    }

    public void PacmanPowerDown()
    {
        SoundManeger.Instance.StopMusic();
        GhostMultiplier = 0;
        powerMode = false;
        foreach (var ghost in ghosts)
        {
            ghost.UnSpooked();
            if(ghost.behaviour == Ghost.Behaviour.GhostHome) continue;
            ghost.behaviour = Ghost.Behaviour.GhostScatter;  
            ghost.ghostBehaviour.currentBehaviour = 0; 
        }    
    }

    public void GhostEaten(Ghost ghost)
    {
        SoundManeger.Instance.PlaySound(SoundManeger.PlayList.Pacman_EatGhost);
        GhostMultiplier++;
        SetScore(ghost.point * GhostMultiplier);
    }

    public void SmallFoodEaten(SmallFood smallFood)
    {
        SoundManeger.Instance.PlaySound(SoundManeger.PlayList.Pacman_Chomp);
        smallFood.transform.gameObject.SetActive(false);
        SetScore(smallFood.point);
        if(!HasRemainingFood())
        {
            SoundManeger.Instance.StopMusic();
            Invoke(nameof(NewRound),2f);
        }
    }

    public void LargeFoodEaten(LargeFood largeFood)
    {  
        PacmanPowerMode(largeFood.duration);
        SmallFoodEaten(largeFood);
    }

    public bool HasRemainingFood()
    {
        foreach (var food in foods)
        {
            if(food.gameObject.activeSelf) return true;
        }
        return false;
    }
}

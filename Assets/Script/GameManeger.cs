using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Threading.Tasks;

public class GameManeger : MonoBehaviour
{
    public static GameManeger Instance;
    public List<SmallFood> foods {get; private set;}
    public List<Ghost> ghosts {get; private set;}
    public List<Ghost> atHomeGhosts {get; private set;}
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
        atHomeGhosts = new List<Ghost>();
        livePrefabs = new List<Transform>();
        high_Socre = 0;
        
    }
    void Start()
    {
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
        GameData gameData = SaveSystem.Instance.LoadGame();
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
                ghosts[i].transform.position = gameData.ghostDatas[i].ghosts_Pos;
                ghosts[i].movement.ChangeDir(gameData.ghostDatas[i].CurrentDirection);
                switch (gameData.ghostDatas[i].ghosts_Behaviour)
                {
                    case 0:
                        ghosts[i].behaviour = Ghost.Behaviour.GhostScatter;
                        break;
                    case  1:
                        ghosts[i].behaviour = Ghost.Behaviour.GhostChase;
                        break;
                    case  2:
                        ghosts[i].behaviour = Ghost.Behaviour.GhostSpooked;
                        break;
                    case  3:
                        ghosts[i].behaviour = Ghost.Behaviour.GhostHome;
                        break;
                }
            }
            this.pacman.transform.position = gameData.pacman_Pos;
            return true;
        }
        return false;
    }

    public void SaveGame()
    {
        GameData gameData = new GameData();
        Vector3 pacman_Pos = this.pacman.transform.position;
        List<GhostData> ghostDatas = new List<GhostData>();
        List<int> newfoods = new List<int>();
        foreach (var food in foods)
        {
            if(food.gameObject.activeSelf == false) newfoods.Add(0);
            else newfoods.Add(1);
        }
        for (int i = 0; i < ghosts.Count; i++)
        {
            GhostData data = new GhostData();
            data.ghosts_Pos = ghosts[i].transform.position;
            data.CurrentDirection = ghosts[i].movement.CurrentDirection;
            switch (ghosts[i].behaviour)
            {
                case Ghost.Behaviour.GhostScatter:
                    data.ghosts_Behaviour = 0;
                    break;
                case  Ghost.Behaviour.GhostChase:
                    data.ghosts_Behaviour= 1;
                    break;
                case Ghost.Behaviour.GhostSpooked:
                    data.ghosts_Behaviour = 2;
                    break;
                case Ghost.Behaviour.GhostHome:
                    data.ghosts_Behaviour = 3;
                    break;
            }
            ghostDatas.Add(data);
        }
        gameData.PutData(score,high_Socre,liveCount,newfoods,ghostDatas,pacman_Pos);
        SaveSystem.Instance.SaveGame(gameData);
    }

    async public void GameStart()
    {
        gameRunning = true;
        LabelTXT.SetText("START");
        await Task.Delay(1000);
        LabelTXT.SetText("");
        score = 0;
        scoreTXT.SetText("0");
        highScoreTXT.SetText(high_Socre.ToString());
        setupLive();
        pause = false;
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
        NewRound();
        liveCount = 3;
        gameRunning = false;
        LabelTXT.SetText("CONTINUE?");
    }

    void ContinueGame()
    {
        powerMode = false;
        powerModeDuration = 0;
        LabelTXT.SetText("PAUSE");
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
        foreach (var food in foods)
        {
            food.gameObject.SetActive(true);
        }
        Restart();
    }

    void Restart()
    {
        powerMode = false;
        powerModeDuration = 0;
        foreach (var ghost in ghosts)
        {
            ghost.Restart();
        }
        pacman.Restart();
        atHomeGhosts.Clear();
        foreach (var ghost in ghosts)
        {
            ghost.gameObject.SetActive(true);
        }
    }

    void GameOver()
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
        Invoke(nameof(NewGame),2f);
    }
    void setupLive()
    {
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
        livePrefabs.Remove(prefab);  

        Destroy(prefab.gameObject);
    }

    public void PacmanEaten()
    {
        
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
        if(powerMode)
        {
            powerModeDuration += duration;
            foreach (var ghost in ghosts)
            {
                ghost.Spooked();
            }
        }
        else
        {
            powerModeDuration = duration;   
            powerMode = true;
            foreach (var ghost in ghosts)
            {
                if(!atHomeGhosts.Contains(ghost) && ghost.behaviour == Ghost.Behaviour.GhostHome) atHomeGhosts.Add(ghost);
                ghost.behaviour = Ghost.Behaviour.GhostSpooked;
                ghost.Spooked();
            }
        }
    }

    public void PacmanPowerLastSec()
    {
        foreach (var ghost in ghosts)
        {           
            ghost.blinky = true;
        }
    }

    public void PacmanPowerDown()
    {
        GhostMultiplier = 0;
        powerMode = false;
        foreach (var ghost in ghosts)
        {
            ghost.UnSpooked();
            if(atHomeGhosts.Contains(ghost))
            {
                ghost.behaviour = Ghost.Behaviour.GhostHome;
                continue;
            } 
            ghost.behaviour = Ghost.Behaviour.GhostScatter;
        }    
        atHomeGhosts.Clear();
    }

    public void GhostEaten(Ghost ghost)
    {
        GhostMultiplier++;
        atHomeGhosts.Add(ghost);
        SetScore(ghost.point * GhostMultiplier);
    }

    public void SmallFoodEaten(SmallFood smallFood)
    {
        smallFood.transform.gameObject.SetActive(false);
        SetScore(smallFood.point);
        if(!HasRemainingFood())
        {
            Invoke(nameof(NewRound),3f);
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

using UnityEngine;
using UnityEngine.UI;
using static NewRandSys.RandomizeInt;
using static NewRandSys.RandomizeIntWhithException;

public class MapGen : MonoBehaviour
{
    GameObject Map;
    private static int mapSize = 15;
    private static int itemsCount = 7;
    //private static int itemNomber = 0;

    private GameObject[,] cells = new GameObject[mapSize, mapSize]; // Клетки игрового поля
    private GameObject[,] mapCells = new GameObject[mapSize, mapSize];

    private bool[,] lab = new bool[mapSize, mapSize]; // Игровое поле

    private int[,] itemMap = new int[mapSize, mapSize]; // предметы на игровом поле 

    private int[] lastPosX = new int[itemsCount];
    private int[] lastPosY = new int[itemsCount];

    // Игровые компоненты
    public GameObject cell;
    public GameObject gameCell;
    public GameObject gameMap;
    public GameObject player;
    public GameObject door;
    public GameObject key;
    public GameObject pit;
    public GameObject vert;
    public GameObject rope;
    public GameObject bomb;

    // Для отключения фона при дебаге
    public GameObject bg;
    public GameObject visiblePl;
    public GameObject debugButton;

    // В канвасе
    public GameObject panel;
    public GameObject keySprite;
    public GameObject bombSprite;
    public GameObject ropeSprite;
    public GameObject music;

    bool haveKey = false;
    bool takeBomb = false;
    bool haveBomb = false;
    bool haveRope = false;
    bool takeRope = false;
    bool itemeState = false;

    public bool debug = false; // Для вклюсения/отключения рендера спрайтов
    public bool devMode = false;


    Sprite[] sWalls;

    public Text moveText;

    // Начало
    private void Start()
    {
        for (int i = 0; i < itemsCount; i++)
        {
            lastPosX[i] = -1;
            lastPosY[i] = -1;
        }

        InitComponents();
        CreateTable();
        StartGeneration();
        SetTexture();

        debugButton.SetActive(devMode);
    }

    // Инициализация игрока и родительского объекта карты
    void InitComponents()
    {
        Map = transform.gameObject;
        player = Instantiate(player);

        door.GetComponent<SpriteRenderer>().enabled = false;
        key.GetComponent<SpriteRenderer>().enabled = false;

    }

    // Запуск генерации карты
    public void StartGeneration()
    {
        WallGenerator();
        InitPlayer();
        GenerateItems();
    }

    // Создаёься игровое поле и задаются имена клеток для игровой карты и поля
    void CreateTable()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                cells[j, i] = Instantiate(cell, new Vector2(j, i), Quaternion.identity);
                cells[j, i].transform.SetParent(Map.transform);
                cells[j, i].name = (j + " " + i);
                cells[j, i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                mapCells[i, j] = Instantiate(gameCell, new Vector2(i + gameMap.transform.position.x, j), Quaternion.identity);
                mapCells[i, j].transform.SetParent(gameMap.transform);
                mapCells[i, j].name = (i + " " + j);
            }
        }
    }

    private void Update()
    {
        //if (debug)
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (lastPosX[i] > -1 && lastPosY[i] > -1)
        //        {
        //            if (lastPosX[0] == lastPosX[1] && lastPosY[0] == lastPosY[1]) print("Error");
        //            if (lastPosX[1] == lastPosX[2] && lastPosY[1] == lastPosY[2]) print("Error");
        //            if (lastPosX[2] == lastPosX[0] && lastPosY[2] == lastPosY[0]) print("Error");
        //        }
        //        else print(i + " = " + lastPosX[i] + " || " + lastPosY[i]);
        //    }
        //}


        if (Input.GetKeyDown(KeyCode.E))
        {
            itemeState = !itemeState;
            //bombSprite.SetActive(haveBomb);
        }


        if (player != null) PlayerMove();
        CheackWay();
    }

    // Генерация самого лаберинта
    void WallGenerator()
    {
        // Заполняем все клетки 
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                lab[i, j] = true;
            }
        }

        bool gen = true;

        // Сартовые координаты
        int x = RandomEven(0, mapSize);
        int y = RandomEven(0, mapSize);

        // Здесь начинаем копать
        while (gen == true)
        {
            lab[x, y] = false;
            int way = Random.Range(0, 4); // Выбор направления 

            //if (debug) print("x: " + x + " | " + "y: " + y + " | " + way);

            switch (way)
            {
                case 0: // Влево
                    if (x - 2 >= 0)
                    {
                        if (lab[x - 2, y])
                        {
                            lab[x - 1, y] = false;
                            lab[x - 2, y] = false;
                            x = x - 2;
                        }
                        else if (!lab[x - 2, y] && !lab[x - 1, y]) x = x - 2;
                    }

                    break;
                case 1: // Вверх
                    if (y + 2 < mapSize)
                    {
                        if (lab[x, y + 2])
                        {
                            lab[x, y + 1] = false;
                            lab[x, y + 2] = false;
                            y = y + 2;
                        }
                        else if (!lab[x, y + 2] && !lab[x, y + 1]) y = y + 2;
                    }

                    break;
                case 2: // Вправо
                    if (x + 2 < mapSize)
                    {
                        if (lab[x + 2, y])
                        {
                            lab[x + 1, y] = false;
                            lab[x + 2, y] = false;
                            x = x + 2;
                        }
                        else if (!lab[x + 2, y] && !lab[x + 1, y]) x = x + 2;
                    }

                    break;
                case 3: // Вниз
                    if (y - 2 >= 0)
                    {
                        if (lab[x, y - 2])
                        {
                            lab[x, y - 1] = false;
                            lab[x, y - 2] = false;
                            y = y - 2;
                        }
                        else if (!lab[x, y - 2] && !lab[x, y - 1]) y = y - 2;
                    }
                    break;


            }

            bool fin = true;

            // Есть ли ещё стены на чётных координатах 
            for (int i = 0; i < mapSize; i = i + 2)
            {
                for (int j = 0; j < mapSize; j = j + 2)
                {
                    if (lab[i, j] == true) fin = false;
                }
            }
            if (fin == true) gen = false;
        }
    }

    // Наложение предметов на игровое поле
    void GenerateItems()
    {
        // Door = 1;
        int x = RandomEvenWE(0, mapSize, lastPosX);
        int y = RandomEvenWE(0, mapSize, lastPosY);

        lastPosX[1] = x;
        lastPosY[1] = y;
        //print("Door " + lastPosX[1] + "||" + lastPosY[1]);

        itemMap[x, y] = 1;
        door = Instantiate(door, new Vector2(x, y), Quaternion.identity);
        door.GetComponent<SpriteRenderer>().enabled = false;

        // Key = 2;
        x = RandomEvenWE(0, mapSize, lastPosX);
        y = RandomEvenWE(0, mapSize, lastPosY);

        lastPosX[2] = x;
        lastPosY[2] = y;
        //print("Key " + lastPosX[2] + "||" + lastPosY[2]);


        itemMap[x, y] = 2;
        key = Instantiate(key, new Vector2(x, y), Quaternion.identity);
        key.GetComponent<SpriteRenderer>().enabled = false;

        // Pit = 3;
        x = RandomEvenWE(0, mapSize, lastPosX);
        y = RandomEvenWE(0, mapSize, lastPosY);

        lastPosX[3] = x;
        lastPosY[3] = y;

        itemMap[x, y] = 3;
        pit = Instantiate(pit, new Vector2(x, y), Quaternion.identity);
        pit.GetComponent<SpriteRenderer>().enabled = false;

        // Vert = 4;
        x = RandomEvenWE(0, mapSize, lastPosX);
        y = RandomEvenWE(0, mapSize, lastPosY);

        itemMap[x, y] = 4;
        vert = Instantiate(vert, new Vector2(x, y), Quaternion.identity);
        vert.GetComponent<SpriteRenderer>().enabled = false;

        lastPosX[4] = x;
        lastPosY[4] = y;

        // rope = 5;
        x = RandomEvenWE(0, mapSize, lastPosX);
        y = RandomEvenWE(0, mapSize, lastPosY);

        itemMap[x, y] = 5;
        rope = Instantiate(rope, new Vector2(x, y), Quaternion.identity);
        rope.GetComponent<SpriteRenderer>().enabled = false;

        lastPosX[5] = x;
        lastPosY[5] = y;

        // Bomb = 6;
        x = RandomEvenWE(0, mapSize, lastPosX);
        y = RandomEvenWE(0, mapSize, lastPosY);

        itemMap[x, y] = 6;
        bomb = Instantiate(bomb, new Vector2(x, y), Quaternion.identity);
        bomb.GetComponent<SpriteRenderer>().enabled = false;

        lastPosX[6] = x;
        lastPosY[6] = y;
    }

    // Инициализация игрока
    void InitPlayer()
    {
        int x = RandomEvenWE(0, mapSize, lastPosX);
        int y = RandomEvenWE(0, mapSize, lastPosY);

        lastPosX[0] = x;
        lastPosY[0] = y;

        mapCells[x, y].GetComponent<SpriteRenderer>().color = Color.black;
        mapCells[x, y].GetComponent<GameMap>().can = false;

        player.transform.position = new Vector2(x, y);
        player.GetComponent<SpriteRenderer>().enabled = false;

    }

    void PlayMusic()
    {
        music.SetActive(true);
    }
    // Проверка на столкновение с предметами
    void CheackWay()
    {
        int x = (int)player.transform.position.x;
        int y = (int)player.transform.position.y;

        if (itemMap[x, y] == 2)
        {
            itemMap[x, y] = 0;
            key.SetActive(false);
            haveKey = true;
            keySprite.gameObject.SetActive(true);
            moveText.gameObject.SetActive(true);
            moveText.color = Color.yellow;
            moveText.text = "Вы наткнулись на ключ";

        }
        else if (itemMap[x, y] == 1 && haveKey)
        {
            panel.SetActive(true);
            Invoke("PlayMusic", 1);

        }
        else if (itemMap[x, y] == 1)
        {
            moveText.gameObject.SetActive(true);
            moveText.color = Color.blue;
            moveText.text = "Вы наткнулись на дверь";
        }
        else if (itemMap[x, y] == 3)
        {
            itemMap[x, y] = 0;
            pit.SetActive(false);
            x = RandomEvenWE(0, mapSize, lastPosX);
            y = RandomEvenWE(0, mapSize, lastPosY);

            player.transform.position = new Vector2(x, y);
        }
        else if (itemMap[x, y] == 4)
        {
            //print("'Vert'");
            int rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    while (y < mapSize - 1 && lab[x, y + 1] == false)
                    {
                        player.transform.position += new Vector3(0, 1, 0);
                        y++;
                    }
                    break;
                case 1:
                    while (x < mapSize - 1 && lab[x + 1, y] == false)
                    {
                        player.transform.position += new Vector3(1, 0, 0);
                        x++;
                    }
                    break;
                case 2:
                    while (y > 0 && lab[x, y - 1] == false)
                    {
                        player.transform.position += new Vector3(0, -1, 0);
                        y--;
                    }
                    break;
                case 3:
                    while (x > 0 && lab[x - 1, y] == false)
                    {
                        player.transform.position += new Vector3(-1, 0, 0);
                        x--;
                    }
                    break;
            }
            itemMap[x, y] = 0;
            vert.gameObject.SetActive(false);
        }
        else if (itemMap[x, y] == 5)
        {
            itemMap[x, y] = 0;
            rope.SetActive(false);
            haveRope = true;
            ropeSprite.gameObject.SetActive(true);
            moveText.text = "Вы нашли лестницу";
            moveText.gameObject.SetActive(true);
            moveText.color = Color.cyan;
        }
        else if (itemMap[x, y] == 6)
        {
            itemMap[x, y] = 0;
            bomb.SetActive(false);
            haveBomb = true;
            bombSprite.gameObject.SetActive(true);
            moveText.gameObject.SetActive(true);
            moveText.color = Color.cyan;
            moveText.text = "Вы нашли бомбу";
        }
    }

    float lastPad;

    // Движение персонажа
    void PlayerMove()
    {
        int x = (int)player.transform.position.x;
        int y = (int)player.transform.position.y;

        lastPad += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // && lastPad > newPad
        {
            if (!takeBomb && !takeRope)
            {
                if (y < mapSize - 1 && lab[x, y + 1] == false)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.green;
                    moveText.text = "Вы переместились вверх";
                    player.transform.position = new Vector2(x, y + 1);
                }
                else if (y == (mapSize - 1))
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.cyan;
                    moveText.text = "Вы уткнулись в границу лаберинта";
                }
                else
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.red;
                    moveText.text = "Вы не переместились вверх";
                }
            }
            else if (takeBomb)
            {
                if (y == mapSize - 1)
                {
                    moveText.text = "Вы не смогли взорвать границу лабиринта";
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                }
                else
                {
                    lab[x, y + 1] = false;
                    if (debug) SetTexture();
                    moveText.text = "Вы взорвали стену сверху";
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                }
                haveBomb = false;
                takeBomb = false;
                bombSprite.SetActive(false);
            }
            else if (takeRope)
            {
                if (y == mapSize - 1)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через границу лабиринта";
                }
                else if (y < mapSize - 2 && (lab[x, y + 1] == true && lab[x, y + 2] == true))
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через стену";
                }
                else if (!lab[x, y + 1])
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Впереди нет стены";
                }
                else
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы перелезли через стену";
                    player.transform.position += new Vector3(0, 2, 0);
                }
                takeRope = false;
                haveRope = false;
                ropeSprite.SetActive(false);
            }
            //print(x + " || " + y + " || " + lab[x, y + 1] + " || " + player.transform.position.x + " || " + player.transform.position.y);
            lastPad = 0;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!takeBomb && !takeRope)
            {
                if (y > 0 && lab[x, y - 1] == false)
                {

                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.green;
                    moveText.text = "Вы переместились вниз";

                    player.transform.position = new Vector2(x, y - 1);
                }
                else if (y == 0)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.cyan;
                    moveText.text = "Вы уткнулись в границу лаберинта";
                }
                else
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.red;
                    moveText.text = "Вы не переместились вниз";
                }
            }
            else if (takeBomb)
            {
                if (y == 1)
                {
                    moveText.text = "Вы не смогли взорвать границу лабиринта";
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                }
                else
                {
                    lab[x, y - 1] = false;
                    if (debug) SetTexture();
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы взорвали стену снизу";
                }
                haveBomb = false;
                takeBomb = false;
                bombSprite.SetActive(false);
            }
            else if (takeRope)
            {
                if (y == 1)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через границу лабиринта";
                }
                else if (y < mapSize - 2 && (lab[x, y - 1] == true && lab[x, y - 2] == true))
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через стену";
                }
                else if (!lab[x, y - 1])
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Впереди нет стены";
                }
                else
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы перелезли через стену";
                    player.transform.position += new Vector3(0, -2, 0);
                }
                takeRope = false;
                haveRope = false;
                ropeSprite.SetActive(false);
            }

            lastPad = 0;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!takeBomb  && !takeRope)
            {
                if (x > 0 && lab[x - 1, y] == false)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.green;
                    moveText.text = "Вы переместились влево";

                    player.transform.position = new Vector2(x - 1, y);
                }
                else if (x == 0)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.cyan;
                    moveText.text = "Вы уткнулись в границу лаберинта";
                }
                else
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.red;
                    moveText.text = "Вы не переместились влево";
                }
            }
            else if (takeBomb)
            {
                if (x == 1)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли взорвать границу лабиринта";
                }
                else
                {
                    lab[x - 1, y] = false;
                    if (debug) SetTexture();
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы взорвали стену слева";
                }
                haveBomb = false;
                takeBomb = false;
                bombSprite.SetActive(false);
            }
            else if (takeRope)
            {
                if (x == 1)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через границу лабиринта";
                }
                else if (x < 2 && (lab[x - 1, y] == true && lab[x - 2, y] == true))
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через стену";
                }
                else if (!lab[x - 1, y])
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Впереди нет стены";
                }
                else
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы перелезли через стену";
                    player.transform.position += new Vector3(-2, 0, 0);
                }
                takeRope = false;
                haveRope = false;
                ropeSprite.SetActive(false);
            }
            lastPad = 0;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!takeBomb && !takeRope)
            {
                if (x < mapSize - 1 && lab[x + 1, y] == false)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.green;
                    moveText.text = "Вы переместились вправо";

                    player.transform.position = new Vector2(x + 1, y);
                }
                else if (x == mapSize - 1)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.cyan;
                    moveText.text = "Вы уткнулись в границу лаберинта";
                }
                else
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.red;
                    moveText.text = "Вы не переместились вправо";
                }
            }
            else if (takeBomb)
            {
                if (x == mapSize - 1)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли взорвать границу лабиринта";
                }
                else
                {
                    lab[x + 1, y] = false;
                    if (debug) SetTexture();
                    moveText.text = "Вы взорвали стену справа";
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                }
                haveBomb = false;
                takeBomb = false;
                bombSprite.SetActive(false);
            }
            else if (takeRope)
            {
                if (x == mapSize - 1)
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через границу лабиринта";
                }
                else if (x < mapSize - 2 && (lab[x + 1, y] == true && lab[x + 2, y] == true))
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы не смогли перелезть через стену";
                }
                else if (!lab[x + 1, y])
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Впереди нет стены";
                }
                else if (lab[x+1,y] && !lab[x+2,y])
                {
                    moveText.gameObject.SetActive(true);
                    moveText.color = Color.magenta;
                    moveText.text = "Вы перелезли через стену";
                    player.transform.position += new Vector3(2, 0, 0);
                }
                takeRope = false;
                haveRope = false;
                ropeSprite.SetActive(false);
            }

            lastPad = 0;
        }
        if (Input.GetKeyDown(KeyCode.Q) && haveBomb)
        {
            moveText.gameObject.SetActive(true);
            moveText.color = Color.magenta;
            if (!takeBomb)
            {
                moveText.gameObject.SetActive(true);
                moveText.color = Color.magenta;
                moveText.text = "Вы взяли бомбу";
            }
            else
            {
                moveText.gameObject.SetActive(true);
                moveText.color = Color.magenta;
                moveText.text = "Вы убрали бомбу";
            }
            takeRope = false;
            takeBomb = !takeBomb;
        }

        if (Input.GetKeyDown(KeyCode.E) && haveRope)
        {
            if (!takeRope)
            {
                moveText.gameObject.SetActive(true);
                moveText.color = Color.magenta;
                moveText.text = "Вы взяли лестницу";
            }
            else
            {
                moveText.gameObject.SetActive(true);
                moveText.color = Color.magenta;
                moveText.text = "Вы убрали лестницу";
            }
            takeBomb = false;
            takeRope = !takeRope;
        }
        if (lastPad >= 2)
        {
            moveText.gameObject.SetActive(false);

            lastPad = 0;
        }

    }

    // Наложение цветов на игровое поле для дебага
    private void SetTexture()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (!lab[j, i]) cells[j, i].GetComponent<SpriteRenderer>().color = Color.white;
                else cells[j, i].GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }
    public void ShowTexture()
    {
        if (debug == false)
        {
            debug = true;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    cells[j, i].GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            bg.GetComponent<SpriteRenderer>().enabled = false;
            visiblePl.GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponent<SpriteRenderer>().enabled = true;
            door.GetComponent<SpriteRenderer>().enabled = true;
            key.GetComponent<SpriteRenderer>().enabled = true;
            pit.GetComponent<SpriteRenderer>().enabled = true;
            vert.GetComponent<SpriteRenderer>().enabled = true;
            rope.GetComponent<SpriteRenderer>().enabled = true;
            bomb.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    cells[j, i].GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            debug = false;
            bg.GetComponent<SpriteRenderer>().enabled = true;
            visiblePl.GetComponent<SpriteRenderer>().enabled = true;
            player.GetComponent<SpriteRenderer>().enabled = false;
            door.GetComponent<SpriteRenderer>().enabled = false;
            key.GetComponent<SpriteRenderer>().enabled = false;
            pit.GetComponent<SpriteRenderer>().enabled = false;
            vert.GetComponent<SpriteRenderer>().enabled = false;
            rope.GetComponent<SpriteRenderer>().enabled = false;
            bomb.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}

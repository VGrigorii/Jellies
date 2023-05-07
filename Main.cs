using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class Main : MonoBehaviour
{
    public Canvas canvas; // Для получения размера экрана
    public GameObject emptyCell; // обьект пустой ячейки префаб
    public GameObject jellyGreen; // обьект желейки префаб
    public GameObject jellyBlue; // обьект желейки префаб
    public GameObject jellyRad; // обьект желейки префаб
    public GameObject jellyYellow; // обьект желейки префаб
    public GameObject jellyPink; // обьект желейки префаб
    public GameObject jellyLeftRight; // обьект желейки бонуса префаб
    public GameObject jellyBow; // обьект желейки бонуса префаб
    public GameObject jellyUpDown; // обьект желейки бонуса префаб
    public GameObject jellyBomb; // обьект желейки бонуса префаб
    public GameObject Edging;
    public GameObject Ice_1; // обьект ловушки префаб
    public GameObject Ice_2; // обьект ловушки префаб
    public GameObject Ice_3; // обьект ловушки префаб
    public GameObject Caramel_1; // обьект ловушки префаб
    public GameObject Caramel_2; // обьект ловушки префаб
    public GameObject Caramel_3; // обьект ловушки префаб

    public GameObject Points; // всплывающие очки при стирании
    //----------------------------------------------------------- Временные для теста -----------------------------------------
    private Coord coord1;
    private Coord coord2;
    //-------------------------------------------------------------------------------------------------------------------------
    private GameObject firstGameObjectСlue; // обьекты для подсказки
    private GameObject secondGameObjectСlue; // обьекты для подсказки
    private bool counterReset = false; // переменная для подсказки
    public float stepMove = 0.01f;  // разобраться почему не работает *?????????????????????????????????????????????????????????
    private bool GameRun; // Для остановки потоков при закрытии игры
    private List<Coord []> coordsDestroy = new List<Coord []>(); // Список координат для удаления из них желеек
    private List<Trap> Traps = new List<Trap>(); // Список ловушек
    private List<GameObject> TrapsObject = new List<GameObject>(); // Список обьектов ловушек 
    private List<GameObject> EdgingCell = new List<GameObject>(); // Список окантовок (для удаления при переходах между уровнями)
    private GameObject [] NewJelliesRow; // скрытый верхний ряд с желейками (без него тупит)

    private float sizeCell; // размер ячейки желе
    private int xSize; // количество желеек по x
    private int ySize; // количество желеек по y
    private float SizeCanvasMainX; // размер ячейки в конкретном уровне по х
    private float SizeCanvasMainY; // размер ячейки в конкретном уровне по у
    private int xCorect = 100; // сдвиг от краёв для canvas
    private GameObject [,] AllJelly; // сетка обьектов уровня
    private GameObject [,] copyAllJelly; // для проверки возможности перемещения
    public int numberOfColors = 5; // Количество цветов в раунде
    private bool direction; // направление стирания
    private Coord checkUnitsCoord; // переменная для создания бонусных желеек (бомбочек)
    private int [, ] LevelJelly; // Для создания первоночального расположения желеек

//---------------------------------------------------------------------- Для перемещения желейки -----------------------------
    private GameObject movedJelly;
    private Vector3 firstVector;
    private Vector3 secondVector;
    private Coord coordMovedJellyFirst;
    private Coord coordMovedJellySecond;
    private Coord coordMovedJellyThird;
    private Coord [] coordsMove;
    private GameObject changeableJelly;
//----------------------------------------------------------------------------------------------------------------------------
    private Random random = new Random(); // рандомная переменная
    
    private void LevelGenerationJelly () // для генерации первичной расстановки желеек
    {
        RectTransform rt = canvas.GetComponent<RectTransform>();
        Vector2 vrt = rt.sizeDelta;
        SizeCanvasMainX = vrt.x - xCorect;
        sizeCell = SizeCanvasMainX / xSize;
        SizeCanvasMainY = ySize * sizeCell;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                AllJelly[i, j] = MakingJellyIndex(LevelJelly[i,j]);
                //------------------------------------------------------------------------ дял проверки временно ----------------------
                if(AllJelly[i, j].GetComponent<Jelly>().IndexJelly == 6)
                {
                    AllJelly[i, j].GetComponent<Jelly>().bonus = true;
                }
                //-------------------------------------------------------------------------------------------------------------------
                AllJelly[i, j].GetComponent<Jelly>().coord = new Coord(i, j);
                AllJelly[i, j].GetComponent<Jelly>().FirstPositionJelly();
                AllJelly[i, j].transform.position = new Vector3(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                ,(((j + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100);                                 
                AllJelly[i, j].transform.localScale = new Vector3(sizeCell, sizeCell, sizeCell);
            }
        }
        // ------------------------------------ Создание окантовки ячеек ---------------------------------------
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(AllJelly[i, j].GetComponent<Jelly>().getIndexJelly() == 0)
                {
                    continue;
                }
                GameObject edging = Instantiate<GameObject>(Edging);
                edging.transform.position = new Vector3(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                ,(((j + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 2);
                edging.transform.localScale = new Vector3(sizeCell, sizeCell, sizeCell);
                EdgingCell.Add(edging);

            }
        }
        //------------------------------------------------------------------------------------------------------ 
        timeHealp();
    }
    private void setTrap(int typeTrap, int lvlTrap, Coord coordTrap) // Создание и добавление ловушки в список
    {
        GameObject trapObject = null;
        switch (typeTrap)
        {
            case 0:
            switch (lvlTrap)
            {
                case 1:
                trapObject = Instantiate<GameObject>(Ice_1);
                break;
                case 2:
                trapObject = Instantiate<GameObject>(Ice_2);
                break;
                case 3:
                trapObject = Instantiate<GameObject>(Ice_3);
                break;
                default:
                break;
            }
            break;
            case 1:
            switch (lvlTrap)
            {
                case 1:
                trapObject = Instantiate<GameObject>(Caramel_1);
                break;
                case 2:
                trapObject = Instantiate<GameObject>(Caramel_2);
                break;
                case 3:
                trapObject = Instantiate<GameObject>(Caramel_3);
                break;
                default:
                break;
            }
            
            break;
            default:
            break;
        }
        if(trapObject != null)
        {
            TrapsObject.Add(trapObject);
            trapObject.GetComponent<Trap>().coord = new Coord (coordTrap.x, coordTrap.y);
            trapObject.GetComponent<Trap>().setLvlTrap(lvlTrap);
            trapObject.GetComponent<Trap>().setTypeTrap(typeTrap);
            trapObject.transform.position = new Vector3(((coordTrap.x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
            ,(((coordTrap.y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, -2);
            trapObject.transform.localScale = new Vector3(sizeCell, sizeCell, sizeCell);
        }
    }
    private GameObject returnTraps(Coord coordTrap) // возвращает обьект ловушку
    {
        if(TrapsObject.Count == 0)
        {
            return null;
        }
        foreach (GameObject item in TrapsObject)
        {
            if(item.GetComponent<Trap>().coord.Equals(coordTrap))
            {
                return item;
            }
        } 
        return null;
    }
    private void destroyTrap(Coord coordTrap) // уничтожает ловушку и при необходимости создаёт на её месте другую низшего уровня
    {
        int lvlTrap = 0;
        int typeTrap = 0;
        foreach (GameObject item in TrapsObject)
        {
            if(item.GetComponent<Trap>().coord.Equals(coordTrap))
            {
                lvlTrap = item.GetComponent<Trap>().getLvlTrap();
                typeTrap = item.GetComponent<Trap>().getTypeTrap();
                if(lvlTrap == 1)               
                {
                    TrapsObject.Remove(item);
                    Destroy(item);
                    return;                   
                }
                else 
                {
                    lvlTrap--;
                    TrapsObject.Remove(item);
                    Destroy(item); 
                    setTrap(typeTrap, lvlTrap, coordTrap);
                    return;
                }
            }
        }
    }
    private bool moveSearch() // Метод проверяющий возможность стирания
    {
        copyAllJelly = AllJelly.Clone() as GameObject[,];
        GameObject temporalObject = null;
        GameObject returnTrap = null;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                temporalObject = null;
                returnTrap = null;
                temporalObject = copyAllJelly[i,j];
                if(copyAllJelly[i,j] != null)
                {
                    returnTrap = returnTraps(copyAllJelly[i,j].GetComponent<Jelly>().coord);
                }
                if(temporalObject == null || returnTrap != null || temporalObject.GetComponent<Jelly>().bonus || temporalObject.GetComponent<Jelly>().getIndexJelly() == 0)
                {
                    continue;
                }
                if(j < ySize - 1)
                {
                    if(copyAllJelly[i,j + 1] != null && !copyAllJelly[i,j + 1].GetComponent<Jelly>().bonus && 
                        returnTraps(copyAllJelly[i,j + 1].GetComponent<Jelly>().coord) == null &&
                        copyAllJelly[i,j + 1].GetComponent<Jelly>().getIndexJelly() != 0)
                    {                    
                        copyAllJelly[i,j] = copyAllJelly[i,j + 1];
                        copyAllJelly[i,j + 1] = temporalObject;
                        if(checkDestroy(copyAllJelly))
                        {
                            coord1 = copyAllJelly[i,j].GetComponent<Jelly>().coord;
                            coord2 = copyAllJelly[i,j + 1].GetComponent<Jelly>().coord;
                            firstGameObjectСlue = AllJelly[i,j];
                            secondGameObjectСlue = AllJelly[i,j + 1];
                            copyAllJelly = null;                           
                            coordsDestroy.Clear();
                            return true;
                        }
                        else
                        {
                            copyAllJelly[i,j + 1] = copyAllJelly[i,j];
                            copyAllJelly[i,j] = temporalObject;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                if(i < xSize - 1)
                {
                    if(copyAllJelly[i + 1, j] != null && !copyAllJelly[i + 1,j].GetComponent<Jelly>().bonus && 
                        returnTraps(copyAllJelly[i + 1,j].GetComponent<Jelly>().coord) == null &&
                        copyAllJelly[i + 1, j].GetComponent<Jelly>().getIndexJelly() != 0)
                    {
                        copyAllJelly[i,j] = copyAllJelly[i + 1,j];
                        copyAllJelly[i + 1,j] = temporalObject;
                        if(checkDestroy(copyAllJelly))
                        {
                            coord1 = copyAllJelly[i,j].GetComponent<Jelly>().coord;
                            coord2 = copyAllJelly[i + 1,j].GetComponent<Jelly>().coord;
                            firstGameObjectСlue = AllJelly[i,j];
                            secondGameObjectСlue = AllJelly[i + 1,j];
                            copyAllJelly = null;
                            coordsDestroy.Clear();
                            return true;
                        }
                        else
                        {
                            copyAllJelly[i + 1,j] = copyAllJelly[i,j];
                            copyAllJelly[i,j] = temporalObject;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(copyAllJelly[i,j] != null)
                {                    
                    if(copyAllJelly[i,j].GetComponent<Jelly>().bonus)
                    {
                        copyAllJelly = null;
                        coordsDestroy.Clear();
                        return true;
                    }
                }
            }
        }
        copyAllJelly = null;
        coordsDestroy.Clear();
        return false;
    }
    private void mixing() // Метод перемешивающий желейки (Если нет хода для стирания)
    {
        Coord [,] mixingCoords = new Coord[xSize, ySize];
        GameObject [,] mixingObjects = new GameObject[xSize, ySize]; 
        GameObject mixing = null;
        GameObject randomObj = null;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(AllJelly[i,j] != null && returnTraps(AllJelly[i,j].GetComponent<Jelly>().coord) == null &&
                    AllJelly[i,j].GetComponent<Jelly>().getIndexJelly() != 0)
                {
                    int xRand = random.Next(xSize);
                    int yRand = random.Next(ySize);
                    mixing = AllJelly[i,j]; 
                    randomObj = AllJelly[xRand, yRand];
                    if(AllJelly[xRand, yRand] != null && returnTraps(AllJelly[xRand, yRand].GetComponent<Jelly>().coord) == null &&
                        AllJelly[xRand, yRand].GetComponent<Jelly>().getIndexJelly() != 0)
                    {
                        mixingObjects[i, j] = mixing;
                        AllJelly[i,j] = null;
                        mixingObjects[xRand, yRand] = randomObj;
                        AllJelly[xRand, yRand] = null;
                        mixingCoords[i,j] = new Coord (randomObj.GetComponent<Jelly>().coord.x, randomObj.GetComponent<Jelly>().coord.y);
                        mixingCoords[xRand, yRand] = new Coord (mixing.GetComponent<Jelly>().coord.x, mixing.GetComponent<Jelly>().coord.y);
                    }
                }
            }
        }
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(mixingCoords[i,j] != null)
                {
                    mixingObjects[i, j].GetComponent<Jelly>().transform.position =  
                    new Vector3(((mixingCoords[i,j].x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100 
                    ,(((mixingCoords[i,j].y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 0);
                    mixingObjects[i, j].GetComponent<Jelly>().coord.x = mixingCoords[i,j].x;
                    mixingObjects[i, j].GetComponent<Jelly>().coord.y = mixingCoords[i,j].y;
                    AllJelly[mixingCoords[i,j].x, mixingCoords[i,j].y] = mixingObjects[i, j];                    
                }
            }
        }
        mixingCoords = null;
        mixingObjects = null;
    }
    public void WindowGame(int _xSize, int _ySize) // Создание Ячеек под уровень
    {
        xSize = _xSize;
        ySize = _ySize;
        AllJelly = new GameObject [xSize,ySize];
        NewJelliesRow = new GameObject [xSize];
        
        LevelJelly =  LevelGeneration.levelGeneration(xSize, ySize, numberOfColors);
        LevelGenerationJelly();
        Filling();
        RectTransform rt = canvas.GetComponent<RectTransform>();
        Vector2 vrt = rt.sizeDelta;
        SizeCanvasMainX = vrt.x - xCorect;
        sizeCell = SizeCanvasMainX / xSize;
        SizeCanvasMainY = ySize * sizeCell;  
        //---------------------------------------- Проверочные ловушки --------------------------------------------
        setTrap(1, 3, new Coord(1, 4));
        setTrap(1, 3, new Coord(5, 4));
        setTrap(0, 3, new Coord(1,2));
        setTrap(0, 3, new Coord(5,2));
        //------------------------------------------------------------------------------------------------------------
    }
    public void Filling() // заполнение верхнего ряда (над основной сеткой) так как без него глючит не успевает создавать обьекты
    { 
        for (int i = 0; i < xSize; i++)
        {
            if(NewJelliesRow[i] == null)
            {
                GameObject jelly = randomMakingColor(numberOfColors);
                //jelly.GetComponent<Jelly>().MoveProgress = false;
                //jelly.GetComponent<Jelly>().FirstPositionJelly();
                jelly.GetComponent<Jelly>().coord = new Coord(i, ySize);
                jelly.transform.localScale = new Vector3(sizeCell, sizeCell, sizeCell);
                jelly.transform.position = new Vector2(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100,
                    (((ySize + 1)*sizeCell) + (sizeCell/2))/50 + sizeCell/50 - SizeCanvasMainY/100);
                
                //jelly.GetComponent<Jelly>().setMoved(jelly.transform.position, new Vector2(((x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100,
                //    (((y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100), stepMove, x, y);    
                NewJelliesRow[i] = jelly;
            }            
        }
    }
    public bool BlockAction() // Проверка перемещения желеек ( true - движения нету)
    {
        for(int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(AllJelly[i, j] != null)
                {
                    if(!AllJelly[i, j].GetComponent<Jelly>().MoveProgress)
                    {
                        return false;
                    }
                }
            }   
        }        
        return true;
    }
    public bool MoveJelly() // перемещение желеек в процессе стирания (если true - движение было)
    {
        bool returnChek = false;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 1; j < ySize; j++)
            {
                if((j == ySize - 1) && AllJelly[i, j] == null)
                {
                    NewJelliesRow[i].GetComponent<Jelly>().setMoved(NewJelliesRow[i].transform.position, new Vector2(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100,
                    (((j + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100), stepMove, i, j);
                    AllJelly[i, j] = NewJelliesRow[i];
                    NewJelliesRow[i] = null;
                    returnChek = true;
                    continue;
                }
                if(AllJelly[i, j] != null && (returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord) == null
                || (returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord) != null && 
                returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)) &&
                AllJelly[i, j].GetComponent<Jelly>().getIndexJelly() != 0)
                {
                    if(AllJelly[i, j - 1] == null)
                    {
                        GameObject jelly = AllJelly[i, j];                     
                        Vector2 firstPosition = new Vector2(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100,
                                                (((j + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100);
                        Vector2 secondPosition = new Vector2(firstPosition.x, firstPosition.y - sizeCell/50);
                        jelly.GetComponent<Jelly>().setMoved(firstPosition, secondPosition, stepMove, i, j - 1);
                        AllJelly[i, j] = null;
                        AllJelly[i, j - 1] = jelly;
                        returnChek = true;
                    }                        
                }
                else
                {
                    if(AllJelly[i, j] == null)
                    {
                        for (int k = ySize - 1; k >= j; k--)
                        {
                            if(AllJelly[i, k] != null && j != k)
                            {
                                if(AllJelly[i, k].GetComponent<Jelly>().getIndexJelly() == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if(AllJelly[i, k] == null && j == k)
                                {
                                    NewJelliesRow[i].GetComponent<Jelly>().setMoved(NewJelliesRow[i].transform.position, new Vector2(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100,
                                    (((j + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100), stepMove, i, j);
                                    AllJelly[i, j] = NewJelliesRow[i];
                                    NewJelliesRow[i] = null;
                                    returnChek = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        } 
        Filling();
        return returnChek;
    }
    public bool MoveJellyLeftRight() // для сдвига желеек под ловушками и т.п.
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 1; j < ySize; j++)
            {
                if(AllJelly[i, j] != null && (returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord) == null
                || (returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord) != null && 
                returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)) &&
                AllJelly[i, j].GetComponent<Jelly>().getIndexJelly() != 0)
                {
                    if(i > 0)
                    {
                        if(AllJelly[i - 1, j - 1] == null)
                        {
                            GameObject jelly = AllJelly[i, j];                     
                            Vector2 firstPosition = jelly.transform.position;
                            Vector2 secondPosition = new Vector2(firstPosition.x - sizeCell/50, firstPosition.y - sizeCell/50);
                            jelly.GetComponent<Jelly>().setMoved(firstPosition, secondPosition, stepMove, i - 1, j - 1);
                            AllJelly[i, j] = null;
                            AllJelly[i - 1, j - 1] = jelly;
                            return true;
                        }
                        else
                        {
                            if(i < xSize - 1)
                            {
                                if(AllJelly[i + 1, j - 1] == null)
                                {
                                    GameObject jelly = AllJelly[i, j];                     
                                    Vector2 firstPosition = jelly.transform.position;
                                    Vector2 secondPosition = new Vector2(firstPosition.x + sizeCell/50, firstPosition.y - sizeCell/50);
                                    jelly.GetComponent<Jelly>().setMoved(firstPosition, secondPosition, stepMove, i + 1, j - 1);
                                    AllJelly[i, j] = null;
                                    AllJelly[i + 1, j - 1] = jelly;
                                    return true;
                                }
                            }
                        }                                
                    }
                    else
                    {
                        if(i < xSize - 1)
                        {
                            if(AllJelly[i + 1, j - 1] == null)
                            {
                                GameObject jelly = AllJelly[i, j];                     
                                Vector2 firstPosition = jelly.transform.position;
                                Vector2 secondPosition = new Vector2(firstPosition.x + sizeCell/50, firstPosition.y - sizeCell/50);
                                jelly.GetComponent<Jelly>().setMoved(firstPosition, secondPosition, stepMove, i + 1, j - 1);
                                AllJelly[i, j] = null;
                                AllJelly[i + 1, j - 1] = jelly;
                                return true;
                            }
                        }
                    }                                
                }
            }
        }
        Filling();
        return false;
    }
    public GameObject MakingJellyIndex(int num) // Создание желеек GameObject по индексу
    {
        GameObject jelly;
        switch (num)
        {
            case 1: return jelly = Instantiate<GameObject>(jellyGreen);
            case 2: return jelly = Instantiate<GameObject>(jellyBlue);
            case 3: return jelly = Instantiate<GameObject>(jellyRad);
            case 4: return jelly = Instantiate<GameObject>(jellyYellow);
            case 5: return jelly = Instantiate<GameObject>(jellyPink);
            case 6: return jelly = Instantiate<GameObject>(jellyLeftRight);
            case 7: return jelly = Instantiate<GameObject>(jellyUpDown);
            case 8: return jelly = Instantiate<GameObject>(jellyBomb);
            case 9: return jelly = Instantiate<GameObject>(jellyBow);
            default:
            return jelly = Instantiate<GameObject>(emptyCell);
        }
    }
    public GameObject randomMakingColor(int quantityColor) // Случайный выбор генерируемой желейки
    {
        GameObject jelly;
        int rand = (random.Next(quantityColor)) + 1;
        switch (rand)
        {
            case 1: return jelly = Instantiate<GameObject>(jellyGreen);
            case 2: return jelly = Instantiate<GameObject>(jellyBlue);
            case 3: return jelly = Instantiate<GameObject>(jellyRad);
            case 4: return jelly = Instantiate<GameObject>(jellyYellow);
            case 5: return jelly = Instantiate<GameObject>(jellyPink);
            default:
            return jelly = Instantiate<GameObject>(jellyGreen);
        }
    }
    public void makingJellyBonus(int i, Coord _coord)  // создание бонусных желе
    {
        if(i == 3)
        {
            return;
        }
        GameObject jellyBonus = null;
        switch(i)
        {
            case 2:
            jellyBonus = Instantiate<GameObject>(jellyBomb);
            break;
            case 3:
            break;
            case 4:
            if(direction)
            {
                jellyBonus = Instantiate<GameObject>(jellyLeftRight);
            }
            else
            {
                jellyBonus = Instantiate<GameObject>(jellyUpDown);
            }
            break;
            default:
            jellyBonus = Instantiate<GameObject>(jellyBow);
            break;
        }
        jellyBonus.GetComponent<Jelly>().coord = _coord;
        jellyBonus.GetComponent<Jelly>().FirstPositionJelly();
        
        AllJelly[_coord.x,_coord.y] = jellyBonus;
        jellyBonus.transform.position = new Vector3(((_coord.x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
        ,(((_coord.y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, -1);
        jellyBonus.transform.localScale = new Vector3(sizeCell, sizeCell, sizeCell);
        jellyBonus.GetComponent<Jelly>().BonusJelly(_coord.x, _coord.y);
    }
    public bool checkDestroy(GameObject [,] allJelly) // Проверка есть ли что стирать (если есть что возвращает true)
    {
        coordsDestroy.Clear();
        GameObject objectJelly = null;
        GameObject returnTrap = null;
        int iter = 0;
        for (int j = 0; j < ySize; j++)
        {
            objectJelly = null;
            iter = 0;
            for (int i = 0; i < xSize; i++)
            {
                if(allJelly[i,j] != null)
                {
                    returnTrap = returnTraps(allJelly[i,j].GetComponent<Jelly>().coord);
                }
                if(allJelly[i,j] == null || (returnTrap != null && returnTrap.GetComponent<Trap>().getTypeTrap() != 1) ||
                    allJelly[i,j].GetComponent<Jelly>().getIndexJelly() == 0) // Если появятся ячейки пустые в которые не сможет упасть желе или является закрыта ловушкой
                {
                    if(iter > 2)
                    {
                        Coord [] destroyCoord = new Coord [iter];
                        for (int k = 1; k <= iter; k++)
                        {
                            destroyCoord[k-1] = new Coord((i - k), j);
                        }
                        coordsDestroy.Add(destroyCoord);
                    }
                    objectJelly = null;
                    iter = 0;
                    continue;
                }
                if(objectJelly == null)
                {
                    objectJelly = allJelly[i,j];
                    iter = 1;
                }
                else
                {
                    if(objectJelly.GetComponent<Jelly>().IndexJelly == allJelly[i,j].GetComponent<Jelly>().IndexJelly & i < xSize - 1)
                    {
                        iter++;
                    }
                    else
                    {
                        if(objectJelly.GetComponent<Jelly>().IndexJelly == allJelly[i,j].GetComponent<Jelly>().IndexJelly & i == xSize - 1 & (iter + 1) > 2)
                        {
                            Coord [] destroyCoord = new Coord [iter + 1];
                            for (int k = 0; k < iter + 1; k++)
                            {
                                destroyCoord[k] = new Coord((i - k), j);
                            }
                            coordsDestroy.Add(destroyCoord);
                        }
                        else
                        {
                            if(iter > 2)
                            {
                                Coord [] destroyCoord = new Coord [iter];
                                for (int k = 1; k <= iter; k++)
                                {
                                    destroyCoord[k-1] = new Coord((i - k), j);
                                }
                                coordsDestroy.Add(destroyCoord);
                                objectJelly =  allJelly[i,j];
                                iter = 1;
                            }
                            else
                            {
                                objectJelly =  allJelly[i,j];
                                iter = 1;
                            }
                        }
                    }
                }
            }
        }
        for (int i = 0; i < xSize; i++)
        {
            objectJelly = null;
            iter = 0;
            for (int j = 0; j < ySize; j++)
            {
                if(allJelly[i,j] != null)
                {
                    returnTrap = returnTraps(allJelly[i,j].GetComponent<Jelly>().coord);
                }
                if(allJelly[i,j] == null || (returnTrap != null && returnTrap.GetComponent<Trap>().getTypeTrap() != 1) ||
                    allJelly[i,j].GetComponent<Jelly>().getIndexJelly() == 0)  // Если появятся ячейки пустые в которые не сможет упасть желе или является закрыта ловушкой
                {
                    if(iter > 2)
                    {
                        Coord [] destroyCoord = new Coord [iter];
                        for (int k = 1; k <= iter; k++)
                        {
                            destroyCoord[k-1] = new Coord(i, (j - k));
                        }
                        coordsDestroy.Add(destroyCoord);
                    }
                    objectJelly = null;
                    iter = 0;
                    continue;
                }
                if(objectJelly == null)
                {
                    objectJelly = allJelly[i,j];
                    iter = 1;
                }
                else
                {
                    if(objectJelly.GetComponent<Jelly>().IndexJelly == allJelly[i,j].GetComponent<Jelly>().IndexJelly & j < ySize - 1)
                    {
                        iter++;
                    }
                    else
                    {
                        if(objectJelly.GetComponent<Jelly>().IndexJelly == allJelly[i,j].GetComponent<Jelly>().IndexJelly & j == ySize - 1 & (iter + 1) > 2)
                        {
                            Coord [] destroyCoord = new Coord [iter + 1];
                            for (int k = 0; k < iter + 1; k++)
                            {
                                destroyCoord[k] = new Coord(i, (j - k));
                            }
                            coordsDestroy.Add(destroyCoord);
                        }
                        else
                        {
                            if(iter > 2)
                            {
                                Coord [] destroyCoord = new Coord [iter];
                                for (int k = 1; k <= iter; k++)
                                {
                                    destroyCoord[k-1] = new Coord(i, (j - k));
                                }
                                coordsDestroy.Add(destroyCoord);
                                objectJelly =  allJelly[i,j];
                                iter = 1;
                            }
                            else
                            {
                                objectJelly =  allJelly[i,j];
                                iter = 1;
                            }
                        }
                    }
                }
            }
        }
        if(coordsDestroy.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void Erasing() // стирание желеек (происходит от checkDestroy)
    {
        bool check = true;
        //--------------------------------------------- Всплывающие очки -----------------------------------------------
        GameObject points = Instantiate<GameObject>(Points);
        int numberOfCells = 0;
        //foreach (Coord [] item in coordsDestroy)
        //{
        //   numberOfCells += item.Length;
        //}
        //points.GetComponent<Points>().Scoring(numberOfCells, coordsDestroy.Count, new Vector3(0, 0, -2));
        //--------------------------------------------------------------------------------------------------------------
        while(check)
        {
            
            if(coordsDestroy.Count == 0)
            {
                check = false;
                break;
            }
            Coord [] maxCount = null;
            foreach (Coord [] item in coordsDestroy)
            {
                if(maxCount == null)
                {
                    maxCount = item;
                }
                else
                {
                    if(maxCount.Length < item.Length)
                    {
                        maxCount = item;
                    }
                } 
            }
            int directionX = 99;
            bool blockMaking = false;
            for (int i = 0; i < maxCount.Length; i++)
            {                
                AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().MoveProgress = false;
                //------------------------ Проверка для создание бомбочки  ------------------------------------------
                if(maxCount.Length == 4 || maxCount.Length == 3)
                {
                    int units = 0;
                    foreach (Coord [] iter in coordsDestroy)
                    {
                        for (int j = 0; j < iter.Length; j++)
                        {
                            if(AllJelly[maxCount[i].x,maxCount[i].y] != null & AllJelly[iter[j].x,iter[j].y] != null)
                            {
                                if(AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().coord.Equals(AllJelly[iter[j].x,iter[j].y].GetComponent<Jelly>().coord))
                                {
                                    units++;                                    
                                }
                            }
                        }
                    }
                    if(units == 2)
                    {
                        checkUnitsCoord = AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().coord;
                    }
                }
                //-----------------------------------------------------------------------------------------------------
                if(AllJelly[maxCount[i].x,maxCount[i].y] != null)
                {
                    //------------------------------ Для определения горизонтального или вертикального бонуса -----------------
                    if(directionX == 99)
                    {
                        directionX = maxCount[i].x;
                    }
                    else
                    {
                        if(directionX == maxCount[i].x)
                        {
                            direction = true;
                        }
                        else
                        {
                            direction = false;
                        }
                    }
                    //----------------------------------------------------------------------------------------------------------
                    if(!AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().bonus)
                    {
                        Coord coordDestroy = new Coord(AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().coord.x,
                                                       AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().coord.y); 
                        if(returnTraps(coordDestroy) != null && returnTraps(coordDestroy).GetComponent<Trap>().getTypeTrap() == 1)
                        {
                            destroyTrap(coordDestroy);
                        }
                        AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyEasy");
                        
                        numberOfCells++;
                        if(maxCount.Length > 3)
                        {
                            
                            if(movedJelly != null && coordDestroy.Equals(movedJelly.GetComponent<Jelly>().coord) && checkUnitsCoord == null)
                            {
                                makingJellyBonus(maxCount.Length, coordDestroy);
                                blockMaking = true;
                            }
                            else
                            {
                                if(changeableJelly != null && coordDestroy.Equals(changeableJelly.GetComponent<Jelly>().coord) && checkUnitsCoord == null)
                                {
                                    makingJellyBonus(maxCount.Length, coordDestroy);
                                    blockMaking = true;
                                }
                                else
                                {
                                    if(i == 2 && !blockMaking  && checkUnitsCoord == null)
                                    {
                                        makingJellyBonus(maxCount.Length, coordDestroy);
                                    }
                                }
                            }
                        }
                        if(checkUnitsCoord != null)
                        {
                            makingJellyBonus(2,checkUnitsCoord); // для создание бомбочки
                            checkUnitsCoord = null;
                        }
                    }    
                } 
            }
            coordsDestroy.Remove(maxCount);
            if(coordsDestroy.Count == 0)
            {
                check = false;
            }
        }
        points.GetComponent<Points>().Scoring(numberOfCells, new Vector3(0, 0, -2));
    }
    public void shortestDistance (Vector3 PositionClick) // получение обьекта на определённом расстоянии
    {
        float minDistance = 0.5f;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(AllJelly[i, j] != null)
                {
                    Vector3 vectorPositionJelly = AllJelly[i, j].transform.position;
                    float distanceJelly = (Math.Abs(vectorPositionJelly.x - PositionClick.x)*Math.Abs(vectorPositionJelly.x - PositionClick.x)) +
                    (Math.Abs(vectorPositionJelly.y - PositionClick.y)*Math.Abs(vectorPositionJelly.y - PositionClick.y));
                    if(distanceJelly < minDistance)
                    {
                        movedJelly = AllJelly[i, j];
                        minDistance = distanceJelly;                                        
                    }
                }
            }
        }
        if(movedJelly != null)
        {
            coordMovedJellyFirst = new Coord(movedJelly.GetComponent<Jelly>().coord.x, movedJelly.GetComponent<Jelly>().coord.y);
        }
    }
    private void destroyBonusJelly(GameObject jellyBonus) // Работа бонусных желеек
    {
        if(jellyBonus == null)
        {
            return;
        }
        Coord coordPosition = new Coord(jellyBonus.GetComponent<Jelly>().coord.x, jellyBonus.GetComponent<Jelly>().coord.y);
        switch(jellyBonus.GetComponent<Jelly>().getIndexJelly())
        {
            case 6: // --------------------------------------------- LeftRight ----------------------------------
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            for (int i = 0; i < xSize; i++)
            {
                Coord temporaryVariable = null;
                if(AllJelly[i, coordPosition.y] != null && i != coordPosition.x)
                {
                    
                    if(AllJelly[i, coordPosition.y].GetComponent<Jelly>().bonus)
                    {
                        if(changeableJelly != null) // Чтобы не удалялась вновь созданая бонусная желейка
                        {
                            if (!AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord.Equals(movedJelly.GetComponent<Jelly>().coord) &&
                            !AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord.Equals(changeableJelly.GetComponent<Jelly>().coord))
                            {
                                temporaryVariable = new Coord(AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord);
                            }
                        }
                        else
                        {
                            temporaryVariable = new Coord(AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord);
                        }
                    }
                    else
                    {
                        if(AllJelly[i, coordPosition.y].GetComponent<Jelly>().getIndexJelly() != 0)
                        {
                            if(returnTraps(AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord) == null || 
                            returnTraps(AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                            {

                                temporaryVariable = new Coord(AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord);
                                destroyTrap(AllJelly[i,coordPosition.y].GetComponent<Jelly>().coord);
                            }
                            else
                            {
                                destroyTrap(AllJelly[i,coordPosition.y].GetComponent<Jelly>().coord);
                            }                            
                        }
                    }
                }
                if (temporaryVariable != null)
                {
                    
                    if(AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().bonus)
                    {
                        AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().MoveProgress = false;
                        AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                        destroyBonusJelly(AllJelly[temporaryVariable.x, temporaryVariable.y]);
                    }
                    else
                    {
                        Coord [] destroyCoord = new Coord[1];
                        destroyCoord[0] = new Coord (AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().coord);
                        coordsDestroy.Add(destroyCoord);
                    }
                }
            }
            Erasing();
            break;
            case 7: // ------------------------------- UpDown -----------------------------------------------
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            for (int i = 0; i < ySize; i++)
            {
                Coord temporaryVariable = null;
                if(AllJelly[coordPosition.x, i] != null && i != coordPosition.y)
                {
                    if(AllJelly[coordPosition.x, i].GetComponent<Jelly>().bonus)
                    {
                        if(changeableJelly != null) // Чтобы не удалялась вновь созданая бонусная желейка
                        {
                            if (!AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord.Equals(movedJelly.GetComponent<Jelly>().coord) &&
                            !AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord.Equals(changeableJelly.GetComponent<Jelly>().coord))
                            {
                                temporaryVariable = new Coord(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord);
                            }
                        }
                        else
                        {
                           temporaryVariable = new Coord(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord);
                        }
                    }
                    else
                    {
                        if(AllJelly[coordPosition.x, i].GetComponent<Jelly>().getIndexJelly() != 0)
                        {
                            if(returnTraps(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord) == null || 
                            returnTraps(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                            {

                                temporaryVariable = new Coord(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord);
                                destroyTrap(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord);
                            }
                            else
                            {
                                destroyTrap(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord);
                            }                            
                        }
                    }
                }
                if (temporaryVariable != null)
                {
                    
                    if(AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().bonus)
                    {
                        AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().MoveProgress = false;
                        AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                        destroyBonusJelly(AllJelly[temporaryVariable.x, temporaryVariable.y]);
                    }
                    else
                    {
                        Coord [] destroyCoord = new Coord[1];
                        destroyCoord[0] = new Coord (AllJelly[temporaryVariable.x, temporaryVariable.y].GetComponent<Jelly>().coord);
                        coordsDestroy.Add(destroyCoord);
                    }
                }
            }  
            Erasing();          
            break;

            case 8: // --------------------------------------------- Bomb ---------------------------------------
            
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            for (int i = 0; i < 3; i++)
            {
                Coord temporaryVariable = null;
                if(((coordPosition.x  - 1) + i) >= 0 && ((coordPosition.x  - 1) + i) < xSize & (coordPosition.y + 2) < ySize)
                {
                    if(AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2] != null)
                    {
                        if(AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2].GetComponent<Jelly>().bonus)
                        {                           
                            AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            destroyBonusJelly(AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2]);                            
                        }
                        else
                        {
                            if(returnTraps(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().coord) == null &&
                                AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().getIndexJelly() != 0)
                            {
                                AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            }
                            else
                            {
                                if(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().getIndexJelly() != 0)
                                {
                                    if(returnTraps(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                                    {
                                        AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().MoveProgress = false;
                                        AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                    }
                                    destroyTrap(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().coord);
                                }
                            }
                        }
                    }
                }
                if(((coordPosition.x  - 1) + i) >= 0 && ((coordPosition.x  - 1) + i) < xSize & (coordPosition.y - 2) >= 0)
                {
                    if(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)] != null)
                    {
                        
                        if(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().bonus)
                        {
                            AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            destroyBonusJelly(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)]);
                        }
                        else 
                        {
                            if(returnTraps(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().coord) == null &&
                                AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().getIndexJelly() != 0)
                            {
                                AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            }
                            else
                            {
                                if(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().getIndexJelly() != 0)
                                {
                                    if(returnTraps(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                                    {
                                        AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().MoveProgress = false;
                                        AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                    }
                                    destroyTrap(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().coord);
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(((coordPosition.x  - 2) + i) >= 0 && ((coordPosition.x  - 2) + i) < xSize && ((coordPosition.y - 1) + j) >= 0 && ((coordPosition.y - 1) + j) < ySize)
                    {
                        if(AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j] != null)
                        {
                            if(AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().bonus)
                            {
                                if(changeableJelly != null)
                                {
                                    if (!AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().coord.Equals(movedJelly.GetComponent<Jelly>().coord) &&
                                    !AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().coord.Equals(changeableJelly.GetComponent<Jelly>().coord))
                                    {
                                        AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().MoveProgress = false;
                                        AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                        destroyBonusJelly(AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j]);
                                    }
                                }
                                else 
                                {
                                    AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().MoveProgress = false;
                                    AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                    destroyBonusJelly(AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j]);
                                }
                            }
                            else
                            {
                                if(returnTraps(AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().coord) == null &&
                                    AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().getIndexJelly() != 0)
                                {
                                    AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().MoveProgress = false;
                                    AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                }
                                else
                                {
                                    if(AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().getIndexJelly() != 0)
                                    {
                                        if(returnTraps(AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                                        {
                                            AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().MoveProgress = false;
                                            AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                        }
                                        destroyTrap(AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().coord);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            break;
            case 9:  // ----------------------------------------- Bow ----------------------------
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            int index = 99;
            if(changeableJelly != null && !changeableJelly.GetComponent<Jelly>().bonus && changeableJelly.GetComponent<Jelly>().getIndexJelly() != 0)
            {
                index = changeableJelly.GetComponent<Jelly>().getIndexJelly();
            }
            else
            {
                index = random.Next(numberOfColors);
            }    
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    if(AllJelly[i,j] != null)
                    {
                        if(AllJelly[i,j].GetComponent<Jelly>().getIndexJelly() == index && returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord) == null)
                        {
                            AllJelly[i,j].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[i,j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                        }
                        else
                        {
                            if(AllJelly[i,j].GetComponent<Jelly>().getIndexJelly() == index && returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord) != null)
                            {
                                if(returnTraps(AllJelly[i,j].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                                {
                                    AllJelly[i,j].GetComponent<Jelly>().MoveProgress = false;
                                    AllJelly[i,j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                }
                                destroyTrap(AllJelly[i,j].GetComponent<Jelly>().coord);
                            }
                        }
                    }
                }
            }
            break;
            default:
            break;
        }
    }
    public Coord [] directionOfTravel(Vector3 secondVector) // Определение в каком направлении перемещается выбранная желейка
    {
        float moveUp = 0f;
        float moveDown = 0f;
        float moveLeft = 0f;
        float moveRight = 0f;
        Coord [] returnCoords = new Coord [2];
        returnCoords[0] = new Coord (coordMovedJellyFirst.x, coordMovedJellyFirst.y);
        returnCoords[1] = null;
        if((Math.Abs(secondVector.x - firstVector.x)*Math.Abs(secondVector.x - firstVector.x)) 
        + (Math.Abs(secondVector.y - firstVector.y)*Math.Abs(secondVector.y - firstVector.y)) > sizeCell/100)
        {
            if(firstVector.y > secondVector.y)
            {
                moveDown = firstVector.y - secondVector.y;
            }
            else
            {
                moveUp = secondVector.y - firstVector.y;
            }
            if(firstVector.x > secondVector.x)
            {
                moveLeft = firstVector.x - secondVector.x;
            }
            else
            {
                moveRight = secondVector.x - firstVector.x;
            }
            if (moveDown > moveUp & moveDown > moveLeft & moveDown > moveRight)
            {
                if(returnCoords[0].y > 0)
                {
                        returnCoords[1] = new Coord(returnCoords[0].x, returnCoords[0].y - 1);
                }
            }
            else
            {
                if (moveUp > moveLeft & moveUp > moveRight)
                {
                    if(returnCoords[0].y < ySize - 1)
                    {
                            returnCoords[1] = new Coord(returnCoords[0].x, returnCoords[0].y + 1);
                    }
                }
                else
                {
                    if (moveLeft > moveRight)
                    {
                        if(returnCoords[0].x > 0)
                        {
                                returnCoords[1] = new Coord(returnCoords[0].x - 1, returnCoords[0].y);
                        }
                    }
                    else
                    {
                        if(returnCoords[0].x < xSize - 1)
                        {
                                returnCoords[1] = new Coord(returnCoords[0].x + 1, returnCoords[0].y);
                        }
                    }
                }
            }
        }
        else
        {
            returnCoords[1] = null;
        }
        return returnCoords; 
    }
    async private void timeHealp() // подсказка 
    {
        
        while (GameRun)
        {
            if(BlockAction())
            {
                if(counterReset)
                {
                    await Task.Delay(3000);
                    counterReset = false;
                    continue;
                }
                if(!moveSearch())
                {
                    mixing();
                    Playback();
                }
                else
                {
                    if(firstGameObjectСlue != null && secondGameObjectСlue != null)
                    {
                        firstGameObjectСlue.GetComponent<Jelly>().animator.Play("AnimDown");
                        secondGameObjectСlue.GetComponent<Jelly>().animator.Play("AnimDown");
                    }                    
                }
                
                await Task.Delay(3000);
            }
            else
            {
                await Task.Delay(1000);
            }
        }
    }
    public void Start()
    {
        GameRun = true;
        WindowGame(7, 8);
    }
    private void OnApplicationQuit() {
        GameRun = false;
    }
    private void OnMouseDown() 
    {

        if(BlockAction() && movedJelly == null)
        {
            firstVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shortestDistance(firstVector);
            if(movedJelly != null)
            {
                if(returnTraps(movedJelly.GetComponent<Jelly>().coord) != null || movedJelly.GetComponent<Jelly>().getIndexJelly() == 0)
                {
                    movedJelly = null;
                }
                movementJellys();
            }
        }
    }
    private void OnMouseUp() 
    {
        if (movedJelly != null & coordMovedJellyThird == null)
        {
            if(movedJelly.GetComponent<Jelly>().bonus)
            {
                movedJelly.GetComponent<Jelly>().MoveProgress = false;
                movedJelly.GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                destroyBonusJelly(movedJelly);
                Playback();
            }     
            movedJelly = null;
            coordsMove = null;
            changeableJelly = null;
            coordMovedJellyFirst = null;
            coordMovedJellySecond = null;
            coordMovedJellyThird = null;          
        }
    }
    async private void movementJellys() // Работа физики игры
    {
        while(GameRun)
        {
            counterReset = true;
            if(BlockAction())
            {
                if(movedJelly == null)
                {
                    return;
                }
                else
                {
                    if(coordMovedJellyThird == null)
                    {
                        Vector3 a = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        coordsMove = directionOfTravel(a);

                        if(coordsMove[1] != null && returnTraps(coordsMove[1]) == null && AllJelly[coordsMove[1].x, coordsMove[1].y].GetComponent<Jelly>().getIndexJelly() != 0)
                        {
                            coordMovedJellyThird = new Coord(coordsMove[1].x, coordsMove[1].y);                        

                            if(AllJelly[coordsMove[1].x, coordsMove[1].y] != null)
                            {
                                changeableJelly = AllJelly[coordsMove[1].x, coordsMove[1].y];
                                coordMovedJellySecond = new Coord(changeableJelly.GetComponent<Jelly>().coord.x , changeableJelly.GetComponent<Jelly>().coord.y);

                                changeableJelly.GetComponent<Jelly>().setMoved(changeableJelly.transform.position, new Vector3(((coordsMove[0].x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                                ,(((coordsMove[0].y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 1));
                                AllJelly[coordsMove[0].x, coordsMove[0].y] = changeableJelly;
                                changeableJelly.GetComponent<Jelly>().coord.x = coordMovedJellyFirst.x;
                                changeableJelly.GetComponent<Jelly>().coord.y = coordMovedJellyFirst.y;
                                movedJelly.GetComponent<Jelly>().setMoved(movedJelly.transform.position, new Vector3(((coordsMove[1].x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                                ,(((coordsMove[1].y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 0));   
                                AllJelly[coordsMove[1].x, coordsMove[1].y] = movedJelly;
                                movedJelly.GetComponent<Jelly>().coord.x = coordsMove[1].x;
                                movedJelly.GetComponent<Jelly>().coord.y = coordsMove[1].y;                                
                                continue;
                            }
                            else
                            {
                                movedJelly.GetComponent<Jelly>().setMoved(movedJelly.transform.position, new Vector3(((coordsMove[1].x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                                ,(((coordsMove[1].y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 0));   
                                AllJelly[coordsMove[1].x, coordsMove[1].y] = movedJelly;
                                movedJelly.GetComponent<Jelly>().coord.x = coordsMove[1].x;
                                movedJelly.GetComponent<Jelly>().coord.y = coordsMove[1].y; 
                                AllJelly[coordsMove[0].x, coordsMove[0].y] = null;
                                continue;
                            }
                        }
                        else
                        {
                            await Task.Delay(20);
                            continue;
                        }
                    }
                    else
                    {
                        if(!checkDestroy(AllJelly))
                        {
                            if(movedJelly.GetComponent<Jelly>().bonus)
                            {
                                movedJelly.GetComponent<Jelly>().MoveProgress = false;
                                movedJelly.GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                destroyBonusJelly(movedJelly);
                                movedJelly = null;     
                                Playback();                                                  
                            }
                            if(changeableJelly != null)
                            {
                                if(changeableJelly.GetComponent<Jelly>().bonus)
                                {
                                    changeableJelly.GetComponent<Jelly>().MoveProgress = false;
                                    changeableJelly.GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                    destroyBonusJelly(changeableJelly);
                                    changeableJelly = null;
                                    Playback();
                                }
                            }
                            if(movedJelly != null && changeableJelly != null)
                            {
                                movedJelly.GetComponent<Jelly>().setMoved(movedJelly.transform.position, new Vector3(((coordsMove[0].x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                                ,(((coordsMove[0].y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 0));
                                movedJelly.GetComponent<Jelly>().coord.x = coordMovedJellyFirst.x;
                                movedJelly.GetComponent<Jelly>().coord.y = coordMovedJellyFirst.y;
                                AllJelly[coordsMove[0].x, coordsMove[0].y] = movedJelly;
                                changeableJelly.GetComponent<Jelly>().setMoved(changeableJelly.transform.position, new Vector3(((coordMovedJellyThird.x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                                ,(((coordMovedJellyThird.y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 0));
                                changeableJelly.GetComponent<Jelly>().coord.x = coordMovedJellySecond.x;
                                changeableJelly.GetComponent<Jelly>().coord.y = coordMovedJellySecond.y;
                                AllJelly[coordMovedJellyThird.x, coordMovedJellyThird.y] = changeableJelly;
                                
                            }
                            else
                            {
                                if(movedJelly != null && changeableJelly == null)
                                {
                                    movedJelly.GetComponent<Jelly>().setMoved(movedJelly.transform.position, new Vector3(((coordsMove[0].x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                                    ,(((coordsMove[0].y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 0));
                                    movedJelly.GetComponent<Jelly>().coord.x = coordMovedJellyFirst.x;
                                    movedJelly.GetComponent<Jelly>().coord.y = coordMovedJellyFirst.y;
                                    AllJelly[coordsMove[0].x, coordsMove[0].y] = movedJelly;
                                    AllJelly[coordsMove[1].x, coordsMove[1].y] = null;
                                }
                            }                   
                            movedJelly = null;
                            changeableJelly = null;
                            coordMovedJellyFirst = null;
                            coordMovedJellySecond = null;
                            coordMovedJellyThird = null;
                            return;
                        }
                        else
                        {                           
                            Erasing();
                            if(movedJelly.GetComponent<Jelly>().bonus)
                            {
                                movedJelly.GetComponent<Jelly>().MoveProgress = false;
                                movedJelly.GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                destroyBonusJelly(movedJelly);
                                Playback();
                            }
                            if(changeableJelly != null)
                            {
                                if(changeableJelly.GetComponent<Jelly>().bonus)
                                {
                                    changeableJelly.GetComponent<Jelly>().MoveProgress = false;
                                    changeableJelly.GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                    destroyBonusJelly(changeableJelly);
                                    Playback();
                                } 
                            }
                            movedJelly = null;
                            changeableJelly = null;
                            coordMovedJellyFirst = null;
                            coordMovedJellySecond = null;
                            coordMovedJellyThird = null;  
                            Playback();
                            return;                 
                        }
                    }
                }
            }
            else
            {
                await Task.Delay(20);
            }
        }
    }
    async private void Playback() // последовательность выполнения методов во время перемещения желеек
    {
        while(GameRun)
        {
            if(BlockAction())
            {
                if(MoveJelly())
                {
                    continue;
                }
                else
                {
                    if(MoveJellyLeftRight())
                    {
                        continue;
                    }
                    else
                    {
                        
                        if(checkDestroy(AllJelly))
                        {
                            Erasing();
                        }
                        else
                        {
                            if(!moveSearch())
                            {
                                mixing();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }  
            else
            {
                await Task.Delay(20);
            }         
        }
    }
}

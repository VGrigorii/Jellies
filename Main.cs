using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class Main : MonoBehaviour
{
    public Canvas canvas; // Для получения размера экрана
    public GameObject jellyGreen; // обьект желейки префаб
    public GameObject jellyBlue; // обьект желейки префаб
    public GameObject jellyRad; // обьект желейки префаб
    public GameObject jellyYellow; // обьект желейки префаб
    public GameObject jellyPink; // обьект желейки префаб
    public GameObject jellyLeftRight; // обьект желейки префаб
    public GameObject jellyBow; // обьект желейки префаб
    public GameObject jellyUpDown; // обьект желейки префаб
    public GameObject jellyBomb; // обьект желейки префаб
    public GameObject Ice_1;
    public GameObject Ice_2;
    public GameObject Ice_3; 
    public GameObject Caramel_1;
    public GameObject Caramel_2;
    public GameObject Caramel_3;
    //----------------------------------------------------------- Временные для теста -----------------------------------------
    private Coord coord1;
    private Coord coord2;
    //-------------------------------------------------------------------------------------------------------------------------
    public float stepMove = 0.01f;  // разобраться почему не работает *?????????????????????????????????????????????????????????
    private bool block = true; // Для блокировки одновременных действий. Своеобразный поток
    //private bool chekDestroyBonusJelly = false;
    private List<Coord []> coordsDestroy = new List<Coord []>(); // Список координат для удаления из них желеек
    private List<Trap> Traps = new List<Trap>(); // Список ловушек
    private List<GameObject> TrapsObject = new List<GameObject>(); // Список обьектов ловушек 

    private float sizeCell; // размер ячейки желе
    private int xSize; // количество желеек по x
    private int ySize; // количество желеек по y
    private float SizeCanvasMainX; // размер ячейки в конкретном уровне по х
    private float SizeCanvasMainY; // размер ячейки в конкретном уровне по у
    private int xCorect = 100; // сдвиг от краёв для canvas
    private GameObject [,] AllJelly; // сетка обьектов уровня
    private GameObject [,] copyAllJelly; // для проверки возможности перемещения
    public int numberOfColors = 3; // Количество цветов в раунде
    private bool direction; // направление стирания
    private Coord checkUnitsCoord; // переменная для создания бонусных желеек (бомбочек)
    private int [, ] LevelJelly; // Для создания первоночального расположения желеек

//---------------------------------------------------------------------- Для перемещения желейки -----------------------------
    private GameObject movedJelly;
    private Vector3 firstVector;
    private Vector3 secondVector;
    private Coord coordMovedJellyFirst;
    private Coord coordMovedJellySecond;
    private Coord coordMovedJellyThree;
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
                AllJelly[i, j].GetComponent<Jelly>().coord = new Coord(i, j);
                if(j != ySize - 1)
                {
                    AllJelly[i, j].GetComponent<Jelly>().FirstPositionJelly();  
                }                           
                AllJelly[i, j].transform.position = new Vector3(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                ,(((j + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100);                                 
                AllJelly[i, j].transform.localScale = new Vector3(sizeCell, sizeCell, sizeCell);
            }
        }
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
    private bool scanTraps(Coord coordTrap) // проверять наличие ловушки в координате
    {
        if(TrapsObject.Count == 0)
        {
            return false;
        }
        foreach (GameObject item in TrapsObject)
        {
            if(item.GetComponent<Trap>().coord.Equals(coordTrap))
            {
                return true;
            }
        } 
        return false;
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
        block = false;
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
                    //print("++++++++++++++++++++++++++");
                    TrapsObject.Remove(item);
                    Destroy(item);
                    block = true;
                    return;                   
                }
                else 
                {
                    //print("-----------------------------------------------");
                    lvlTrap--;
                    TrapsObject.Remove(item);
                    Destroy(item); 
                    setTrap(typeTrap, lvlTrap, coordTrap);
                    block = true;
                    return;
                    //item.GetComponent<Trap>().removeLvlTrap();
                }
            }
        }
        block = true;
    }
    private bool moveSearch() // Метод проверяющий возможность стирания
    {
        block = false;
        copyAllJelly = AllJelly.Clone() as GameObject[,];
        GameObject temporalObject = null;
        GameObject returnTrap = null;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize - 1; j++)
            {
                temporalObject = null;
                returnTrap = null;
                temporalObject = copyAllJelly[i,j];
                if(copyAllJelly[i,j] != null)
                {
                    returnTrap = returnTraps(copyAllJelly[i,j].GetComponent<Jelly>().coord);
                }
                if(temporalObject == null || returnTrap != null || temporalObject.GetComponent<Jelly>().bonus)
                {
                    continue;
                }
                if(j < ySize - 2)
                {
                    if(copyAllJelly[i,j + 1] != null && !copyAllJelly[i,j + 1].GetComponent<Jelly>().bonus && returnTraps(copyAllJelly[i,j + 1].GetComponent<Jelly>().coord) == null)
                    {                    
                        copyAllJelly[i,j] = copyAllJelly[i,j + 1];
                        copyAllJelly[i,j + 1] = temporalObject;
                        if(checkDestroy(copyAllJelly))
                        {
                            coord1 = copyAllJelly[i,j].GetComponent<Jelly>().coord;
                            coord2 = copyAllJelly[i,j + 1].GetComponent<Jelly>().coord;
                            copyAllJelly = null;
                            return true;
                        }
                        else
                        {
                            copyAllJelly[i,j + 1] = copyAllJelly[i,j];
                            copyAllJelly[i,j] = temporalObject;
                        }
                    }
                }
                if(i < xSize - 1)
                {
                    if(copyAllJelly[i + 1, j] != null && !copyAllJelly[i,j + 1].GetComponent<Jelly>().bonus && returnTraps(copyAllJelly[i + 1,j].GetComponent<Jelly>().coord) == null)
                    {
                        continue;
                    }
                    copyAllJelly[i,j] = copyAllJelly[i + 1,j];
                    copyAllJelly[i + 1,j] = temporalObject;
                    if(checkDestroy(copyAllJelly))
                    {
                        coord1 = copyAllJelly[i,j].GetComponent<Jelly>().coord;
                        coord2 = copyAllJelly[i,j + 1].GetComponent<Jelly>().coord;
                        copyAllJelly = null;
                        return true;
                    }
                    else
                    {
                        copyAllJelly[i + 1,j] = copyAllJelly[i,j];
                        copyAllJelly[i,j] = temporalObject;
                    }
                }
            }
        }
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize - 1; j++)
            {
                if(copyAllJelly[i,j] != null)
                {
                    
                if(AllJelly[i,j].GetComponent<Jelly>().bonus)
                {
                    copyAllJelly = null;
                    return true;
                }
                }
            }
        }
        print("Выводит что нечего стирать!");
        copyAllJelly = null;
        block = true;
        return false;
    }
    private void mixing() // Метод перемешивающий желейки ( Если нет хода для стирания) (не учтены ловушки)(вроде учтены)
    {
        block = false;
        Coord [,] mixingCoords = new Coord[xSize, ySize - 1];
        GameObject [,] mixingObjects = new GameObject[xSize, ySize - 1]; 
        GameObject mixing = null;
        GameObject randomObj = null;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize - 1; j++)
            {
                if(AllJelly[i,j] != null && !scanTraps(AllJelly[i,j].GetComponent<Jelly>().coord))
                {
                    //mixingObjects[i, j] = AllJelly[i,j];
                    int xRand = random.Next(xSize);
                    int yRand = random.Next(ySize - 1);
                    mixing = AllJelly[i,j]; 
                    randomObj = AllJelly[xRand, yRand];
                    if(AllJelly[xRand, yRand] != null && !scanTraps(AllJelly[xRand, yRand].GetComponent<Jelly>().coord))
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
            for (int j = 0; j < ySize - 1; j++)
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
        block = true;
    }
    public void WindowGame(int _xSize, int _ySize) // Создание Ячеек под уровень
    {
        xSize = _xSize;
        ySize = _ySize;
        AllJelly = new GameObject [xSize,ySize];
        LevelJelly =  LevelGeneration.levelGeneration(xSize, ySize, numberOfColors);
        LevelGenerationJelly();
        setTrap(1, 3, new Coord(1, 4));
        setTrap(1, 3, new Coord(5, 4));
        setTrap(0, 3, new Coord(1,2));
        setTrap(0, 3, new Coord(5,2));
    }
    public void Filling(){ // заполнение желейками верхнего скрытого ряда (хз зачем, чтобы из невидимости появлялись) с последующим падением этих желеек
        RectTransform rt = canvas.GetComponent<RectTransform>();
        Vector2 vrt = rt.sizeDelta;
        SizeCanvasMainX = vrt.x - xCorect;
        sizeCell = SizeCanvasMainX / xSize;
        SizeCanvasMainY = ySize * sizeCell;        
        for (int i = 0; i < xSize; i++)
        {
            if(AllJelly[i, ySize - 1] == null){
                GameObject jelly = randomMakingColor(numberOfColors);
                jelly.GetComponent<Jelly>().coord = new Coord(i, ySize - 1);
                AllJelly[i,ySize - 1] = jelly;
                jelly.transform.position = new Vector3(((i*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                ,((ySize*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100);
                jelly.transform.localScale = new Vector3(sizeCell, sizeCell, sizeCell);
            }
        }
    }
    public bool BlockAction() // Проверка движения желеек ( true - движения нету)
    {
        for(int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if(AllJelly[i, j] != null)
                {
                    if(!AllJelly[i, j].GetComponent<Jelly>().MoveProgress)
                    {
                        //print(i + " То что блокирует " + j);
                        return false;
                    }
                }
            }   
        }
        return true;
    }
    public bool MoveJelly() // перемещение желеек в процессе стирания (если true - движение было)
    {
        block = false;
        bool returnChek = false;
        if(BlockAction())
        {
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 1; j < ySize; j++)
                {
                    if((AllJelly[i, j] != null && !scanTraps(AllJelly[i, j].GetComponent<Jelly>().coord))
                    || (AllJelly[i, j] != null && scanTraps(AllJelly[i, j].GetComponent<Jelly>().coord) && 
                    returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1))
                    {
                        if(AllJelly[i, j - 1] == null)
                        {
                            GameObject jelly = AllJelly[i, j];                     
                            Vector2 firstPosition = jelly.transform.position;
                            Vector2 secondPosition = new Vector2(firstPosition.x, firstPosition.y - sizeCell/50);
                            jelly.GetComponent<Jelly>().setMoved(firstPosition, secondPosition, stepMove, i, j - 1);
                            AllJelly[i, j] = null;
                            AllJelly[i, j - 1] = jelly;
                            returnChek = true;
                        }
                        
                    }
                }
            } 
            Filling();
        } 
        block = true;
        return returnChek;
    }
    public bool MoveJellyLeftRight() // для сдвига желеек под ловушками и т.п.
    {
        block = false;
        bool returnChek = false;
        if(BlockAction())
        {
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 1; j < ySize; j++)
                {
                    if((AllJelly[i, j] != null && !scanTraps(AllJelly[i, j].GetComponent<Jelly>().coord))
                    || (AllJelly[i, j] != null && scanTraps(AllJelly[i, j].GetComponent<Jelly>().coord) && 
                    returnTraps(AllJelly[i, j].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1))
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
                                block = true;
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
                                        block = true;
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
                                    block = true;
                                    return true;
                                }
                            }
                        }                                
                    }
                }
            }
          Filling();
        } 
        block = true;
        return returnChek;
    }
    public GameObject MakingJellyIndex(int num) // Создание желеек GameObject по индексу
    {
        GameObject jelly;
        switch (num)
        {
            case 0: return jelly = Instantiate<GameObject>(jellyGreen);
            case 1: return jelly = Instantiate<GameObject>(jellyBlue);
            case 2: return jelly = Instantiate<GameObject>(jellyRad);
            case 3: return jelly = Instantiate<GameObject>(jellyYellow);
            case 4: return jelly = Instantiate<GameObject>(jellyPink);
            default:
            return jelly = Instantiate<GameObject>(jellyGreen);
        }
    }
    public GameObject randomMakingColor(int quantityColor) // Случайный выбор генерируемой желейки
    {
        GameObject jelly;
        int rand = random.Next(quantityColor);
        switch (rand)
        {
            case 0: return jelly = Instantiate<GameObject>(jellyGreen);
            case 1: return jelly = Instantiate<GameObject>(jellyBlue);
            case 2: return jelly = Instantiate<GameObject>(jellyRad);
            case 3: return jelly = Instantiate<GameObject>(jellyYellow);
            case 4: return jelly = Instantiate<GameObject>(jellyPink);
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
        block = false;
        coordsDestroy.Clear();
        GameObject objectJelly = null;
        GameObject returnTrap = null;
        int iter = 0;
        for (int j = 0; j < ySize - 1; j++)
        {
            objectJelly = null;
            iter = 0;
            for (int i = 0; i < xSize; i++)
            {
                if(allJelly[i,j] != null)
                {
                    returnTrap = returnTraps(allJelly[i,j].GetComponent<Jelly>().coord);
                }
                if(allJelly[i,j] == null || (returnTrap != null && returnTrap.GetComponent<Trap>().getTypeTrap() != 1)) // Если появятся ячейки пустые в которые не сможет упасть желе или является закрыта ловушкой
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
            for (int j = 0; j < ySize - 1; j++)
            {
                if(allJelly[i,j] != null)
                {
                    returnTrap = returnTraps(allJelly[i,j].GetComponent<Jelly>().coord);
                }
                if(allJelly[i,j] == null || (returnTrap != null && returnTrap.GetComponent<Trap>().getTypeTrap() != 1))  // Если появятся ячейки пустые в которые не сможет упасть желе или является закрыта ловушкой
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
                    if(objectJelly.GetComponent<Jelly>().IndexJelly == allJelly[i,j].GetComponent<Jelly>().IndexJelly & j < ySize - 2)
                    {
                        iter++;
                    }
                    else
                    {
                        if(objectJelly.GetComponent<Jelly>().IndexJelly == allJelly[i,j].GetComponent<Jelly>().IndexJelly & j == ySize - 2 & (iter + 1) > 2)
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
        block = true;
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
        checkDestroy(AllJelly);
        block = false;
        bool check = true;
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
                        if(scanTraps(coordDestroy) && returnTraps(coordDestroy).GetComponent<Trap>().getTypeTrap() == 1)
                        {
                            destroyTrap(coordDestroy);
                        }
                        AllJelly[maxCount[i].x,maxCount[i].y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyEasy");

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
                            makingJellyBonus(2,checkUnitsCoord); // Это костыль дял создание бомбочки
                            checkUnitsCoord = null;
                        }
                    }    
                } 
            }
            coordsDestroy.Remove(maxCount);
            if(coordsDestroy.Count == 0)
            {
                check = false;
                MoveJelly();
            }
        }
        block = true;
    }
    public void shortestDistance (Vector3 PositionClick) // получение обьекта на определённом расстоянии
    {
        float minDistance = 1f;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize - 1; j++)
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
        block = false;
        if(jellyBonus == null)
        {
            return;
        }
        Coord coordPosition = new Coord(jellyBonus.GetComponent<Jelly>().coord.x, jellyBonus.GetComponent<Jelly>().coord.y);
        switch(jellyBonus.GetComponent<Jelly>().getIndexJelly())
        {
            case 5: // --------------------------------------------- LeftRight ----------------------------------
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            for (int i = 0; i < xSize; i++)
            {
                if(AllJelly[i, coordPosition.y] != null && i != coordPosition.x)
                {
                    
                    if(!AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord.Equals(coordPosition) &&
                        AllJelly[i, coordPosition.y].GetComponent<Jelly>().bonus)
                    {
                        if(changeableJelly != null)
                        {
                            if (!AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord.Equals(movedJelly.GetComponent<Jelly>().coord) ||
                            !AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord.Equals(changeableJelly.GetComponent<Jelly>().coord))
                            {
                                AllJelly[i, coordPosition.y].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[i, coordPosition.y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                destroyBonusJelly(AllJelly[i, coordPosition.y]);
                            }
                        }
                        else
                        {
                            AllJelly[i, coordPosition.y].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[i, coordPosition.y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            destroyBonusJelly(AllJelly[i, coordPosition.y]);
                        }
                    }
                    else
                    {
                        if(!scanTraps(AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord))
                        {
                            AllJelly[i, coordPosition.y].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[i, coordPosition.y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                        }
                        else
                        {
                            if(returnTraps(AllJelly[i, coordPosition.y].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                            {
                                AllJelly[i, coordPosition.y].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[i, coordPosition.y].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            }
                            destroyTrap(AllJelly[i,coordPosition.y].GetComponent<Jelly>().coord);
                        }                        
                    }
                }
            }
            break;
            case 8:  // ----------------------------------------- Bow ----------------------------
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            int index = 99;
            if(changeableJelly != null && !changeableJelly.GetComponent<Jelly>().bonus)
            {
                index = changeableJelly.GetComponent<Jelly>().getIndexJelly();
            }
            else
            {
                index = random.Next(numberOfColors);
            }    
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize - 1; j++)
                {
                    if(AllJelly[i,j] != null)
                    {
                        if(AllJelly[i,j].GetComponent<Jelly>().getIndexJelly() == index && !scanTraps(AllJelly[i, j].GetComponent<Jelly>().coord))
                        {
                            AllJelly[i,j].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[i,j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            //AllJelly[i,j] = null;
                        }
                        else
                        {
                            if(AllJelly[i,j].GetComponent<Jelly>().getIndexJelly() == index && scanTraps(AllJelly[i, j].GetComponent<Jelly>().coord))
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
            case 6: // ------------------------------- UpDown -----------------------------------------------
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            for (int i = 0; i < ySize - 1; i++)
            {
                if(AllJelly[coordPosition.x, i] != null && i != coordPosition.y)
                {
                    if(!AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord.Equals(coordPosition) &&
                        AllJelly[coordPosition.x, i].GetComponent<Jelly>().bonus)
                    {
                        if(changeableJelly != null)
                        {
                            if (!AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord.Equals(movedJelly.GetComponent<Jelly>().coord) &&
                            !AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord.Equals(changeableJelly.GetComponent<Jelly>().coord))
                            {
                                AllJelly[coordPosition.x, i].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[coordPosition.x, i].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                destroyBonusJelly(AllJelly[coordPosition.x, i]);
                            }
                        }
                        else
                        {
                            AllJelly[coordPosition.x, i].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[coordPosition.x, i].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            destroyBonusJelly(AllJelly[coordPosition.x, i]);
                        }
                    }
                    else
                    {
                        if(!scanTraps(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord))
                        {
                            AllJelly[coordPosition.x, i].GetComponent<Jelly>().MoveProgress = false;
                            AllJelly[coordPosition.x, i].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            //AllJelly[coordPosition.x, i] = null;
                        }
                        else
                        {
                            if(returnTraps(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord).GetComponent<Trap>().getTypeTrap() == 1)
                            {
                                AllJelly[coordPosition.x, i].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[coordPosition.x, i].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                            }
                            destroyTrap(AllJelly[coordPosition.x, i].GetComponent<Jelly>().coord);
                        }
                    }
                }
            }            
            break;

            case 7: // --------------------------------------------- Bomb ---------------------------------------
            
            AllJelly[coordPosition.x ,coordPosition.y] = null;
            for (int i = 0; i < 3; i++)
            {
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
                            if(!scanTraps(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y + 2)].GetComponent<Jelly>().coord))
                            {
                                AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                //AllJelly[(coordPosition.x  - 1) + i, coordPosition.y + 2] = null;
                            }
                            else
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
                            if(!scanTraps(AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().coord))
                            {
                                AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().MoveProgress = false;
                                AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                //AllJelly[((coordPosition.x  - 1) + i), (coordPosition.y - 2)] = null;
                            }
                            else
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
                                if(!scanTraps(AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().coord))
                                {
                                    AllJelly[(coordPosition.x  - 2) + i, (coordPosition.y - 1) + j].GetComponent<Jelly>().MoveProgress = false;
                                    AllJelly[((coordPosition.x  - 2) + i), (coordPosition.y - 1) + j].GetComponent<Jelly>().animator.SetTrigger("AnimDestroyer");
                                }
                                else
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
            break;
            default:
            break;
        }
        block = true;
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
                    if(returnCoords[0].y < ySize - 2)
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
    public void Start()
    {
        WindowGame(7, 8);
    }
    private void OnMouseDown() {
        if(block && BlockAction() && movedJelly == null)
        {
            firstVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shortestDistance(firstVector);
            if(movedJelly != null)
            {
                if(scanTraps(movedJelly.GetComponent<Jelly>().coord))
                {
                    movedJelly = null;
                }
                movementJellys();
            }
        }
    }
    private void OnMouseUp() {
        if (movedJelly != null & coordMovedJellyThree == null)
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
            coordMovedJellyThree = null;          
        }
    }
    async private void movementJellys()
    {
        while(true)
        {
            if(BlockAction())
            {
                if(movedJelly == null)
                {
                    return;
                }
                else
                {
                    if(coordMovedJellyThree == null)
                    {
                        Vector3 a = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        coordsMove = directionOfTravel(a);

                        if(coordsMove[1] != null && !scanTraps(coordsMove[1]))
                        {
                            coordMovedJellyThree = new Coord(coordsMove[1].x, coordsMove[1].y);                        

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
                                changeableJelly.GetComponent<Jelly>().setMoved(changeableJelly.transform.position, new Vector3(((coordMovedJellyThree.x*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainX/100
                                ,(((coordMovedJellyThree.y + 1)*sizeCell) + (sizeCell/2))/50 - SizeCanvasMainY/100, 0));
                                changeableJelly.GetComponent<Jelly>().coord.x = coordMovedJellySecond.x;
                                changeableJelly.GetComponent<Jelly>().coord.y = coordMovedJellySecond.y;
                                AllJelly[coordMovedJellyThree.x, coordMovedJellyThree.y] = changeableJelly;
                                
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
                            coordMovedJellyThree = null;
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
                            coordMovedJellyThree = null;  
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
    async private void Playback()
    {
        while(true)
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
                                print("Нечего стирать");
                                mixing();
                                print("перемешанно");
                            }
                            else
                            {
                                print("Есть что стирать");
                                print(coord1.x + "  " + coord1.y + " second " + coord2.x + "   " + coord2.y);
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
    public void Update() 
    {
            
    }
}

using Random = System.Random;
/*
0 - пустая ячейка (ничего нету)
1 - зелёный мишка
2 - синий мишка
3 - красный мишка
4 - желтый мишка
5 - розовый мишка
6 - горизонтальный бонус
7 - вертикальный бонус
8 - бомбочка бонус
9 - разноцветный мишка бонус
10 - лёд ловушка 3
11 - лёд ловушка 2
12 - лёд ловушка 1
13 - карамель 3
14 - карамель 2 
15 - карамель 1
*/
public class LevelGeneration
{
    public static int [, ] levelGeneration (int x, int y, int indexColor)
    {
        int [, ] returnInt = new int [x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {                
                bool chek = true;
                while(chek)
                {
                    int random = new Random().Next(indexColor); 
                    if(i > 1)
                    {
                        if(returnInt[i - 1, j] != random && returnInt[i - 2, j] != random)
                        {
                            if(j > 1)
                            {
                                if(returnInt[i, j - 1] != random && returnInt[i, j - 2] != random)
                                {
                                    returnInt[i, j] = random;
                                    chek = false;
                                }
                            }
                            else
                            {
                                returnInt[i, j] = random;
                                chek = false;
                            }
                        }
                    }
                    else
                    {
                        if(j > 1)
                        {
                            if(returnInt[i, j - 1] != random && returnInt[i, j - 2] != random)
                            {
                                returnInt[i, j] = random;
                                chek = false;
                            }
                        }
                        else
                        {
                            returnInt[i, j] = random;
                            chek = false;
                        }
                    }                    
                }
            }
        }
        return returnInt;
    }
    
}

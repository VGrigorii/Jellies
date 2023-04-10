using Random = System.Random;
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

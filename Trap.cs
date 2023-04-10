using UnityEngine;
public class Trap : MonoBehaviour
{
    public int typeTrap;
    public int lvlTrap;
    public bool MoveProgress {set; get;}
    public Coord coord {set; get;} 

    public void setTypeTrap(int _typeTrap)
    {
        typeTrap = _typeTrap;
    }
    public int getTypeTrap ()
    {
        return typeTrap;
    }
    //public void removeLvlTrap ()
    //{
    //    lvlTrap--;
    //}
    public void setLvlTrap(int _lvlTrap)
    {
        lvlTrap = _lvlTrap;
    }
    public int getLvlTrap ()
    {
        return lvlTrap;
    }
}

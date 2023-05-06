using UnityEngine;

public class Coord
{
    public int x {set; get;}
    public int y {set; get;}
    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public Coord(Coord _coord) {
       x = _coord.x;
       y = _coord.y;
    }
    // override object.Equals
    public override bool Equals(object obj)
    {        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Coord coordObj = (Coord)obj;
        return x == coordObj.x & y == coordObj.y;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        throw new System.NotImplementedException();
        //return base.GetHashCode();
    }
}

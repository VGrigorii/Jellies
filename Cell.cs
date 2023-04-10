using UnityEngine;

public class Cell
{
   [HideInInspector]
   public Coord coord {private set; get;}
   [HideInInspector]
   public int ColorCell {set; get;}
   [HideInInspector]
   public bool busy {set; get;}
   public Cell (Coord _coord)
   {
      coord = _coord;
      busy = false;
   }
}

using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Points : MonoBehaviour
{
    private int points = 0;
    private int cellPrice = 100;
    public Text PointsText;
    private float stepMove = 0.1f;
    private float progress = 0f;
    private Vector3 firstPosition;
    private Vector3 secondPosition;

    public void Scoring (int numberOfCells, Vector3 _firstPosition)
    {
        points = cellPrice*numberOfCells;
        PointsText.text = points.ToString();
        firstPosition = _firstPosition;
        secondPosition = (new Vector3 (0, 1, 0)) + firstPosition; 
        gameObject.SetActive(true);
        destroyPoints();
    }
    public async void destroyPoints()
    {
        await Task.Delay(500);
        Destroy(gameObject);
    }
    public void FixedUpdate() 
    {
        if(gameObject.activeInHierarchy)
        {
            transform.position = Vector3.Lerp(firstPosition, secondPosition, progress);
            progress += stepMove;
        }
    }
}
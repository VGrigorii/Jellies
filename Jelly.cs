using UnityEngine;

public class Jelly : MonoBehaviour
{
    public Animator animator{set; get;}
    public bool destroyBonusJelly{set; get;} = false;
    public bool bonus{set; get;}
    public Coord coord{set; get;}
    public int IndexJelly;
    private Vector3 firstPosition;
    private Vector3 secondPosition;
    private float stepMove;
    private float progress;
    private SpriteRenderer jellyRender;
    private float Velocity = 0.0f;
    public bool MoveProgress {set; get;}
    public bool firstPositionJelly{get; set;} = false; // изменил для проверки вернуть приват и убрать сетер и гетер
    public int getIndexJelly()
    {
        return IndexJelly;
    }
    public void setMoved(Vector2 _firstPosition, Vector2 _secondPosition, float _stepMove, int i, int j)
    {
        coord.x = i;
        coord.y = j;
        firstPosition = _firstPosition;
        secondPosition = _secondPosition;
        stepMove = (firstPosition.y - secondPosition.y);
        progress = 0.0f;
        MoveProgress = false;
    }
    public void setMoved(Vector3 _firstPosition, Vector3 _secondPosition)
    {
        firstPosition = _firstPosition;
        secondPosition = _secondPosition;
        stepMove = 0.07f;
        progress = 0.0f;
        MoveProgress = false;
    }
    private void Moved()
    {
        if(firstPosition != secondPosition)
        {
            if(progress >= 1)
            {
                transform.position = secondPosition;
                progress = -1f;
                animator.SetTrigger("AnimTrigger");
                MoveProgress = true;
            }
            else
            {
                transform.position = Vector3.Lerp(firstPosition, secondPosition, progress);
                progress += stepMove;
            }
        }
    }
    public void BonusJelly(int i, int j)
    {
        coord.x = i;
        coord.y = j;
        bonus = true; 
    }
    public void FirstPositionJelly()
    {
        firstPositionJelly = true;
    }
    public void firstFace()
    {
        transform.position += new Vector3(0, 0, -1);
    }
    public void destroyJelly()
    {
        Destroy(gameObject);
    }
    public void Start() 
    {
        animator = GetComponent<Animator>();
        MoveProgress = true;
        jellyRender = GetComponent<SpriteRenderer>();
        jellyRender.color = new Color (jellyRender.color.r, jellyRender.color.g, jellyRender.color.b, 0); 
    }
    public void FixedUpdate() 
    {
        if (progress >= 0)
        {
            Moved();
        }        
        if(jellyRender.color.a < 255 && !MoveProgress)
        {
            jellyRender.color = new Color (jellyRender.color.r, jellyRender.color.g, jellyRender.color.b, Mathf.Lerp(0f, 100f, Velocity));
            Velocity += 0.05f * Time.deltaTime;
        }
        if((transform.position == secondPosition & MoveProgress & jellyRender.color.a < 255) || firstPositionJelly)
        {
            MoveProgress = true;
            jellyRender.color = new Color (jellyRender.color.r, jellyRender.color.g, jellyRender.color.b, 255);
            firstPositionJelly = false;
        }        
    }
}

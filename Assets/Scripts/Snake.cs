using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour
{
    //Script for Snake Movement

    private enum Direction{
      Left,
      Right,
      Up,
      Down
    }
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;

    private int snakeBodySize;

    private List<SnakeMovePosition> snakeMovePositionList;

    private List<SnakeBodyPart> snakeBodyPartList;



    public void Setup(LevelGrid levelGrid)
    {
       this.levelGrid=levelGrid;
    }
   
   private void Awake()
   {
       gridPosition=new Vector2Int(10,10);
       gridMoveTimerMax=1f;
       gridMoveTimer=gridMoveTimerMax;
       gridMoveDirection=Direction.Right;
       
       snakeMovePositionList=new List<SnakeMovePosition>();
       snakeBodySize=0;

       snakeBodyPartList=new List<SnakeBodyPart>();
   }

   private void Update()
   {
       HandleInput();

       HandleGridMovement();

   }

//Keyboard Input Method()
   private void HandleInput()
   {
     if(Input.GetKeyDown(KeyCode.UpArrow))
       {
           if(gridMoveDirection!=Direction.Down)
           {
           gridMoveDirection=Direction.Up;
           
           }
       }

       if(Input.GetKeyDown(KeyCode.DownArrow))
       {
           if(gridMoveDirection!=Direction.Up)
           {
             gridMoveDirection=Direction.Down;
             
           }
       }

       if(Input.GetKeyDown(KeyCode.LeftArrow))
       {
           if(gridMoveDirection!=Direction.Right)
           {
             gridMoveDirection=Direction.Left;
             
           }
       }

       if(Input.GetKeyDown(KeyCode.RightArrow))
       {
            if(gridMoveDirection!=Direction.Left)
            {
             gridMoveDirection=Direction.Right;
             
            }
       }
   }

//Method for Snake Movement
   private void HandleGridMovement()
   {
     gridMoveTimer+=Time.timeScale=.05f;
    
     if(gridMoveTimer>=gridMoveTimerMax)
       {
         gridMoveTimer-=gridMoveTimerMax;

         SnakeMovePosition previousSnakeMovePosition=null;

         if(snakeMovePositionList.Count>0)
         {
           previousSnakeMovePosition=snakeMovePositionList[0];
         }
         SnakeMovePosition snakeMovePosition= new SnakeMovePosition(previousSnakeMovePosition,gridPosition,gridMoveDirection);
         snakeMovePositionList.Insert(0,snakeMovePosition);

         Vector2Int gridMoveDirectionVector;
         switch(gridMoveDirection)
         {
           default:
           case Direction.Right:      gridMoveDirectionVector=new Vector2Int(+1,0); break;
           case Direction.Left:      gridMoveDirectionVector=new Vector2Int(-1,0); break;
           case Direction.Up:      gridMoveDirectionVector=new Vector2Int(0,+1); break;
           case Direction.Down:      gridMoveDirectionVector=new Vector2Int(0,-1); break;
         }

         gridPosition+=gridMoveDirectionVector;

         bool snakeAteFood=levelGrid.TrySnakeEatFood(gridPosition);
         if(snakeAteFood)
         {
          // snakeatefood, grow body
          snakeBodySize++;
          CreateSnakeBodyPart();

         }

         if(snakeMovePositionList.Count>=snakeBodySize+1)
         {
           snakeMovePositionList.RemoveAt(snakeMovePositionList.Count-1);
         }

        /* for(int i=0;i<snakeMovePositionList.Count;i++)
         {
           Vector2Int snakeMovePosition=snakeMovePositionList[i];
           World_Sprite worldSprite=World_Sprite.Create(new Vector3(snakeMovePosition.x,snakeMovePosition.y),Vector3.one*.5f,Color.white);
           FunctionTimer.Create(worldSprite.DestroySelf,gridMoveTimerMax);
         }*/
         transform.position=new Vector3(gridPosition.x,gridPosition.y);
         transform.eulerAngles=new Vector3(0,0,GetAngleFromVector(gridMoveDirectionVector)-90);

         UpdateSnakeBodyParts();

         
       }
   }

   /*private void CreateSnakeBody()
   {
     GameObject snakeBodyGameObject=new GameObject("Snake Body",typeof(SpriteRenderer));
     snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite=GameAssets.i.snakeBodySprite;

     snakeBodyTransformList.Add(snakeBodyGameObject.transform);

     snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder=-snakeBodyTransformList.Count;
   } 
*/
   private void CreateSnakeBodyPart()
   {
     snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
   }

   private void UpdateSnakeBodyParts()
   {
     for(int i=0;i<snakeBodyPartList.Count;i++)
         {
           //Vector3 snakeBodyPosition=new Vector3(snakeMovePositionList[i].x,snakeMovePositionList[i].y);
           snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);

         }
   }
   private float GetAngleFromVector(Vector2Int dir)
   {
     float n=Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg;
     if(n<0) n+=360;
     return n;
   }

   public Vector2Int GetGridPosition()
   {
return gridPosition;
   }
    
    //Return full list of position occupied by snake: head and body
   public List<Vector2Int> GetFullSnakeGridPositionList()
   {
     List<Vector2Int> gridPositionList= new List<Vector2Int>() { gridPosition };
     foreach(SnakeMovePosition snakeMovePosition in snakeMovePositionList)
     {
       gridPositionList.Add(snakeMovePosition.GetGridPosition());
     }
     //gridPositionList.AddRange(snakeMovePositionList);
     return gridPositionList;
   }

   private class SnakeBodyPart{
 
   private SnakeMovePosition snakeMovePosition;
     private Vector2Int gridPosition;
     private Transform transform;

     public SnakeBodyPart(int bodyIndex)
     {
       GameObject snakeBodyGameObject=new GameObject("Snake Body",typeof(SpriteRenderer));
       snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite=GameAssets.i.snakeBodySprite;
       snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder=-bodyIndex;
       transform=snakeBodyGameObject.transform;
     }

     public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
     {
       this.snakeMovePosition=snakeMovePosition;
       transform.position=new Vector3(snakeMovePosition.GetGridPosition().x,snakeMovePosition.GetGridPosition().y);

       float angle;
       switch(snakeMovePosition.GetDirection())
       {
         default:
         case Direction.Up:    
              switch(snakeMovePosition.GetPreviousDirection())
              {
                     default: angle= 90; break;
                     case Direction.Left:   angle= 0 + 45;  break;
                     case Direction.Right:   angle= 0 - 45;  break;
              }break;
         case Direction.Down:  
              switch(snakeMovePosition.GetPreviousDirection())
              {
                     default: angle= 180; break;
                     case Direction.Left:   angle= 180 + 45;  break;
                     case Direction.Right:   angle= 180 - 45;  break;
              }break;
         case Direction.Left:  
              switch(snakeMovePosition.GetPreviousDirection())
              {
                     default: angle= -90; break;
                     case Direction.Down:   angle= -45;  break;
                     case Direction.Up:   angle= 45;  break;
              }break;
         case Direction.Right: 
              switch(snakeMovePosition.GetPreviousDirection())
              {
                     default: angle= 90; break;
                     case Direction.Down:   angle= 45;  break;
                     case Direction.Up:   angle= -45;  break;
              }
              break;
       }
       transform.eulerAngles=new Vector3(0,0,angle);
     }
   }

   private class SnakeMovePosition{

     private SnakeMovePosition previousSnakeMovePosition;
     private Vector2Int gridPosition;

     private Direction direction;

     public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition,Vector2Int gridPosition,Direction direction)
     {
       this.previousSnakeMovePosition=previousSnakeMovePosition;
       this.gridPosition=gridPosition;
       this.direction=direction;
     }

     public Vector2Int GetGridPosition()
     {
       return gridPosition;
     }

     public Direction GetDirection()
     {
       return direction;
     }

     public Direction GetPreviousDirection()
     {
       if(previousSnakeMovePosition==null)
       {
         return Direction.Right;
       }
       else
       {
         return previousSnakeMovePosition.direction;
       }
     }
   }
}

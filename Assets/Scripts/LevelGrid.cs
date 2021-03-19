using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGrid 
{
    //Script for Food Spawning
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;

private int width;
private int height;

private Snake m_snake;



public Snake snake
{
    get{
        return m_snake; 
       }
    set{
        m_snake=value;
       }  
}


public LevelGrid(int width, int height)
{
this.width=width;
this.height=height;

}

public void Setup(Snake snake)
{
this.snake=snake;
SpawnFood();
}

 private void SpawnFood()
    {
        do{
            foodGridPosition=new Vector2Int(Random.Range(0,width),Random.Range(0,height));
          }while(snake.GetGridPosition()==foodGridPosition);

      foodGameObject= new GameObject("FoodApple",typeof(SpriteRenderer));

      foodGameObject.GetComponent<SpriteRenderer>().sprite=GameAssets.i.foodSprite;
      foodGameObject.transform.position=new Vector3(foodGridPosition.x,foodGridPosition.y);
      Debug.Log("Food Spawned");
    }

    public void SnakeMoved(Vector2Int snakeGridPosition)
    {
     if(snakeGridPosition==foodGridPosition)
     {
      Object.Destroy(foodGameObject);
    
      
      
      Debug.Log("Food Destroyed");
      SpawnFood();
      
     }
    }
}

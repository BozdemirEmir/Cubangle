using UnityEngine;


public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public  TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    public Vector2Int startPosition = Vector2Int.zero;

    static bool canGenerate = true;

    private void Start()
    {
        if (canGenerate)
        {
            foreach (var item in GameObject.FindGameObjectsWithTag("Item"))
            {
                Destroy(item);
            }
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(enemy);
            }
            foreach (var gun in GameObject.FindGameObjectsWithTag("Gun"))
            {
                Destroy(gun);
            }
            GenerateDungeon();
            Time.timeScale = 1;

            canGenerate = false;
        }
        

    }
    protected void GenerateDungeon()
    {
        

        tilemapVisualizer.Clear();
        RunProceduralGeneration();

    }

    protected abstract  void RunProceduralGeneration();

    public static void ChangeCanGenerate(bool isCanGenerate)
    {
        canGenerate = isCanGenerate;
    }


}

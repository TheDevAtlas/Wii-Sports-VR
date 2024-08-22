using UnityEngine;
using System.Collections.Generic;

public class SpriteSpawner : MonoBehaviour
{
    public Sprite[] spriteList; // List of sprites to choose from
    public int spritesPerRow = 5; // Number of sprites per row
    public int numberOfRows = 3; // Number of rows
    public float spriteSpacing = 1.0f; // Space between sprites
    public float rowSpacing = 1.5f; // Space between rows
    public float movementSpeed = 2.0f; // Speed at which the rows move
    public float wrapBoundary = 10.0f; // Boundary at which sprites wrap around

    private List<GameObject> rows = new List<GameObject>();

    void Start()
    {
        SpawnRows();
    }

    void Update()
    {
        MoveRows();
    }

    void SpawnRows()
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            GameObject row = new GameObject("Row" + i);
            row.transform.parent = transform;

            for (int j = 0; j < spritesPerRow; j++)
            {
                GameObject spriteObject = new GameObject("Sprite" + j);
                SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = spriteList[Random.Range(0, spriteList.Length)];

                spriteRenderer.color = new Color(1,1,1,0.1f);

                float xPos = j * spriteSpacing;
                float yPos = -i * rowSpacing;
                spriteObject.transform.position = new Vector3(xPos, yPos, 0);
                spriteObject.transform.parent = row.transform;
            }

            rows.Add(row);
        }
    }

    void MoveRows()
    {
        foreach (var row in rows)
        {
            float direction = (rows.IndexOf(row) % 2 == 0) ? -1 : 1; // Alternate direction per row

            foreach (Transform sprite in row.transform)
            {
                sprite.Translate(Vector3.right * direction * movementSpeed * Time.deltaTime);

                // Check if the sprite has moved too far to the left or right and wrap it
                if (direction == -1 && sprite.position.x < 0)
                {
                    sprite.position = new Vector3(wrapBoundary, sprite.position.y, sprite.position.z);
                }
                else if (direction == 1 && sprite.position.x > wrapBoundary)
                {
                    sprite.position = new Vector3(0, sprite.position.y, sprite.position.z);
                }
            }
        }
    }
}

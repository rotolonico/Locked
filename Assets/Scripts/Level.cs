using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level
{
    public string id = "";
    public string name;
    public string author;
    public string authorId;
    public int levelRows;
    public int levelColumns;
    public Cell[] level;

    public Level()
    {
        name = EditorHandler.levelNameString;
        author = EditorHandler.levelAuthorString;
        levelRows = EditorHandler.levelRows;
        levelColumns = EditorHandler.levelColumns;

        var emptyTileCount = 0;
    
        var rawLevel = new Cell[levelRows*levelColumns];
        for (var i = 0; i < rawLevel.Length; i++)
        {
            var x = i % levelColumns;
            var y = (int)Math.Truncate((double) (i / levelColumns));
            Vector2 circlePos;
            circlePos = new Vector2(-levelColumns / 2 + x + 1, levelRows / 2 - y);

            var objectColliders = Physics2D.OverlapCircleAll(circlePos, 0.3f);

            if (objectColliders.Length != 0)
            {
                var blocks = new BlockProprieties[objectColliders.Length];
                for (var j = 0; j < blocks.Length; j++)
                {
                    if (objectColliders[j].GetComponent<SelectSprite>() != null)
                    {
                        // Block Id
                        blocks[j] = new BlockProprieties
                            {id = objectColliders[j].GetComponent<SelectSprite>().spriteGameObjectId};

                        // Player
                        if (blocks[j].id == 1)
                        {
                            blocks[j].movesLimit = objectColliders[j].GetComponent<SelectSprite>().movesLimit;
                        }
                        
                        // Limited Block
                        if (blocks[j].id == 25)
                        {
                            blocks[j].limitedStep = objectColliders[j].GetComponent<SelectSprite>().limitedStep;
                        }
                        
                        // Random Block
                        if (blocks[j].id == 34)
                        {
                            blocks[j].randomType = objectColliders[j].GetComponent<SelectSprite>().randomType;
                        }
                    }
                }

                rawLevel[i] = new Cell {position = i, blocks = blocks};
            }
            else
            {
                emptyTileCount++;
            }
            
            level = new Cell[levelRows*levelColumns-emptyTileCount];
            var cellNumber = 0;
            foreach (var cell in rawLevel)
            {
                if (cell != null)
                {
                    level[cellNumber] = cell;
                    cellNumber++;
                }
            }
        }
        
        
    }
}

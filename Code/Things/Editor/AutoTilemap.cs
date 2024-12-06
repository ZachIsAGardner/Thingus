using System.Numerics;

namespace Thingus;

public class AutoTilemapCell
{
    public int Number = 1;
    public List<List<int>> Condition;

    public AutoTilemapCell() { }
    public AutoTilemapCell(int number, List<List<int>> condition)
    {
        Number = number;
        Condition = condition;
    }
}

// 0: must not be match
// 1: must be match
// 2: center
// 3: doesn't matter
public class AutoTilemap
{
    public List<AutoTilemapCell> Cells = new List<AutoTilemapCell>() { };

    public AutoTilemap() { }
    public AutoTilemap(List<AutoTilemapCell> cells)
    {
        Cells = cells;
    }

    public int? GetTileNumber(MapCell cell)
    {
        // Check for matching neighbors
        bool left = DoesCellExist(cell, new Vector2(-1, 0));
        bool leftUp = DoesCellExist(cell, new Vector2(-1, -1));
        bool up = DoesCellExist(cell, new Vector2(0, -1));
        bool rightUp = DoesCellExist(cell, new Vector2(1, -1));
        bool right = DoesCellExist(cell, new Vector2(1, 0));
        bool rightDown = DoesCellExist(cell, new Vector2(1, 1));
        bool down = DoesCellExist(cell, new Vector2(0, 1));
        bool leftDown = DoesCellExist(cell, new Vector2(-1, 1));

        return DigestAdjacentResult(left, leftUp, up, rightUp, right, rightDown, down, leftDown);
    }

    private bool DoesCellExist(MapCell cell, Vector2 offset)
    {
        MapCell other = cell.Map.GetCell(cell.Position + (offset * CONSTANTS.TILE_SIZE), cell.Import.Layer);

        // Found neighbor
        if (other != null)
        {
            // Same as me?
            if (other.Name == cell.Name)
            {
                return true;
            }
            // We're different, but do we share any auto keys?
            else
            {
                if (other.Import.AutoKeys.Any(ok => cell.Import.AutoKeys.Any(ck => ok == ck)))
                {
                    return true;
                }
            }
        }
        // No matching neighbor
        else
        {
            // TODO: Check OOB
        }

        return false;
    }

    public int? DigestAdjacentResult(bool left, bool leftUp, bool up, bool rightUp, bool right, bool rightDown, bool down, bool leftDown)
    {
        // Find the appropriate AutoTilemapCell

        AutoTilemapCell result = Cells.Find(c =>
        {
            int conditionLeft = c.Condition[1][0];
            int conditionLeftUp = c.Condition[0][0];
            int conditionUp = c.Condition[0][1];
            int conditionRightUp = c.Condition[0][2];
            int conditionRight = c.Condition[1][2];
            int conditionRightDown = c.Condition[2][2];
            int conditionDown = c.Condition[2][1];
            int conditionLeftDown = c.Condition[2][0];

            return ((left && conditionLeft == 1) || (!left && conditionLeft == 0))
                && ((leftUp && conditionLeftUp == 1) || (!leftUp && conditionLeftUp == 0))
                && ((up && conditionUp == 1) || (!up && conditionUp == 0))
                && ((rightUp && conditionRightUp == 1) || (!rightUp && conditionRightUp == 0))
                && ((right && conditionRight == 1) || (!right && conditionRight == 0))
                && ((rightDown && conditionRightDown == 1) || (!rightDown && conditionRightDown == 0))
                && ((down && conditionDown == 1) || (!down && conditionDown == 0))
                && ((leftDown && conditionLeftDown == 1) || (!leftDown && conditionLeftDown == 0));
        });

        // Search again with more lax criteria
        if (result == null)
        {
            result = Cells.Find(c =>
            {
                int conditionLeft = c.Condition[1][0];
                int conditionLeftUp = c.Condition[0][0];
                int conditionUp = c.Condition[0][1];
                int conditionRightUp = c.Condition[0][2];
                int conditionRight = c.Condition[1][2];
                int conditionRightDown = c.Condition[2][2];
                int conditionDown = c.Condition[2][1];
                int conditionLeftDown = c.Condition[2][0];

                return ((left && conditionLeft == 1) || (!left && conditionLeft == 0) || conditionLeft == 3)
                    && ((leftUp && conditionLeftUp == 1) || (!leftUp && conditionLeftUp == 0) || conditionLeftUp == 3)
                    && ((up && conditionUp == 1) || (!up && conditionUp == 0) || conditionUp == 3)
                    && ((rightUp && conditionRightUp == 1) || (!rightUp && conditionRightUp == 0) || conditionRightUp == 3)
                    && ((right && conditionRight == 1) || (!right && conditionRight == 0) || conditionRight == 3)
                    && ((rightDown && conditionRightDown == 1) || (!rightDown && conditionRightDown == 0) || conditionRightDown == 3)
                    && ((down && conditionDown == 1) || (!down && conditionDown == 0) || conditionDown == 3)
                    && ((leftDown && conditionLeftDown == 1) || (!leftDown && conditionLeftDown == 0) || conditionLeftDown == 3);
            });
        }

        return result?.Number;
    }

    public static AutoTilemap AutoTilemapWall = new AutoTilemap(new List<AutoTilemapCell>()
        {
            new AutoTilemapCell(0, new List<List<int>>()
            {
                new List<int>() { 3, 0, 0 },
                new List<int>() { 3, 2, 1 },
                new List<int>() { 3, 1, 1 }
            }),
            new AutoTilemapCell(1, new List<List<int>>()
            {
                new List<int>() { 0, 0, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 1 }
            }),
            new AutoTilemapCell(2, new List<List<int>>()
            {
                new List<int>() { 0, 0, 3 },
                new List<int>() { 1, 2, 3 },
                new List<int>() { 1, 1, 3 }
            }),
            new AutoTilemapCell(3, new List<List<int>>()
            {
                new List<int>() { 0, 0, 0 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 0, 1, 0 }
            }),

            new AutoTilemapCell(4, new List<List<int>>()
            {
                new List<int>() { 3, 1, 1 },
                new List<int>() { 3, 2, 1 },
                new List<int>() { 3, 0, 0 }
            }),
            new AutoTilemapCell(5, new List<List<int>>()
            {
                new List<int>() { 1, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 0, 0 }
            }),
            new AutoTilemapCell(6, new List<List<int>>()
            {
                new List<int>() { 1, 1, 3 },
                new List<int>() { 1, 2, 3 },
                new List<int>() { 0, 0, 3 }
            }),
            new AutoTilemapCell(7, new List<List<int>>()
            {
                new List<int>() { 0, 1, 0 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 0, 0, 0 }
            })
        });

    public static AutoTilemap AutoTilemap16 = new AutoTilemap(new List<AutoTilemapCell>()
        {
            new AutoTilemapCell(0, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(1, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(2, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(3, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(4, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(5, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(6, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(7, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(8, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(9, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(10, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(11, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(12, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(13, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(14, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(15, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
        });

    public static AutoTilemap AutoTilemap47 = new AutoTilemap(new List<AutoTilemapCell>()
        {
            new AutoTilemapCell(0, new List<List<int>>()
            {
                new List<int>() { 1, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 1 }
            }),
            new AutoTilemapCell(1, new List<List<int>>()
            {
                new List<int>() { 0, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 1 }
            }),
            new AutoTilemapCell(2, new List<List<int>>()
            {
                new List<int>() { 1, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 1 }
            }),
            new AutoTilemapCell(3, new List<List<int>>()
            {
                new List<int>() { 0, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 1 }
            }),
            new AutoTilemapCell(4, new List<List<int>>()
            {
                new List<int>() { 1, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 0 }
            }),
            new AutoTilemapCell(5, new List<List<int>>()
            {
                new List<int>() { 0, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 0 }
            }),
            new AutoTilemapCell(6, new List<List<int>>()
            {
                new List<int>() { 1, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 0 }
            }),
            new AutoTilemapCell(7, new List<List<int>>()
            {
                new List<int>() { 0, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 0 }
            }),
            new AutoTilemapCell(8, new List<List<int>>()
            {
                new List<int>() { 1, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 1 }
            }),
            new AutoTilemapCell(9, new List<List<int>>()
            {
                new List<int>() { 0, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 1 }
            }),
            new AutoTilemapCell(10, new List<List<int>>()
            {
                new List<int>() { 1, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 1 }
            }),
            new AutoTilemapCell(11, new List<List<int>>()
            {
                new List<int>() { 0, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 1 }
            }),
            new AutoTilemapCell(12, new List<List<int>>()
            {
                new List<int>() { 1, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 0 }
            }),
            new AutoTilemapCell(13, new List<List<int>>()
            {
                new List<int>() { 0, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 0 }
            }),
            new AutoTilemapCell(14, new List<List<int>>()
            {
                new List<int>() { 1, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 0 }
            }),
            new AutoTilemapCell(15, new List<List<int>>()
            {
                new List<int>() { 0, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 0 }
            }),
            new AutoTilemapCell(16, new List<List<int>>()
            {
                new List<int>() { 3, 1, 1 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 1 }
            }),
            new AutoTilemapCell(17, new List<List<int>>()
            {
                new List<int>() { 3, 1, 0 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 1 }
            }),
            new AutoTilemapCell(18, new List<List<int>>()
            {
                new List<int>() { 3, 1, 1 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 0 }
            }),
            new AutoTilemapCell(19, new List<List<int>>()
            {
                new List<int>() { 3, 1, 0 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 0 }
            }),
            new AutoTilemapCell(20, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 1 }
            }),
            new AutoTilemapCell(21, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 1, 1, 0 }
            }),
            new AutoTilemapCell(22, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 1 }
            }),
            new AutoTilemapCell(23, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 0, 1, 0 }
            }),
            new AutoTilemapCell(24, new List<List<int>>()
            {
                new List<int>() { 1, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 1, 1, 3 }
            }),
            new AutoTilemapCell(25, new List<List<int>>()
            {
                new List<int>() { 1, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 0, 1, 3 }
            }),
            new AutoTilemapCell(26, new List<List<int>>()
            {
                new List<int>() { 0, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 1, 1, 3 }
            }),
            new AutoTilemapCell(27, new List<List<int>>()
            {
                new List<int>() { 0, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 0, 1, 3 }
            }),
            new AutoTilemapCell(28, new List<List<int>>()
            {
                new List<int>() { 1, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(29, new List<List<int>>()
            {
                new List<int>() { 0, 1, 1 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(30, new List<List<int>>()
            {
                new List<int>() { 1, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(31, new List<List<int>>()
            {
                new List<int>() { 0, 1, 0 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(32, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(33, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(34, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 1 }
            }),
            new AutoTilemapCell(35, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 1, 0 }
            }),
            new AutoTilemapCell(36, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 1, 1, 3 }
            }),
            new AutoTilemapCell(37, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 0, 1, 3 }
            }),
            new AutoTilemapCell(38, new List<List<int>>()
            {
                new List<int>() { 1, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(39, new List<List<int>>()
            {
                new List<int>() { 0, 1, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(40, new List<List<int>>()
            {
                new List<int>() { 3, 1, 1 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(41, new List<List<int>>()
            {
                new List<int>() { 3, 1, 0 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(42, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 1, 3 }
            }),
            new AutoTilemapCell(43, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 1 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(44, new List<List<int>>()
            {
                new List<int>() { 3, 1, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(45, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 1, 2, 0 },
                new List<int>() { 3, 0, 3 }
            }),
            new AutoTilemapCell(46, new List<List<int>>()
            {
                new List<int>() { 3, 0, 3 },
                new List<int>() { 0, 2, 0 },
                new List<int>() { 3, 0, 3 }
            })
        });
}

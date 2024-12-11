namespace Thingus;

public static class ListExtensions
{
    public static void Preadd<T>(this List<T> list, T item)
    {
        list.Insert(0, item);
    }

    public static List<T> Concat<T>(this List<T> list, List<T> other)
    {
        list.AddRange(other);
        return list;
    }
    
    public static Vector2Int? Coordinates<T>(this List<List<T>> matrix, T target)
    {
        int y = 0;
        foreach (List<T> row in matrix)
        {
            int x = 0;
            foreach (T item in row)
            {
                if (EqualityComparer<T>.Default.Equals(item, target)) return new Vector2Int(x, y);
                
                x++;
            }
            y++;
        }

        return null;
    }

    public static List<List<T>> Swap<T>(this List<List<T>> matrix, T a, T b)
    {
        Vector2Int? coordA = matrix.Coordinates(a);
        Vector2Int? coordB = matrix.Coordinates(b);
        if (coordA == null || coordB == null) return matrix;
        return Swap(matrix, coordA.Value, coordB.Value);
    }

    public static List<List<T>> Swap<T>(this List<List<T>> matrix, Vector2Int coordA, Vector2Int coordB)
    {
        T tmp = matrix[coordA.Y][coordA.X];
        matrix[coordA.Y][coordA.X] = matrix[coordB.Y][coordB.X];
        matrix[coordB.Y][coordB.X] = tmp;
        return matrix;
    }

    public static List<T> Swap<T>(this List<T> list, T a, T b)
    {
        return Swap(list, list.IndexOf(a), list.IndexOf(b));
    }

    public static List<T> Swap<T>(this List<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
        return list;
    }

    public static void Resize<T>(this List<T> list, int index)
    {
        while (list.Count() < index)
        {
            list.Add(default(T));
        }
    }

    public static void Resize<T>(this List<List<T>> matrix, int row, int column)
    {
        while (matrix.Count() <= row)
        {
            matrix.Add(new List<T>(column) { });
        }

        for (int r = 0; r < matrix.Count(); r++)
        {
            while (matrix[r].Count() <= column)
            {
                matrix[r].Add(default(T));
            }
        }
    }

    public static T Get<T>(this List<T> list, int index)
    {
        if (index < 0) return  default(T);
        if (index >= list.Count) list.Resize(index);

        return list[index];
    }

    public static T Get<T>(this List<List<T>> list, int row, int column)
    {
        if (row < 0 || column < 0) return default(T);
        if (row >= list.Count || column >= list[row].Count) list.Resize(row, column);
        
        return list[row][column];
    }

    public static void Set<T>(this List<T> list, int index, T value)
    {
        if (index < 0) return;
        if (index >= list.Count) list.Resize(index);

        list[index] = value;
    }

    public static void Set<T>(this List<List<T>> list, int row, int column, T value)
    {
        if (row < 0 || column < 0) return;
        if (row >= list.Count || column >= list[row].Count) list.Resize(row, column);
        
        list[row][column] = value;
    }

    public static T Next<T>(this List<T> list, T current) 
    {
        if (list == null || list.Count <= 0 || !list.Contains(current)) 
        {
            return default(T);
        };
        int index = list.IndexOf(current);
        index++;
        if (index >= list.Count) index = 0;
        return list[index];
    }
    public static string Join<T>(this List<T> list, string seperator)
    {
        return String.Join(seperator, list);
    }

    public static string Join<T>(this IEnumerable<T> list, string seperator)
    {
        return String.Join(seperator, list);
    }

    public static T Random<T>(this List<T> list)
    {
        if (list == null || list.Count <= 0)
        {
            return default(T);
        }

        int index = Chance.Range(0, list.Count);
        return list[index];
    }

    public static List<T> Clone<T>(this List<T> list)
    {
        if (list == null) return null;
        List<T> result = new List<T>() { };
        foreach (T item in list) result.Add(item);
        return result;
    }

    public static List<T> Reversed<T>(this List<T> list)
    {
        List<T> result = list.Clone();
        result.Reverse();
        return result;
    }

    public static bool None<T>(this List<T> list, Func<T, bool> predicate)
    {
        return !list.Any(predicate);
    }

    public static bool None<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        return !list.Any(predicate);
    }
}

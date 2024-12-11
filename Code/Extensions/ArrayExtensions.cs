namespace Thingus;

public static class ArrayExtensions
{
    // public static void Resize<T>(this T[][] matrix, int row, int column)
    // {
    //     if (!(row < 0 || row >= matrix.Count() || column < 0 || column >= matrix[row].Count()))
    //     {
    //         return;
    //     }

    //     while (matrix.Count() <= row)
    //     {
    //         matrix.Push(new T[column]);
    //     }

    //     for (int r = 0; r < matrix.Count(); r++)
    //     {
    //         while (matrix[r].Count() <= column)
    //         {
    //             matrix[r].Push(default(T));
    //         }
    //     }
    // }

    public static T Get<T>(this T[][] matrix, int row, int column)
    {
        if (row < 0 || row >= matrix.Count() || column < 0 || column >= matrix[row].Count())
        {
            return default(T);
        }

        return matrix[row][column];
    }

    public static void Set<T>(this T[][] matrix, int row, int column, T value)
    {
        if (row < 0 || row >= matrix.Count() || column < 0 || column >= matrix[row].Count())
        {
            return;
        }

        matrix[row][column] = value;
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public class Program
{
    public static void Main()
    {
        using (var fileStream = new StreamWriter("log.txt", false, Encoding.UTF8))
        {
            fileStream.AutoFlush = true;
            var multiWriter = new MultiTextWriter(Console.Out, fileStream);
            Console.SetOut(multiWriter);

            for (int i = 5000; i <= 45000; i += 5000)
            {
                    Console.WriteLine($"Testando com {i} elementos:");
                    TestarAlgoritmo(i, BubbleSort, "Bubble Sort");
                    TestarAlgoritmo(i, SelectionSort, "Selection Sort");
                    TestarAlgoritmo(i, InsertionSort, "Insertion Sort");
                    TestarAlgoritmo(i, ShellSort, "Shell Sort");
                    TestarAlgoritmo(i, MergeSort, "Merge Sort");
                    TestarAlgoritmo(i, QuickSort, "Quick Sort");
                    TestarAlgoritmo(i, HeapSort, "Heap Sort");
            }
        }
    }

    public class MultiTextWriter : TextWriter
    {
        private readonly TextWriter[] writers;

        public MultiTextWriter(params TextWriter[] writers)
        {
            this.writers = writers;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(string value)
        {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void WriteLine(string value)
        {
            foreach (var writer in writers)
                writer.WriteLine(value);
        }

        public override void Flush()
        {
            foreach (var writer in writers)
                writer.Flush();
        }
    }

    static void TestarAlgoritmo(int tamanho, Action<Pessoa[]> algoritmo, string nomeAlgoritmo)
    {
        double[] tempos = new double[5];
        long[] memoriaUsada = new long[5];

        for (int i = 0; i < 5; i++)
        {
            Pessoa[] array = Pessoa.GerarArray(tamanho);
            Stopwatch sw = Stopwatch.StartNew();
            long memoriaAntes = GC.GetTotalMemory(true);
            algoritmo(array);
            long memoriaDepois = GC.GetTotalMemory(true);
            sw.Stop();

            tempos[i] = sw.Elapsed.TotalMilliseconds;
            memoriaUsada[i] = memoriaDepois - memoriaAntes;
        }

        Array.Sort(tempos);
        double mediaTempo = (tempos[1] + tempos[2] + tempos[3]) / 3;
        Array.Sort(memoriaUsada);
        long mediaMemoria = (memoriaUsada[1] + memoriaUsada[2] + memoriaUsada[3]) / 3;

        Console.WriteLine($"{nomeAlgoritmo}: Tempo médio = {mediaTempo:F3} ms, Memória usada = {mediaMemoria} bytes");
    }

    static void BubbleSort(Pessoa[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (array[j].Idade > array[j + 1].Idade)
                {
                    var temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }

    static void SelectionSort(Pessoa[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int minIdx = i;
            for (int j = i + 1; j < n; j++)
            {
                if (array[j].Idade < array[minIdx].Idade)
                    minIdx = j;
            }
            var temp = array[minIdx];
            array[minIdx] = array[i];
            array[i] = temp;
        }
    }

    static void InsertionSort(Pessoa[] array)
    {
        int n = array.Length;
        for (int i = 1; i < n; i++)
        {
            var chave = array[i];
            int j = i - 1;
            while (j >= 0 && array[j].Idade > chave.Idade)
            {
                array[j + 1] = array[j];
                j--;
            }
            array[j + 1] = chave;
        }
    }
    static void ShellSort(Pessoa[] array)
    {
        int n = array.Length;
        for (int gap = n / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < n; i++)
            {
                Pessoa temp = array[i];
                int j = i;
                while (j >= gap && array[j - gap].Idade > temp.Idade)
                {
                    array[j] = array[j - gap];
                    j -= gap;
                }
                array[j] = temp;
            }
        }
    }

    static void MergeSort(Pessoa[] array)
    {
        MergeSortRec(array, 0, array.Length - 1);
    }

    static void MergeSortRec(Pessoa[] array, int left, int right)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            MergeSortRec(array, left, mid);
            MergeSortRec(array, mid + 1, right);
            Merge(array, left, mid, right);
        }
    }

    static void Merge(Pessoa[] array, int left, int mid, int right)
    {
        int n1 = mid - left + 1;
        int n2 = right - mid;

        Pessoa[] L = new Pessoa[n1];
        Pessoa[] R = new Pessoa[n2];

        for (int i = 0; i < n1; i++)
            L[i] = array[left + i];
        for (int j = 0; j < n2; j++)
            R[j] = array[mid + 1 + j];

        int iL = 0, iR = 0;
        int k = left;

        while (iL < n1 && iR < n2)
        {
            if (L[iL].Idade <= R[iR].Idade)
            {
                array[k] = L[iL];
                iL++;
            }
            else
            {
                array[k] = R[iR];
                iR++;
            }
            k++;
        }

        while (iL < n1)
        {
            array[k] = L[iL];
            iL++;
            k++;
        }
        while (iR < n2)
        {
            array[k] = R[iR];
            iR++;
            k++;
        }
    }

    static void QuickSort(Pessoa[] array)
    {
        QuickSortRec(array, 0, array.Length - 1);
    }

    static void QuickSortRec(Pessoa[] array, int left, int right)
    {
        if (left < right)
        {
            int pivotIndex = Partition(array, left, right);
            QuickSortRec(array, left, pivotIndex - 1);
            QuickSortRec(array, pivotIndex + 1, right);
        }
    }

    static int Partition(Pessoa[] array, int left, int right)
    {
        Pessoa pivot = array[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (array[j].Idade < pivot.Idade)
            {
                i++;
                Pessoa temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        Pessoa tempPivot = array[i + 1];
        array[i + 1] = array[right];
        array[right] = tempPivot;

        return i + 1;
    }

    static void HeapSort(Pessoa[] array)
    {
        int n = array.Length;
        for (int i = n / 2 - 1; i >= 0; i--)
            Heapify(array, n, i);

        for (int i = n - 1; i > 0; i--)
        {
            Pessoa temp = array[0];
            array[0] = array[i];
            array[i] = temp;
            Heapify(array, i, 0);
        }
    }

    static void Heapify(Pessoa[] array, int n, int i)
    {
        int maior = i;
        int esquerda = 2 * i + 1;
        int direita = 2 * i + 2;

        if (esquerda < n && array[esquerda].Idade > array[maior].Idade)
            maior = esquerda;

        if (direita < n && array[direita].Idade > array[maior].Idade)
            maior = direita;

        if (maior != i)
        {
            Pessoa swap = array[i];
            array[i] = array[maior];
            array[maior] = swap;
            Heapify(array, n, maior);
        }
    }
}



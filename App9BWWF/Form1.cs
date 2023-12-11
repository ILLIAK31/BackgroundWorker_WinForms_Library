using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassBackgroundWorker;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace App9BWWF
{
    public partial class Form1 : Form
    {
        private ClassBW backgroundWorker;
        public Form1()
        {
            InitializeComponent();
            backgroundWorker = new ClassBW();
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.Error += BackgroundWorker_Error;
        }
        private int[,] GenerateRandomMatrix(int rows, int cols)
        {
            Random rand = new Random();
            int[,] matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rand.Next(1, 10); 
                }
            }
            return matrix;
        }
        private void MultiplyMatrices(object argument)
        {
            if (argument is not MatrixMultiplicationArgs args)
                return;
            int[,] matrixA = args.MatrixA;
            int[,] matrixB = args.MatrixB;
            int rowsA = matrixA.GetLength(0);
            int colsA = matrixA.GetLength(1);
            int colsB = matrixB.GetLength(1);
            int[,] result = new int[rowsA, colsB];
            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    for (int k = 0; k < colsA; k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                    int progress = (i * colsB + j + 1) * 100 / (rowsA * colsB);
                    backgroundWorker.ReportProgress(progress);
                }
            }
        }
        private async void BackgroundWorker_ProgressChanged(object sender, ClassBackgroundWorker.ProgressChangedEventArgs e)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => BackgroundWorker_ProgressChanged(sender, e)));
            }
            else
            {
                progressBar1.Value = e.ProgressPercentage;
                if (e.ProgressPercentage == 100)
                {
                    await Task.Delay(500); 
                    MessageBox.Show("Matrix multiplication completed!", "Task Complete");
                }
            }
        }
        private void BackgroundWorker_Error(object sender, ClassBackgroundWorker.ErrorEventArgs e)
        {
            MessageBox.Show($"Error occurred: {e.Exception.Message}");
        }
        private class MatrixMultiplicationArgs
        {
            public int[,] MatrixA { get; }
            public int[,] MatrixB { get; }
            public MatrixMultiplicationArgs(int[,] matrixA, int[,] matrixB)
            {
                MatrixA = matrixA;
                MatrixB = matrixB;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int size = 1000; // Set the size for both matrices to be size x size
            int[,] matrixA = GenerateRandomMatrix(size, size);
            int[,] matrixB = GenerateRandomMatrix(size, size);
            backgroundWorker.RunWorkerAsync(MultiplyMatrices, new MatrixMultiplicationArgs(matrixA, matrixB));
        }
    }
}

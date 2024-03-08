using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMLaba4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeCells();
            InitializeTimer();
        }

        private const int Rows = 20;
        private const int Columns = 20;
        private bool[,] cells;
        private DataGridView dataGridView;
        private Timer timer;

        private void InitializeDataGridView()
        {
            dataGridView = new DataGridView
            {
                Size = new System.Drawing.Size(400, 400),
                ColumnCount = Columns,
                RowCount = Rows,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                //AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            dataGridView.CellClick += DataGridView_CellClick;
            Controls.Add(dataGridView);
        }

        private void InitializeCells()
        {
            cells = new bool[Rows, Columns];
            Random random = new Random();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    cells[i, j] = random.Next(2) == 0; // randomly set cells to alive or dead
                }
            }
            UpdateDataGridView();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000; // Set the interval in milliseconds (1 second)
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Evolve();
            UpdateDataGridView();
        }

        private void UpdateDataGridView()
        {
            if (dataGridView.InvokeRequired)
            {
                dataGridView.Invoke((MethodInvoker)delegate
                {
                    UpdateDataGridView();
                });
            }
            else
            {
                dataGridView.SuspendLayout();
                dataGridView.Rows.Clear();
                dataGridView.Columns.Clear();
                dataGridView.ColumnCount = Columns;
                dataGridView.RowCount = Rows;

                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        dataGridView[j, i].Value = cells[i, j] ? "X" : "";
                    }
                }

                dataGridView.ResumeLayout();
            }
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;
            cells[rowIndex, columnIndex] = !cells[rowIndex, columnIndex];
            UpdateDataGridView();
        }

        private void Evolve()
        {
            bool[,] nextGeneration = new bool[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int neighbors = CountNeighbors(i, j);
                    if (cells[i, j])
                    {
                        // Any live cell with fewer than two live neighbors dies, as if by underpopulation.
                        // Any live cell with two or three live neighbors lives on to the next generation.
                        // Any live cell with more than three live neighbors dies, as if by overpopulation.
                        nextGeneration[i, j] = (neighbors == 2 || neighbors == 3);
                    }
                    else
                    {
                        // Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
                        nextGeneration[i, j] = (neighbors == 3);
                    }
                }
            }
            cells = nextGeneration;
        }

        private int CountNeighbors(int x, int y)
        {
            int count = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < Rows && j >= 0 && j < Columns && !(i == x && j == y))
                    {
                        if (cells[i, j])
                            count++;
                    }
                }
            }
            return count;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
                StartButton.Text = "Start";
            }
            else
            {
                timer.Start();
                StartButton.Text = "Stop";
            }
        }
    }
}

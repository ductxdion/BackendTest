namespace TodoApi.Models
{
    public class MatrixCheckRequest
    {
        public int N { get; set; }
        public int M { get; set; }
        public int P { get; set; }
        public string MatrixName { get; set; } = string.Empty;
        public required int[][] Matrix { get; set; }
        public required List<Cell> SelectedCells { get; set; }
    }
    public class Cell
    {
        public int I { get; set; }
        public int J { get; set; }
    }
}

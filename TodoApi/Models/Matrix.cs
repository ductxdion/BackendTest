namespace TodoApi.Models
{
    public class Matrix
    {
        public int Id { get; set; }
        public int M { get; set; }
        public int N { get; set; }
        public int P { get; set; }
        public string MatrixData { get; set; } = string.Empty;
        public string MatrixName { get; set; } = string.Empty;
    }
}

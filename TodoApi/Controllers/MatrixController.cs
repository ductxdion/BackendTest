using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatrixController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MatrixController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("check")]
        public IActionResult Check([FromBody] MatrixCheckRequest request)
        {
            if (request == null || request.Matrix == null || request.SelectedCells == null)
            {
                return BadRequest("Invalid request data.");
            }

            int n = request.N;
            int m = request.M;
            int p = request.P;

            var keyPoints = new Dictionary<int, List<Point>>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var key = request.Matrix[i][j];
                    if (!keyPoints.ContainsKey(key))
                    {
                        keyPoints[key] = new List<Point>();
                    }
                    keyPoints[key].Add(new Point(i, j));
                }
            }

            var previousLevel = new List<Path>
            {
                new Path(0.0, new Point(0, 0), new List<Point> { new Point(0, 0) })
            };

            var start = request.Matrix[0][0] == 1 ? 2 : 1; // Tim key tiep theo khi vi tri start la 1
            for (int i = start; i <= p; i++)
            {
                var currentLevel = new List<Path>();
                foreach (var prev in previousLevel)
                {
                    foreach (var point in keyPoints[i])
                    {
                        var fuelUsed = prev.FuelUsed + prev.Location.DistanceTo(point);
                        var path = new List<Point>(prev.Points) { point };
                        currentLevel.Add(new Path(fuelUsed, point, path));
                    }
                }
                previousLevel = currentLevel;
            }

            var minFuelPath = previousLevel.OrderBy(x => x.FuelUsed).First();
            bool isEqual = ArePointsAndCellsSame(minFuelPath.Points, request.SelectedCells);

            return Ok(new { message = isEqual ? "OK" : "Not OK", fuelUsed = minFuelPath.FuelUsed });
        }

        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> InsertMatrix([FromBody] MatrixCheckRequest request)
        {
            if (request == null)
            {
                return BadRequest("Matrix data is null.");
            }

            var matrix = new Matrix
            {
                M = request.M,
                N = request.N,
                P = request.P,
                MatrixData = JsonConvert.SerializeObject(request.Matrix),
                MatrixName = request.MatrixName
            };

            _context.Matrices.Add(matrix);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteMatrix(int id)
        {
            var matrix = await _context.Matrices.FindAsync(id);
            if (matrix == null)
            {
                return NotFound("Matrix not found.");
            }

            _context.Matrices.Remove(matrix);
            await _context.SaveChangesAsync();

            return Ok("Matrix deleted successfully.");
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetMatrices()
        {
            var matrices = await _context.Matrices.ToListAsync();
            return Ok(matrices);
        }

        private bool ArePointsAndCellsSame(List<Point> Points, List<Cell> SelectedCellsData)
        {
            if (Points.Count != SelectedCellsData.Count)
            {
                return false;
            }

            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].X != SelectedCellsData[i].I || Points[i].Y != SelectedCellsData[i].J)
                {
                    return false;
                }
            }

            return true;
        }
    }

    class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Point other)
        {
            return Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
        }
    }

    class Path
    {
        public double FuelUsed { get; }
        public Point Location { get; }
        public List<Point> Points { get; }

        public Path(double fuelUsed, Point location, List<Point> points)
        {
            FuelUsed = fuelUsed;
            Location = location;
            Points = points;
        }
    }
}

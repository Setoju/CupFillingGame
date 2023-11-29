using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CupFilling.Level1Window;

namespace CupFilling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {       
        public MainWindow()
        {
            InitializeComponent();                        
        }                       
        private void Level1Button_Click(object sender, RoutedEventArgs e)
        {
            Level1 start = new Level1();
            start.StartLevel();
        }

        private void Level2Button_Click(object sender, RoutedEventArgs e)
        {
            Level2Window.Level2 start = new Level2Window.Level2();
            start.StartLevel();
        }

        private void Level3Button_Click(object sender, RoutedEventArgs e)
        {
            Level3Window.Level3 start = new Level3Window.Level3();
            start.StartLevel();
        }
    }
    public abstract class Level
    {
        public int LevelNumber { get; protected set; }

        public abstract void StartLevel();
        
    }       
    public class WaterSource
    {
        private int _waterAmount;
        // We will initialize the amount of water in the StartLevel() method to adjust the difficulty
        public WaterSource(int waterAmount)
        {
            _waterAmount= waterAmount;
        }
        public int GetWaterAmount()
        {
            return _waterAmount;
        }
        public void ReleaseWater()
        {
            _waterAmount -= 1;            
        }
    }

    public class Cup
    {
        private PointCollection _position;
        private int _capacity;
        private int _currentWaterAmount;

        public Cup(PointCollection position, int capacity)
        {
            _position = position;
            _capacity = capacity;
            _currentWaterAmount = 0;
        }
        public Polyline ShowCup()
        {
            return new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Points = _position
            };
        }

        public bool FillIfNotFull()
        {
            // We should call this method every time the ball falls into the cup
            if(_currentWaterAmount < _capacity)
            {
                _currentWaterAmount += 1;
                return true;
            }                            
            else
            {
                // <ERROR> Fix the bug when you spam left click and we have too many message boxes
                MessageBox.Show("Congratulations, you won!");                
                return false;
            }
        }
    }

    public class Wall
    {
        private Rectangle _position;
        private double _angle;

        public Wall(Rectangle position, double angle)
        {
            _position = position;
            _angle = angle;
        }
        public bool IsColliding(Ellipse ball)
        {
            double angleInRadians = _angle * Math.PI / 180.0;

            Rect rectBounds = _position.TransformToAncestor((Visual)_position.Parent)
                .TransformBounds(new Rect(_position.RenderSize));

            Rect ellipseBounds = ball.TransformToAncestor((Visual)ball.Parent)
                .TransformBounds(new Rect(ball.RenderSize));

            // Check for intersection of bounding rectangles
            if (!rectBounds.IntersectsWith(ellipseBounds))
            {
                return false;
            }

            // Check for more precise collision using geometric calculations
            Point rectCenter = new Point(rectBounds.X + rectBounds.Width / 2, rectBounds.Y + rectBounds.Height / 2);
            Point ellipseCenter = new Point(ellipseBounds.X + ellipseBounds.Width / 2, ellipseBounds.Y + ellipseBounds.Height / 2);

            double distanceX = Math.Abs(rectCenter.X - ellipseCenter.X);
            double distanceY = Math.Abs(rectCenter.Y - ellipseCenter.Y);

            double halfRectWidth = rectBounds.Width / 2;
            double halfRectHeight = rectBounds.Height / 2;

            double radiusX = ellipseBounds.Width / 2;
            double radiusY = ellipseBounds.Height / 2;

            // Rotate the point of the rectangle center
            double rotatedX = Math.Cos(angleInRadians) * (rectCenter.X - rectBounds.X) -
                              Math.Sin(angleInRadians) * (rectCenter.Y - rectBounds.Y) +
                              rectBounds.X;

            double rotatedY = Math.Sin(angleInRadians) * (rectCenter.X - rectBounds.X) +
                              Math.Cos(angleInRadians) * (rectCenter.Y - rectBounds.Y) +
                              rectBounds.Y;

            // Check if the point is on the other side of the rotated rectangle
            if (rotatedX < rectBounds.X || rotatedX > rectBounds.X + rectBounds.Width ||
                rotatedY < rectBounds.Y || rotatedY > rectBounds.Y + rectBounds.Height)
            {
                return false;
            }

            double rotatedDistanceX = Math.Abs(rotatedX - ellipseCenter.X);
            double rotatedDistanceY = Math.Abs(rotatedY - ellipseCenter.Y);

            if (rotatedDistanceX > (halfRectWidth + radiusX) || rotatedDistanceY > (halfRectHeight + radiusY))
            {
                return false;
            }

            if (rotatedDistanceX <= halfRectWidth || rotatedDistanceY <= halfRectHeight)
            {
                return true;
            }

            double cornerDistanceSquared = Math.Pow(rotatedDistanceX - halfRectWidth, 2) +
                                           Math.Pow(rotatedDistanceY - halfRectHeight, 2);

            return cornerDistanceSquared <= Math.Pow(radiusX, 2);
        }
        public Rectangle GetPosition()
        {
            return _position;
        }        
        public double GetAngle()
        {
            return _angle;
        }
    }
}

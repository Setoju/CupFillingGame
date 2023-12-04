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

        public static int CollisionCheck(Canvas levelCanvas, Ellipse ball)
        {
            foreach (var x in levelCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "wall")
                {
                    Rect ballBounds = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                    Point ballCenter = new Point(ballBounds.X + ballBounds.Width / 2, ballBounds.Y + ballBounds.Height / 2);

                    // Get the rotation angle of the wall
                    var rotateTransform = x.RenderTransform as RotateTransform;
                    double angle = rotateTransform != null ? -rotateTransform.Angle : 0;

                    // Calculate the rotated position of the ball center
                    double radians = Math.PI * angle / 180.0;
                    double rotatedX = Math.Cos(radians) * (ballCenter.X - Canvas.GetLeft(x)) - Math.Sin(radians) * (ballCenter.Y - Canvas.GetTop(x)) + Canvas.GetLeft(x);
                    double rotatedY = Math.Sin(radians) * (ballCenter.X - Canvas.GetLeft(x)) + Math.Cos(radians) * (ballCenter.Y - Canvas.GetTop(x)) + Canvas.GetTop(x);

                    // Create a rotated bounding box for the ball
                    Rect rotatedBallBounds = new Rect(rotatedX - ballBounds.Width / 2, rotatedY - ballBounds.Height / 2, ballBounds.Width, ballBounds.Height);

                    // Check for intersection between the rotated ball and the rotated wall
                    if (rotatedBallBounds.IntersectsWith(new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height)))
                    {
                        if (rotateTransform.Angle == 45)
                        {
                            return 5;
                        }
                        else if (rotateTransform.Angle == 135)
                        {
                            return -5;
                        }
                    }
                }
            }
            return 0;
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
            if (_currentWaterAmount == _capacity - 1)
            {
                return false;
            }
            else
            {
                _currentWaterAmount += 1;
                return true;
            }
            //if (_currentWaterAmount < _capacity)
            //{
            //    _currentWaterAmount += 1;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }

    //public class Wall
    //{
    //    private Rectangle _position;
    //    private double _angle;        

    //    public Wall(Rectangle position, double angle)
    //    {
    //        _position = position;
    //        _angle = angle;           
    //    }
    //    public bool IsColliding(Ellipse ball)
    //    {
    //        return true;
    //    }
    //    public Rectangle GetPosition()
    //    {
    //        return _position;
    //    }        
    //    public double GetAngle()
    //    {
    //        return _angle;
    //    }
    //}
}

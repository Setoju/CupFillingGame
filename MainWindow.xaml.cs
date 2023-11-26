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
            // Add logic for dropping balls from the position of the mouse
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
            // We should call this method every time the ball drops into the cup
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
            Rect ballRect = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
            Rect wallRect = new Rect(Canvas.GetLeft(_position), Canvas.GetTop(_position), _position.Width, _position.Height);

            Point wallRotationCenter = new Point(Canvas.GetLeft(_position) + _position.Width / 2, Canvas.GetTop(_position) + _position.Height / 2);
            RotateTransform wallRotation = new RotateTransform(-_angle, wallRotationCenter.X, wallRotationCenter.Y);           

            // Transform the rotated wall rectangle
            wallRect = wallRotation.TransformBounds(wallRect);


            return ballRect.IntersectsWith(wallRect);
        }
        public Rectangle GetPosition()
        {
            return _position;
        }        
    }
}

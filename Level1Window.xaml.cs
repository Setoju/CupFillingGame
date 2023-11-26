using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CupFilling
{
    /// <summary>
    /// Interaction logic for Level1Window.xaml
    /// </summary>
    public partial class Level1Window : Window
    {
        private List<Wall> walls = new List<Wall>();
        private static PointCollection cupPoints = new PointCollection
            {
                new Point(390, 700),
                new Point(390, 800),
                new Point(310, 800),
                new Point(310, 700)

            };
        private Cup firstLevelCup = new Cup(cupPoints, 10);
        private WaterSource firstWaterSource = new WaterSource(15);
        public Level1Window()
        {
            InitializeComponent();
                                              
            FirstLevelCanvas.Children.Add(firstLevelCup.ShowCup());
            AddWalls();
            FirstLevelCanvas.MouseLeftButtonDown += OnCanvasClick;
        }
        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            CreateFallingBall(e.GetPosition(FirstLevelCanvas));
        }

        private double previousBallTop = 0;

        private void CreateFallingBall(Point clickPosition)
        {
            remainingWaterText.Text = $"Remaining Water: {firstWaterSource.GetWaterAmount()}";

            if (firstWaterSource.GetWaterAmount() < 0)
            {
                MessageBox.Show("You failed");
                this.Close();
            }            

            Ellipse ball = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Blue
            };

            // Set the initial position of the ball to the click position
            Canvas.SetLeft(ball, clickPosition.X - ball.Width / 2);

            // Set the position of the ball above the previous ball
            Canvas.SetTop(ball, previousBallTop);

            FirstLevelCanvas.Children.Add(ball);

            firstWaterSource.ReleaseWater();

            // Start a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5; // Adjust the falling speed as needed
                //!!!                                                        
                // Maybe when the ball is outside of the window we should stop its timer to optimize the game
                //!!!^

                foreach (var wall in walls)
                {
                    if (wall.IsColliding(ball))
                    {                        
                        timer.Stop();
                        return;
                    }
                }

                if (newY + ball.Height >= 800 && Canvas.GetLeft(ball) >= 310 && Canvas.GetLeft(ball) + ball.Width <= 390)
                {
                    if (!firstLevelCup.FillIfNotFull())
                        this.Close();
                    timer.Stop();
                }
                else
                {
                    // Move the ball down                        
                    Canvas.SetTop(ball, newY);
                }
                // Check if the ball has reached the cup                
            };

            timer.Start();

            // Update the previousBallTop for the next ball
            previousBallTop += ball.Height + 1; // You can adjust the gap between balls as needed
        }

        public class Level1 : Level
        {
            Level1Window startLevel1 = new Level1Window();
            public Level1()
            {
                LevelNumber = 1;
            }

            public override void StartLevel()
            {
                startLevel1.Show();
                MessageBox.Show("Level 1 started!");
            }            
        }
        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            Level2Window.Level2 startNext = new Level2Window.Level2();
            this.Close();
            startNext.StartLevel();
        }
        private void AddWalls()
        {            
            Rectangle wallRectangle = new Rectangle
            {
                Width = 100,
                Height = 5,
                Fill = Brushes.Gray
            };

            Canvas.SetLeft(wallRectangle, 200);
            Canvas.SetTop(wallRectangle, 600);

            RotateTransform rotateTransform = new RotateTransform(45); // Set the desired angle
            wallRectangle.RenderTransform = rotateTransform;

            Wall diagonalWall = new Wall(wallRectangle, 45); // Angle in degrees
            walls.Add(diagonalWall);

            FirstLevelCanvas.Children.Add(diagonalWall.GetPosition());
        }
    }
}

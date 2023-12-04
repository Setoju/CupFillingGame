using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CupFilling
{
    /// <summary>
    /// Interaction logic for Level1Window.xaml
    /// </summary>
    public partial class Level1Window : Window
    {
        //private List<Wall> walls = new List<Wall>();
        private bool gameEnded = false;
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
            //AddWalls();
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

            // Seting the initial position of the ball
            Canvas.SetLeft(ball, clickPosition.X - ball.Width / 2);

            // Seting the position of the ball above the previous ball
            Canvas.SetTop(ball, previousBallTop);

            FirstLevelCanvas.Children.Add(ball);

            firstWaterSource.ReleaseWater();

            // Starting a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5; // Adjusting the ball falling speed
                double newX = Canvas.GetLeft(ball);    //!!!                                                        
                                                       // Maybe when the ball is outside of the window we should stop its timer to optimize the game
                                                       //!!!^
                // Checking if the ball is colliding with any wall                                                      
                Rect ballBounds = new Rect(Canvas.GetLeft(ball), Canvas.GetTop(ball), ball.Width, ball.Height);
                foreach (var x in FirstLevelCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)x.Tag == "wall")
                    {
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
                                newX += 5;
                            }
                            else if(rotateTransform.Angle == 135)
                            {
                                newX -= 5;
                            }                            
                            // Assuming you want to stop the timer immediately upon collision
                        }
                    }
                }
                // Checking if the ball has reached the cup 
                if (newY + ball.Height >= 800 && Canvas.GetLeft(ball) >= 310 && Canvas.GetLeft(ball) + ball.Width <= 390)
                {
                    if (!firstLevelCup.FillIfNotFull())
                    {
                        timer.Stop();
                        if (!gameEnded)
                        {
                            gameEnded = true;

                            MessageBoxResult result = MessageBox.Show("Congratulations, you won. Do you want to play the next level?", "Confirmation", MessageBoxButton.YesNo);

                            if (result == MessageBoxResult.Yes)
                            {
                                StartNextLevel();
                            }
                            else
                            {
                                this.Close();
                            }
                        }                        
                    }
                    timer.Stop();
                }
                else
                {
                    // Moving the ball down                        
                    Canvas.SetTop(ball, newY);
                    Canvas.SetLeft(ball, newX);
                }
            };

            timer.Start();

            // ???????
            previousBallTop += ball.Height;
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
            StartNextLevel();
        }
        private void StartNextLevel()
        {
            Level2Window.Level2 startNext = new Level2Window.Level2();
            this.Close();
            startNext.StartLevel();
        }
        // Method for adding walls to the canvas

        //private void AddWalls()
        //{
        //    Rectangle wallRectangle = new Rectangle
        //    {
        //        Width = 100,
        //        Height = 5,
        //        Fill = Brushes.Gray
        //    };

        //    Canvas.SetLeft(wallRectangle, 200);
        //    Canvas.SetTop(wallRectangle, 600);

        //    RotateTransform rotateTransform = new RotateTransform(45); // Wall angle
        //    wallRectangle.RenderTransform = rotateTransform;                        

        //    Wall diagonalWall = new Wall(wallRectangle, 45);
        //    walls.Add(diagonalWall);

        //    foreach (Wall wall in walls)
        //    {
        //        FirstLevelCanvas.Children.Add(wall.GetPosition());
        //    }
        //}                
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
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
        //private List<Wall> walls = new List<Wall>();        
        private bool gameEnded = false;
        private byte imagePointer = 0;
        //private static PointCollection cupPoints = new PointCollection
        //    {
        //        new Point(390, 700),
        //        new Point(390, 800),
        //        new Point(310, 800),
        //        new Point(310, 700)

        //    };
        private Cup firstLevelCup = new Cup(/*cupPoints,*/ 10);
        private WaterSource firstWaterSource = new WaterSource(15);
        public Level1Window()
        {
            InitializeComponent();
                                                 
            //FirstLevelCanvas.Children.Add(firstLevelCup.ShowCup());
            progress.Text = $"Filled: {cupFilling.Value}/10";
            remainingWaterText.Text = $"Remaining Water: 15";
            //AddWalls();
            FirstLevelCanvas.MouseLeftButtonDown += OnCanvasClick;

        }
        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            CreateFallingBall(e.GetPosition(FirstLevelCanvas));
        }        

        private void CreateFallingBall(Point clickPosition)
        {                        
            if (firstWaterSource.GetWaterAmount() == 0)
            {
                gameEnded = true;                
                MessageBox.Show("You failed");                
                this.Close();
            }

            Ellipse ball = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Blue
            };

            // Seting the initial position of the ball
            Canvas.SetLeft(ball, clickPosition.X - ball.Width / 2);            
            Canvas.SetTop(ball, 0);

            FirstLevelCanvas.Children.Add(ball);

            firstWaterSource.ReleaseWater();
            remainingWaterText.Text = $"Remaining Water: {firstWaterSource.GetWaterAmount()}";
            // Starting a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5; // Adjusting the ball falling speed
                double newX = Canvas.GetLeft(ball);

                newX += MainWindow.CollisionCheck(FirstLevelCanvas, ball);

                if (MainWindow.IsBallInTheCup(ball, Cup))
                {
                    timer.Stop();
                    FirstLevelCanvas.Children.Remove(ball);
                    if (imagePointer < 10)
                    {
                        imagePointer++;
                    }                    
                    Cup.Source = new BitmapImage(new Uri($"H:\\Project\\CupFilling\\images\\Cup{imagePointer}.jpg", UriKind.RelativeOrAbsolute));
                    cupFilling.Value += 1;
                    progress.Text = $"Filled: {cupFilling.Value}/10";
                    if (!firstLevelCup.FillIfNotFull())
                    {
                        if (!gameEnded)
                        {                            
                            gameEnded = true;
                            MainWindow.gameCompletion[1] = true;

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
                }                
                else
                {
                    // Moving the ball down
                    if (!gameEnded)
                    {
                        Canvas.SetTop(ball, newY);
                        Canvas.SetLeft(ball, newX);
                    }                    
                }
            };

            timer.Start();          
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
            if (MainWindow.gameCompletion[1])
            {
                buttonClick.Position = TimeSpan.Zero;
                buttonClick.Play();
                StartNextLevel();
            }
            else
            {
                MessageBox.Show("You have to complete this level first!");
            }
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

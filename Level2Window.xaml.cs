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
    /// Interaction logic for Level2Window.xaml
    /// </summary>
    public partial class Level2Window : Window
    {
        private bool gameEnded = false;
        private byte imagePointer = 0;
        //private static PointCollection cupPoints = new PointCollection
        //    {
        //        new Point(500, 700),
        //        new Point(500, 800),
        //        new Point(420, 800),
        //        new Point(420, 700)

        //    };
        private Cup secondLevelCup = new Cup(/*cupPoints,*/ 10);
        private WaterSource secondWaterSource = new WaterSource(15);
        public Level2Window()
        {
            InitializeComponent();
                        
            progress.Text = $"Filled: {cupFilling.Value}/10";
            remainingWaterText.Text = $"Remaining Water: 15";
            SecondLevelCanvas.MouseLeftButtonDown += OnCanvasClick;
        }
        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            CreateFallingBall(e.GetPosition(SecondLevelCanvas));
        }        

        private void CreateFallingBall(Point clickPosition)
        {            
            if (secondWaterSource.GetWaterAmount() == 0)
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

            SecondLevelCanvas.Children.Add(ball);

            secondWaterSource.ReleaseWater();
            remainingWaterText.Text = $"Remaining Water: {secondWaterSource.GetWaterAmount()}";
            // Starting a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5; // Adjusting the falling speed as needed
                double newX = Canvas.GetLeft(ball);
                                                                                    
                newX += MainWindow.CollisionCheck(SecondLevelCanvas, ball);

                // Check if the ball has reached the cup
                if (MainWindow.IsBallInTheCup(ball, Cup))
                {
                    timer.Stop();
                    SecondLevelCanvas.Children.Remove(ball);
                    if (imagePointer < 10)
                    {
                        imagePointer++;
                    }                    
                    Cup.Source = new BitmapImage(new Uri($"H:\\Project\\CupFilling\\images\\Cup{imagePointer}.jpg", UriKind.RelativeOrAbsolute));
                    cupFilling.Value += 1;
                    progress.Text = $"Filled: {cupFilling.Value}/10";
                    if (!secondLevelCup.FillIfNotFull())
                    {
                        if (!gameEnded)
                        {
                            gameEnded = true;
                            MainWindow.gameCompletion[2] = true;

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
        public class Level2 : Level
        {
            Level2Window startLevel2 = new Level2Window();
            public Level2()
            {
                LevelNumber = 2;
            }

            public override void StartLevel()
            {
                startLevel2.Show();
                MessageBox.Show("Level 2 started! But you gotta be carefull, there's a trap ;)");
            }                   
        }
        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.gameCompletion[2])
            {                
                StartNextLevel();
            }
            else
            {
                MessageBox.Show("You have to finish this level first!");
            }
        }
        private void StartNextLevel()
        {
            Level3Window.Level3 startNext = new Level3Window.Level3();
            this.Close();            
            startNext.StartLevel();            
        }
    }
}

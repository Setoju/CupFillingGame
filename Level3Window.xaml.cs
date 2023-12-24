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
    /// Interaction logic for Level3Window.xaml
    /// </summary>
    public partial class Level3Window : Window
    {
        private bool gameEnded = false;
        private byte imagePointer = 0;
        //private static PointCollection cupPoints = new PointCollection
        //    {
        //        new Point(200, 700),
        //        new Point(200, 800),
        //        new Point(120, 800),
        //        new Point(120, 700)

        //    };
        private Cup thirdLevelCup = new Cup(/*cupPoints,*/ 10);
        private WaterSource thirdWaterSource = new WaterSource(15);
        public Level3Window()
        {
            InitializeComponent();

            progress.Text = $"Filled: {cupFilling.Value}/10";
            remainingWaterText.Text = $"Remaining Water: 15";

            ThirdLevelCanvas.MouseLeftButtonDown += OnCanvasClick;
        }
        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            CreateFallingBall(e.GetPosition(ThirdLevelCanvas));
        }   

        private void CreateFallingBall(Point clickPosition)
        {            
            if (thirdWaterSource.GetWaterAmount() == 0)
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

            // Seting the initial position of the ball to the click position
            Canvas.SetLeft(ball, clickPosition.X - ball.Width / 2);
            Canvas.SetTop(ball, 0);

            ThirdLevelCanvas.Children.Add(ball);

            thirdWaterSource.ReleaseWater();
            remainingWaterText.Text = $"Remaining Water: {thirdWaterSource.GetWaterAmount()}";

            // Start a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5; // Adjust the falling speed as needed
                double newX = Canvas.GetLeft(ball);

                newX += MainWindow.CollisionCheck(ThirdLevelCanvas, ball);

                // Check if the ball has reached the cup
                if (MainWindow.IsBallInTheCup(ball, Cup))
                {
                    timer.Stop();
                    ThirdLevelCanvas.Children.Remove(ball);
                    if(imagePointer < 10)
                    {
                        imagePointer++;
                    }                    
                    Cup.Source = new BitmapImage(new Uri($"H:\\Project\\CupFilling\\images\\Cup{imagePointer}.jpg", UriKind.RelativeOrAbsolute));
                    cupFilling.Value += 1;
                    progress.Text = $"Filled: {cupFilling.Value}/10";
                    if (!thirdLevelCup.FillIfNotFull())
                    {
                        if (!gameEnded)
                        {
                            gameEnded = true;
                            MainWindow.gameCompletion[3] = true;

                            MessageBox.Show("Congratulations, you won. Now you can play the bonus level!");
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
        public class Level3 : Level
        {
            Level3Window startLevel3 = new Level3Window();
            public Level3()
            {
                LevelNumber = 3;
            }

            public override void StartLevel()
            {
                startLevel3.Show();
                MessageBox.Show("Level 3 started!");
            }
        }        
    }
}

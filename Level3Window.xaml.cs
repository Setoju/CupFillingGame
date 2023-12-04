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
    /// Interaction logic for Level3Window.xaml
    /// </summary>
    public partial class Level3Window : Window
    {
        private bool gameEnded = false;
        private static PointCollection cupPoints = new PointCollection
            {
                new Point(200, 700),
                new Point(200, 800),
                new Point(120, 800),
                new Point(120, 700)

            };
        private Cup thirdLevelCup = new Cup(cupPoints, 10);
        private WaterSource thirdWaterSource = new WaterSource(15);
        public Level3Window()
        {
            InitializeComponent();                       

            ThirdLevelCanvas.Children.Add(thirdLevelCup.ShowCup());

            ThirdLevelCanvas.MouseLeftButtonDown += OnCanvasClick;
        }
        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            CreateFallingBall(e.GetPosition(ThirdLevelCanvas));
        }   

        private void CreateFallingBall(Point clickPosition)
        {
            remainingWaterText.Text = $"Remaining Water: {thirdWaterSource.GetWaterAmount()}";

            if (thirdWaterSource.GetWaterAmount() == 0)
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

            // Seting the initial position of the ball to the click position
            Canvas.SetLeft(ball, clickPosition.X - ball.Width / 2);
            Canvas.SetTop(ball, 0);

            ThirdLevelCanvas.Children.Add(ball);

            thirdWaterSource.ReleaseWater();

            // Start a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5; // Adjust the falling speed as needed
                double newX = Canvas.GetLeft(ball);

                newX += MainWindow.CollisionCheck(ThirdLevelCanvas, ball);

                // Check if the ball has reached the cup
                if (newY + ball.Height >= 800 && Canvas.GetLeft(ball) >= 120 && Canvas.GetLeft(ball) + ball.Width <= 200)
                {
                    // Ball is inside the cup, stop the timer and remove the ball
                    if (!thirdLevelCup.FillIfNotFull())
                    {
                        if (!gameEnded)
                        {
                            gameEnded = true;

                            MessageBox.Show("Congratulation, you've completed all levels!!! Go treat yourself ;)");
                        }
                    }                                          
                }
                else
                {
                    // Move the ball down
                    Canvas.SetTop(ball, newY);
                    Canvas.SetLeft(ball, newX);
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

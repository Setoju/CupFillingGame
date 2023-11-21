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
    /// Interaction logic for Level2Window.xaml
    /// </summary>
    public partial class Level2Window : Window
    {
        private static PointCollection cupPoints = new PointCollection
            {
                new Point(500, 700),
                new Point(500, 800),
                new Point(420, 800),
                new Point(420, 700)

            };
        private Cup secondLevelCup = new Cup(cupPoints, 10);
        private WaterSource secondWaterSource = new WaterSource(15);
        public Level2Window()
        {
            InitializeComponent();
            
            SecondLevelCanvas.Children.Add(secondLevelCup.ShowCup());

            SecondLevelCanvas.MouseLeftButtonDown += OnCanvasClick;
        }
        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            CreateFallingBall(e.GetPosition(SecondLevelCanvas));
        }

        private double previousBallTop = 0;

        private void CreateFallingBall(Point clickPosition)
        {
            remainingWaterText.Text = $"Remaining Water: {secondWaterSource.GetWaterAmount()}";

            if (secondWaterSource.GetWaterAmount() == 0)
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

            SecondLevelCanvas.Children.Add(ball);

            secondWaterSource.ReleaseWater();

            // Start a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 20; // Adjust the falling speed as needed

                // Check if the ball has reached the cup
                if (newY + ball.Height >= 800 && Canvas.GetLeft(ball) >= 420 && Canvas.GetLeft(ball) + ball.Width <= 500)
                {
                    // Ball is inside the cup, stop the timer and remove the ball
                    if (!secondLevelCup.FillIfNotFull())
                        this.Close();
                    timer.Stop();
                }
                else
                {
                    // Move the ball down
                    Canvas.SetTop(ball, newY);
                }
            };

            timer.Start();

            // Update the previousBallTop for the next ball
            previousBallTop += ball.Height + 1; // You can adjust the gap between balls as needed
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
                MessageBox.Show("Level 2 started!");
            }                   
        }
        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            Level3Window.Level3 startNext = new Level3Window.Level3();
            this.Close();
            startNext.StartLevel();
        }
    }
}

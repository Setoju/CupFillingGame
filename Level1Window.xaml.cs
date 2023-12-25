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
        private bool gameEnded = false;
        private byte imagePointer = 0;        
        private Cup firstLevelCup = new Cup(10);
        private WaterSource firstWaterSource = new WaterSource(15);
        public Level1Window()
        {
            InitializeComponent();
                                                             
            progress.Text = $"Filled: {cupFilling.Value}/10";
            remainingWaterText.Text = $"Remaining Water: 15";
           
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

            // Playing the sound of placing the water
            placingWater.Position = TimeSpan.Zero;
            placingWater.Play();

            remainingWaterText.Text = $"Remaining Water: {firstWaterSource.GetWaterAmount()}";
            // Starting a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5;
                double newX = Canvas.GetLeft(ball);

                // Checking for collisions with walls
                newX += MainWindow.CollisionCheck(FirstLevelCanvas, ball);

                // Checking if the ball has reached the cup
                if (MainWindow.IsBallInTheCup(ball, Cup))
                {
                    // Playing the sound of the water falling in the cup
                    waterDrop.Position = TimeSpan.Zero;
                    waterDrop.Play();

                    timer.Stop();
                    FirstLevelCanvas.Children.Remove(ball);
                    // Changing the image of the cup to create "filling" animation
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
                            levelFinished.Position = TimeSpan.Zero;
                            levelFinished.Play();

                            MessageBox.Show("Congratulations, you won.");
                            this.Close();
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
    }
}

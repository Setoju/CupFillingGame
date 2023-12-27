using System;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Security.Cryptography;

namespace CupFilling
{
    /// <summary>
    /// Interaction logic for BonusLevelWindow.xaml
    /// </summary>
    public partial class BonusLevelWindow : Window
    {
        private bool gameEnded = false;
        private byte imagePointer = 0;
        private Cup bonusLevelCup = new Cup(10);     
        private Random rnd = new Random();
        private Timer randomTimer = new Timer();              
        public BonusLevelWindow()
        {
            InitializeComponent();            

            progress.Text = $"Filled: {cupFilling.Value}/10";

            randomTimer.Elapsed += RandomTimerElapsed;
            SetRandomInterval();            
        }
        // Placing the water at random intervals using CryptoServiceProvider
        private void RandomTimerElapsed(object sender, ElapsedEventArgs e)
        {            
            Dispatcher.Invoke(() =>
            {
                CreateFallingBall();                
            });
            
            SetRandomInterval();
        }
        private void SetRandomInterval()
        {            
            int interval = rnd.Next(1000, 2000);
            randomTimer.Interval = interval;
            randomTimer.Start();
        }
        // Checking for pressing the mouse on the cup to move it by x axis        
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {                
                Canvas.SetLeft(Cup, e.GetPosition(BonusLevelCanvas).X);                
            }
        }
       
        // Getting random placements using CryptoServiceProvider that runs longer than Random.Next, but ensures the randomness
        private static RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();

        public int GetRandomNumber(int minValue, int maxValue)
        {
            byte[] randomNumber = new byte[4];
            rngCrypto.GetBytes(randomNumber);

            int result = BitConverter.ToInt32(randomNumber, 0);
            return Math.Abs(result % (maxValue - minValue)) + minValue;
        }
        private void CreateFallingBall()
        {            
            Ellipse ball = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Blue
            };

            // Seting the initial position of the ball
            Canvas.SetLeft(ball, GetRandomNumber(10, 1000));
            Canvas.SetTop(ball, 0);

            BonusLevelCanvas.Children.Add(ball);            
            // Starting a DispatcherTimer to animate the falling of the ball
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };

            timer.Tick += (s, e) =>
            {
                double newY = Canvas.GetTop(ball) + 5;
                double newX = Canvas.GetLeft(ball);

                newX += MainWindow.CollisionCheck(BonusLevelCanvas, ball);

                if (MainWindow.IsBallInTheCup(ball, Cup))
                {
                    waterDrop.Position = TimeSpan.Zero;
                    waterDrop.Play();

                    timer.Stop();
                    BonusLevelCanvas.Children.Remove(ball);
                    if (imagePointer < 10)
                    {
                        imagePointer++;
                    }
                    Cup.Source = new BitmapImage(new Uri($"H:\\Project\\CupFilling\\images\\Cup{imagePointer}.jpg", UriKind.RelativeOrAbsolute));
                    cupFilling.Value += 1;
                    progress.Text = $"Filled: {cupFilling.Value}/10";
                    if (!bonusLevelCup.FillIfNotFull())
                    {
                        if (!gameEnded)
                        {
                            gameEnded = true;
                            levelFinished.Position = TimeSpan.Zero;
                            levelFinished.Play();

                            MessageBox.Show("Congratulations, you've completed the game, now go treat yourself ;)");
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

        public class BonusLevel : Level
        {
            BonusLevelWindow startBonusLevel = new BonusLevelWindow();
            public BonusLevel()
            {
                LevelNumber = 0;
            }

            public override void StartLevel()
            {
                startBonusLevel.Show();
                MessageBox.Show("Bonus level started!");
            }
        }
    }
}

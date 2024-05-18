using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyGame
{
    public partial class Form1 : Form
    {
        //Player Properties
        bool isMovingLeft, isMovingRight;
        int playerMoveSpeed = 10;
        bool isShooting;

        //Enemy Properties
        int enemyMoveSpeed = 4;
        int enemyBulletTimer = 300;
        PictureBox[] enemiesArray;
        int numberOfEnemies = 30;
        int enemyOffsetTop = 40;
        int enemyOffsetLeft = 20;

        //Other
        bool isGameOver;
        int score = 0;
        int screenWidth = 820;
        int screenHeight = 620;

        public Form1()
        {
            InitializeComponent();
            gameInitialization();
        }

        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = "Score: " + score;

            //Player moving
            if (isMovingLeft && player.Left > 0)
            {
                player.Left -= playerMoveSpeed;
            }
            if (isMovingRight && player.Left < this.ClientSize.Width - player.Width)
            {
                player.Left += playerMoveSpeed;
            }

            //Enemy bullets cooldown
            enemyBulletTimer -= 10;
            if (enemyBulletTimer < 1)
            {
                enemyBulletTimer = 300;
                bulletInit("enemyBullet");
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "enemy")
                {
                    //Enemy moving to the screen side
                    x.Left += enemyMoveSpeed;
                    if (x.Left > screenWidth)
                    {
                        x.Top += enemyOffsetTop;
                        x.Left = enemyOffsetLeft;
                    }
                    //Enemy collides with Player
                    if (x.Bounds.IntersectsWith(player.Bounds))
                    {
                        gameOver("Przegrałeś, spróbuj ponownie klikając Escape.");
                    }

                    //If enemy is shoot
                    foreach (Control y in this.Controls)
                    {
                        if (y is PictureBox && (string)y.Tag == "bullet")
                        {
                            if (y.Bounds.IntersectsWith(x.Bounds))
                            {
                                this.Controls.Remove(x);
                                this.Controls.Remove(y);
                                score += 1;
                                isShooting = false;
                            }
                        }
                    }
                }

                //If bullet ofscreen
                if (x is PictureBox && (string)x.Tag == "bullet")
                {
                    x.Top -= 20;

                    if (x.Top < 15)
                    {
                        this.Controls.Remove(x);
                        isShooting = false;
                    }
                }

                //If enemy bullet collides with player
                if (x is PictureBox && (string)x.Tag == "enemyBullet")
                {
                    x.Top += 20;

                    if (x.Top > screenHeight)
                    {
                        this.Controls.Remove(x);
                    }

                    if (x.Bounds.IntersectsWith(player.Bounds))
                    {
                        this.Controls.Remove(x);
                        gameOver("Przegrałeś, spróbuj ponownie klikając Escape.");
                    }
                }
            }

            //If half enemies killed, then speed them up
            if (score > enemiesArray.Length/2)
            {
                enemyMoveSpeed = 8;
            }

            //If have last enemies
            if (score > enemiesArray.Length/1.2)
            {
                enemyMoveSpeed = 12;
            }

            //If killed all enemies then won game
            if (score == enemiesArray.Length)
            {
                gameOver("Wygrałeś, ale zagraj ponownie klikając Escape.");
            }
        }

        //If keys pressed
        private void keyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                isMovingLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                isMovingRight = true;
            }
            if (e.KeyCode == Keys.Space && !isShooting)
            {
                isShooting = true;
                bulletInit("bullet");
            }
        }

        //If keys released
        private void keyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                isMovingLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                isMovingRight = false;
            }
            if (e.KeyCode == Keys.Space)
            {
                isShooting = false;
            }
            if (e.KeyCode == Keys.Escape && isGameOver == true)
            {
                removeAllComponents();
                gameInitialization();
            }
        }

        //Init new game
        private void gameInitialization()
        {
            score = 0;
            txtScore.Text = "Score: " + score;
            isGameOver = false;
            enemyBulletTimer = 300;
            enemyMoveSpeed = 5;
            isShooting = false;

            makeEnemies();
            gameTimer.Start();
        }

        private void gameOver(string message)
        {
            isGameOver = true;
            gameTimer.Stop();
            txtScore.Text = "Score: " + score + " " + message;
        }

        private void removeAllComponents()
        {
            //Remove enemies
            foreach (PictureBox i in enemiesArray)
            {
                this.Controls.Remove(i);
            }

            //Remove bullets
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox)
                {
                    if ((string)x.Tag == "bullet" || (string)x.Tag == "enemyBullet")
                    {
                        this.Controls.Remove(x);
                    }
                }
            }
        }

        private void makeEnemies()
        {
            enemiesArray = new PictureBox[numberOfEnemies];

            int left = 0;
            int width = 32;
            int height = 32;
            int positionOffset = 40;

            for (int i = 0; i < enemiesArray.Length; i++)
            {
                enemiesArray[i] = new PictureBox();
                enemiesArray[i].Size = new Size(width, height);
                enemiesArray[i].Image = Properties.Resources.Enemy2;
                enemiesArray[i].Top = 5;
                enemiesArray[i].Tag = "enemy";
                enemiesArray[i].Left = left;
                enemiesArray[i].SizeMode = PictureBoxSizeMode.AutoSize;
                this.Controls.Add(enemiesArray[i]);
                left = left - positionOffset;
            }
        }

        private void bulletInit(string bulletTag)
        {
            int width = 8;
            int height = 8;
            PictureBox bullet = new PictureBox();
            bullet.Image = Properties.Resources.Bullet;
            bullet.Size = new Size(width, height);
            bullet.Left = player.Left + player.Width / 2 - bullet.Width / 2;
            bullet.Tag = bulletTag;

            if (bulletTag == "bullet")
            {
                bullet.Top = player.Top - bullet.Height;
            }
            else if (bulletTag == "enemyBullet")
            {
                bullet.Top = -100;
            }

            this.Controls.Add(bullet);
            bullet.BringToFront();
        }
    }
}

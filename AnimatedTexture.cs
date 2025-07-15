using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarSmuggler
{
    /// <summary>
    /// A helper class for handling animated textures in horizontal strip layout.
    /// </summary>
    public class AnimatedTexture
    {
        // Number of frames in the animation.
        private int frameCount;
        
        // The animation spritesheet.
        private Texture2D myTexture;
        
        // Public property to access texture dimensions
        public Texture2D Texture => myTexture;
        
        // The number of frames to draw per second.
        private float timePerFrame;
        
        // The current frame being drawn.
        private int frame;
        
        // Total amount of time the animation has been running.
        private float totalElapsed;
        
        // Is the animation currently running?
        private bool isPaused;
        
        // Is the animation set to loop?
        private bool isLooping;

        // The current rotation, scale and draw depth for the animation.
        public float Rotation, Scale, Depth;
        
        // The origin point of the animated texture.
        public Vector2 Origin;
        
        // Animation completion callback
        public System.Action OnAnimationComplete;

        public AnimatedTexture(Vector2 origin, float rotation = 0f, float scale = 1f, float depth = 0f, bool isLooping = true)
        {
            this.Origin = origin;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Depth = depth;
            this.isLooping = isLooping;
        }

        public void Load(ContentManager content, string asset, int frameCount, int framesPerSec)
        {
            this.frameCount = frameCount;
            myTexture = content.Load<Texture2D>(asset);
            timePerFrame = (float)1 / framesPerSec;
            frame = 0;
            totalElapsed = 0;
            isPaused = false;
        }

        public void UpdateFrame(float elapsed)
        {
            if (isPaused)
                return;
                
            totalElapsed += elapsed;
            if (totalElapsed > timePerFrame)
            {
                frame++;
                totalElapsed -= timePerFrame;
                
                // Handle animation completion
                if (frame >= frameCount)
                {
                    if (isLooping)
                    {
                        frame = 0; // Loop back to start
                    }
                    else
                    {
                        frame = frameCount - 1; // Stay on last frame
                        isPaused = true; // Stop animation
                        OnAnimationComplete?.Invoke(); // Call completion callback
                    }
                }
            }
        }

        public void DrawFrame(SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame(batch, frame, screenPos);
        }

        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
        {
            int frameWidth = myTexture.Width / frameCount;
            Rectangle sourcerect = new Rectangle(frameWidth * frame, 0,
                frameWidth, myTexture.Height);
            batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                Rotation, Origin, Scale, SpriteEffects.None, Depth);
        }
        
        // Draw the animation to fill the entire screen
        public void DrawFullScreen(SpriteBatch batch, GraphicsDevice graphics)
        {
            int frameWidth = myTexture.Width / frameCount;
            Rectangle sourcerect = new Rectangle(frameWidth * frame, 0,
                frameWidth, myTexture.Height);
            Rectangle destinationRect = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
            batch.Draw(myTexture, destinationRect, sourcerect, Color.White,
                Rotation, Vector2.Zero, SpriteEffects.None, Depth);
        }

        public bool IsPaused
        {
            get { return isPaused; }
        }
        
        public bool IsComplete
        {
            get { return !isLooping && frame >= frameCount - 1; }
        }

        public void Reset()
        {
            frame = 0;
            totalElapsed = 0f;
            isPaused = false;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Play()
        {
            isPaused = false;
        }

        public void Pause()
        {
            isPaused = true;
        }
    }
}

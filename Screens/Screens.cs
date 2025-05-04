using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarSmuggler.Screens
{
    public interface IScreen
    {
        void LoadContent(GraphicsDevice graphics, ContentManager content);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);

        void Refresh(ContentManager content); 
    }
}

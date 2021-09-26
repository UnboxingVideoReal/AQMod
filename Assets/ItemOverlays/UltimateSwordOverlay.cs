﻿using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Assets.ItemOverlays
{
    public class UltimateSwordOverlay : ItemOverlay
    {
        public override void DrawHeld(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            Texture2D texture = SpriteUtils.Textures.Glows[GlowID.UltimateSword];
            var drawColor = new Color(128, 128, 128, 0);

            if (player.gravDir == -1f)
            {
                DrawData drawData = new DrawData(texture, new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y)), new Rectangle(0, 0, texture.Width, texture.Height), item.GetAlpha(drawColor), player.itemRotation, new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f), item.scale, info.spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
                return;
            }

            var swordOrigin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
            var drawPosition = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            drawColor = item.GetAlpha(drawColor);
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, drawColor, player.itemRotation, swordOrigin, item.scale, info.spriteEffects, 0));
            texture = AQTextureAssets.GetItem(item.type);
            float x = (float)Math.Sin(Main.GlobalTime / 2f) * 4f;
            drawColor *= 0.5f;
            Main.playerDrawData.Add(new DrawData(texture, drawPosition + new Vector2(x, 0f), drawFrame, drawColor, player.itemRotation, swordOrigin, item.scale, info.spriteEffects, 0));
            Main.playerDrawData.Add(new DrawData(texture, drawPosition + new Vector2(-x, 0f), drawFrame, drawColor, player.itemRotation, swordOrigin, item.scale, info.spriteEffects, 0));
        }

        public override void DrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var drawColor = new Color(128, 128, 128, 0);
            var texture = SpriteUtils.Textures.Glows[GlowID.UltimateSword];
            Vector2 drawPosition = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            var origin = Main.itemTexture[item.type].Size() / 2;
            Main.spriteBatch.Draw(texture, drawPosition, null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);

            float x = (float)Math.Sin(Main.GlobalTime / 2f) * 4f;
            drawColor *= 0.25f;
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(x, 0f), null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(-x, 0f), null, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
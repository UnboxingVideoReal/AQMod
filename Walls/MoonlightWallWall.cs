﻿using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Walls
{
    public class MoonlightWallWall : ModWall
    {
        public static Color GetColor(float value = 0f)
        {
            return new Color(128, 128, 128, 10) * ((float)Math.Sin((Main.GlobalTime + value * 0.35f) * 2f) * 0.2f + 0.8f);
        }

        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            Main.wallLight[Type] = true;
            dustType = 203;
            drop = ModContent.ItemType<Items.Placeable.Walls.MoonlightWall>();
            AddMapEntry(new Color(12, 12, 12));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            var c = GetColor(i + j) * 0.25f;
            r = c.R / 255f;
            g = c.G / 255f;
            b = c.B / 255f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var texture = SpriteUtils.Textures.Glows[GlowID.MoonlightWallWall];
            DrawMethods.DrawWall(i, j, texture, GetColor(i + j));
        }
    }
}
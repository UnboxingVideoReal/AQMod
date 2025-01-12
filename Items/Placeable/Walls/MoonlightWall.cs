﻿using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Walls
{
    public class MoonlightWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new DynamicGlowmask(GlowID.MoonlightWall, getColor), item.type);
        }

        private static Color getColor() => MoonlightWallWall.GetColor(0f);

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTime = 7;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createWall = ModContent.WallType<MoonlightWallWall>();
            item.consumable = true;
            item.autoReuse = true;
            item.useTurn = true;
        }

        public override void CaughtFishStack(ref int stack)
        {
            stack = Main.rand.Next(80, stack + 120);
        }
    }

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
            drop = ModContent.ItemType<MoonlightWall>();
            AddMapEntry(new Color(12, 12, 48));
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
            var texture = DrawUtils.Textures.Glows[GlowID.MoonlightWallWall];
            DrawMethods.DrawWall(i, j, texture, GetColor(i + j));
        }
    }
}
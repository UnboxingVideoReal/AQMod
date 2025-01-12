﻿using AQMod.Assets;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod.Common.Utilities
{
    internal static class AQUtils
    {
        public const int UI_SIZE = 10;

        private static MemberInfoCache Cache { get; set; }

        public static void Setup()
        {
            Cache = new MemberInfoCache();
        }

        public static void Unload()
        {
            Cache = null;
        }

        private class MemberInfoCache
        {
            private FieldInfo _armorShaderData_uImage;
            public FieldInfo ArmorShaderData_uImage
            {
                get
                {
                    if (_armorShaderData_uImage != null)
                        return _armorShaderData_uImage;
                    else
                    {
                        return _armorShaderData_uImage = typeof(ArmorShaderData).GetField("_uImage", BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                }
            }
        }

        public static Point FluffizePoint(Point point, int fluff = 10)
        {
            point.Fluffize(fluff);
            return point;
        }

        public static void Fluffize(this ref Point point, int fluff = 10)
        {
            if (point.X < fluff)
            {
                point.X = fluff;
            }
            else if (point.X > Main.maxTilesX - fluff)
            {
                point.X = Main.maxTilesX - fluff;
            }
            if (point.Y < fluff)
            {
                point.Y = fluff;
            }
            else if (point.Y > Main.maxTilesY - fluff)
            {
                point.Y = Main.maxTilesY - fluff;
            }
        }

        public static Vector2 GetSwordTipOffset(Player player, Item item)
        {
            return new Vector2(item.width * player.direction, -item.height * player.gravDir).RotatedBy(player.itemRotation + player.fullRotation) * item.scale;
        }

        public static string AddZerosToUnreachedDigits(int number, int zerosCount)
        {
            if (number == 0)
            {
                string zeros = "";
                for (int i = 0; i < zerosCount; i++)
                {
                    zeros += "0";
                }
                return zeros;
            }
            int digits = number / 10 + 1;
            string text = "";
            for (int i = 0; i < zerosCount - digits; i++)
            {
                text += "0";
            }
            text += number.ToString();
            return text;
        }

        public static string TimeText2(double time)
        {
            int seconds = (int)(time / 60);
            int minutes = seconds / 60;
            seconds %= 60;
            int hours = minutes / 60;
            minutes %= 60;
            return AddZerosToUnreachedDigits(hours, 2) + ":" + AddZerosToUnreachedDigits(minutes, 2) + ":" + AddZerosToUnreachedDigits(seconds, 2);
        }

        public static string TimeText3(double time)
        {
            int seconds = (int)(time / 60);
            int minutes = seconds / 60;
            seconds %= 60;
            return AddZerosToUnreachedDigits(minutes, 2) + ":" + AddZerosToUnreachedDigits(seconds, 2);
        }

        public static string TimeText(double time)
        {
            string text = "AM";
            if (!Main.dayTime)
                time += 54000.0;
            time = time / 86400.0 * 24.0;
            time = time - 7.5 - 12.0;
            if (time < 0.0)
                time += 24.0;
            if (time >= 12.0)
                text = "PM";
            int intTime = (int)time;
            double deltaTime = time - intTime;
            deltaTime = (int)(deltaTime * 60.0);
            string text2 = string.Concat(deltaTime);
            if (deltaTime < 10.0)
                text2 = "0" + text2;
            if (intTime > 12)
                intTime -= 12;
            if (intTime == 0)
                intTime = 12;
            return string.Concat(intTime, ":", text2, " ", text);
        }

        public static int GetIntOrDefault(this TagCompound tag, string key, int defaultValue)
        {
            if (tag.ContainsKey(key))
                return tag.GetInt(key);
            return defaultValue;
        }

        public static ArmorShaderData UseTexture2D(this ArmorShaderData a, Texture2D texture)
        {
            Cache.ArmorShaderData_uImage.SetValue(a, new Ref<Texture2D>(texture));
            return a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments">Index 0 is the origin vector</param>
        /// <param name="distanceBetweenSegments">The distance between each point</param>
        /// <param name="target">The target position for the segment chain</param>
        public static Vector3[] Fabrik3D(this Vector3[] segments, float distanceBetweenSegments, Vector3 target)
        {
            var origin = segments[0];
            segments[segments.Length - 1] = target;
            for (int i = segments.Length - 1; i > 0; i--)
            {
                segments[i - 1] = segments[i] + Vector3.Normalize(segments[i - 1] - segments[i]) * distanceBetweenSegments;
            }
            segments[0] = origin;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                segments[i + 1] = segments[i] + Vector3.Normalize(segments[i + 1] - segments[i]) * distanceBetweenSegments;
            }
            return segments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments">Index 0 is the origin vector</param>
        /// <param name="distanceBetweenSegments">The distance between each point</param>
        /// <param name="target">The target position for the segment chain</param>
        public static Vector2[] Fabrik2D(this Vector2[] segments, float distanceBetweenSegments, Vector2 target)
        {
            var origin = segments[0];
            segments[segments.Length - 1] = target;
            for (int i = segments.Length - 1; i > 0; i--)
            {
                segments[i - 1] = segments[i] + Vector2.Normalize(segments[i - 1] - segments[i]) * distanceBetweenSegments;
            }
            segments[0] = origin;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                segments[i + 1] = segments[i] + Vector2.Normalize(segments[i + 1] - segments[i]) * distanceBetweenSegments;
            }
            return segments;
        }

        public static string KnockbackItemTooltip(float knockback)
        {
            if (knockback == 0f)
                return Lang.tip[14].Value;
            else if (knockback <= 1.5)
            {
                return Lang.tip[15].Value;
            }
            else if (knockback <= 3f)
            {
                return Lang.tip[16].Value;
            }
            else if (knockback <= 4f)
            {
                return Lang.tip[17].Value;
            }
            else if (knockback <= 6f)
            {
                return Lang.tip[18].Value;
            }
            else if (knockback <= 7f)
            {
                return Lang.tip[19].Value;
            }
            else if (knockback <= 9f)
            {
                return Lang.tip[20].Value;
            }
            else if (knockback <= 11f)
            {
                return Lang.tip[21].Value;
            }
            return Lang.tip[22].Value;
        }

        public static Color colorLerps(Color[] colors, float time)
        {
            int index = (int)time;
            return Color.Lerp(colors[index % colors.Length], colors[(index + 1) % colors.Length], time % 1f);
        }

        public static void UpdatePlayerVisualAccessories(Item item, Player player)
        {
            if (item.handOnSlot > 0)
                player.handon = item.handOnSlot;
            if (item.handOffSlot > 0)
                player.handoff = item.handOffSlot;
            if (item.backSlot > 0)
                player.back = item.backSlot;
            if (item.frontSlot > 0)
                player.front = item.frontSlot;
            if (item.shoeSlot > 0)
                player.shoe = item.shoeSlot;
            if (item.waistSlot > 0)
                player.waist = item.waistSlot;
            if (item.shieldSlot > 0)
                player.shield = item.shieldSlot;
            if (item.neckSlot > 0)
                player.neck = item.neckSlot;
            if (item.faceSlot > 0)
                player.face = item.faceSlot;
            if (item.balloonSlot > 0)
                player.balloon = item.balloonSlot;
            if (item.wingSlot > 0)
                player.wings = item.wingSlot;
        }

        public static void UpdatePlayerVisualAccessoriesDyes(Item item, Item dye, Player player)
        {
            if (item.handOnSlot > 0)
                player.cHandOn = dye.dye;
            if (item.handOffSlot > 0)
                player.cHandOff = dye.dye;
            if (item.backSlot > 0)
                player.cBack = dye.dye;
            if (item.frontSlot > 0)
                player.cFront = dye.dye;
            if (item.shoeSlot > 0)
                player.cShoe = dye.dye;
            if (item.waistSlot > 0)
                player.cWaist = dye.dye;
            if (item.shieldSlot > 0)
                player.cShield = dye.dye;
            if (item.neckSlot > 0)
                player.cNeck = dye.dye;
            if (item.faceSlot > 0)
                player.cFace = dye.dye;
            if (item.balloonSlot > 0)
                player.cBalloon = dye.dye;
            if (item.wingSlot > 0)
                player.cWings = dye.dye;
            if (item.type == ItemID.FlyingCarpet)
                player.cCarpet = dye.dye;
        }

        public static bool CanNPCBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (npc.dontTakeDamage || (projectile.usesLocalNPCImmunity || projectile.usesIDStaticNPCImmunity) && (!projectile.usesLocalNPCImmunity || projectile.localNPCImmunity[npc.whoAmI] != 0) && (!projectile.usesIDStaticNPCImmunity || !Projectile.IsNPCImmune(projectile.type, npc.whoAmI)))
                return false;
            return true;
        }

        public static void Sort<T>(ref T[] array, Comparison<T> comparison)
        {
            List<T> arrayAsList = new List<T>(array);
            arrayAsList.Sort(comparison);
            array = arrayAsList.ToArray();
        }

        public static void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            var difference = end - start;
            Main.spriteBatch.Draw(TextureCache.Pixel.Value, start, null, color, difference.ToRotation() - MathHelper.PiOver2, new Vector2(0.5f, 0f), new Vector2(width, difference.Length()), SpriteEffects.None, 0f);
        }

        public static T[][] CreateSameLengthArrayArray<T>(int length1, int length2)
        {
            var array = new T[length1][];
            for (int i = 0; i < length1; i++)
            {
                array[i] = new T[length2];
            }
            return array;
        }

        public static byte[] ObjectToByteArray(this object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static string GetPath(ModItem m)
        {
            return GetPath(m.GetType()) + "/" + m.Name;
        }

        public static string GetPath(Type t)
        {
            return t.Namespace.Replace('.', '/');
        }

        public static bool Check2x2ThenCut(int x, int y)
        {
            if ((!Framing.GetTileSafely(x, y).active() || Main.tileCut[Main.tile[x, y].type]) &&
                (!Framing.GetTileSafely(x + 1, y).active() || Main.tileCut[Main.tile[x + 1, y].type]) &&
                (!Framing.GetTileSafely(x, y + 1).active() || Main.tileCut[Main.tile[x, y + 1].type]) &&
                (!Framing.GetTileSafely(x + 1, y + 1).active() || Main.tileCut[Main.tile[x + 1, y + 1].type]))
            {
                WorldGen.KillTile(x, y);
                WorldGen.KillTile(x + 1, y);
                WorldGen.KillTile(x, y + 1);
                WorldGen.KillTile(x + 1, y + 1);
                return true;
            }
            return false;
        }

        public static bool CanReach_IgnoreItemTileBoost(this Player player)
            => !player.noBuilding && player.position.X / 16f - Player.tileRangeX - player.blockRange <= Player.tileTargetX && (player.position.X + player.width) / 16f + Player.tileRangeX - 1f + player.blockRange >= Player.tileTargetX && player.position.Y / 16f - Player.tileRangeY - player.blockRange <= Player.tileTargetY && (player.position.Y + player.height) / 16f + Player.tileRangeY + 2f + player.blockRange >= Player.tileTargetY;

        public static bool IsCloseEnoughTo(this float comparison, float intendedValue, float closeEnoughMargin = 1f)
        {
            return (comparison - intendedValue).Abs() <= closeEnoughMargin;
        }

        public static string Info_GetDepthMeter(float worldY)
        {
            int depth = (int)(worldY * 2f / 16f - Main.worldSurface * 2.0);
            float num17 = Main.maxTilesX / 4200;
            num17 *= num17;
            int num18 = 1200;
            float space = (float)(((Main.screenPosition.Y + Main.screenHeight / 2) / 16f - (65f + 10f * num17)) / (Main.worldSurface / 5.0));
            string textValue = worldY > (Main.maxTilesY - 204) * 16 ? Language.GetTextValue("GameUI.LayerUnderworld") : worldY > Main.rockLayer * 16.0 + num18 / 2 + 16.0 ? Language.GetTextValue("GameUI.LayerCaverns") : depth > 0 ? Language.GetTextValue("GameUI.LayerUnderground") : !(space >= 1f) ? Language.GetTextValue("GameUI.LayerSpace") : Language.GetTextValue("GameUI.LayerSurface");
            depth = Math.Abs(depth);
            return (depth != 0 ? Language.GetTextValue("GameUI.Depth", depth) : Language.GetTextValue("GameUI.DepthLevel")) + " " + textValue;
        }

        public static string Info_GetCompass(float worldX)
        {
            int x = (int)(worldX * 2f / 16f - Main.maxTilesX);
            return x > 0 ? Language.GetTextValue("GameUI.CompassEast", x) : x >= 0 ? Language.GetTextValue("GameUI.CompassCenter") : Language.GetTextValue("GameUI.CompassWest", -x);
        }

        public static Color ToColor(this Vector4 value)
        {
            return new Color(value.X, value.Y, value.Z, value.W);
        }

        public static bool HasDoubleJumpLeft(this Player player)
        {
            return player.jumpAgainCloud || player.jumpAgainBlizzard || player.jumpAgainSandstorm || player.jumpAgainFart || player.jumpAgainSail || player.jumpAgainUnicorn;
        }

        public static bool IsMinion(this Projectile projectile)
        {
            return projectile.minion && !ProjectileID.Sets.MinionShot[projectile.type] && !ProjectileID.Sets.SentryShot[projectile.type] && !projectile.sentry && ProjectileID.Sets.MinionSacrificable[projectile.type] && projectile.minionSlots > 0f && Main.projPet[projectile.type];
        }

        public static Point Get2x2_16x16FrameTopLeft(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX % 36 != 0)
                i--;
            if (tile.frameY != 0)
                j--;
            return new Point(i, j);
        }

        public static Color GetItemRarityColor(int itemID)
        {
            var item = new Item();
            item.SetDefaults(itemID);
            return GetItemRarityColor(item);
        }

        public static Color GetItemRarityColor(Item item)
        {
            return InvokeTooltipLineAndGetColor("ItemName", item.type).GetValueOrDefault(GetRarityColor(item.rare));
        }

        public static Color? InvokeTooltipLineAndGetColor(string lineName, int itemID)
        {
            int useless1 = -1;
            int tooltipAmount = 1;
            bool[] useless2 = new bool[] { false, };
            bool[] useless6 = new bool[] { false, };
            string[] useless3 = new string[] { lineName, };
            string[] useless5 = new string[] { lineName, };
            var item = new Item();
            item.SetDefaults(itemID);
            var lines = ItemLoader.ModifyTooltips(item, ref tooltipAmount, useless5, ref useless3, ref useless6, ref useless2, ref useless1, out Color?[] overrideColor);
            for (int i = 0; i < tooltipAmount; i++)
            {
                var t = lines[i];
                if (t.mod == "Terraria" && t.Name == "ItemName")
                    return t.overrideColor != null ? (Color?)(Color)overrideColor[i] : null;
            }
            return null;
        }

        public static Color GetRarityColor(int rarity)
        {
            switch (rarity)
            {
                default:
                return new Color(255, 255, 255, 255);

                case ItemRarityID.Blue:
                return Colors.RarityBlue;

                case ItemRarityID.Green:
                return Colors.RarityGreen;

                case ItemRarityID.Orange:
                return Colors.RarityOrange;

                case ItemRarityID.LightRed:
                return Colors.RarityRed;

                case ItemRarityID.Pink:
                return Colors.RarityPink;

                case ItemRarityID.LightPurple:
                return Colors.RarityPurple;

                case ItemRarityID.Lime:
                return Colors.RarityLime;

                case ItemRarityID.Yellow:
                return Colors.RarityYellow;

                case ItemRarityID.Cyan:
                return Colors.RarityCyan;

                // TODO: look into the vanilla source for the Red and Purple rarities

                case ItemRarityID.Gray:
                return Colors.RarityTrash;

                case ItemRarityID.Expert:
                return Main.DiscoColor;
            }
        }

        public static DrawData DrawRectangle_Data(Rectangle rectangle, Color color, Vector2 adjustment)
        {
            return new DrawData(TextureCache.Pixel.Value, new Vector2(rectangle.X, rectangle.Y) + adjustment, null, color, 0f, new Vector2(0f, 0f), new Vector2(rectangle.Width, rectangle.Height), SpriteEffects.None, 0);
        }

        public static void DrawRectangle(Rectangle rectangle, Color color, Vector2 adjustment)
        {
            Main.spriteBatch.Draw(TextureCache.Pixel.Value, new Vector2(rectangle.X, rectangle.Y) + adjustment, null, color, 0f, new Vector2(0f, 0f), new Vector2(rectangle.Width, rectangle.Height), SpriteEffects.None, 0f);
        }

        public static void DrawUIBox(int width, int height, Vector2 topLeft, Color color)
        {
            Vector2 orig = new Vector2(0f, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft, new Rectangle(0, 0, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f); // top left
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(width, 0f), new Rectangle(24, 0, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f); // top right
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(0f, height), new Rectangle(0, 24, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f); // bottom left
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(width, height), new Rectangle(24, 24, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f); // bottom right
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(0f, UI_SIZE), new Rectangle(0, 16, 10, 2), color, 0f, orig, new Vector2(1f, (height - 10f) / 2f), SpriteEffects.None, 0f); // top left -> bottom left
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(width, UI_SIZE), new Rectangle(24, 16, 10, 2), color, 0f, orig, new Vector2(1f, (height - 10f) / 2f), SpriteEffects.None, 0f); // top right -> bottom right
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(UI_SIZE, 0f), new Rectangle(16, 0, 2, 10), color, 0f, orig, new Vector2((width - 10f) / 2f, 1f), SpriteEffects.None, 0f); // top left -> top right
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(UI_SIZE, height), new Rectangle(16, 24, 2, 10), color, 0f, orig, new Vector2((width - 10f) / 2f, 1f), SpriteEffects.None, 0f); // bottom left -> bottom right
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(UI_SIZE, UI_SIZE), new Rectangle(16, 16, 2, 2), color, 0f, orig, new Vector2((width - 10f) / 2f, (height - 10f) / 2f), SpriteEffects.None, 0f); // filling
        }

        public static void DrawUIBox_HightlightedTop(int width, int height, Vector2 topLeft, Color color)
        {
            Vector2 orig = new Vector2(0f, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft, new Rectangle(0, 0, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(width, 0f), new Rectangle(24, 0, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(0f, height), new Rectangle(0, 24, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(width, height), new Rectangle(24, 24, 10, 10), color, 0f, orig, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(0f, UI_SIZE), new Rectangle(0, 16, 10, 2), color, 0f, orig, new Vector2(1f, (height - 10f) / 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(width, UI_SIZE), new Rectangle(24, 16, 10, 2), color, 0f, orig, new Vector2(1f, (height - 10f) / 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(UI_SIZE, 0f), new Rectangle(38, 0, 2, 10), color, 0f, orig, new Vector2((width - 10f) / 2f, 1f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(UI_SIZE, height), new Rectangle(16, 24, 2, 10), color, 0f, orig, new Vector2((width - 10f) / 2f, 1f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(DrawUtils.Textures.UI, topLeft + new Vector2(UI_SIZE, UI_SIZE), new Rectangle(16, 16, 2, 2), color, 0f, orig, new Vector2((width - 10f) / 2f, (height - 10f) / 2f), SpriteEffects.None, 0f);
        }

        public static void UpdateFilter(bool active, string name, Vector2 position = default(Vector2), params object[] args)
        {
            if (active != Filters.Scene[name].IsActive())
            {
                if (active)
                    Filters.Scene[name].Activate(position, args);
                else
                {
                    Filters.Scene[name].Deactivate(args);
                }
            }
        }

        public static void UpdateSky(bool active, string name)
        {
            if (active != SkyManager.Instance[name].IsActive())
            {
                if (active)
                    SkyManager.Instance.Activate(name, default(Vector2));
                else
                {
                    SkyManager.Instance.Deactivate(name);
                }
            }
        }

        public static void UpdateOverlay(bool active, string name)
        {
            if (Overlays.Scene[name] != null && active != (Overlays.Scene[name].Mode != OverlayMode.Inactive))
            {
                if (active)
                    Overlays.Scene.Activate(name);
                else
                {
                    Overlays.Scene[name].Deactivate();
                }
            }
        }

        public static void PrepareForTeleport(this Player player)
        {
            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int p = 0; p < 1000; p++)
            {
                if (Main.projectile[p].active && Main.projectile[p].owner == player.whoAmI && Main.projectile[p].aiStyle == 7)
                    Main.projectile[p].Kill();
            }
        }

        public static int NextVRand(this UnifiedRandom rand, int min, int max)
        {
            return min + rand.Next(max - min + 1);
        }

        public static Vector2 TrueMouseworld => Vector2.Transform(Main.ReverseGravitySupport(Main.MouseScreen, 0f), Matrix.Invert(Main.GameViewMatrix.ZoomMatrix)) + Main.screenPosition;

        /// <summary>
        /// Method taken from 1.4, remove when porting.
        /// </summary>
        /// <param name="plr"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <returns></returns>
        public static bool IsInTileInteractionRange(this Player plr, int targetX, int targetY)
        {
            if (plr.position.X / 16f - Player.tileRangeX <= targetX && (plr.position.X + plr.width) / 16f + Player.tileRangeX - 1f >= targetX && plr.position.Y / 16f - Player.tileRangeY <= targetY)
                return (plr.position.Y + plr.height) / 16f + Player.tileRangeY - 2f >= targetY;
            return false;
        }

        public static float GetGrad(float min, float max, float x)
        {
            float xGradient = (x - min) / (float)(max - min);
            return 1f - (float)Math.Pow(1f - xGradient, 2);
        }

        public static void RectangleMethod(Rectangle rect, Utils.PerLinePoint method)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    method(i, j);
                }
            }
        }

        public static void TileABLine(int x, int y, int x2, int y2, Utils.PerLinePoint method)
        {
            int xDir = x > x2 ? -1 : 1;
            int yDir = y > y2 ? -1 : 1;
            int xDifference = (x - x2).Abs();
            int yDifference = (y - y2).Abs();
            for (int i = 0; i < xDifference + 1; i++)
            {
                method(x + xDir * i, y);
            }
            for (int i = 0; i < yDifference + 1; i++)
            {
                method(x + xDir * xDifference, y + yDir * i);
            }
        }

        public static Color UseR(this Color color, int R) => new Color(R, color.G, color.B, color.A);
        public static Color UseR(this Color color, float R) => new Color((int)(R * 255), color.G, color.B, color.A);

        public static Color UseG(this Color color, int G) => new Color(color.R, G, color.B, color.A);
        public static Color UseG(this Color color, float G) => new Color(color.R, (int)(G * 255), color.B, color.A);

        public static Color UseB(this Color color, int B) => new Color(color.R, color.G, B, color.A);
        public static Color UseB(this Color color, float B) => new Color(color.R, color.G, (int)(B * 255), color.A);

        public static Color UseA(this Color color, int alpha) => new Color(color.R, color.G, color.B, alpha);
        public static Color UseA(this Color color, float alpha) => new Color(color.R, color.G, color.B, (int)(alpha * 255));

        public static AQPlayer GetAQPlayer(this Player plr) => plr.GetModPlayer<AQPlayer>();

        public static string GetKeyNames(this ModHotKey key, int keyValue = 0)
        {
            List<string> keys = key.GetAssignedKeys();
            if (keys == null || keys.Count == 0)
                return Language.GetTextValue(AQText.Key + "Common.UnassignedKey" + keyValue);
            else
            {
                if (keys.Count == 1)
                    return keys[0];
                string textValue = "";
                int index = 0;
                while (true)
                {
                    textValue += keys[index];
                    if (index == keys.Count - 1)
                        return textValue;
                    textValue += ", ";
                    index++;
                }
            }
        }

        public static Vector3 NormalizedRotations(Vector3 rotations)
        {
            return Vector3.Transform(Vector3.One, Matrix.CreateFromYawPitchRoll(rotations.X, rotations.Y, rotations.Z));
        }

        public static Vector3 GetRotations(this Vector3 value)
        {
            return new Vector3((float)Math.Atan2(value.Z, value.Y), (float)Math.Atan2(value.Z, value.X), (float)Math.Atan2(value.Y, value.X));
        }

        public static void SetItemHoldout(this Player player, float rotation, int direction)
        {
            player.itemRotation = rotation;
            if (player.direction != direction)
                player.ChangeDir(direction);
            if (direction == 1)
                player.itemRotation -= MathHelper.Pi;
        }

        public static Color MovingRainbow(float position) => LerpColors(new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Violet, Color.Magenta, }, position);

        public static Color LerpColors(Color[] colors, float position)
        {
            int index = (int)(position % colors.Length);
            return Color.Lerp(colors[index], colors[(index + 1) % (colors.Length - 1)], position % 1f);
        }

        public static Vector2[] GetPointCircle(Vector2 center, float radius, int amount = 20)
        {
            Vector2[] points = new Vector2[amount];
            float rot = MathHelper.TwoPi / amount;
            float j = 0f;
            for (int i = 0; i < amount; i++)
            {
                points[i] = center + radius * new Vector2((float)Math.Cos(j), (float)Math.Sin(j));
                j += rot;
            }
            return points;
        }

        public static bool GetBit(this byte b, byte bit)
        {
            return (b & 1 << bit) != 0;
        }

        public static byte SetBit(this ref byte b, byte bit, bool value)
        {
            return value ? (b |= (byte)(1 << bit)) : (b &= (byte)~(1 << bit));
        }

        public static bool IsntFriendly(this NPC npc)
        {
            return npc.active && npc.lifeMax > 5 && !npc.friendly && !npc.townNPC;
        }

        public static void SetLiquidSpeed(this NPC npc, float water = 0.5f, float lava = 0.5f, float honey = 0.25f)
        {
            var type = typeof(NPC);
            type.GetField("waterMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, water);
            type.GetField("lavaMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, lava);
            type.GetField("honeyMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, honey);
        }

        public static float GetHue(this Color color)
        {
            float min = Math.Min(Math.Min(color.R, color.G), color.B);
            float max = Math.Max(Math.Max(color.R, color.G), color.B);
            if (min == max)
                return 0;
            float hue = 0f;
            if (max == color.R)
                hue = (color.G - color.B) / (max - min);
            else if (max == color.G)
            {
                hue = 2f + (color.B - color.R) / (max - min);
            }
            else
            {
                hue = 4f + (color.R - color.G) / (max - min);
            }
            hue *= 60;
            if (hue < 0)
                hue += 360;
            return hue;
        }

        public static int Abs(this int value)
        {
            return value >= 0 ? value : value * -1;
        }

        public static float Abs(this float value)
        {
            return value >= 0 ? value : value * -1f;
        }

        public static void BeginNPCDraw(SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            Main.spriteBatch.Begin(sortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
        }
    }
}
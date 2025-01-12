﻿using AQMod.Assets.SceneLayers;
using AQMod.Content.WorldEvents.DemonSiege;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class GoreNest : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileHammer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.addTile(Type);
            dustType = DustID.Blood;
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.DemonAltar };
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("{$Mods.AQMod.ItemName.GoreNestItem}");
            AddMapEntry(new Color(175, 15, 15), name);
        }


        private static (DemonSiegeUpgrade? upgrade, Item item) getUpgradeableItem(Player player)
        {
            for (int i = 0; i < Main.maxInventory; i++)
            {
                var upgrade = DemonSiege.GetUpgrade(player.inventory[i]);
                if (upgrade != null)
                {
                    return (upgrade, player.inventory[i]);
                }
            }
            return (null, null);
        }

        public override bool HasSmartInteract()
        {
            if (DemonSiege.IsActive)
            {
                return false;
            }
            return getUpgradeableItem(Main.LocalPlayer).item != null;
        }

        public override void MouseOver(int i, int j)
        {
            if (DemonSiege.IsActive)
            {
                return;
            }
            var player = Main.player[Main.myPlayer];
            var upgradeableItem = getUpgradeableItem(player);
            if (upgradeableItem.item != null && upgradeableItem.item.type > ItemID.None)
            {
                player.noThrow = 2;
                player.showItemIcon = true;
                player.showItemIcon2 = upgradeableItem.item.type;
            }
        }
        public override bool NewRightClick(int i, int j)
        {
            if (DemonSiege.IsActive)
            {
                return false;
            }
            var player = Main.player[Main.myPlayer];
            var upgradeableItem = getUpgradeableItem(player);
            if (upgradeableItem.item != null && upgradeableItem.item.type > ItemID.None)
            {
                DemonSiege.Activate(i, j, player.whoAmI, upgradeableItem.item);
                Main.PlaySound(SoundID.DD2_EtherianPortalOpen, new Vector2(i * 16f, j * 16f));
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return Main.hardMode && (!DemonSiege.IsActive || !DemonSiege.altarRectangle().Contains(i, j));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placeable.GoreNestItem>());
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            if (Main.tile[i, j].frameX == 0 && Main.tile[i, j].frameY == 0)
                GoreNestWorldOverlay.AddCorrds(i, j);
        }
    }
}
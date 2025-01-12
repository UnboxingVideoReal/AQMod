﻿using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Amulets
{
    public class SpiritAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Lime;
            item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<AQPlayer>().spiritAmuletHeld = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.spiritAmulet = true;
            aQPlayer.spiritAmuletHeld = true;
        }
    }
}
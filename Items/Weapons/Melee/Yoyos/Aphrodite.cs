﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Yoyos
{
    public class Aphrodite : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.melee = true;
            item.damage = 15;
            item.knockBack = 4.11f;
            item.value = Item.sellPrice(silver: 10);
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Blue;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.Yoyos.Aphrodite>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SilverBar, 8);
            recipe.AddIngredient(ItemID.Coral, 2);
            recipe.AddIngredient(ItemID.Starfish, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
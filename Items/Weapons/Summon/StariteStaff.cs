﻿using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Summon
{
    public class StariteStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.StariteStaff), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.summon = true;
            item.mana = 10;
            item.width = 26;
            item.height = 28;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = AQItem.GlimmerWeaponValue;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item44;
            item.shoot = ModContent.ProjectileType<Projectiles.Minions.StariteMinionLeader>();
            item.buffType = ModContent.BuffType<Buffs.Minion.StariteMinion>();
            item.autoReuse = true;
            item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            int stariteParent = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            int stariteChild = Projectile.NewProjectile(position + new Vector2(30f, 10f), new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.Minions.StariteMinion>(), damage, knockBack, player.whoAmI, stariteParent + 1);
            Main.projectile[stariteChild].minionSlots = 0;
            stariteChild = Projectile.NewProjectile(position + new Vector2(-30f, 10f), new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.Minions.StariteMinion>(), damage, knockBack, player.whoAmI, stariteParent + 1);
            Main.projectile[stariteChild].minionSlots = 0;
            return false;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RavenStaff);
            recipe.AddIngredient(ItemID.FragmentSolar, 18);
            recipe.AddIngredient(ItemID.HallowedBar, 20);
            recipe.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
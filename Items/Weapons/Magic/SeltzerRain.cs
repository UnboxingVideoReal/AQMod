﻿using AQMod.Assets.Enumerators;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Items.Fishing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class SeltzerRain : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 23;
            item.magic = true;
            item.knockBack = 1.25f;
            item.width = 40;
            item.height = 40;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 40;
            item.useAnimation = 40;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FizzyBubble>();
            item.shootSpeed = 16.88f;
            item.noMelee = true;
            item.UseSound = SoundID.Item8;
            item.mana = 11;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int count = 5;
            float mult = 1f / (count + 1);
            var staffDirection = new Vector2(speedX, speedY);
            position += Vector2.Normalize(staffDirection) * (50f * item.scale);
            if (Main.myPlayer == player.whoAmI)
            {
                int dustCount = 10;
                float rot = MathHelper.PiOver2 / dustCount;
                float rot2 = staffDirection.ToRotation();
                var dustType = ModContent.DustType<MonoDust>();
                float off = 5;
                var plrCenter = player.Center;
                float speed = staffDirection.Length();
                for (int j = 0; j < count; j++)
                {
                    float mult2 = mult * (j + 1f);
                    var color = new Color(50, 15, 190, 0) * (mult2 + 0.1f);
                    float dustSpeed = speed * mult2;
                    for (int i = 0; i < dustCount; i++)
                    {
                        var normal = new Vector2(1f, 0f).RotatedBy(rot * i - MathHelper.PiOver4 + rot2);
                        int d = Dust.NewDust(position + normal * off, 2, 2, dustType, 0f, 0f, 0, color);
                        Main.dust[d].velocity = normal * dustSpeed;
                    }
                }
            }
            for (int i = 0; i < count - 1; i++)
            {
                Projectile.NewProjectile(position, staffDirection * (mult * (i + 1f)), type, damage, knockBack, player.whoAmI);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddIngredient(ModContent.ItemType<Fizzler>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class FizzyBubble : ModProjectile
    {
        private const int MouseDistance = 100;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
                projectile.ai[0] = projectile.velocity.Length();
            if (Main.myPlayer == projectile.owner)
            {
                if (projectile.ai[0] > 1f)
                {
                    projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, Main.MouseWorld - projectile.Center, 0.005f)) * projectile.ai[0];
                    projectile.ai[0] *= 0.96f;
                    if (projectile.ai[0] <= 1f)
                        projectile.ai[0] = -0.01f;
                }
                else
                {
                    float distance = Vector2.Distance(projectile.Center, Main.MouseWorld);
                    if (distance < MouseDistance)
                    {
                        float speed = MathHelper.Lerp(2f, 1f, distance / MouseDistance) * projectile.ai[0];
                        projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, (Main.MouseWorld - projectile.Center) * speed, 0.025f));
                    }
                    if (projectile.ai[0] > -1f)
                        projectile.ai[0] -= 0.03f;
                }
                projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item118, projectile.position);
            int count = 20;
            float rot = MathHelper.TwoPi / count;
            var center = projectile.Center + new Vector2(-1f, -1f);
            var type = ModContent.DustType<MonoDust>();
            var color = new Color(50, 15, 190, 0);
            float off = projectile.width / 2f;
            float dustSpeed = projectile.velocity.Length() / 2f;
            for (int i = 0; i < count; i++)
            {
                var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                int d = Dust.NewDust(center + normal * off, 2, 2, type, 0f, 0f, 0, color);
                Main.dust[d].velocity = normal * dustSpeed;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f - Main.screenPosition.X, projectile.height / 2f - Main.screenPosition.Y);
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            var color = new Color(250, 250, 250, 128);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            color *= 0.5f;
            float colorMult = 1f / trailLength;
            for (int i = 0; i < trailLength; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero)
                    break;
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, color * (colorMult * (trailLength - i + 1)), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            float dist = Vector2.Distance(projectile.Center, Main.MouseWorld);
            if (dist < 200f)
            {
                colorMult = 1f - dist / MouseDistance;
                texture = DrawUtils.Textures.Lights[LightID.Spotlight10x50];
                frame = new Rectangle(0, 0, texture.Width, texture.Height);
                color = new Color(50, 15, 190, 0) * colorMult;
                origin = texture.Size() / 2f;

                var texture2 = DrawUtils.Textures.Lights[LightID.Spotlight20x20];

                Main.spriteBatch.Draw(texture2, projectile.position + offset, null, color, projectile.rotation, texture2.Size() / 2f, projectile.scale * (colorMult * colorMult), SpriteEffects.None, 0f);

                var scale = new Vector2(projectile.scale * (0.55f - (1f - colorMult) * 0.2f), projectile.scale * (0.95f - (1f - colorMult) * 0.55f));

                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);

                scale *= 0.9f;

                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 1.4f, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 1.4f, projectile.rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);

                scale *= 0.9f;
                scale *= 1f - (float)Math.Sin(projectile.timeLeft) * 0.1f;

                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.8f, projectile.rotation + MathHelper.PiOver4, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.8f, projectile.rotation + MathHelper.PiOver4 * 3f, origin, scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
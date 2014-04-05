﻿using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Timers;
using TShockAPI;
using TShockAPI.Extensions;
using Terraria;
using Newtonsoft.Json;
using TerrariaApi;
using TerrariaApi.Server;
using TShockAPI.DB;
using Wolfje.Plugins.SEconomy;

namespace CawAIO
{

    /* Instead of using player.SendMessage("string", Color.{Yellow/Red/etc}), there are defined methods for this already.
     * player.SendInfoMessage("string") sends a yellow message. SendWarningMessage is orange. SendErrorMessage is red. 
     * The same goes for using Log 
     */

    [ApiVersion(1, 15)]
    public class CawAIO : TerrariaPlugin
    {
        public override Version Version
        {
            get { return new Version("1.6.2"); }
        }

        public override string Name
        {
            get { return "CAWAIO"; }
        }

        public override string Author
        {
            get { return "CAWCAWCAW"; }
        }

        public override string Description
        {
            get { return "Randomized Commands"; }
        }

        public CawAIO(Main game)
            : base(game)
        {
            Order = 1;
        }
        public override void Initialize()
        {
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.smack", Smack, "smack"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.bunny", Bunny, "bunny"));
            TShockAPI.Commands.ChatCommands.Add(new Command("tshock.world.sethalloween", Forcehalloween, "forcehalloween"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.ownercast", Cawcast, "cc", "oc"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.gamble", Gamble, "gamble"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.reload", Reload_Config, "creload"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.randomtp", RandomTp, "randomtp", "rtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.randommaptp", RandomMapTp, "randommaptp", "rmtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.monstergamble", MonsterGamble, "monstergamble", "mg"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.jester", Jester, "jester", "j"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.townnpc", TownNpc, "townnpc"));
            //TShockAPI.Commands.ChatCommands.Add(new Command("caw.toggle", toggle, "duckhunttoggle"));
            ServerApi.Hooks.ServerChat.Register(this, Bannedwords);
            ServerApi.Hooks.GameUpdate.Register(this, Configevents);
            ServerApi.Hooks.ServerChat.Register(this, Actionfor);
            ServerApi.Hooks.GameUpdate.Register(this, DisableShadowDodgeBuff);
            ReadConfig();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, Bannedwords);
                ServerApi.Hooks.GameUpdate.Deregister(this, Configevents);
                ServerApi.Hooks.ServerChat.Deregister(this, Actionfor);
                ServerApi.Hooks.GameUpdate.Deregister(this, DisableShadowDodgeBuff);
            }
            base.Dispose(disposing);
        }

        /* Checks every active player to see if they have the shadow dodge buff active. */
        private void DisableShadowDodgeBuff(EventArgs e)
        {
            foreach (TSPlayer p in TShock.Players)
            {
                if (p != null && p.Active && p.ConnectionAlive)
                {
                    for (int i = 0; i < p.TPlayer.buffType.Length; i++)
                    {
                        if (p.TPlayer.buffType[i] == 59 && p.TPlayer.buffTime[i] > 5 && !p.Group.HasPermission("caw.staff"))
                            //If the buff time > 7 seconds
                        {
                            p.TPlayer.buffTime[i] = 0;
                            p.SendErrorMessage("You are not allowed to use shadow dodge!");
                            p.Disable("using Shadow Dodge buff for >7 seconds", true);
                        }
                    }
                }
            }
        }


        public void TownNpc(CommandArgs args)
        {
            int killcount = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].townNPC)
                {
                    TSPlayer.Server.StrikeNPC(i, 99999, 90f, 1);
                    killcount++;
                }
            }
            TSPlayer.All.SendInfoMessage(string.Format("{0} killed {1} friendly NPCs and spawned all town NPCs.", args.Player.Name, killcount));

            /* There's an easier way to do this, but it requires all NPCs to be spawned at least once first. Oh well */
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(19).type, TShock.Utils.GetNPCById(19).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(54).type, TShock.Utils.GetNPCById(54).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(209).type, TShock.Utils.GetNPCById(209).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(38).type, TShock.Utils.GetNPCById(38).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(20).type, TShock.Utils.GetNPCById(20).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(207).type, TShock.Utils.GetNPCById(207).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(107).type, TShock.Utils.GetNPCById(107).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(22).type, TShock.Utils.GetNPCById(22).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(124).type, TShock.Utils.GetNPCById(124).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(17).type, TShock.Utils.GetNPCById(17).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(18).type, TShock.Utils.GetNPCById(18).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(227).type, TShock.Utils.GetNPCById(227).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(208).type, TShock.Utils.GetNPCById(208).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(229).type, TShock.Utils.GetNPCById(229).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(142).type, TShock.Utils.GetNPCById(142).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(178).type, TShock.Utils.GetNPCById(178).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(353).type, TShock.Utils.GetNPCById(353).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(368).type, TShock.Utils.GetNPCById(368).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(160).type, TShock.Utils.GetNPCById(160).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(228).type, TShock.Utils.GetNPCById(228).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
            TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(108).type, TShock.Utils.GetNPCById(108).name, 1, args.Player.TileX, args.Player.TileY, 20, 20);
        }

        private void RandomMapTp(CommandArgs args)
        {
            Random rnd = new Random();
            int x = rnd.Next(0, Main.maxTilesX);
            int y = rnd.Next(0, Main.maxTilesY);
            args.Player.Teleport(x * 16, y * 16);
        }

        private void RandomTp(CommandArgs args)
        {
            if (TShock.Utils.ActivePlayers() <= 1)
            {
                args.Player.SendErrorMessage("There is only you on the server, you cannot teleport to yourself!", Color.Red);
                return;
            }
            Random rnd = new Random();
            TSPlayer ts = TShock.Players[rnd.Next(0, TShock.Utils.ActivePlayers() - 1)];
            if (!ts.TPAllow && !args.Player.Group.HasPermission("permissions.tpall"))
            {
                args.Player.SendErrorMessage(ts.Name + " has prevented users from teleporting to them.");
                ts.SendInfoMessage(args.Player.Name + " attempted to teleport to you.");
                return;
            }
            args.Player.Teleport(ts.TileX * 16, ts.TileY * 16);
        }

        private static Config config;

        private static void Jester(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /jester [200bunnies/400pigron/200slimes/50kings/allbosses]");
                return;
            }
            switch (args.Parameters[0])
            {
                case "200bunnies":
                    int amount = 200;
                    int monsteramount = 303;
                    NPC npcs = TShock.Utils.GetNPCById(monsteramount);
                    TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                    break;
                case "400pigron":
                    int monsteramountp = 170;
                    int monsteramountp1 = 171;
                    int amount1 = 200;
                    NPC npcs1 = TShock.Utils.GetNPCById(monsteramountp);
                    NPC npcs2 = TShock.Utils.GetNPCById(monsteramountp1);
                    TSPlayer.Server.SpawnNPC(npcs1.type, npcs1.name, amount1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(npcs2.type, npcs2.name, amount1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawned {1} {2} time(s).", args.Player.Name, npcs1.name, amount1));
                    break;
                case "50kings":
                    int amountk = 50;
                    int monsteramountk = 50;
                    NPC npcsk = TShock.Utils.GetNPCById(monsteramountk);
                    TSPlayer.Server.SpawnNPC(npcsk.type, npcsk.name, amountk, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawned {1} {2} time(s).", args.Player.Name, npcsk.name, amountk));
                    break;
                case "allbosses":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(50).type, TShock.Utils.GetNPCById(50).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(4).type, TShock.Utils.GetNPCById(4).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(65).type, TShock.Utils.GetNPCById(65).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(125).type, TShock.Utils.GetNPCById(125).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(126).type, TShock.Utils.GetNPCById(126).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(127).type, TShock.Utils.GetNPCById(126).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(134).type, TShock.Utils.GetNPCById(134).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(266).type, TShock.Utils.GetNPCById(266).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(222).type, TShock.Utils.GetNPCById(222).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(262).type, TShock.Utils.GetNPCById(262).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(245).type, TShock.Utils.GetNPCById(245).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(328).type, TShock.Utils.GetNPCById(328).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(325).type, TShock.Utils.GetNPCById(325).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(344).type, TShock.Utils.GetNPCById(344).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(345).type, TShock.Utils.GetNPCById(345).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(346).type, TShock.Utils.GetNPCById(346).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawned every boss.", args.Player.Name));
                    break;
            }
        }

        private static void MonsterGamble(CommandArgs args)
        {
            Random random = new Random();
            int amount = random.Next(1, 50);
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;
            var selectedPlayer = SEconomyPlugin.GetEconomyPlayerByBankAccountNameSafe(args.Player.UserAccountName);
            var playeramount = selectedPlayer.BankAccount.Balance;
            Money moneyamount = -config.MonsterGambleCost;
            Money moneyamount2 = config.MonsterGambleCost;
            if (config.SEconomy)
            {
                {
                    if (!args.Player.Group.HasPermission("caw.gamble"))
                    {
                        if (playeramount > moneyamount2)
                        {
                            int monsteramount;
                            do
                            {
                                monsteramount = random.Next(1, Main.maxNPCs);
                                args.Player.SendInfoMessage("You have gambled a banned monster, attempting to regamble...", Color.Yellow);
                            } while (config.MonsterExclude.Contains(monsteramount));

                            NPC npcs = TShock.Utils.GetNPCById(monsteramount);
                            TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                            TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                            args.Player.SendSuccessMessage("You have lost {0} for monster gambling.", moneyamount2);
                            SEconomyPlugin.WorldAccount.TransferToAsync(selectedPlayer.BankAccount, moneyamount, Journalpayment, string.Format("{0} has been lost for monster gambling", moneyamount2, args.Player.Name), string.Format("CawAIO: " + "Monster Gambling"));
                        }
                        else
                        {
                            args.Player.SendErrorMessage("You need {0} to gamble, you have {1}.", moneyamount2, selectedPlayer.BankAccount.Balance);
                        }
                    }
                    else
                    {
                        if (args.Player.Group.HasPermission("caw.gamble"))
                        {
                            int monsteramount;
                            do
                            {
                            monsteramount = random.Next(1, Main.maxNPCs);
                            } while (config.MonsterExclude.Contains(monsteramount));
                            NPC npcs = TShock.Utils.GetNPCById(monsteramount);
                            TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                            TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                            args.Player.SendSuccessMessage("You have lost nothing for monster gambling.");
                        }
                    }
                }
            }
            else
            {
                int monsteramount;

                do monsteramount = random.Next(1, Main.maxNPCs); /* This isn't an amount of monsters. */
                while (config.MonsterExclude.Contains(monsteramount));

                NPC npcs = TShock.Utils.GetNPCById(monsteramount);
                TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);

                TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawned {1} {2} time(s).", args.Player.Name,
                    npcs.name, amount));
            }
        }

        //private static void test(CommandArgs args)
        //{
        //    Random random = new Random();
        //    int count = 0;
        //        int test;
        //        do
        //        {
        //        count++;
        //        test = random.Next(1, 10);
        //        int amount = 200;
        //        int monsterid = 303;
        //        NPC npcs = TShock.Utils.GetNPCById(monsterid);
        //        TSPlayer.All.SendSuccessMessage("below 5 exists! count = {0} number = {1}", count, test);
        //        TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
        //        } while (config.ItemExclude.Contains(test));
        //        TSPlayer.All.SendInfoMessage("You have won! count = {0}", count);
        //    }

        private static void Gamble(CommandArgs args)
        {
            Random random = new Random();

            /* random.Next(1, 100); Wouldn't it be better to define the amount after you know the max stack? */
            int itemAmount = 0;
            int prefixId = random.Next(1, 83);

            /* Why is this named selectedPlayer? You're not selecting a player */
            var selectedPlayer = SEconomyPlugin.GetEconomyPlayerByBankAccountNameSafe(args.Player.UserAccountName);
            var playeramount = selectedPlayer.BankAccount.Balance;
            Money amount = -config.GambleCost;
            Money amount2 = config.GambleCost;
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;
            if (config.SEconomy)
            {
                int itemName;

                do itemName = random.Next(-48, Main.maxItems);
                while (config.ItemExclude.Contains(itemName));

                Item item = TShock.Utils.GetItemById(itemName);

                if (args.Player != null && selectedPlayer != null)
                {
                    /* if (itemAmount > item.maxStack)
                        itemAmount = item.maxStack; */
                    itemAmount = random.Next(1, item.maxStack);  /* Now the item stack is much more dynamic */

                    if (playeramount > amount2)
                    {
                        if (args.Player.InventorySlotAvailable || item.name.ToLower().Contains("coin"))
                        {
                            if (!args.Player.Group.HasPermission("caw.gamble"))
                            {
                                item.prefix = (byte)prefixId;

                                args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);

                                SEconomyPlugin.WorldAccount.TransferToAsync(selectedPlayer.BankAccount, amount, 
                                    Journalpayment, string.Format("{0} has been lost for gambling", amount2, args.Player.Name),
                                    string.Format("CawAIO: " + "Gambling"));
                                
                                args.Player.SendSuccessMessage("You have lost {0} and gambled {1} {2}(s).", amount2, itemAmount,
                                    item.AffixName());

                                /* Putting Color.Red into Log.ConsoleInfo makes no sense. Log.ConsoleInfo has its own colour,
                                 * and no where in the parameters does it accept another colour */
                                Log.ConsoleInfo("{0} has gambled {1} {2}(s)", args.Player.Name, itemAmount, item.AffixName());


                                foreach (TSPlayer player in TShock.Players)
                                {
                                    if (player != null)
                                        if (player.Group.HasPermission("caw.staff"))
                                            player.SendInfoMessage("[Gamble] " + args.Player.Name + " has gambled " + itemAmount +
                                                " " + item.AffixName());
                                }
                            }
                            else
                            {
                                item.prefix = (byte)prefixId;

                                args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);

                                args.Player.SendSuccessMessage("You have lost nothing and gambled {0} {1}(s).", itemAmount,
                                    item.AffixName());

                                Log.ConsoleInfo("{0} has gambled {1} {2}(s)", args.Player.Name, itemAmount, item.AffixName());

                                foreach (TSPlayer player in TShock.Players)
                                {
                                    if (player != null)
                                    {
                                        if (player.Group.HasPermission("caw.staff"))
                                        {
                                            player.SendInfoMessage("[Gamble] " + args.Player.Name + " has gambled " + itemAmount +
                                                " " + item.AffixName());
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Your inventory seems full.");
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("You need {0} to gamble, you have {1}.", amount2,
                            selectedPlayer.BankAccount.Balance);
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("The server could not find a valid bank account for the username {0}",
                        args.Player.Name);
                }
            }
            else
            {
                int itemName;
                do
                {
                    itemName = random.Next(-48, Main.maxItems);
                } while (config.ItemExclude.Contains(itemName));

                Item item = TShock.Utils.GetItemById(itemName);

                if (args.Player != null && selectedPlayer != null)
                {
                    if (itemAmount > item.maxStack)
                    {
                        itemAmount = item.maxStack;
                    }
                    if (args.Player.InventorySlotAvailable || item.name.Contains("Coin"))
                    {
                        item.prefix = (byte)prefixId;
                        args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);
                        Log.ConsoleInfo("{0} has gambled {1} {2}(s)", args.Player.Name, itemAmount, item.AffixName(), Color.Red);

                        foreach (TSPlayer player in TShock.Players)
                        {
                            if (player != null)
                            {
                                if (player.Group.HasPermission("caw.staff"))
                                {
                                    player.SendMessage("[Gamble] " + args.Player.Name + " has gambled " + itemAmount + " " + item.AffixName(), Color.Yellow);
                                }
                            }
                        }
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("Your inventory seems full.");
                }
            }
        }

        private static void Smack(CommandArgs args)
        {
            /* Not strictly required, unless you definitely want to restrict them to 1 parameter. */
            //if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            //{
            //    args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /smack <player>");
            //    return;
            //}
            //if (args.Parameters[0].Length == 0)
            //{
            //    args.Player.SendErrorMessage("Missing player name.");
            //    return;
            //}

            if (args.Parameters.Count > 0)
            {
                string plStr = string.Join(" ", args.Parameters);  /* This takes all parameters and joins them together.
                                                                    * This way, players do not need to use "" for multi-word
                                                                    * names. */
                var players = TShock.Utils.FindPlayer(plStr);
                if (players.Count == 0)
                    args.Player.SendErrorMessage("No player matched your query '{0}'", plStr);
                else if (players.Count > 1)
                    TShock.Utils.SendMultipleMatchError(args.Player, players.Select(p => p.Name));
                    /* Uses the TShock multi-match error to send a list of all matching players */

                /* What? */
                //var PlayersFound = TShock.Utils.FindPlayer(args.Parameters[0]);
                //if (PlayersFound.Count != 1)
                //{
                //    args.Player.SendErrorMessage(PlayersFound.Count < 1 ? "No players matched." : "More than one player matched.");
                //    return;
                //}

                else
                {
                    var plr = players[0];
                    //if (args.Parameters.Count == 1) /* This was checked earlier */
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} smacked {1}.",
                                                         args.Player.Name, plr.Name));
                    Log.Info(args.Player.Name + " smacked " + plr.Name);
                }
            }
            else
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /smack <player>");
        }

        //private DateTime LastCheck = DateTime.UtcNow;
        //private int Duckhunt = 10;
        //private bool DuckhuntToggle = config.DuckhuntToggle;
        //public void OnUpdatetest(EventArgs args)
        //{
        //    if (DuckhuntToggle && ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1))
        //    {
        //        LastCheck = DateTime.UtcNow;
        //        if (Duckhunt == config.GambleCooldown)
        //        {
        //            TSPlayer.All.SendInfoMessage("This is a test");
        //        }
        //        else if (Duckhunt == config.DuckhuntTimer + 10)
        //        {
        //            spawnducks();
        //            TSPlayer.All.SendInfoMessage("test");
        //        }
        //    }
        //}

        //private void toggle(CommandArgs args)
        //{
        //    DuckhuntToggle = !DuckhuntToggle;
        //    if (DuckhuntToggle == true || DuckhuntToggle == false)
        //    {
        //        args.Player.SendMessage("Duckhunt now: " + ((DuckhuntToggle) ? "Enabled" : "Disabled"), Color.Aquamarine);
        //    }
        //}
        //private TShockAPI.DB.Region arenaregion = new TShockAPI.DB.Region();
        //private void spawnducks()
        //{
        //    arenaregion = TShock.Regions.GetRegionByName("duckarena");
        //    int arenaX = arenaregion.Area.X + (arenaregion.Area.Width / 2);
        //    int arenaY = arenaregion.Area.Y + (arenaregion.Area.Height / 2);
        //    TSPlayer.All.SendInfoMessage("The ducks fly tonight.");
        //    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(362).type, TShock.Utils.GetNPCById(362).name, 20, arenaX, arenaY, (arenaregion.Area.Width / 2), (arenaregion.Area.Height / 2));

        //}


        private static void Configevents(EventArgs args)
        {
            if (config.ForceHalloween)
            {
                TShock.Config.ForceHalloween = true;
                Main.checkHalloween();
            }
            else
            {
                TShock.Config.ForceHalloween = false;
                Main.checkHalloween();
            }
        }

        private static void Forcehalloween(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                if (args.Parameters[0].ToLower() == "true")
                {
                    TShock.Config.ForceHalloween = true;
                    Main.checkHalloween();
                }
                else if (args.Parameters[0].ToLower() == "false")
                {
                    TShock.Config.ForceHalloween = false;
                    Main.checkHalloween();
                }
                else
                {
                    args.Player.SendErrorMessage("Usage: /forcehalloween [true/false]");
                    return;
                }
                args.Player.SendInfoMessage(
                        String.Format("The server is currently {0} force Halloween mode.",
                        (TShock.Config.ForceHalloween ? "in" : "not in")));
            }
            else
            {
                args.Player.SendErrorMessage("Usage: /forcehalloween [true/false]");
                args.Player.SendInfoMessage(
                    String.Format("The server is currently {0} force Halloween mode.",
                                (TShock.Config.ForceHalloween ? "in" : "not in")));
            }
        }
        private void Bunny(CommandArgs args)
        {
            TSPlayer player = TShock.Players[args.Player.Index];
            {
                player.SendMessage("You have been buffed with a pet bunny (I think it wants your carrot).", Color.Green);
                player.SetBuff(40, 60, true);
            }
        }

        private static void Cawcast(CommandArgs args)
        {
            string message = string.Join(" ", args.Parameters);

            TShock.Utils.Broadcast(
                "(Owner Broadcast) " + message, Color.Aqua);
        }

        /* This should really have a better name, because it's not checking banned words.
         * It's checking if someones trying to use the shadow dodge buff */
        private void Bannedwords(ServerChatEventArgs args)
        {
            if (args.Handled)
            {
                return;
            }
            TSPlayer player = TShock.Players[args.Who];
            if (player == null)
            {
                args.Handled = true;
                return;
            }

            if (config.BlockShadowDodgeBuff)
            {
                if (args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("shadow d") ||
                    args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("\"shadow d") ||
                    args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("59"))
                {
                    if (player.Group.HasPermission("caw.staff"))
                    {
                        args.Handled = false;
                    }
                    else
                    {
                        args.Handled = true;
                        player.SendInfoMessage("Shadow Dodge is not a buff you can use on this server through commands.");
                    }
                }
            }
        }

        public void Actionfor(ServerChatEventArgs args)
        {
            var player = TShock.Players[args.Who];
            if (!args.Text.ToLower().StartsWith("/") || args.Text.ToLower().StartsWith("/w") ||
                args.Text.ToLower().StartsWith("/r") || args.Text.ToLower().StartsWith("/me") ||
                args.Text.ToLower().StartsWith("/c") || args.Text.ToLower().StartsWith("/party"))
            {
                foreach (string Word in config.BanWords)
                {
                    if (player.Group.HasPermission("caw.staff"))
                    {
                        args.Handled = false;
                    }

                    else if (args.Text.ToLower().Contains(Word))
                    {
                        switch (config.Action)
                        {
                            case "kick":
                                args.Handled = true;
                                TShock.Utils.Kick(player, config.KickMessage, true, false);
                                break;
                            case "ignore":
                                args.Handled = true;
                                player.SendErrorMessage("Your message has been ignored for saying: {0}", Word);
                                break;
                            case "censor":
                                /* Handle the text, so it's not sent. Then create a variable that is the same as args.Text
                                 * Replace the banned word with the ***, then create a fake player message and broadcast it */
                                args.Handled = true;
                                var text = args.Text;
                                text = args.Text.Replace(Word, new string('*', Word.Length));
                                player.SendMessageFromPlayer(text, player.Group.R, player.Group.G,
                                    player.Group.B, player.Index);
                                break;
                            case "donothing":
                                args.Handled = false;
                                break;
                        }
                    }
                }
            }
            else
            {
                args.Handled = false;
            }
        }

        private static void CreateConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "CawAIO.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        config = new Config();
                        var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
        }

        private static bool ReadConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "CawAIO.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            config = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    Log.ConsoleError("CawAIO config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
            return false;
        }

        public class Config
        {
            /* These could really use some better names.
             * 'Action' should be something more like 'ActionOnBannedWordUse'. Always assume the end user is as thick as
             * a plank.
             * 'SEconomy' => 'EnableSEconomyExtension' 
             * Etc */
            public string Action = "ignore";
            public string[] BanWords = { "yolo", "swag", "can i be staff", "can i be admin" };
            public bool ForceHalloween = false;
            public bool SEconomy = false;
            public bool BlockShadowDodgeBuff = false;
            //public bool DuckhuntToggle = false;
            public int GambleCost = 50000;
            public int MonsterGambleCost = 50000;
            public string KickMessage = "You have said a banned word.";
            public int[] ItemExclude = { 665, 666, 667, 668, 1131, 1554, 1555, 1556, 1557, 1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567, 1568 };
            public int[] MonsterExclude = { 9, 22, 68, 17, 18, 37, 38, 19, 20, 37, 54, 68, 106, 123, 124, 107, 108, 113, 142, 178, 207, 208, 209, 227, 228, 160, 229, 353, 368 };
            //public int MonsterGambleCooldown = 0;
            //public int GambleCooldown = 0;
            //public int DuckhuntTimer = 10;

        }

        private void Reload_Config(CommandArgs args)
        {
            if (ReadConfig())
            {
                args.Player.SendMessage("CawAIO config reloaded sucessfully.", Color.Yellow);
            }
            else
            {
                args.Player.SendErrorMessage("CawAIO config reloaded unsucessfully. Check logs for details.");
            }
        }

    }
}

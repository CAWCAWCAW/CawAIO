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
using Wolfje.Plugins.SEconomy.Journal;

namespace CawAIO
{
    [ApiVersion(1, 16)]
    public class CawAIO : TerrariaPlugin
    {
        public int WarningCount = 0;
        private Config config;
        public DateTime LastCheck = DateTime.UtcNow;
        public DateTime SLastCheck = DateTime.UtcNow;
        public CPlayers[] Playerlist = new CPlayers[256];
        DateTime DLastCheck = DateTime.UtcNow;

        public override Version Version
        {
            get { return new Version("1.9.5"); }
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

        #region Initialize
        public override void Initialize()
        {
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.smack", Smack, "smack"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.bunny", Bunny, "bunny"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.ownercast", Cawcast, "cc", "oc"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.gamble", Gamble, "gamble"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.reload", Reload_Config, "creload"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.randomtp", RandomTp, "randomtp", "rtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.randommaptp", RandomMapTp, "randommaptp", "rmtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.monstergamble", MonsterGamble, "monstergamble", "mg"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.jester", Jester, "jester", "j"));
            TShockAPI.Commands.ChatCommands.Add(new Command("caw.townnpc", TownNpc, "townnpc"));
            //TShockAPI.Commands.ChatCommands.Add(new Command("caw.toggle", toggle, "duckhunttoggle"));
            //TShockAPI.Commands.ChatCommands.Add(new Command("caw.toggle", DuckHuntStart, "duckhuntstart", "dhstart"));
            ServerApi.Hooks.ServerChat.Register(this, ShadowDodgeCommandBlock);
            ServerApi.Hooks.ServerChat.Register(this, Actionfor);
            ServerApi.Hooks.GameUpdate.Register(this, DisableShadowDodgeBuff);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            ServerApi.Hooks.GameUpdate.Register(this, Cooldowns);
            ReadConfig();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, ShadowDodgeCommandBlock);
                ServerApi.Hooks.ServerChat.Deregister(this, Actionfor);
                ServerApi.Hooks.GameUpdate.Deregister(this, DisableShadowDodgeBuff);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
                ServerApi.Hooks.GameUpdate.Deregister(this, Cooldowns);
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Playerlist OnJoin/OnLeave
        public void OnJoin(JoinEventArgs args)
        {
            Playerlist[args.Who] = new CPlayers(args.Who);
        }
        public void OnLeave(LeaveEventArgs args)
        {
            Playerlist[args.Who] = null;
        }
        #endregion

        #region Disable Shadow Dodge Buff
        private void DisableShadowDodgeBuff(EventArgs e)
        {
            if ((DateTime.UtcNow - SLastCheck).TotalSeconds >= config.BlockShadowDodgeTimerInSeconds)
            {
                foreach (TSPlayer p in TShock.Players)
                {
                    if (p != null && p.Active && p.ConnectionAlive)
                    {
                        for (int i = 0; i < p.TPlayer.buffType.Length; i++)
                        {
                            if (p.TPlayer.buffType[i] == 59 && p.TPlayer.buffTime[i] > 20 && !p.Group.HasPermission("caw.shadowbypass"))
                            {
                                p.TPlayer.buffTime[i] = 0;
                                p.SendErrorMessage("You are not allowed to use shadow dodge!");
                                p.Disable("Using Shadow Dodge buff for greater than 20 seconds.", true);
                            }
                        }
                    }
                }
                SLastCheck = DateTime.UtcNow;
            }
        }
        #endregion

        #region Spawn Town Npcs
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
        #endregion

        #region Cooldowns
        private void Cooldowns(EventArgs args)
        {
            if ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1)
            {
                LastCheck = DateTime.UtcNow;
            foreach (var player in Playerlist)
            {
                if (player == null)
                {
                    continue;
                }
                if (player.GambleCooldown > 0)
                {
                    player.GambleCooldown--;
                }
                if (player.MonsterGambleCooldown > 0)
                {
                    player.MonsterGambleCooldown--;
                }
                if (player.RandomMapTeleportCooldown > 0)
                {
                    player.RandomMapTeleportCooldown--;
                }
                if (player.RandomPlayerTeleport > 0)
                {
                    player.RandomPlayerTeleport--;
                }
            }
            }
        }
        #endregion

        #region Teleport to random map coordinate
        private void RandomMapTp(CommandArgs args)
        {
            var player = Playerlist[args.Player.Index];

            if ( player.RandomMapTeleportCooldown == 0)
            {
                Random rnd = new Random();
                int x = rnd.Next(0, Main.maxTilesX);
                int y = rnd.Next(0, Main.maxTilesY);
                args.Player.Teleport(x * 16, y * 16);
                if (!args.Player.Group.HasPermission("caw.nocooldown"))
                {
                    player.RandomMapTeleportCooldown = config.RandomMapTeleportCooldown;
                }
            }
            else
            {
                args.Player.SendErrorMessage("This command is on cooldown for {0} seconds.", (player.RandomMapTeleportCooldown));
            }
        }
        #endregion

        #region Teleport to random player
        private void RandomTp(CommandArgs args)
        {
            var player = Playerlist[args.Player.Index];

            if (player.RandomPlayerTeleport == 0)
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

                if (!args.Player.Group.HasPermission("caw.nocooldown"))
                {
                    player.RandomMapTeleportCooldown = config.RandomPlayerTeleportCooldown;
                }
            }
            else
            {
                args.Player.SendErrorMessage("This command is on cooldown for {0} seconds.", player.RandomPlayerTeleport);
            }
        }
        #endregion

        #region Jester
        private void Jester(CommandArgs args)
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
        #endregion

        #region Monster Gambling
        private void MonsterGamble(CommandArgs args)
        {
            Random random = new Random();
            int amount = random.Next(1, 50);
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;
            var selectedPlayer = SEconomyPlugin.Instance.GetBankAccount(args.Player.UserAccountName);
            var playeramount = selectedPlayer.Balance;
            var player = Playerlist[args.Player.Index];
            Money moneyamount = -config.MonsterGambleCost;
            Money moneyamount2 = config.MonsterGambleCost;
            
            if (player.MonsterGambleCooldown == 0)
            {
                if (!args.Player.Group.HasPermission("caw.nocooldown"))
                {
                    player.MonsterGambleCooldown = config.MonsterGambleCooldown;
                }

                if (config.SEconomy)
                {
                    {
                        if (!args.Player.Group.HasPermission("caw.gamble.nocost"))
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
                                SEconomyPlugin.Instance.WorldAccount.TransferToAsync(selectedPlayer, moneyamount, Journalpayment, string.Format("{0} has been lost for monster gambling", moneyamount2, args.Player.Name), string.Format("CawAIO: " + "Monster Gambling"));
                                Log.ConsoleInfo("{0} has spawnned {1} {2}.", args.Player.Name, amount, npcs.name);
                            }
                            else
                            {
                                args.Player.SendErrorMessage("You need {0} to gamble, you have {1}.", moneyamount2, selectedPlayer.Balance);
                            }
                        }
                        else
                        {
                            if (args.Player.Group.HasPermission("caw.gamble.nocost"))
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
                                Log.ConsoleInfo("{0} has spawnned {1} {2}.", args.Player.Name, amount, npcs.name);
                            }
                        }
                    }
                }
                else
                {
                    int Randnpc;

                    do Randnpc = random.Next(1, Main.maxNPCs);
                    while (config.MonsterExclude.Contains(Randnpc));

                    NPC npcs = TShock.Utils.GetNPCById(Randnpc);
                    TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);

                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawned {1} {2} time(s).", args.Player.Name,
                        npcs.name, amount));
                    Log.ConsoleInfo("{0} has spawnned {1} {2}.", args.Player.Name, amount, npcs.name);
                }
            }
                    else
                    {
                        args.Player.SendErrorMessage("This command is on cooldown for {0} seconds.", (player.MonsterGambleCooldown));
                    }
        }
        #endregion

        #region Normal Gambling
        private void Gamble(CommandArgs args)
        {
            Random random = new Random();
            int itemAmount = 0;
            int prefixId = random.Next(1, 83);
            var UsernameBankAccount = SEconomyPlugin.Instance.GetBankAccount(args.Player.UserAccountName);
            var playeramount = UsernameBankAccount.Balance;
            var player = Playerlist[args.Player.Index];
            Money amount = -config.GambleCost;
            Money amount2 = config.GambleCost;
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;
            
            if (player.MonsterGambleCooldown == 0)
            {
                if (!args.Player.Group.HasPermission("caw.nocooldown"))
                {
                    player.MonsterGambleCooldown = config.GambleCooldown;
                }

            if (config.SEconomy)
            {
                int itemName;

                do itemName = random.Next(-48, Main.maxItems);
                while (config.ItemExclude.Contains(itemName));

                Item item = TShock.Utils.GetItemById(itemName);

                if (args.Player != null && UsernameBankAccount != null)
                {
                    itemAmount = random.Next(1, item.maxStack);

                    if (playeramount > amount2)
                    {
                        if (args.Player.InventorySlotAvailable || item.name.ToLower().Contains("coin"))
                        {
                            if (!args.Player.Group.HasPermission("caw.gamble.nocost"))
                            {
                                item.prefix = (byte)prefixId;

                                args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);

                                SEconomyPlugin.Instance.WorldAccount.TransferToAsync(UsernameBankAccount, amount,
                                    Journalpayment, string.Format("{0} has been lost for gambling", amount2, args.Player.Name),
                                    string.Format("CawAIO: " + "Gambling"));

                                args.Player.SendSuccessMessage("You have lost {0} and gambled {1} {2}(s).", amount2, itemAmount,
                                    item.AffixName());

                                Log.ConsoleInfo("{0} has gambled {1} {2}(s)", args.Player.Name, itemAmount, item.AffixName());


                                foreach (TSPlayer staffplayer in TShock.Players)
                                {
                                    if (staffplayer != null)
                                        if (staffplayer.Group.HasPermission("caw.gamblevision"))
                                            staffplayer.SendInfoMessage("[Gamble] " + args.Player.Name + " has gambled " + itemAmount +
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

                                foreach (TSPlayer staffplayer in TShock.Players)
                                {
                                    if (staffplayer != null)
                                    {
                                        if (staffplayer.Group.HasPermission("caw.gamblevision"))
                                        {
                                            staffplayer.SendInfoMessage("[Gamble] " + args.Player.Name + " has gambled " + itemAmount +
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
                            UsernameBankAccount.Balance);
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

                if (args.Player != null && UsernameBankAccount != null)
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

                        foreach (TSPlayer staffplayer in TShock.Players)
                        {
                            if (staffplayer != null)
                            {
                                if (staffplayer.Group.HasPermission("caw.gamblevision"))
                                {
                                    staffplayer.SendMessage("[Gamble] " + args.Player.Name + " has gambled " + itemAmount + " " + item.AffixName(), Color.Yellow);
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
            else
            {
                args.Player.SendErrorMessage("This command is on cooldown for another {0} seconds.", player.GambleCooldown);
            }
        }
        #endregion

        #region Smack command
        private void Smack(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string plStr = string.Join(" ", args.Parameters);
                var players = TShock.Utils.FindPlayer(plStr);
                if (players.Count == 0)
                    args.Player.SendErrorMessage("No player matched your query '{0}'", plStr);
                else if (players.Count > 1)
                    TShock.Utils.SendMultipleMatchError(args.Player, players.Select(p => p.Name));
                else
                {
                    var plr = players[0];
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} smacked {1}.",
                                                         args.Player.Name, plr.Name));
                    Log.Info(args.Player.Name + " smacked " + plr.Name);
                }
            }
            else
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /smack <player>");
        }
        #endregion

        //private bool DuckhuntToggle;
        //public void DuckHuntStart(CommandArgs args)
        //{
        //    if (DuckhuntToggle && ((DateTime.UtcNow - DLastCheck).TotalSeconds >= config.DuckhuntTimer))
        //    {
        //        TSPlayer.All.SendInfoMessage("The ducks fly tonight.");
        //        TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(362).type, TShock.Utils.GetNPCById(362).name, 20, args.Player.TileX, args.Player.TileY, 50, 20);
        //        DLastCheck = DateTime.UtcNow;
        //    }
        //    else
        //    {
        //        int CD = config.DuckhuntTimer - DLastCheck.Second;
        //        args.Player.SendErrorMessage("This command is on cooldown for another {0} seconds.", CD);   
        //    }
        //}

        //private void toggle(CommandArgs args)
        //{
        //    DateTime LastCheck = DateTime.UtcNow;
        //    DuckhuntToggle = !DuckhuntToggle;
        //    if (DuckhuntToggle == true || DuckhuntToggle == false)
        //    {
        //        args.Player.SendMessage("Duckhunt now: " + ((DuckhuntToggle) ? "Enabled" : "Disabled"), Color.Aquamarine);
        //    }
        //}

        #region Bunny Command
        private void Bunny(CommandArgs args)
        {
            TSPlayer player = TShock.Players[args.Player.Index];
            {
                player.SendMessage("You have been buffed with a pet bunny (I think it wants your carrot).", Color.Green);
                player.SetBuff(40, 60, true);
            }
        }
        #endregion

        #region Cawcast Command
        private void Cawcast(CommandArgs args)
        {
            string message = string.Join(" ", args.Parameters);

            TShock.Utils.Broadcast(
                "(Owner Broadcast) " + message, Convert.ToByte(config.OwnerBroadcastColorRGB[0]), Convert.ToByte(config.OwnerBroadcastColorRGB[1]), Convert.ToByte(config.OwnerBroadcastColorRGB[2]));
        }
        #endregion

        #region Block Shadow Dodge Command Usage
        private void ShadowDodgeCommandBlock(ServerChatEventArgs args)
        {
            if (args.Handled)
            {
                return;
            }
            TSPlayer player = TShock.Players[args.Who];
            if (player == null)
            {
                return;
            }

            if (config.BlockShadowDodgeBuff)
            {
                if (args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("shadow d") ||
                    args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("\"shadow d") ||
                    args.Text.ToLower().StartsWith("/buff") && args.Text.ToLower().Contains("59"))
                {
                    if (player.Group.HasPermission("caw.shadowbypass"))
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
        #endregion

        #region Block Banned Words
        private void Actionfor(ServerChatEventArgs args)
        {
            var ignored = new List<string>();
            var censored = new List<string>();
            var warningwords = new List<string>();
            var player = TShock.Players[args.Who];
            var text = args.Text;

            if (player == null)
            {
                return;
            }

                    if (!args.Text.ToLower().StartsWith("/") || args.Text.ToLower().StartsWith("/w") ||
                        args.Text.ToLower().StartsWith("/r") || args.Text.ToLower().StartsWith("/me") ||
                        args.Text.ToLower().StartsWith("/c") || args.Text.ToLower().StartsWith("/party"))
                    {
                        foreach (string Word in config.BanWords)
                        {
                            if (player.Group.HasPermission("caw.filterbypass"))
                            {
                                args.Handled = false;
                            }

                            else if (args.Text.ToLower().Equals(Word))
                            {
                                if (player.mute)
                                {
                                    player.SendErrorMessage("You are muted!");
                                    return;
                                }
                                else
                                {
                                    switch (config.ActionForBannedWord)
                                    {
                                        case "tempban":
                                            args.Handled = true;
                                            if (config.WarningSystem)
                                            {
                                                foreach (var wplayer in Playerlist)
                                                {
                                                    if (wplayer == null)
                                                    {
                                                        return;
                                                    }
                                                        if (wplayer.WarningCount >= config.AmountofWarningBeforeAction)
                                                        {
                                                            TShock.Bans.AddBan(player.IP, player.Name, player.UUID, config.KickMessage, false, player.UserAccountName, DateTime.UtcNow.AddMinutes(config.BanTimeInMinutes).ToString("m"));
                                                        }
                                                        else
                                                        {
                                                            wplayer.WarningCount += 1;
                                                            warningwords.Add(Word);
                                                            player.SendErrorMessage("You have said a banned word: " + string.Join(" ", warningwords) + " You will be temp-banned in " + (config.AmountofWarningBeforeAction - wplayer.WarningCount) + " more incidents.");
                                                        }
                                                    }
                                            }
                                            else
                                            {
                                                TShock.Bans.AddBan(player.IP, player.Name, player.UUID, config.KickMessage, false, player.UserAccountName, DateTime.UtcNow.AddMinutes(config.BanTimeInMinutes).ToString("m"));
                                            }
                                            return;
                                        case "ban":
                                            args.Handled = true;
                                            if (config.WarningSystem)
                                            {
                                                foreach (var wplayer in Playerlist)
                                                {
                                                    if (wplayer == null)
                                                    {
                                                        return;
                                                    }
                                                        if (wplayer.WarningCount >= config.AmountofWarningBeforeAction)
                                                        {
                                                            TShock.Bans.AddBan(player.IP, player.Name, player.UUID, config.KickMessage, false, player.UserAccountName);
                                                        }
                                                        else
                                                        {
                                                            wplayer.WarningCount += 1;
                                                            warningwords.Add(Word);
                                                            player.SendErrorMessage("You have said a banned word: " + string.Join(" ", warningwords) + " You will be banned in " + (config.AmountofWarningBeforeAction - wplayer.WarningCount) + " more incidents.");
                                                        }
                                                }
                                            }
                                            else
                                            {
                                                TShock.Bans.AddBan(player.IP, player.Name, player.UUID, config.KickMessage, false, player.UserAccountName);
                                            }
                                            return;
                                        case "kick":
                                            args.Handled = true;
                                            if (config.WarningSystem)
                                            {
                                                foreach (var wplayer in Playerlist)
                                                {
                                                    if (wplayer == null)
                                                    {
                                                        return;
                                                    }
                                                        if (wplayer.WarningCount >= config.AmountofWarningBeforeAction)
                                                        {
                                                            TShock.Utils.Kick(player, config.KickMessage, true, false);
                                                        }
                                                        else
                                                        {
                                                            wplayer.WarningCount += 1;
                                                            warningwords.Add(Word);
                                                            player.SendErrorMessage("You have said a banned word: " + string.Join(" ", warningwords) + " You will be kicked in " + (config.AmountofWarningBeforeAction - wplayer.WarningCount) + " more incidents.");
                                                        }
                                                }
                                            }
                                            else
                                            {
                                                TShock.Utils.Kick(player, config.KickMessage, true, false);
                                            }
                                            return;
                                        case "ignore":
                                            args.Handled = true;
                                            ignored.Add(Word);
                                            break;
                                        case "censor":
                                            args.Handled = true;
                                            text = args.Text;
                                            text = args.Text.Replace(Word, new string('*', Word.Length));
                                            string.Format(config.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, text, player.Group.R, player.Group.G, player.Group.B);
                                            //TSPlayer.All.SendMessage("<" + "(" + player.Group.Name + ") " + player.Name + ">" + text, player.Group.R, player.Group.G, player.Group.B);
                                            //TSPlayer.All.SendMessage(player.Group.Prefix + player.Name + ": " + text, player.Group.R, player.Group.G, player.Group.B);
                                            return;
                                        case "donothing":
                                            args.Handled = false;
                                            break;
                                    }
                                }
                            }
                        }
                        if (warningwords.Count > 0 && WarningCount < 3)
                        {
                            player.SendErrorMessage("Your message has been ignored for saying: " + string.Join(", ", warningwords));
                            player.SendErrorMessage("Your warning count is now: {0}. After {1} warnings you will be {2}", WarningCount, config.AmountofWarningBeforeAction, config.ActionForBannedWord.ToString());
                        }
                        if (ignored.Count > 0)
                        {
                            player.SendErrorMessage("Your message has been ignored for saying: " + string.Join(", ", ignored));
                            return;
                        }
                    }
                    else
                    {
                        args.Handled = false;
                    }
                }
        #endregion

        #region Create Config File
        private void CreateConfig()
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
        #endregion

        #region Read Config File
        private bool ReadConfig()
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
        #endregion

        #region Config Class
        public class Config
        {
            public string ActionForBannedWord = "ignore";
            public string ChatFormat = "[{0}]{2}{3}{4}: {5}";
            public float[] OwnerBroadcastColorRGB = new float[3];
            public bool WarningSystem = true;
            public int AmountofWarningBeforeAction = 3;
            public string[] BanWords = { "yolo", "swag", "can i be staff", "can i be admin" };
            public string KickMessage = "You have said a banned word.";
            public int BanTimeInMinutes = 10;
            public bool SEconomy = false;
            public bool BlockShadowDodgeBuff = false;
            public int BlockShadowDodgeTimerInSeconds = 1;
            //public bool DuckhuntToggle = false;
            public int GambleCost = 50000;
            public int GambleCooldown = 0;
            public int MonsterGambleCost = 50000;
            public int MonsterGambleCooldown = 0;
            public int RandomMapTeleportCooldown = 0;
            public int RandomPlayerTeleportCooldown = 0;
            public int[] ItemExclude = { 665, 666, 667, 668, 1131, 1554, 1555, 1556, 1557, 1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567, 1568 };
            public int[] MonsterExclude = { 9, 22, 68, 17, 18, 37, 38, 19, 20, 37, 54, 68, 106, 123, 124, 107, 108, 113, 142, 178, 207, 208, 209, 227, 228, 160, 229, 353, 368 };
            //public int DuckhuntTimer = 10;

        }
        #endregion

        #region Reload Config File
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
        #endregion
    }
}
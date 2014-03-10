﻿using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
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
    [ApiVersion(1, 15)]

    public class CawAIO : TerrariaPlugin
    {
        public override Version Version
        {
            get { return new Version("1.0"); }
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
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.smack", Smack, "smack"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.bunny", Bunny, "bunny"));
            TShockAPI.Commands.ChatCommands.Add(new Command("tshock.world.sethalloween", Forcehalloween, "forcehalloween"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.ownercast", Cawcast, "cc", "oc"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.gamble", Gamble, "gamble"));
            //TShockAPI.Commands.ChatCommands.Add(new Command("cawaio.reload", Reload_Config, "creload"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.randomtp", randomtp, "randomtp", "rtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.randommaptp", randommaptp, "randommaptp", "rmtp"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.monstergamble", MonsterGamble, "monstergamble", "mg"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.jester", Jester, "jester", "j"));
            TShockAPI.Commands.ChatCommands.Add(new Command("permissions.townnpc", townnpc, "townnpc"));
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            ServerApi.Hooks.ServerChat.Register(this, Bannedwords);
            //CreateConfig();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
                ServerApi.Hooks.ServerChat.Deregister(this, Bannedwords);
            }
            base.Dispose(disposing);
        }

        public void townnpc(CommandArgs args)
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

        private void randommaptp(CommandArgs args)
        {
            Random rnd = new Random();
            int x = rnd.Next(0, Main.maxTilesX);
            int y = rnd.Next(0, Main.maxTilesY);
            args.Player.Teleport(x * 16, y * 16);
        }

        private void randomtp(CommandArgs args)
        {
            if (TShock.Utils.ActivePlayers() <= 1)
            {
                args.Player.SendErrorMessage("There is only you on the server, you cannot teleport to yourself!", Color.Red);
                return;
            }
            Random rnd = new Random();
            TSPlayer ts = TShock.Players[rnd.Next(0, TShock.Utils.ActivePlayers() - 1)];
            if (ts.TPAllow && !args.Player.Group.HasPermission("permissions.tpallow"))
            {
                args.Player.SendErrorMessage(ts.Name + " has prevented users from teleporting to them.");
                ts.SendInfoMessage(args.Player.Name + " attempted to teleport to you.");
                return;
            }
            args.Player.Teleport(ts.TileX * 16, ts.TileY * 16);
        }

        //private static Config config;

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
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawnned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                    break;
                case "400pigron":
                    int monsteramountp = 170;
                    int monsteramountp1 = 171;
                    int amount1 = 200;
                    NPC npcs1 = TShock.Utils.GetNPCById(monsteramountp);
                    NPC npcs2 = TShock.Utils.GetNPCById(monsteramountp1);
                    TSPlayer.Server.SpawnNPC(npcs1.type, npcs1.name, amount1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(npcs2.type, npcs2.name, amount1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawnned {1} {2} time(s).", args.Player.Name, npcs1.name, amount1));
                    break;
                case "50kings":
                    int amountk = 50;
                    int monsteramountk = 50;
                    NPC npcsk = TShock.Utils.GetNPCById(monsteramountk);
                    TSPlayer.Server.SpawnNPC(npcsk.type, npcsk.name, amountk, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawnned {1} {2} time(s).", args.Player.Name, npcsk.name, amountk));
                    break;
                case "allbosses":
                    TSPlayer.Server.SetTime(false, 0.0);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(50).type, TShock.Utils.GetNPCById(50).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(4).type, TShock.Utils.GetNPCById(4).name, 2, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(65).type, TShock.Utils.GetNPCById(65).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(125).type, TShock.Utils.GetNPCById(125).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.Server.SpawnNPC(TShock.Utils.GetNPCById(126).type, TShock.Utils.GetNPCById(126).name, 1, args.Player.TileX, args.Player.TileY, 50, 20);
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
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has spawnned every boss.", args.Player.Name));
                    break;
            }
        }

        private static void MonsterGamble(CommandArgs args)
        {
            Random random = new Random();
            int monsteramount = random.Next(1, Main.maxNPCs);
            int amount = random.Next(1, 50);
            NPC npcs = TShock.Utils.GetNPCById(monsteramount);
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;
            var selectedPlayer = SEconomyPlugin.GetEconomyPlayerByBankAccountNameSafe(args.Player.Name);
            var playeramount = selectedPlayer.BankAccount.Balance;
            Money moneyamount = -50000;

            if (!args.Player.Group.HasPermission("bank.worldtransfer"))
            {
                if (playeramount > -moneyamount && (monsteramount == 22 || monsteramount == 17 || monsteramount == 18 || monsteramount == 37 || monsteramount == 38 || monsteramount == 19 || monsteramount == 20 || monsteramount == 37 || monsteramount == 54 || monsteramount == 68 || monsteramount == 124 || monsteramount == 107 || monsteramount == 108 || monsteramount == 113 || monsteramount == 142 || monsteramount == 178 || monsteramount == 207 || monsteramount == 208 || monsteramount == 209 || monsteramount == 227 || monsteramount == 228 || monsteramount == 160 || monsteramount == 229 || monsteramount == 353 || monsteramount == 368))
                {
                    int monsteramount2 = random.Next(1, Main.maxNPCs);
                    NPC npcs2 = TShock.Utils.GetNPCById(monsteramount2);
                    TSPlayer.Server.SpawnNPC(npcs2.type, npcs2.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawnned {1} {2} time(s).", args.Player.Name, npcs2.name, amount));
                    args.Player.SendSuccessMessage("You have lost 5g for monster gambling.");
                    SEconomyPlugin.WorldAccount.TransferToAsync(selectedPlayer.BankAccount, amount, Journalpayment, string.Format("5g has been lost for monster gambling", args.Player.Name), string.Format("CawAIO: " + "Monster Gambling"));
                }
                else
                {
                    TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawnned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                    args.Player.SendSuccessMessage("You have lost 5g for monster gambling.");
                    SEconomyPlugin.WorldAccount.TransferToAsync(selectedPlayer.BankAccount, amount, Journalpayment, string.Format("5g has been lost for monster gambling", args.Player.Name), string.Format("CawAIO: " + "Monster Gambling"));
                }
            }
            else
            {
                if (args.Player.Group.HasPermission("bank.worldtransfer") && (monsteramount == 22 || monsteramount == 17 || monsteramount == 18 || monsteramount == 37 || monsteramount == 38 || monsteramount == 19 || monsteramount == 20 || monsteramount == 37 || monsteramount == 54 || monsteramount == 68 || monsteramount == 124 || monsteramount == 107 || monsteramount == 108 || monsteramount == 113 || monsteramount == 142 || monsteramount == 178 || monsteramount == 207 || monsteramount == 208 || monsteramount == 209 || monsteramount == 227 || monsteramount == 228 || monsteramount == 160 || monsteramount == 229 || monsteramount == 353 || monsteramount == 368))
                {
                    int monsteramount2 = random.Next(1, Main.maxNPCs);
                    NPC npcs2 = TShock.Utils.GetNPCById(monsteramount2);
                    TSPlayer.Server.SpawnNPC(npcs2.type, npcs2.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawnned {1} {2} time(s).", args.Player.Name, npcs2.name, amount));
                    args.Player.SendSuccessMessage("You have lost nothing for monster gambling.");
                }
                else
                {
                    TSPlayer.Server.SpawnNPC(npcs.type, npcs.name, amount, args.Player.TileX, args.Player.TileY, 50, 20);
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} has randomly spawnned {1} {2} time(s).", args.Player.Name, npcs.name, amount));
                    args.Player.SendSuccessMessage("You have lost nothing for monster gambling.");
                }
            }
        }

        private static void Gamble(CommandArgs args)
        {
            Random random = new Random();
            int itemAmount = random.Next(1, 100);
            int prefixId = random.Next(1, 83);
            int itemName = random.Next(1, Main.maxItems);
            Item item;
            item = TShock.Utils.GetItemById(itemName);
            var selectedPlayer = SEconomyPlugin.GetEconomyPlayerByBankAccountNameSafe(args.Player.Name);
            Money amount = -50000;
            var Journalpayment = Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender;
            var playeramount = selectedPlayer.BankAccount.Balance;
            if (args.Player != null && selectedPlayer != null)
            {
                if (itemAmount > item.maxStack)
                {
                    itemAmount = item.maxStack;
                }
                if (playeramount > -amount)
                {

                    if (args.Player.InventorySlotAvailable || item.name.Contains("Coin"))
                    {
                        if (!args.Player.Group.HasPermission("bank.worldtransfer"))
                        {
                            item.prefix = (byte)prefixId;
                            args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);
                            SEconomyPlugin.WorldAccount.TransferToAsync(selectedPlayer.BankAccount, amount, Journalpayment, string.Format("5g has been lost for gambling", args.Player.Name), string.Format("CawAIO: " + "Gambling"));
                            args.Player.SendSuccessMessage("You have lost 5 gold and gambled {0} {1}(s).", itemAmount, item.AffixName());
                        }
                        else
                        {
                            item.prefix = (byte)prefixId;
                            args.Player.GiveItemCheck(item.type, item.name, item.width, item.height, itemAmount, prefixId);
                            args.Player.SendSuccessMessage("You have lost no gold and gambled {0} {1}(s).", itemAmount, item.AffixName());
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Your inventory seems full.");
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("You need 5 gold to gamble, you have {0}.", selectedPlayer.BankAccount.Balance);
                }
            }
            else
            {
                args.Player.SendErrorMessage("The server could not find a valid bank account for the username {0}", args.Player.Name);
            }
        }

        private static void Smack(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /smack <player>");
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendErrorMessage("Missing player name.");
                return;
            }

            string plStr = args.Parameters[0];
            var players = TShock.Utils.FindPlayer(plStr);
            if (players.Count == 0)
            {
                args.Player.SendErrorMessage("Invalid player!");
            }
            var PlayersFound = TShock.Utils.FindPlayer(args.Parameters[0]);
            if (PlayersFound.Count != 1)
            {
                args.Player.SendErrorMessage(PlayersFound.Count < 1 ? "No players matched." : "More than one player matched.");
                return;
            }
            else
            {
                var plr = players[0];
                if (args.Parameters.Count == 1)
                    TSPlayer.All.SendSuccessMessage(string.Format("{0} smacked {1}.",
                                                         args.Player.Name, plr.Name));
                Log.Info(args.Player.Name + " smacked " + plr.Name);
            }
        }

        private static void Forcehalloween(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendErrorMessage("Usage: /forcehalloween [true/false]");
                args.Player.SendInfoMessage(
                    String.Format("The server is currently {0} force Halloween mode.",
                                (TShock.Config.ForceHalloween ? "in" : "not in")));
                return;
            }

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
            if (args.Text.ToLower().Contains("yolo") || args.Text.ToLower().Contains("YOLO") || args.Text.ToLower().Contains("Y.O.L.O") || args.Text.ToLower().Contains("Yolo") || args.Text.ToLower().Contains("swag") || args.Text.ToLower().Contains("SWAG") || args.Text.ToLower().Contains("Swag"))
            {
                args.Handled = true;
                if (player.Group.HasPermission("permissions.staff"))
                {
                    args.Handled = false;
                }
                else
                {
                    TShock.Utils.Kick(player, "This is a yolo and swag free server.", true, true);
                    TShock.Utils.Broadcast("The server has force kicked " + player.Name + " for saying yolo or swag.", Color.Yellow);
                }
            }
            if (args.Text.StartsWith("/buff shadow d"))
            {
                args.Handled = true;
                if (player.Group.HasPermission("permissions.staff"))
                {
                    args.Handled = false;
                }
                else
                {
                    player.SendMessage("Shadow Dodge is not a buff you can use on this server through commands.", Color.Yellow);
                }
            }
        }

        private void OnChat(ServerChatEventArgs args)
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
            if (args.Text.StartsWith(".") || args.Text.StartsWith("!"))
            {
                args.Handled = true;
                string[] words = args.Text.Split();
                string cmd = words[0].Substring(1);
                words[0] = "(" + player.Group + ") " + player.Name + ":";
                switch (cmd)
                {
                    case "red":
                        Chat(Color.Red, words);
                        break;
                    case "green":
                        Chat(Color.Green, words);
                        break;
                    case "blue":
                        Chat(Color.Blue, words);
                        break;
                    case "yellow":
                        Chat(Color.Yellow, words);
                        break;
                    case "rainbow":
                        player.SendMessage("Do not use .rainbow again", Color.Orange);
                        break;
                    default:
                        player.SendMessage("That is an invalid command.", Color.Plum);
                        break;
                }
            }
        }

        private void Chat(Color color, string[] words)
        {
            String message = String.Join(" ", words);
            TSPlayer.All.SendMessage(message, color);
        }

        //private static void CreateConfig()
        //{
        //    string filepath = Path.Combine(TShock.SavePath, "CawAIO.json");
        //    try
        //    {
        //        using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
        //        {
        //            using (var sr = new StreamWriter(stream))
        //            {
        //                config = new Config();
        //                var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
        //                sr.Write(configString);
        //            }
        //            stream.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.ConsoleError(ex.Message);
        //        config = new Config();
        //    }
        //}

        //private static bool ReadConfig()
        //{
        //    string filepath = Path.Combine(TShock.SavePath, "CawAIO.json");
        //    try
        //    {
        //        if (File.Exists(filepath))
        //        {
        //            using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //            {
        //                using (var sr = new StreamReader(stream))
        //                {
        //                    var configString = sr.ReadToEnd();
        //                    config = JsonConvert.DeserializeObject<Config>(configString);
        //                }
        //                stream.Close();
        //            }
        //            return true;
        //        }
        //        else
        //        {
        //            Log.ConsoleError("CawAIO config not found. Creating new one...");
        //            CreateConfig();
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.ConsoleError(ex.Message);
        //    }
        //    return false;
        //}

        //class Config
        //{
        //    public bool GiveSEconomyCurrency = false;
        //    public int CurrencyAmount = 100;
        //}

        //private void Reload_Config(CommandArgs args)
        //{
        //    if (ReadConfig())
        //    {
        //        args.Player.SendMessage("CawAIO config reloaded sucessfully.", Color.Green);
        //    }
        //    else
        //    {
        //        args.Player.SendErrorMessage("CawAIO config reloaded unsucessfully. Check logs for details.");
        //    }
        //}

    }
}

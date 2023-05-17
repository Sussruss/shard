﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Engines;
using Server.Engines.XmlSpawner2;
using Server.Ethics;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands.GMUtils
{
    public static class GMExtendMethods
    {
        public enum EmotionalTextHue
        {
            NormalAction = 72,
            StrangeAction = 1510,
            HeroicAction = 55,
            AttackAction = 33,
            PainFeel = 25,
            HappyFeel = 26,
            BoringFeel = 77
        }

        public static void SayAction(this Mobile mob, EmotionalTextHue emote, string text, string self="")
        {
            if (!mob.Hidden && mob.AccessLevel < AccessLevel.Counselor)
            {
                string name = "Action";
                if (emote == EmotionalTextHue.BoringFeel || emote == EmotionalTextHue.PainFeel || emote == EmotionalTextHue.HappyFeel)
                    name = "Emote";
                mob.PublicOverheadMessageLos(MessageType.Emote, (int)emote, name, $"* {text} *", self, true);
            }
        }

        public static void PublicOverheadMessageLos(this Mobile mob, MessageType type, int hue, string name, string text, string self, bool noLineOfSight)
        {
            if (mob.Map != null)
            {
                Packet p = null;


                IPooledEnumerable eable = mob.Map.GetClientsInRange(mob.Location);

                foreach (NetState state in eable)
                {
                    if (state.Mobile!=mob && state.Mobile.CanSee(mob) && mob.InLOS(state.Mobile))
                    {
                        if (p == null)
                        {
                            //p = new AsciiMessage(mob.Serial, mob.Body, type, hue, 2, name, text);
                            p = new UnicodeMessage(mob.Serial, mob.Body, type, hue, 3,"ENU", name, $"* {text} *");
                            p.Acquire();
                        }

                        state.Send(p);
                    }

                    if (state.Mobile == mob && !string.IsNullOrEmpty(self))
                    {
                        p = new UnicodeMessage(mob.Serial, mob.Body, type, hue, 3, "ENU", name, $"* {self} *");
                    }
                }

                Packet.Release(p);

                eable.Free();
            }
        }

        public static int ChangeDamage(this Mobile mob, Mobile def, int damage)
        {
            if (mob.IsNight == def.IsNight)
                return damage;
            if (mob.IsNight)
                return (int)(damage * UtilityWorldTime.NightMultiplier());
            if (def.IsNight)
                return Utility.LimitMin(1, (int)(damage / UtilityWorldTime.NightMultiplier()));
            return damage;
        }
    }
}
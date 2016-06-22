using System;

public class DebugCommandLevelup : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length >= 3)
        {
            if ((Turn.Character == null) || (Location.Current == null))
            {
                return base.Error("location not loaded");
            }
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window == null)
            {
                return base.Error("window not loaded");
            }
            string[] textArray1 = new string[] { "power", "hand", "prof", "skill", "card" };
            if (!base.IsArgValid(args[1], textArray1))
            {
                return base.Error("strange parameter " + base.Parameter(args[1]));
            }
            if (args[1] == "power")
            {
                string power = args[2];
                Turn.Character.Levelup(power);
                window.powersPanel.Refresh();
                if (Turn.Character.HasPower(power))
                {
                    return base.Success(base.Parameter(Turn.Character.DisplayName) + " gained " + base.Parameter(power));
                }
                return base.Error(base.Parameter(Turn.Character.DisplayName) + " did not gain " + base.Parameter(power));
            }
            if (args[1] == "hand")
            {
                int result = 0;
                int.TryParse(args[2], out result);
                Turn.Character.Levelup(result);
                return base.Success(base.Parameter(Turn.Character.DisplayName) + " increased hand size by " + base.Parameter(result.ToString()));
            }
            if (args[1] == "prof")
            {
                string[] textArray2 = new string[] { "LA", "HA", "WP" };
                if (!base.IsArgValid(args[2], textArray2))
                {
                    return base.Error("strange parameter " + base.Parameter(args[2]));
                }
                if (args[2] == "LA")
                {
                    Turn.Character.Levelup(ProficencyType.LightArmor);
                }
                if (args[2] == "HA")
                {
                    Turn.Character.Levelup(ProficencyType.HeavyArmor);
                }
                if (args[2] == "WP")
                {
                    Turn.Character.Levelup(ProficencyType.Weapons);
                }
                return base.Success(base.Parameter(Turn.Character.DisplayName) + " gained proficiency " + base.Parameter(args[2]));
            }
            if (args[1] == "skill")
            {
                string[] textArray3 = new string[] { "STR", "DEX", "CON", "INT", "WIS", "CHA" };
                if (!base.IsArgValid(args[2], textArray3))
                {
                    return base.Error("strange parameter " + base.Parameter(args[2]));
                }
                if (args[2] == "STR")
                {
                    Turn.Character.Levelup(AttributeType.Strength, 1);
                }
                if (args[2] == "DEX")
                {
                    Turn.Character.Levelup(AttributeType.Dexterity, 1);
                }
                if (args[2] == "CON")
                {
                    Turn.Character.Levelup(AttributeType.Constitution, 1);
                }
                if (args[2] == "INT")
                {
                    Turn.Character.Levelup(AttributeType.Intelligence, 1);
                }
                if (args[2] == "WIS")
                {
                    Turn.Character.Levelup(AttributeType.Wisdom, 1);
                }
                if (args[2] == "CHA")
                {
                    Turn.Character.Levelup(AttributeType.Charisma, 1);
                }
                return base.Success(base.Parameter(Turn.Character.DisplayName) + " increased " + base.Parameter(args[2]) + " +1");
            }
            if (args[1] == "card")
            {
                string[] textArray4 = new string[] { "WP", "SP", "AR", "IT", "AL", "BL" };
                if (!base.IsArgValid(args[2], textArray4))
                {
                    return base.Error("strange parameter " + base.Parameter(args[2]));
                }
                if (args[2] == "WP")
                {
                    Turn.Character.Levelup(CardType.Weapon, 1);
                }
                if (args[2] == "SP")
                {
                    Turn.Character.Levelup(CardType.Spell, 1);
                }
                if (args[2] == "AR")
                {
                    Turn.Character.Levelup(CardType.Armor, 1);
                }
                if (args[2] == "IT")
                {
                    Turn.Character.Levelup(CardType.Item, 1);
                }
                if (args[2] == "AL")
                {
                    Turn.Character.Levelup(CardType.Ally, 1);
                }
                if (args[2] == "BL")
                {
                    Turn.Character.Levelup(CardType.Blessing, 1);
                }
            }
        }
        return this.HelpText;
    }

    public override string Command =>
        "levelup";

    public override string HelpText =>
        "Syntax: levelup [power|hand|prof|skill|card] [id][#][LA|HA|WP][STR|DEX|CON|INT|WIS|CHA][WP|SP|AR|IT|AL|BL]";
}


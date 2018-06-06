using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats
{
    public int str, dex, intell, wis, con;

    public Stats(int Str, int Dex, int Intell, int Wis, int Con)
    {
        str = Str;
        dex = Dex;
        intell = Intell;
        wis = Wis;
        con = Con;
    }
}

public class characterStats : MonoBehaviour {

    /* Class/Race Stuff */
    public string className, raceName;
    public Text classText, raceText, strText, dexText, intellText, wisText, conText;
    public Stats fighterStartingStats, clericStartingStats, wizardStartingStats, rogueStartingStats;
    public Stats tempClassStartingStats, tempRaceStartingStats;

    // Use this for initialization
    void Start () {
        fighterStartingStats = new Stats(16, 14, 8, 10, 14);
        clericStartingStats = new Stats(12, 14, 10, 16, 12);
        wizardStartingStats = new Stats(8, 12, 16, 10, 10);
        rogueStartingStats = new Stats(12, 16, 14, 8, 12);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void chosenClass(string name)
    {
        if(name == "Fighter")
        {
            classText.text = "Fighter";
            className = "Fighter";
            strText.text = fighterStartingStats.str.ToString();
            dexText.text = fighterStartingStats.dex.ToString();
            conText.text = fighterStartingStats.con.ToString();
            wisText.text = fighterStartingStats.wis.ToString();
            intellText.text = fighterStartingStats.intell.ToString();
            tempClassStartingStats = fighterStartingStats;
        }
        if (name == "Cleric")
        {
            classText.text = "Cleric";
            className = "Cleric";
            strText.text = clericStartingStats.str.ToString();
            dexText.text = clericStartingStats.dex.ToString();
            conText.text = clericStartingStats.con.ToString();
            wisText.text = clericStartingStats.wis.ToString();
            intellText.text = clericStartingStats.intell.ToString();
            tempClassStartingStats = clericStartingStats;
        }
        if (name == "Wizard")
        {
            classText.text = "Wizard";
            className = "Wizard";
            strText.text = wizardStartingStats.str.ToString();
            dexText.text = wizardStartingStats.dex.ToString();
            conText.text = wizardStartingStats.con.ToString();
            wisText.text = wizardStartingStats.wis.ToString();
            intellText.text = wizardStartingStats.intell.ToString();
            tempClassStartingStats = wizardStartingStats;
        }
        if (name == "Rogue")
        {
            classText.text = "Rogue";
            className = "Rogue";
            strText.text = rogueStartingStats.str.ToString();
            dexText.text = rogueStartingStats.dex.ToString();
            conText.text = rogueStartingStats.con.ToString();
            wisText.text = rogueStartingStats.wis.ToString();
            intellText.text = rogueStartingStats.intell.ToString();
            tempClassStartingStats = rogueStartingStats;
        }
    }

    public void chosenRace(string name)
    {
        if (name == "Human")
        {
            raceText.text = "Human";
            raceName = "Human";
            strText.text = strText.text + "(0)";
            dexText.text = dexText.text + "(0)";
            conText.text = conText.text + "(0)";
            wisText.text = wisText.text + "(0)";
            intellText.text = intellText.text + "(0)";
            tempRaceStartingStats = new Stats(0, 0, 0, 0, 0);
        }
        if (name == "Orc")
        {
            raceText.text = "Orc";
            raceName = "Orc";
            strText.text = strText.text + "(+2)";
            dexText.text = dexText.text + "(0)";
            conText.text = conText.text + "(0)";
            wisText.text = wisText.text + "(-2)";
            intellText.text = intellText.text + "(+2)";
            tempRaceStartingStats = new Stats(2, 0, 0, -2, 2);
        }
        if (name == "Dwarf")
        {
            raceText.text = "Dwarf";
            raceName = "Dwarf";
            strText.text = strText.text + "(0)";
            dexText.text = dexText.text + "(0)";
            conText.text = conText.text + "(-2)";
            wisText.text = wisText.text + "(+2)";
            intellText.text = intellText.text + "(+2)";
            tempRaceStartingStats = new Stats(0, 0, -2, 2, 2);
        }
        if (name == "Elf")
        {
            raceText.text = "Elf";
            raceName = "Elf";
            strText.text = strText.text + "(0)";
            dexText.text = dexText.text + "(+2)";
            conText.text = conText.text + "(0)";
            wisText.text = wisText.text + "(0)";
            intellText.text = intellText.text + "(0)";
            tempRaceStartingStats = new Stats(0, 2, 0, 0, 0);
        }
    }

    public void confirmChoice()
    {
        Player.playerStats = new Stats(0, 0, 0, 0, 0);
        Player.playerStats.str = tempClassStartingStats.str + tempRaceStartingStats.str;
        Player.playerStats.dex = tempClassStartingStats.dex + tempRaceStartingStats.dex;
        Player.playerStats.con = tempClassStartingStats.con + tempRaceStartingStats.con;
        Player.playerStats.intell = tempClassStartingStats.intell + tempRaceStartingStats.intell;
        Player.playerStats.wis = tempClassStartingStats.wis + tempRaceStartingStats.wis;
    }
}

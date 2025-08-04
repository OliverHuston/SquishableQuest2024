using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SQuestUtilities
{

    /// <summary>Roll</summary>
    /// <param name="s">string to be evaluated</param>
    /// <returns>result of evaluated string</returns>
    /// NB: Works for addition/subtraction, multiplication, and any XdX --- but not division or XdXdX...
    public static int Roll(string s)
    {
        int total = 0;
        s=s.ToLower();
        s=s.Replace(" ", "");
        if(s[0]=='-') { s = "0" + s; }
        s=s.Replace("-", "+-");

        string[] additionSegs = s.Split('+');
        if (additionSegs.Length > 1) {
            foreach (string seg in additionSegs)
            {
                if (seg[0] == '-') { total -= Roll(seg[1..]); }
                else { total += Roll(seg); }
            }
        }
        else {
            string[] multipSegs = additionSegs[0].Split('*');

            if (multipSegs.Count() > 1)
            {
                if (total == 0) total = 1;

                foreach (string seg in multipSegs)
                    total *= Roll(seg);
            }
            else { 
                string[] diceSegs = s.Split('d');

                if (diceSegs.Length > 1)
                {
                    int numOfDice;
                    if (!int.TryParse(diceSegs[0], out numOfDice)) { numOfDice = 1; }
                    int diceType = int.Parse(diceSegs[1]);

                    for (int i = 0; i < numOfDice; i++)
                    {
                        total += Random.Range(1, diceType + 1);
                    }
                }
                else { total += int.Parse(diceSegs[0]); }
            }
        }

        //Debug.Log(total);
        return total;
    }

    public static bool RollXPlus(int target)
    {
        if (target < 2) target = 2;
        //else if (target > 6) target = 6;

        if (Roll("d6") >= target) return true;
        return false;
    }
}
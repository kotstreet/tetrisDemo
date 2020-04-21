using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public  class Player
{
    public string Name { get; set; }
    public int Score { get; set; }

    /// <summary>
    /// Create player by score withoout name
    /// </summary>
    /// <param name="score">score of the player</param>
    public Player(int score)
    {
        Score = score;
        Name = String.Empty;
    }

    /// <summary>
    /// Converted line from file to Player
    /// </summary>
    private Player(string lineFromFile)
    {
        var stringsFromFile = lineFromFile.Split(Constants.SeparatorForFile);

        Score = Convert.ToInt32(stringsFromFile[0]);
        Name = stringsFromFile[1];
    }

    /// <summary>
    /// Read data from file 
    /// </summary>
    /// <returns>top-15 people</returns>
    public static List<Player> Read()
    {
        //check for exists any player in file
        var havePlayers = CheckForFile();
        if (havePlayers == false)
        {
            return new List<Player>() { new Player(0) };
        }

        CheckForFile();

        var players = new List<Player>();

        //read data from file
        using (var streamReader = new StreamReader(Constants.FileName))
        {
            string line;

            //read line
            while ((line = streamReader.ReadLine()) != null)
            {
                //add player to list
                var player = new Player(line);
                players.Add(player);
            }
        }

        return players;
    }

    /// <summary>
    /// Write data to file 
    /// </summary>
    /// <param name="players">data for write, ordered by score</param>
    public static void Write(List<Player> players)
    {
        using (var streamWriter = new StreamWriter(Constants.FileName, false))
        {
            players.ForEach(player => streamWriter.WriteLine($"{player.Score}{Constants.SeparatorForFile}{player.Name}"));
        }
    }

    /// <summary>
    /// checks if the player enter to top-15
    /// </summary>
    /// <returns>true if the player entered</returns>
    public bool CheckForNeedWrite(out List<Player> players)
    {
        players = Read();
        var count = players.Count;
        var minScore = count > 0 ? players.Min(player => player.Score) : 0; 

        if(count < Constants.TopFifteen)
        {
            return true;
        }
        else
        {
            return this.Score > minScore;
        }
    }

    /// <summary>
    /// Check if exists any player in the file
    /// </summary>
    /// <returns>true if any player exists</returns>
    private static bool CheckForFile()
    {
        //check for file exists
        if (File.Exists(Constants.FileName) == false)
        {
            File.Create(Constants.FileName).Close();
            return false;
        }

        return true;
    }

}

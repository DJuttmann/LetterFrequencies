//========================================================================================
// LetterFrequencies by Daan Juttmann
// Created: 2017-11-06
// License: GNU General Public License 3.0 (https://www.gnu.org/licenses/gpl-3.0.en.html).
//========================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LetterFrequencies
{
  class CharacterCounter
  {
    private Dictionary <char, int> Frequencies;

    // METHODS

    
    private int LessFrequentThan (char a, char b)
    {
      return GetCount (b) - GetCount (a);
    }


    // Constructor.
    public CharacterCounter ()
    {
      Frequencies = new Dictionary <char, int> ();
    }


    // Count character
    public void Add (char c)
    {
      if (c == '\x0A' || c == '\x0D') // ignore new line characters
        return;
      if (Frequencies.ContainsKey (c))
      {
        Frequencies [c]++;
      }
      else
      {
        Frequencies.Add (c, 1);
      }
    }


    // Get the frequency of a character.
    public int GetCount (char c)
    {
      if (Frequencies.ContainsKey (c))
      {
        return Frequencies [c];
      }
      else
      {
        return 0;
      }
    }


    // Return a sorted list of all characters from the dictionary.
    public List <char> Characters (FrequencyAnalyzer.SortingMode sort)
    {
      Dictionary <char, int>.KeyCollection Keys = Frequencies.Keys;
      List <char> keyList = new List <char> (Keys.Count);
      foreach (char letter in Keys) {
        keyList.Add (letter);
      }
      switch (sort)
      {
      case FrequencyAnalyzer.SortingMode.Frequency:
        keyList.Sort (LessFrequentThan);
        break;
      case FrequencyAnalyzer.SortingMode.Unicode:
      default:
        keyList.Sort ();
        break;
      }


      return keyList;
    }
  }


  class FrequencyAnalyzer
  {
    public enum SortingMode {Unicode, Frequency}

    CharacterCounter Counter = new CharacterCounter ();

    // METHODS

    // Analyze the character frequencies of a text file and save frequency file.
    // Read file on the fly.
    public void Analyze (string inputFile, string outputFile, SortingMode sort)
    {
      StreamReader input;
      StreamWriter output;
      try
      {
        input = new StreamReader (new FileStream (inputFile, FileMode.Open));
        output = new StreamWriter (new FileStream (outputFile, FileMode.Create));
        char c;

        while (!input.EndOfStream)
        {
          c = (char) input.Read (); // [wip] why does this return an int32?
          Counter.Add (c);
        }
        List <char> letters = Counter.Characters (sort);
        foreach (char letter in letters)
        {
          output.WriteLine ("{0}\t{1}", letter, Counter.GetCount (letter));
        }
        input.Dispose ();
        output.Dispose ();
      }
      catch (Exception ex)
      {
        Console.WriteLine ("Error Occured");
        Console.WriteLine (ex.Message);
      }
    }


    /* Analyze the character frequencies of a text file and save frequency file.
    // Read file all at once.
    public void Analyze2 (string inputFile, string outputFile)
    {
      StreamWriter output;
      string [] text;
      try
      {
        text = System.IO.File.ReadAllLines (inputFile);
      }
      catch (Exception ex)
      {
        Console.WriteLine (ex.Message);
        return;
      }

      for (int i = 0; i < text.Length; i++)
      {
        for (int j = 0; j < text [i].Length; j++)
        {
          Counter.Add (text [i] [j]);
        }
      }
      List <char> letters = Counter.Characters (SortingMode.Unicode);

      try
      {
        output = new StreamWriter (new FileStream (outputFile, FileMode.Create));
        foreach (char letter in letters)
        {
          output.WriteLine ("{0}\t{1}", letter, Counter.GetCount (letter));
        }
        output.Dispose ();
      }
      catch (Exception ex)
      {
        Console.WriteLine ("Error Occured");
        Console.WriteLine (ex.Message);
      }
    }*/
  }


  class LetterFrequencies
  {
    // Remove the extension from a file name or path + file name
    static string TruncateFileExtension (string s)
    {
      for (int i = s.Length - 1; i >= 0; i--)
      {
        switch (s [i])
        {
        case '.':
          return s.Substring (0, i);
        case '\\':
        case '/':
          return s;
        default:
          break;
        }
      }
      return s;
    }


    // Parse the command line parameters.
    private static void ParseArguments (string [] args, ref string outputFile,
                                        ref FrequencyAnalyzer.SortingMode sort)
    {
      for (int i = 1; i < args.Length; i++)
      {
        if (args [i] == "-o" && i + 1 < args.Length)
        {
          i++;
          outputFile = args [i];
        }
        if (args [i] == "-sort" && i + 1 < args.Length)
        {
          i++;
          if (args [i] == "uni")
            sort = FrequencyAnalyzer.SortingMode.Unicode;
          if (args [i] == "freq")
            sort = FrequencyAnalyzer.SortingMode.Frequency;
        }
      }
    }


    // Main.
    static void Main (string [] args)
    {
      string inputFile = "";
      string outputFile = "";
      FrequencyAnalyzer.SortingMode sort = FrequencyAnalyzer.SortingMode.Unicode;
      if (args.Length == 0)
        return;
      inputFile = args [0];
      outputFile = TruncateFileExtension (inputFile) + " [freq].txt";
      ParseArguments (args, ref outputFile, ref sort);

      FrequencyAnalyzer textAnalyzer = new FrequencyAnalyzer ();
      textAnalyzer.Analyze (inputFile, outputFile, sort);
//      Console.ReadKey ();
    }
  }
}
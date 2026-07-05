using System.Collections.Generic;
using System.Linq;
using System;

public class Kata
{
  public static string SpinWords(string sentence)
  {
    string[] words = sentence.Split(' ');
    var result = new List<String>();
    
    foreach(var word in words){
      if(word.Length >= 5){
        char[] chars = word.ToCharArray();
        Array.Reverse(chars);
        string reversed = new string(chars);
        result.Add(reversed);
      } else {
        result.Add(word);
      }
    }
    return string.Join(' ', result);
  }
}
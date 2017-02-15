using NBPClient.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Core.Infrastructure
{
    public class Randomizer
    {
        public static List<RateModel> RandomArrayEntries(List<RateModel> arrayItems, int count)
        {
            var listToReturn = new List<RateModel>();

            if (arrayItems.Count != count)
            {
                var deck = CreateShuffledDeck(arrayItems);

                for (var i = 0; i < count; i++)
                {
                    var arrayItem = deck.Pop();
                    listToReturn.Add(arrayItem);
                }

                return listToReturn;
            }

            return arrayItems;
        }
        public static Stack<RateModel> CreateShuffledDeck(List<RateModel> values)
        {

            var random = new Random();
            var list = new List<RateModel>(values);
            var stack = new Stack<RateModel>();
                while (list.Count > 0)
                {  // Get the next item at random.
                    var randomIndex = random .Next(0, list.Count);
                    var randomItem = list[randomIndex];
                    // Remove the item from the list and push it to the top of the deck. 
                    list.RemoveAt(randomIndex);
                    stack.Push(randomItem );
                }  return stack;
         }

  }
 }
